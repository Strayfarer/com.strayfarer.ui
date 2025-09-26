using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    [MovedFrom(true, "Retropair.UXML", "Retropair")]
    public sealed partial class BindableButton : Button {
        [Header(nameof(BindableButton))]
        [UxmlAttribute]
        [MethodReference]
        new string onClick;

        public BindableButton() {
            AddToClassList("bindable-button");

            clicked += OnClick;
        }

        void OnClick() {
            if (string.IsNullOrEmpty(onClick)) {
                Debug.LogWarning($"No onClick defined for {this}");
                return;
            }

            if (GetHierarchicalDataSourceContext() is not { dataSource: object source }) {
                Debug.LogWarning($"Failed to find dataSource for {this}");
                return;
            }

            if (source.GetType().GetMethod(onClick, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) is not { } method) {
                Debug.LogWarning($"Failed to find method '{onClick}' on dataSource '{source}' for {this}");
                return;
            }

            object[] parameters = method.GetParameters().Length == 0
                ? Array.Empty<object>()
                : new object[] { this };

            method.Invoke(source, parameters);
        }
    }
}
