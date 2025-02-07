using System;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using RenderSettings = UnityEngine.RenderSettings;
using Vector3 = UnityEngine.Vector3;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Camera")] [SerializeField] private Camera camera;
    [SerializeField] private List<Camera> cameras;

    [Header("Characters")] [SerializeField]
    private GameObject toji;
    [SerializeField] private GameObject gojo;
    [SerializeField] private GameObject tojiRotator;
    [SerializeField] private GameObject tojisKnife;
    [SerializeField] private GameObject fakeToji;

    [Header("Placements")] [SerializeField]
    private List<Transform> placements;

    private Chain chain;

    [Header("Lights")] 
    [SerializeField] private Light pointLight;
    [SerializeField] private Color skyboxMaterial;

    [FormerlySerializedAs("text")] [Header("UI")] [SerializeField] private TextMeshProUGUI textUI;
    
    [SerializeField] private List<String> dialogues = new List<String>();

    [SerializeField] private Color purpleColor = Color.magenta;
    
    [Header("Misc")]
    [SerializeField] private float timeScale = 1f;

    private float _knifeRotation = 0;
    
    private void Start()
    {
        chain = tojisKnife.GetComponent<Chain>();
        Sequence seq = DOTween.Sequence();

        var originalCameraRotation = camera.transform.rotation;
        var newRotationEuler = originalCameraRotation.eulerAngles;
        newRotationEuler.x = 13;


        // Intro sequence: The camera looks at toji and focus on his face
        // Rotate the camera from the ground towards toji
        seq.Append(camera.transform.DORotate(newRotationEuler, 2f).SetEase(Ease.OutQuad));
        seq.AppendInterval(0.5f);

        // Make toji sit by rotating him
        var tojiOriginalRotation = toji.transform.rotation;
        var tojiRotationEuler = tojiOriginalRotation.eulerAngles;
        tojiRotationEuler.z = 0;

        seq.Append(toji.transform.DORotate(tojiRotationEuler, 1f).SetEase(Ease.InCubic));

        // Add camera transition to his face
        // This move does nothing but we need the OnComplete Callback
        seq.Append(
            toji.transform.DOMove(toji.transform.position, 0.5f)
                .OnComplete(() => SetMainCameraTransform(0)));

        // Add a bit of breathing to toji to make it feel more natural
        var maxBreathHeight = 0.03f;
        seq.Append(
            toji.transform.DOMove(toji.transform.position + maxBreathHeight * Vector3.up, 0.75f)
                .SetEase(Ease.InOutSine)
                .SetLoops(4, LoopType.Yoyo)
                // Now focus on Gojo, who should be floating in the air right now
                .OnComplete(() => SetMainCameraTransform(1))
        );

        // Make gojo float a bit
        var maxFloatHeight = 0.8f;
        seq.Append(
            gojo.transform.DOMove(gojo.transform.position + maxFloatHeight * Vector3.up, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(1, LoopType.Yoyo)

                // Focus on gojo's face
                .OnComplete(() => SetMainCameraTransform(2))
        );

        // Make him float again
        seq.Append(
            gojo.transform.DOMove(gojo.transform.position + 0.5f * maxFloatHeight * Vector3.up, 3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(1, LoopType.Yoyo)
                // Turn camera back to toji
                .OnComplete(() => SetMainCameraTransform(3))
        );

        // Make toji Stand
        seq.AppendInterval(0.75f);
        seq.Append(
            toji.transform.DOMoveY(0.7f, 0.25f)
        );

        seq.AppendInterval(0.8f);

        // Then make it go a step further and activate the weapon
        seq.Append(
            toji.transform.DOMove(placements[0].transform.position, 0.25f)
                .OnComplete(
                    () => tojisKnife.SetActive(true))
        );

        // make the weapon spin a bit
        seq.Append(
            tojisKnife.transform.DOMove(placements[1].transform.position, 0.25f)
        );
        var rotation = placements[1].rotation.eulerAngles;
        seq.Append(tojisKnife.transform.DORotate(rotation, 0.25f)
            .OnComplete(() =>
            {
                chain.toFollow = placements[1].gameObject;
                placements[1].transform.SetParent(tojiRotator.transform);
            }));
        
        // now rotate the chain 
        seq.Append(
            tojiRotator.transform.DOLocalRotate(270* Vector3.up, 0.5f)
                .SetLoops(4, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    chain.toFollow = null;
                    tojisKnife.transform.LookAt(camera.transform);
                })
        );
        
        // Throw knife to camera
        seq.Append(
            tojisKnife.transform.DOMove(gojo.transform.position, 0.5f)
                .OnComplete(()=>
                {
                    tojisKnife.SetActive(false);
                    // Put camera in gojos face
                    SetMainCameraTransform(4);
                    fakeToji.SetActive(true);
                })
        );

        // Add sequence with slow mo knife going through gojos face
        var fakeTojiOriginalRotation = fakeToji.transform.rotation.eulerAngles;
        var fakeTojiFinalRotation = fakeTojiOriginalRotation;
        fakeTojiFinalRotation.y = 2;
        seq.Append(
            fakeToji.transform.DORotate(fakeTojiFinalRotation, 3f)            
                .SetEase(Ease.Linear)
        );
        // Make face light brighter
        seq.Append(
            pointLight.DOIntensity(1.19f, 2)
        );
        seq.Append(
            pointLight.DOIntensity(1.19f, 0.75f) // just a delay, does nothing
                .OnComplete(()=>
                {
                    SetMainCameraTransform(5);
                    fakeToji.SetActive(false);
                })
        );
        // Look at gojo from above
        seq.Append(
            pointLight.DOIntensity(1.19f, 2f) // Another delay, still does nothing
                .OnComplete(() =>
                {
                    SetMainCameraTransform(6);
                    // Play first Dialogue
                    PlayText(dialogues[0], 1f);
                })
            );
        // Make gojo float again
        seq.Append(
            gojo.transform.DOMove(gojo.transform.position + maxFloatHeight * Vector3.up, 1.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(3, LoopType.Yoyo)
                .OnComplete(()=>
                {
                    // Prepare gojo for next scene
                    textUI.text = "";
                    // Set camera looking to the sky
                    SetMainCameraTransform(7);
                    
                    // Put gojo under the camera to move it upwards in the next tween
                    gojo.transform.position = placements[2].position;
                    gojo.transform.rotation = placements[2].rotation;
                })
            );

        // Move gojo towards
        seq.Append(
            gojo.transform.DOMove(placements[3].position, 1f)
                // Play dialogue: "Throughout the heavens and earth..."
                .OnComplete(() => PlayText(dialogues[1], 1f))
            );
        seq.Append(
                // Does nothing, it's a delay to play the second line later
                gojo.transform.DOMove(placements[3].position, 2f) 
                    // Play dialogue: "I alone am the honored one"
                .OnComplete(() =>
                    {
                        RenderSettings.skybox.color = purpleColor;
                        PlayText(dialogues[2], 1f);
                    })
            );
        seq.AppendInterval(1);
        
        // Now change to looking to the angle view and tint the lights purple
        // seq.Append(
        //     // changing the tint color of the skybox material will change global light
        // );


        seq.Play();
    }

    private void Update()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;
    }

    // Used to set the main camera transform to another cameras transform
    void SetMainCameraTransform(int cameraIndex)
    {
        // Vector3.forward
        camera.transform.position = cameras[cameraIndex].transform.position;
        camera.transform.rotation = cameras[cameraIndex].transform.rotation;
    }

    void PlayText(string text, float time)
    {
        float timePerChar = time / text.Length;

        IEnumerator<WaitForSeconds> PlayTextCoroutine()
        {
            for (int i = 1; i <= text.Length; i++)
            {
                var newMsg = text.Substring(0, i);
                textUI.text = newMsg;
                yield return new WaitForSeconds(timePerChar);
            }
        }
        
        StartCoroutine(PlayTextCoroutine());
    }

    void SetSkyboxTint()
    {
        RenderSettings.skybox.color = purpleColor;
    }
}