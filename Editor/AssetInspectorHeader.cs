#nullable enable
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public sealed class AssetInspectorHeader : VisualElement {
        const string NAMEOF_SCRIPT = "m_Script";
        const string LABEL_SCRIPT = "Script";

        const string LABEL_ASSET = "Asset";

        public AssetInspectorHeader(SerializedObject serialized) {
            this.AddKebabToClassList(nameof(AssetInspectorHeader));
            this.AddEditorStyleSheet();

            Add(CreateScriptField(serialized));
            Add(CreateAssetField(serialized));
        }

        static VisualElement CreateScriptField(SerializedObject serialized) {
            if (serialized.FindProperty(NAMEOF_SCRIPT) is { objectReferenceValue: { } script }) {
                return SetDisabled(new ObjectField(LABEL_SCRIPT) {
                    value = script
                });
            }

            VisualElement root = new();
            foreach (string name in serialized.targetObjects.Select(asset => asset.GetType().FullName).Distinct()) {
                root.Add(SetDisabled(new TextField(LABEL_SCRIPT) {
                    value = name
                }));
            }

            return root;
        }

        static VisualElement CreateAssetField(SerializedObject serialized) {
            VisualElement root = new();

            foreach (var asset in serialized.targetObjects) {
                root.Add(SetDisabled(new ObjectField(LABEL_ASSET) {
                    value = asset,
                }));
            }

            return root;
        }

        static VisualElement SetDisabled(VisualElement element) {
            element.SetEnabled(false);
            return element;
        }
    }
}
