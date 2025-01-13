using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Viewer.Runtime.UI
{
    [RequireComponent(typeof(RectTransform))] 
    public sealed class Tooltip : MonoBehaviour 
    { 
        private static Tooltip tooltip;
        private static string currentText; 
        private static GameObject currentHoverObject;

        public static void ShowTooltip(string text, RectTransform hoverObject, Vector3? positionOverride) 
        {
            if (tooltip == null)
            {
                Debug.LogWarning("No tooltip object exists in the scene");
                return;
            }

            if (hoverObject.gameObject == currentHoverObject
                && text != null && currentText != null 
                && text.Length == currentText.Length 
                && text == currentText)
                return;

            tooltip.Hide();
            
            if (text == null) return;

            tooltip.Show(text, hoverObject, positionOverride);
        }

        private string text;
        private RectTransform rectTransform;
        private RectTransform parentRectTransform;
        private GameObject hoverObject;
        private Canvas canvas;
        private RectTransform canvasTransform;
        private TMP_Text tmpText;


        [SerializeField] private Vector2 offset = new (20, 0);

        private void Show(string text, RectTransform hover, Vector3? positionOverride)
        {
            this.text = text;
            this.hoverObject = hover.gameObject;
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 canvasPosition;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform,
                mousePosition,
                canvas.worldCamera,
                out canvasPosition
            );
            
            if (positionOverride != null)
            {
                canvasPosition = (Vector2)(hover.position + positionOverride);
            }

            rectTransform.localPosition = canvasPosition + offset;
            tmpText.text = text; 
            
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (tooltip != null) throw new Exception("Duplicate Tooltip object detected!");

            currentText = null; 
            tooltip = this;
        }

        void Start()
        {
            rectTransform = transform.GetComponent<RectTransform>();
            parentRectTransform = transform.parent.GetComponent<RectTransform>(); 
            this.canvas = GetComponentInParent<Canvas>();
            this.canvasTransform = canvas.GetComponent<RectTransform>();
            this.tmpText = GetComponentInChildren<TMP_Text>();
            
            Hide();
        }

        void Update() 
        { 
            bool hover = false;
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                
                EventSystem.current.RaycastAll(pointerData, results);
        
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject != hoverObject) continue; 
                    
                    hover = true;
                    
                    break;
                }
            }

            gameObject.SetActive(hover || EventSystem.current.currentSelectedGameObject == hoverObject);
        }
    }
}