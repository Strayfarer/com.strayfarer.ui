#nullable enable
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    public sealed partial class BorderedBox : GroupBox {
        [Header(nameof(BorderedBox))]
        [UxmlAttribute]
        public Color borderColor {
            get => style.borderTopColor.value;
            set {
                style.borderTopColor = value;
                style.borderRightColor = value;
                style.borderBottomColor = value;
                style.borderLeftColor = value;
            }
        }

        [UxmlAttribute]
        public int borderWidth {
            get => Mathf.RoundToInt(style.borderTopWidth.value);
            set {
                style.borderTopWidth = value;
                style.borderRightWidth = value;
                style.borderBottomWidth = value;
                style.borderLeftWidth = value;
            }
        }

        [UxmlAttribute]
        public int borderRadius {
            get => Mathf.RoundToInt(style.borderTopRightRadius.value.value);
            set {
                style.borderTopRightRadius = value;
                style.borderBottomRightRadius = value;
                style.borderBottomLeftRadius = value;
                style.borderTopLeftRadius = value;
            }
        }

        public BorderedBox() : this(default) {
        }

        public BorderedBox(Color? borderColor = default, int borderWidth = 1, int borderRadius = 3, params VisualElement[] children) {
            this.borderColor = borderColor ?? Color.gray;
            this.borderWidth = borderWidth;
            this.borderRadius = borderRadius;

            foreach (var child in children) {
                Add(child);
            }
        }
    }
}
