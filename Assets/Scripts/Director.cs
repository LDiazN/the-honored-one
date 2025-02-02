using System;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
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

    [Header("Placements")] [SerializeField]
    private List<Transform> placements;

    private Chain chain;
    
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
                    SetMainCameraTransform(4);
                })
        );
        
        


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
}