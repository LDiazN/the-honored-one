using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private List<Camera> cameras;
    
    [Header("Characters")]
    [SerializeField]
    private GameObject toji;

    [SerializeField] private GameObject gojo;

    private void Start()
    {
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
            gojo.transform.DOMove(gojo.transform.position + maxFloatHeight * Vector3.up, 3f)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)
            
            // Focus on gojo's face
            .OnComplete(() => SetMainCameraTransform(2))
        );
        
        // Make him float again
        seq.Append(
            gojo.transform.DOMove(gojo.transform.position + 0.5f*maxFloatHeight * Vector3.up, 3f)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo)
        );
        
        
        


        seq.Play();
    }
    
    // Used to set the main camera transform to another cameras transform
    void SetMainCameraTransform(int cameraIndex)
    {
        camera.transform.position = cameras[cameraIndex].transform.position;
        camera.transform.rotation = cameras[cameraIndex].transform.rotation;
    }
    
    
}
