using UnityEditor;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    [CustomPropertyDrawer(typeof(PageIndexAttribute))]
    sealed class PageIndexDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            return new PageIndexField(property);
        }
    }
}