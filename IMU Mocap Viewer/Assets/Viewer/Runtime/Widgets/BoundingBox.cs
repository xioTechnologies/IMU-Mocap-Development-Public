using System;
using UnityEngine;
using Viewer.Runtime.Draw;

namespace Viewer.Runtime.Widgets
{
    public class BoundingBox : MonoBehaviour
    {
        [Header("Drawing")] [SerializeField] private DrawingGroup group;
        [SerializeField] private int maxLineCount = 1000;
        [SerializeField] private Mesh lineMesh;
        [SerializeField] private Material instanceMaterial;
        
        [Header("Line Properties")] 
        [SerializeField, Range(0f, 10f)] private float lineWidthPixels = 1f;
        [SerializeField] private Color color;
        
        private StretchableDrawBatch lines;
        private new Camera camera;
        
        public Bounds Bounds { get; set; }

        private void Awake()
        {
            lines = new StretchableDrawBatch(maxLineCount, lineMesh, instanceMaterial);
            
            camera = Camera.main;
        }
        
        private void OnEnable() => group.RegisterSource(lines);

        private void OnDisable() => group.UnregisterSource(lines);

        private void OnDestroy() => lines?.Dispose();
        
        void Update() 
        { 
            lines.Clear();
            
            var min = Bounds.min;
            var max = Bounds.max;
            
            var p0 = new Vector3(min.x, min.y, min.z);                
            var p1 = new Vector3(max.x, min.y, min.z);
            var p2 = new Vector3(max.x, min.y, max.z);
            var p3 = new Vector3(min.x, min.y, max.z);
            
            var p4 = new Vector3(min.x, max.y, min.z);
            var p5 = new Vector3(max.x, max.y, min.z);
            var p6 = new Vector3(max.x, max.y, max.z);
            var p7 = new Vector3(min.x, max.y, max.z);

            float lineWidth =  lineWidthPixels * PixelScaleUtility.DpiScaleFactor;
            
            lines.AddLine(p0, p1, lineWidth, color, color);
            lines.AddLine(p1, p2, lineWidth, color, color);
            lines.AddLine(p2, p3, lineWidth, color, color);
            lines.AddLine(p3, p0, lineWidth, color, color);
            
            lines.AddLine(p4, p5, lineWidth, color, color);
            lines.AddLine(p5, p6, lineWidth, color, color);
            lines.AddLine(p6, p7, lineWidth, color, color);
            lines.AddLine(p7, p4, lineWidth, color, color);
            
            lines.AddLine(p0, p4, lineWidth, color, color);
            lines.AddLine(p1, p5, lineWidth, color, color);
            lines.AddLine(p2, p6, lineWidth, color, color);
            lines.AddLine(p3, p7, lineWidth, color, color);
        }

        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
    }
}