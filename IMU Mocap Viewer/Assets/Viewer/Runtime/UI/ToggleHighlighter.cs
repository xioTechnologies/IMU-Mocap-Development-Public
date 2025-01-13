using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Viewer.Runtime.UI
{
    [RequireComponent(typeof(Toggle), typeof(RectTransform))]
    public sealed class ToggleHighlighter : MonoBehaviour
    {
        [Header("Color Settings")] [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private string tooltipText = "Tooltip";
        [SerializeField] private Vector3 tooltipOffset;
        private Toggle toggle;
        private Selectable selectable;
        private ColorBlock colors;
        private new RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            toggle = GetComponent<Toggle>();
            selectable = GetComponent<Selectable>();
            colors = selectable.colors;

            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            
            OnToggleValueChanged(toggle.isOn);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            colors.normalColor = isOn ? highlightColor : normalColor;
            colors.highlightedColor = isOn ? highlightColor : normalColor;
            colors.selectedColor = isOn ? highlightColor : normalColor;

            selectable.colors = colors;
        }

        private void OnDestroy()
        {
            if (toggle == null) return;

            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
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
                    var highlighter = result.gameObject.GetComponentInParent<ToggleHighlighter>();
                    
                    if (highlighter == null) continue;

                    if (highlighter.gameObject != gameObject) continue;
                    
                    hover = true;
                    
                    break;
                }
            }
            
            if (hover || EventSystem.current.currentSelectedGameObject == toggle.gameObject)
                Tooltip.ShowTooltip(tooltipText, rectTransform, tooltipOffset);
        }
    }
}