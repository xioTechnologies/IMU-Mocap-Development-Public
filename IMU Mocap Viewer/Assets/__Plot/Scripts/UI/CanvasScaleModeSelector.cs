using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public sealed class CanvasScaleModeSelector : MonoBehaviour
{
    [SerializeField] private float mininmumWidthInPixels = 500f; 

    private CanvasScaler canvasScaler;
    
    private bool isScalingEnabled = false;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        
        CheckAndUpdateScaleMode(true);
    }

    private void Update() => CheckAndUpdateScaleMode(false);

    private void CheckAndUpdateScaleMode(bool apply)
    {
        float currentScreenWidth = Screen.width;

        if (currentScreenWidth < mininmumWidthInPixels)
        {
            if (apply == false && isScalingEnabled) return;
                
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(mininmumWidthInPixels, canvasScaler.referenceResolution.y);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0f; 
            isScalingEnabled = true;
            
            return;
        }

        if ((apply == false && isScalingEnabled == false)) return; 

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        isScalingEnabled = false;
    }
}