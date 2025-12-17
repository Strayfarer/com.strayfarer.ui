using Strayfarer.UI.Editor;
using UnityEditor;
using UnityEngine.UIElements;
using UEditor = UnityEditor.Editor;

namespace Strayfarer.UI {
    [CustomEditor(typeof(TestAsset))]
    [CanEditMultipleObjects]
    public sealed class TestAssetEditor : UEditor {
        public override VisualElement CreateInspectorGUI() {
            return new AssetHeader(serializedObject);
        }
    }
}