using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    sealed class MethodReferenceField : DropdownField {
        readonly SerializedProperty property;
        readonly Type uxmlType;

        public MethodReferenceField(SerializedProperty property) : base() {
            this.property = property;

            choices = new();
            label = property.displayName;
            uxmlType = property.GetUxmlType();

            this.BindProperty(property);
            this.AddFieldAlignmentToClassList();
            this.AddKebabToClassList(nameof(MethodReferenceField));
        }

        protected override void HandleEventBubbleUp(EventBase evt) {
            base.HandleEventBubbleUp(evt);

            UpdateDataSourceType();
        }

        void UpdateDataSourceType() {
            targetDataSourceType = property.TryGetUxmlDataSourceType(out var type)
                ? type
                : null;
        }

        Type _targetDataSourceType;
        Type targetDataSourceType {
            get => _targetDataSourceType;
            set {
                if (_targetDataSourceType != value) {
                    _targetDataSourceType = value;

                    choices.Clear();
                    choices.AddRange(targetDataSourceMethods);
                }
            }
        }

        IEnumerable<string> targetDataSourceMethods => targetDataSourceType is null
            ? Enumerable.Empty<string>()
            : targetDataSourceType
                .GetMethods()
                .Where(IsValidMethod)
                .Select(m => m.Name).Distinct()
                .OrderBy(n => n);

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
