using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    public partial class FlexVectorImage : Image {
        [Header(nameof(FlexVectorImage))]
        [UxmlAttribute]
        [CreateProperty]
        public new VectorImage vectorImage {
            get => base.vectorImage;
            set => base.vectorImage = value;
        }

        public FlexVectorImage() {
            AddToClassList($"flexVectorImage");
        }

        protected override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode) {
            if (!vectorImage) {
                return new(float.NaN, float.NaN);
            }

            if (vectorImage is not { height: > 0, width: > 0 }) {
                return new(float.NaN, float.NaN);
            }

            float scaledWidth = desiredHeight * vectorImage.width / vectorImage.height;
            float scaledHeight = desiredWidth * vectorImage.height / vectorImage.width;

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