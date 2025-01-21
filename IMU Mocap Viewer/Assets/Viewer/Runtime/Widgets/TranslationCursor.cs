using UnityEngine;

namespace Viewer.Runtime.Widgets
{
    public sealed class TranslationCursor : MonoBehaviour
    {
        [SerializeField, Range(0, 500)] private float objectSize = 10f;
        
        [SerializeField] private bool updateRotation = false;
        
        private new Camera camera;
        
        void Start() => camera = Camera.main;

        private void Update()
        {
            transform.localScale = PixelScaleUtility.GetWorldScaleFromPixels(objectSize, transform.position) * PixelScaleUtility.DpiScaleFactor;
            
            if (updateRotation == false) return;

            Vector3 cameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up).normalized;
            Quaternion floorAlignment = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.rotation = floorAlignment * Quaternion.Euler(90, 0, 0);
        }

        public void Hide() => gameObject.SetActive(false);

        public void ShowAt(Vector3 getPoint)
        {
            transform.position = getPoint;
            gameObject.SetActive(true);
        }
    }
    
    // public sealed class TranslationCursor : MonoBehaviour
    // {
    //     [SerializeField, Range(0, 500)] private float objectSize = 10f;
    //     
    //     [SerializeField] private bool updateRotation = false;
    //
    //     private new Camera camera;
    //     
    //     void Start() => camera = Camera.main;
    //
    //     private void Update()
    //     {
    //         transform.localScale = PixelScaleUtility.GetWorldScaleFromPixels(objectSize, transform.position) * PixelScaleUtility.DpiScaleFactor;
    //
    //         if (updateRotation == false) return;
    //         
    //         Quaternion lookAt = Quaternion.LookRotation(camera.transform.position._x0z() - transform.position._x0z());
    //
    //         transform.rotation = lookAt * Quaternion.Euler(90, 0, 0);
    //     }
    //
    //     public void Hide() => gameObject.SetActive(false);
    //
    //     public void ShowAt(Vector3 getPoint)
    //     {
    //         transform.position = getPoint;
    //
    //         gameObject.SetActive(true);
    //     }
    // }
}