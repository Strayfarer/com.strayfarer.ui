using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    static class VisualTreeAssetExtensions {
        static readonly Regex basePathExpression = new(@"m_VisualElementAssets\.Array\.data\[\d+\]", RegexOptions.Compiled);

        static SerializedProperty GetUxmlElementProperty(this SerializedProperty property) {
            var match = basePathExpression.Match(property.propertyPath);
            if (!match.Success) {
                throw new Exception($"Failed to match '{basePathExpression}' to path '{property.propertyPath}'");
            }

            if (match.Value == property.propertyPath) {
                return property;
            }

            if (property.serializedObject.FindProperty(match.Value) is not { } elementProperty) {
                throw new Exception($"Failed to find '{match.Value}' in asset '{property.serializedObject.targetObject}'");
            }

            return elementProperty;
        }

        static SerializedProperty FindPropertyRelativeOrThrow(this SerializedProperty property, string relativePropertyPath) {
            return property.FindPropertyRelative(relativePropertyPath) ?? throw new Exception($"Failed to find '{relativePropertyPath}' in property '{property.propertyPath}' of asset '{property.serializedObject.targetObject}'");
        }

        static bool TryGetUxmlParentProperty(this SerializedProperty property, out SerializedProperty parentProperty) {
            int id = property.GetUxmlElementProperty().FindPropertyRelativeOrThrow("m_ParentId").intValue;

            if (id == 0) {
                parentProperty = default;
                return false;
            }

            var elementsProperty = property.serializedObject.FindProperty("m_VisualElementAssets");

            for (int i = 0; i < elementsProperty.arraySize; i++) {
                var elementProperty = elementsProperty.GetArrayElementAtIndex(i);
                if (elementProperty.FindPropertyRelativeOrThrow("m_Id").intValue == id) {
                    parentProperty = elementProperty;
                    return true;
                }
            }

            parentProperty = default;
            return false;
        }

        internal static IEnumerable<SerializedProperty> GetUxmlChildElements(this SerializedProperty property) {
            int id = property.GetUxmlElementProperty().FindPropertyRelativeOrThrow("m_Id").intValue;

            var elementsProperty = property.serializedObject.FindProperty("m_VisualElementAssets");

            for (int i = 0; i < elementsProperty.arraySize; i++) {
                var elementProperty = elementsProperty.GetArrayElementAtIndex(i);
                if (elementProperty.FindPropertyRelativeOrThrow("m_ParentId").intValue == id) {
                    yield return elementProperty;
                }
            }
        }

        internal static string GetUxmlTag(this SerializedProperty property) {
            var tagProperty = property.GetUxmlElementProperty().FindPropertyRelativeOrThrow("m_FullTypeName");

            return tagProperty.stringValue;
        }

        internal static Type GetUxmlType(this SerializedProperty property) {
            string tag = property.GetUxmlTag();
            if (tag == typeof(VisualElement).FullName) {
                return typeof(VisualElement);
            }

            return TypeCache
                .GetTypesDerivedFrom<VisualElement>()
                .FirstOrDefault(t => t.FullName == tag);
        }

        internal static bool TryGetUxmlAncestorOrSelf<T>(this SerializedProperty property, out SerializedProperty ancestorProperty) where T : VisualElement {
            do {
                if (property.GetUxmlType() == typeof(T)) {
                    ancestorProperty = property;
                    return true;
                }
            } while (property.TryGetUxmlParentProperty(out property));

            ancestorProperty = default;
            return false;
        }

        internal static bool TryGetUxmlAttribute(this SerializedProperty property, string attribute, out string value) {
            var propertiesProperty = property.GetUxmlElementProperty().FindPropertyRelativeOrThrow("m_Properties");

            for (int i = 0; i < propertiesProperty.arraySize; i += 2) {
                if (propertiesProperty.GetArrayElementAtIndex(i).stringValue == attribute) {
                    value = propertiesProperty.GetArrayElementAtIndex(i + 1).stringValue;
                    if (!string.IsNullOrEmpty(value)) {
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        internal static bool TryGetUxmlAttributeInParent(this SerializedProperty property, string attribute, out string value) {
            do {
                if (property.TryGetUxmlAttribute(attribute, out value)) {
                    return true;
                }
            } while (property.TryGetUxmlParentProperty(out property));

            value = default;
            return false;
        }

        internal static bool TryGetUxmlDataSourceType(this SerializedProperty property, out Type type) {
            do {
                if (property.TryGetUxmlAttribute("data-source-type", out string typeString)) {
                    type = Type.GetType(typeString);
                    return true;
                }

                if (property.TryGetUxmlAttribute("data-source", out string assetUrl) && ToolkitUtils.LoadAsset<ScriptableObject>(assetUrl) is { } asset) {
                    type = asset.GetType();
                    return true;
                }
            } while (property.TryGetUxmlParentProperty(out property));

            type = default;
            return false;
        }

        internal static string GetUxmlDisplayName(this SerializedProperty property) {
            if (!property.TryGetUxmlAttribute("name", out string name)) {
                string[] substrings = property.GetUxmlTag().Split('.');
                name = substrings[^1];
            } else {
                name = "#" + name;
            }

            return name;
        }
    }
}
