using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Viewer.Runtime.UI
{
    [RequireComponent(typeof(Toggle))]
    public sealed class ToggleHighlighter : MonoBehaviour
    {
        [Header("Color Settings")] [SerializeField]
        private Color normalColor = Color.white;

        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private TMP_Text text; 

        private Toggle toggle;
        private Selectable selectable;
        private ColorBlock colors;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            selectable = GetComponent<Selectable>();
            colors = selectable.colors;

            toggle.onValueChanged.AddListener(OnToggleValueChanged);
            
            OnToggleValueChanged(toggle.isOn);
        }
        
        void Update()
        {
            bool hover = false;
            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // if the pointer is over the game object, then show the tooltip text
                
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
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

            text.gameObject.SetActive(hover || EventSystem.current.currentSelectedGameObject == toggle.gameObject);
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
    }
}