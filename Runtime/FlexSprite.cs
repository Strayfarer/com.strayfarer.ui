#nullable enable
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    public partial class FlexSprite : Image {
        [Header(nameof(FlexSprite))]
        [UxmlAttribute]
        [CreateProperty]
#nullable disable
        public new Sprite sprite {
#nullable enable
            get => base.sprite;
            set => base.sprite = value;
        }

        public FlexSprite() {
            AddToClassList("flexSprite");
            AddToClassList("flex-image");
            AddToClassList("flex-image--sprite");
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode) {
            if (!sprite) {
                return new(float.NaN, float.NaN);
            }

            if (sprite is not { rect: { height: > 0, width: > 0 } }) {
                return new(float.NaN, float.NaN);
            }

            float scaledWidth = desiredHeight * sprite.rect.width / sprite.rect.height;
            float scaledHeight = desiredWidth * sprite.rect.height / sprite.rect.width;

            return (widthMode, heightMode) switch {
                (MeasureMode.AtMost, MeasureMode.AtMost) => scaledWidth <= desiredWidth
                    ? new(scaledWidth, desiredHeight)
                    : new(desiredWidth, scaledHeight),
                (MeasureMode.Exactly, MeasureMode.Exactly) => new(desiredWidth, desiredHeight),
                (MeasureMode.Exactly, _) => new(desiredWidth, scaledHeight),
                (_, MeasureMode.Exactly) => new(scaledWidth, desiredHeight),
                _ => new(float.NaN, float.NaN)
            };
        }
    }
}