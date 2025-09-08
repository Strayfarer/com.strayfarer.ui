using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    sealed class MethodReferenceField : VisualElement {
        readonly SerializedProperty property;
        readonly Type uxmlType;

        readonly PopupField<string> popup = new() {
            choices = new()
        };

        public MethodReferenceField(SerializedProperty property) : base() {
            this.property = property;

            AddToClassList("method-reference-field");

            uxmlType = property.GetUxmlType();

            popup.label = property.displayName;
            popup.RegisterValueChangedCallback(OnPopupChanged);
            popup.AddToClassList("unity-base-field__aligned");

            popup.labelElement.AddToClassList("unity-property-field__label");
            popup.ElementAt(1).AddToClassList("unity-property-field__input");

            Add(popup);
        }

        void OnPopupChanged(ChangeEvent<string> evt) {
            property.stringValue = evt.newValue;
            property.serializedObject.ApplyModifiedProperties();
        }

        protected override void HandleEventBubbleUp(EventBase evt) {
            base.HandleEventBubbleUp(evt);

            UpdatePopup();
        }

        void UpdatePopup() {
            popup.choices.Clear();
            if (property.TryGetUxmlDataSourceType(out var type)) {
                popup.choices.AddRange(type.GetMethods().Where(IsValidMethod).Select(m => m.Name).Distinct().OrderBy(n => n));
            }

            popup.value = property.stringValue;
        }

        bool IsValidMethod(MethodInfo method) {
            if (method.ReturnType != typeof(void)) {
                return false;
            }

            var parameters = method.GetParameters();
            return parameters.Length switch {
                0 => true,
                1 when parameters[0].ParameterType.IsAssignableFrom(uxmlType) => true,
                _ => false,
            };
        }
    }
}
