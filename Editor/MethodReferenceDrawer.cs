using Strayfarer;
using UnityEditor;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    [CustomPropertyDrawer(typeof(MethodReferenceAttribute))]
    sealed class MethodReferenceDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            return new MethodReferenceField(property);
        }
    }
}