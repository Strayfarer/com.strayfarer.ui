using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public sealed class BorderedBox : VisualElement {
        static Color borderColor => Color.gray;

        public BorderedBox(params VisualElement[] children) {
            var box = new GroupBox();
            box.style.borderTopWidth = 1;
            box.style.borderRightWidth = 1;
            box.style.borderBottomWidth = 1;
            box.style.borderLeftWidth = 1;

            box.style.borderTopRightRadius = 3;
            box.style.borderBottomRightRadius = 3;
            box.style.borderBottomLeftRadius = 3;
            box.style.borderTopLeftRadius = 3;

            box.style.borderTopColor = borderColor;
            box.style.borderRightColor = borderColor;
            box.style.borderBottomColor = borderColor;
            box.style.borderLeftColor = borderColor;

            foreach (var child in children) {
                box.contentContainer.Add(child);
            }

            Add(box);
        }
    }
}
