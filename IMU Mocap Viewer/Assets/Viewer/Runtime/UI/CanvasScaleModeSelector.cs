﻿using UnityEngine;
using UnityEngine.UI;

namespace Viewer.Runtime.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class CanvasScaleModeSelector : MonoBehaviour
    {
        [SerializeField] private float mininmumWidthInPixels = 500f;
        [SerializeField] private float maximumHeightInPixels = 1080f;
        // [SerializeField, Range(1f, 2f)] private float scaleOverride = 1f;

        private CanvasScaler canvasScaler;
        
        enum ScaleMode
        {
            None, 
            TooSmallWidth,
            ConstantPixelSize
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
            canvasScaler.scaleFactor = PixelScaleUtility.DpiScaleFactor;
            
            float currentScreenWidth = Screen.width;
            
            if (currentScreenWidth < mininmumWidthInPixels * PixelScaleUtility.DpiScaleFactor)
            {
                if (mode == ScaleMode.TooSmallWidth) return;

                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(mininmumWidthInPixels, maximumHeightInPixels);
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = 0f;
                mode = ScaleMode.TooSmallWidth;

                return;
            }

            if (mode == ScaleMode.ConstantPixelSize) return;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            mode = ScaleMode.ConstantPixelSize;
        }
    }
}