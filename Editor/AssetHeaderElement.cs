#nullable enable
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public sealed class AssetHeaderElement : VisualElement {
        const string NAMEOF_SCRIPT = "m_Script";
        const string LABEL_SCRIPT = "Script";

        const string LABEL_ASSET = "Asset";

        public AssetHeaderElement(SerializedObject serialized) {
            style.marginBottom = EditorGUIUtility.singleLineHeight;

            Add(CreateScriptField(serialized));
            Add(CreateAssetField(serialized));
        }

        VisualElement CreateScriptField(SerializedObject serialized) {
            if (serialized.FindProperty(NAMEOF_SCRIPT) is { objectReferenceValue: { } script }) {
                return new ObjectField(LABEL_SCRIPT) {
                    value = script,
                    enabledSelf = false
                };
            }

            VisualElement root = new();
            foreach (string name in serialized.targetObjects.Select(asset => asset.GetType().FullName).Distinct()) {
                root.Add(new TextField(LABEL_SCRIPT) {
                    value = name,
                    enabledSelf = false
                });
            }

            return root;
        }

        VisualElement CreateAssetField(SerializedObject serialized) {
            VisualElement root = new();

            foreach (var asset in serialized.targetObjects) {
                root.Add(new ObjectField(LABEL_ASSET) {
                    value = asset,
                    enabledSelf = false,
                });
            }

            return root;
        }
    }
}
