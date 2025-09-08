using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    static class Extensions {
        public static void AddBoxStyle(this VisualElement element, Color? bg = null) {
            element.style.borderBottomWidth = 1;
            element.style.borderTopWidth = 1;
            element.style.borderLeftWidth = 1;
            element.style.borderRightWidth = 1;
            element.style.marginBottom = 10;
            element.style.paddingTop = 4;
            element.style.paddingBottom = 4;
            element.style.paddingLeft = 6;
            element.style.paddingRight = 6;
            element.style.backgroundColor = bg ?? new Color(0.95f, 0.95f, 0.95f);
        }

        public static void SetFont(this VisualElement element, string fontName) {
            element.style.unityFontDefinition = new(Font.CreateDynamicFontFromOSFont(fontName, Mathf.RoundToInt(element.resolvedStyle.fontSize)));
        }
    }
}
