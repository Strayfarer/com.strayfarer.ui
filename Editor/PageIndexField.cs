using System;
using Strayfarer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public class PageIndexField : VisualElement {
        // TODO: should be readonly
        SerializedProperty property;

        // TODO: should be removed
        readonly Type uxmlType;

        // TODO: should be PopupField<string> and use PopupField.index
        readonly PopupField<(int index, string name)> popup = new() {
            choices = new()
        };

        public PageIndexField(SerializedProperty property) : base() {
            this.property = property;
            uxmlType = property.GetUxmlType();

            // TODO: add "page-index-field" uss class

            popup.label = property.displayName;
            popup.RegisterValueChangedCallback(OnPopupChanged);
            popup.AddToClassList("unity-base-field__aligned");

            popup.labelElement.AddToClassList("unity-property-field__label");
            popup.ElementAt(1).AddToClassList("unity-property-field__input");

            popup.formatSelectedValueCallback = FormatItem;
            popup.formatListItemCallback = FormatItem;
            Add(popup);
        }

        void OnPopupChanged(ChangeEvent<(int index, string name)> evt) {
            property.intValue = evt.newValue.index;
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
                // TODO: use UxmlTag if name is unset
                child.TryGetUxmlAttribute("name", out string name);
                popup.choices.Add((index, name));
                index++;
            }

            popup.value = popup.choices.Find(c => c.index == property.intValue);
        }
        string FormatItem((int index, string name) popup) {
            return popup.index + ": " + popup.name;
        }
    }
}