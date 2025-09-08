using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public class PageIndexField : VisualElement {
        readonly SerializedProperty property;

        readonly PopupField<string> popup = new() {
            choices = new()
        };

        public PageIndexField(SerializedProperty property) : base() {
            this.property = property;

            popup.label = property.displayName;
            popup.RegisterValueChangedCallback(OnPopupChanged);
            popup.AddToClassList("unity-base-field__aligned");
            popup.AddToClassList("page-index-field");

            popup.labelElement.AddToClassList("unity-property-field__label");
            popup.ElementAt(1).AddToClassList("unity-property-field__input");

            popup.formatSelectedValueCallback = s => s;
            popup.formatListItemCallback = s => s;
            Add(popup);
        }

        void OnPopupChanged(ChangeEvent<string> evt) {
            property.intValue = popup.index;
            property.serializedObject.ApplyModifiedProperties();
        }
        protected override void HandleEventBubbleUp(EventBase evt) {
            base.HandleEventBubbleUp(evt);

            UpdatePopup();
        }

        void UpdatePopup() {
            popup.choices.Clear();
            int index = 0;
            if (!property.TryGetUxmlAncestorOrSelf<PageView>(out var currentPageView)) {
                Debug.LogError("No PageView found! Page Index Attribute cannot be used!");
            }

            foreach (var child in currentPageView.GetUxmlChildElements()) {
                popup.choices.Add(index + ": " + child.GetUxmlDisplayName().Replace("#", "\u200B#"));
                index++;
            }

            popup.index = property.intValue;
        }
        string FormatItem((int index, string name) popup) {
            return popup.index + ": " + popup.name;
        }
    }
}