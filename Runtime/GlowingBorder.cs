using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Strayfarer.UI {
    [UxmlElement]
    public sealed partial class GlowingBorder : VisualElement {
        [Header(nameof(PageButton))]

        [UxmlAttribute]
        internal Color innerColor {
            get => _innerColor ?? Color.clear;
            set { _innerColor = value; MarkDirtyRepaint(); }
        }
        Color? _innerColor = Color.gray;
        float _borderWidth;

        [UxmlAttribute, Range(0, 50)]
        internal float outerWidthPercent {
            get => _outerWidthPercent;
            set { _outerWidthPercent = value; MarkDirtyRepaint(); }
        }
        float _outerWidthPercent;

        public GlowingBorder() {
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
        }

        //private static CustomStyleProperty<Texture2D> s_ImageProperty = new CustomStyleProperty<Texture2D>("--unity-image");
        void OnCustomStyleResolved(CustomStyleResolvedEvent evt) {
            //_borderWidth = resolvedStyle.borderBottomWidth;
            //MarkDirtyRepaint();
    }

        void OnGenerateVisualContent(MeshGenerationContext context) {
            _borderWidth = resolvedStyle.borderBottomWidth;
            CalculateGlowingFrame(out var verts, out var indices);
            var mwd = context.Allocate(verts.Count, indices.Count);
            mwd.SetAllVertices(verts.ToArray());
            mwd.SetAllIndices(indices.ToArray());

            verts.Clear();
            indices.Clear();
        }

        void CalculateGlowingFrame(out List<Vertex> verts, out List<ushort> indices) {
            verts = new List<Vertex>(4);
            indices = new List<ushort>(28);
            var r = localBound;

            // ----- Inner Border ----
            float outerWidth = _outerWidthPercent / 100 * _borderWidth;
            float widthDelta = _borderWidth - outerWidth;
            // Top left
            verts.Add(new Vertex {
                position = new Vector3(outerWidth, outerWidth, Vertex.nearZ),
                tint = innerColor,
            });
            // Top right
            verts.Add(new Vertex {
                position = new Vector3(r.width - outerWidth, outerWidth, Vertex.nearZ),
                tint = innerColor,
            });
            // Bottom right
            verts.Add(new Vertex {
                position = new Vector3(r.width - outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = innerColor,
            });
            //Bottom left
            verts.Add(new Vertex {
                position = new Vector3(outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = innerColor,
            });
            // Top left (inner)
            verts.Add(new Vertex {
                position = new Vector3(widthDelta, widthDelta, Vertex.nearZ),
                tint = innerColor,
            });
            // Top right (inner)
            verts.Add(new Vertex {
                position = new Vector3(r.width - widthDelta, widthDelta, Vertex.nearZ),
                tint = innerColor,
            });
            // Bottom right (inner)
            verts.Add(new Vertex {
                position = new Vector3(r.width - widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = innerColor,
            });
            //Bottom left (inner)
            verts.Add(new Vertex {
                position = new Vector3(widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = innerColor,
            });

            // Inner border indices
            // Top
            indices.Add(0);
            indices.Add(1);
            indices.Add(5);
            indices.Add(5);
            indices.Add(4);
            indices.Add(0);
            // Right
            indices.Add(5);
            indices.Add(1);
            indices.Add(2);
            indices.Add(2);
            indices.Add(6);
            indices.Add(5);
            // Bottom
            indices.Add(6);
            indices.Add(2);
            indices.Add(3);
            indices.Add(3);
            indices.Add(7);
            indices.Add(6);
            //Left
            indices.Add(7);
            indices.Add(3);
            indices.Add(0);
            indices.Add(0);
            indices.Add(4);
            indices.Add(7);
        }
    }
}