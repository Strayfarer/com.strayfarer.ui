#nullable enable
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    [UxmlElement]
    public sealed partial class BorderedBox : VisualElement {
        [Header(nameof(BorderedBox))]
        [UxmlAttribute]
        public Color borderColor {
            get => box.style.borderTopColor.value;
            set {
                box.style.borderTopColor = value;
                box.style.borderRightColor = value;
                box.style.borderBottomColor = value;
                box.style.borderLeftColor = value;
            }
        }

        [UxmlAttribute]
        public int borderWidth {
            get => Mathf.RoundToInt(box.style.borderTopWidth.value);
            set {
                box.style.borderTopWidth = value;
                box.style.borderRightWidth = value;
                box.style.borderBottomWidth = value;
                box.style.borderLeftWidth = value;
            }
        }

        [UxmlAttribute]
        public int borderRadius {
            get => Mathf.RoundToInt(box.style.borderTopRightRadius.value.value);
            set {
                box.style.borderTopRightRadius = value;
                box.style.borderBottomRightRadius = value;
                box.style.borderBottomLeftRadius = value;
                box.style.borderTopLeftRadius = value;
            }
        }

        readonly GroupBox box = new();

        public BorderedBox() : this(default) {
        }

        public BorderedBox(Color? borderColor = default, int borderWidth = 1, int borderRadius = 3, params VisualElement[] children) {
            this.borderColor = borderColor ?? Color.gray;
            this.borderWidth = borderWidth;
            this.borderRadius = borderRadius;

            Add(box);

            foreach (var child in children) {
                AddInsideBox(child);
            }
        }

        public void AddInsideBox(VisualElement child) {
            box.contentContainer.Add(child);
        }
    }
}
