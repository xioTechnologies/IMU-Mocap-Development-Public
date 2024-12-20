using UnityEngine;
using UnityEngine.UI;

namespace Viewer.Runtime.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class CanvasScaleModeSelector : MonoBehaviour
    {
        [SerializeField] private float mininmumWidthInPixels = 500f;
        [SerializeField] private float maximumHeightInPixels = 1080f;

        private CanvasScaler canvasScaler;
        
        enum ScaleMode
        {
            None, 
            TooSmallWidth,
            ConstantPixelSize,
            ToLargeHeight
        }
        
        private ScaleMode mode = ScaleMode.None;
        
        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            
            mode = ScaleMode.None;
            
            CheckAndUpdateScaleMode();
        }

        private void Update() => CheckAndUpdateScaleMode();

        private void CheckAndUpdateScaleMode()
        {
            float currentScreenWidth = Screen.width;
            float currentScreenHeight = Screen.height;

            if (currentScreenWidth < mininmumWidthInPixels)
            {
                if (mode == ScaleMode.TooSmallWidth) return;

                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(mininmumWidthInPixels, maximumHeightInPixels);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0f;
                mode = ScaleMode.TooSmallWidth;

                return;
            }

            if (currentScreenHeight > maximumHeightInPixels) 
            { 
                if (mode == ScaleMode.ToLargeHeight) return;
                
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(mininmumWidthInPixels, maximumHeightInPixels);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 1f;
                mode = ScaleMode.ToLargeHeight;

                return;
            }

            if (mode == ScaleMode.ConstantPixelSize) return;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            mode = ScaleMode.ConstantPixelSize;
        }
    }
}