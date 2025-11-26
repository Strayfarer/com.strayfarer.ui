using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Strayfarer.UI {
    [UxmlElement]
    public sealed partial class GlowingBorder : VisualElement {
        [Header(nameof(PageButton))]

        Color _innerColor = Color.gray;
        Color _glowColor = Color.white;
        Color clearColor = Color.clear;
        float _outerBorderWidthPercent;
        float _borderWidth;
        float glowWidth;

        static readonly CustomStyleProperty<Color> k_innerColorProperty = new CustomStyleProperty<Color>("--inner-border-color");
        static readonly CustomStyleProperty<Color> k_glowColorProperty = new CustomStyleProperty<Color>("--glow-color");
        static readonly CustomStyleProperty<float> k_outerBorderWidthPercentProperty = new CustomStyleProperty<float>("--outer-border-width-percent");
        static readonly CustomStyleProperty<float> k_glowBorderWidthProperty = new CustomStyleProperty<float>("--glow-border-width");

        public GlowingBorder() {
            AddToClassList("GlowingBorder");
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
        }

        void OnCustomStyleResolved(CustomStyleResolvedEvent evt) {
            if (evt.customStyle.TryGetValue(k_innerColorProperty, out var innerBorderColor)) {
                _innerColor = innerBorderColor;
            }
            if (evt.customStyle.TryGetValue(k_glowColorProperty, out var glowBorderColor)) {
                _glowColor = glowBorderColor;
            }
            if (evt.customStyle.TryGetValue(k_outerBorderWidthPercentProperty, out float outerBorderWidthPercent)) {
                _outerBorderWidthPercent = outerBorderWidthPercent;
            }
            if (evt.customStyle.TryGetValue(k_glowBorderWidthProperty, out float glowBorderWidth)) {
                glowWidth = glowBorderWidth;
            }
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

            CreateInnerBorder(verts, indices, r);
            CreateGlow(verts, indices, r);
        }

        void CreateInnerBorder(List<Vertex> verts, List<ushort> indices, Rect r) {
            // ----- Inner Border ----
            float outerWidth = _outerBorderWidthPercent / 100 * _borderWidth;
            float widthDelta = _borderWidth - outerWidth;
            // Top left (0)
            verts.Add(new Vertex {
                position = new Vector3(outerWidth, outerWidth, Vertex.nearZ),
                tint = _innerColor,
            });
            // Top right (1)
            verts.Add(new Vertex {
                position = new Vector3(r.width - outerWidth, outerWidth, Vertex.nearZ),
                tint = _innerColor,
            });
            // Bottom right (2)
            verts.Add(new Vertex {
                position = new Vector3(r.width - outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = _innerColor,
            });
            //Bottom left (3)
            verts.Add(new Vertex {
                position = new Vector3(outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = _innerColor,
            });
            // Top left (inner) (4)
            verts.Add(new Vertex {
                position = new Vector3(widthDelta, widthDelta, Vertex.nearZ),
                tint = _innerColor,
            });
            // Top right (inner) (5)
            verts.Add(new Vertex {
                position = new Vector3(r.width - widthDelta, widthDelta, Vertex.nearZ),
                tint = _innerColor,
            });
            // Bottom right (inner) (6)
            verts.Add(new Vertex {
                position = new Vector3(r.width - widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = _innerColor,
            });
            //Bottom left (inner) (7)
            verts.Add(new Vertex {
                position = new Vector3(widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = _innerColor,
            });

            // Number of corners: 4
            for (int i = 0; i < 4; i++) {
                int v0 = i;
                int v1 = v0 + 4;

                indices.Add((ushort)v0);

                if (i < 3) {
                    indices.Add((ushort)(v0 + 1));
                    indices.Add((ushort)(v1 + 1));
                    indices.Add((ushort)(v1 + 1));
                } else {
                    indices.Add((ushort)(v1 + 1 - 8));
                    indices.Add((ushort)(v0 + 1));
                    indices.Add((ushort)(v0 + 1));
                }
                indices.Add((ushort)v1);
                indices.Add((ushort)v0);
            }
        }

        void CreateGlow(List<Vertex> verts, List<ushort> indices, Rect r) {
            // ----- Glowing Border -----

            // Outer Glow Vertices
            // Top left (8)
            verts.Add(new Vertex {
                position = new Vector3(-glowWidth, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            // Top right (9)
            verts.Add(new Vertex {
                position = new Vector3(r.width + glowWidth, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            // Bottom right (10)
            verts.Add(new Vertex {
                position = new Vector3(r.width + glowWidth, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            //Bottom left (11)
            verts.Add(new Vertex {
                position = new Vector3(-glowWidth, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            });

            // Base Box Vertices
            // Top left (12)
            verts.Add(new Vertex {
                position = new Vector3(0, 0, Vertex.nearZ),
                tint = _glowColor,
            });
            // Top right (13)
            verts.Add(new Vertex {
                position = new Vector3(r.width, 0, Vertex.nearZ),
                tint = _glowColor,
            });
            // Bottom right (14)
            verts.Add(new Vertex {
                position = new Vector3(r.width, r.height, Vertex.nearZ),
                tint = _glowColor,
            });
            //Bottom left (15)
            verts.Add(new Vertex {
                position = new Vector3(0, r.height, Vertex.nearZ),
                tint = _glowColor,
            });

            // Outer Glow indices
            int startIndex = 8;
            // Number of corners: 4
            for (int i = 0; i < 4; i++) {
                int v0 = startIndex + i;
                int v1 = v0 + 4;

                indices.Add((ushort)v0);

                if (i < 3) {
                    indices.Add((ushort)(v0 + 1));
                    indices.Add((ushort)(v1 + 1)); // modulo with highest vertex nr + 1
                    indices.Add((ushort)(v1 + 1));
                } else {
                    indices.Add((ushort)(v1 + 1 - 8));
                    indices.Add((ushort)(v0 + 1));
                    indices.Add((ushort)(v0 + 1));
                }
                indices.Add((ushort)v1);
                indices.Add((ushort)v0);
            }

            // Inner Border Box
            // Top left (16)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth, _borderWidth, Vertex.nearZ),
                tint = _glowColor,
            });
            // Top right (17)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth, _borderWidth, Vertex.nearZ),
                tint = _glowColor,
            });
            // Bottom right (18)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth, r.height - _borderWidth, Vertex.nearZ),
                tint = _glowColor,
            });
            //Bottom left (19)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth, r.height - _borderWidth, Vertex.nearZ),
                tint = _glowColor,
            });

            // Inner Glow Vertices
            // Top left (20)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth + glowWidth, _borderWidth + glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            // Top right (21)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth - glowWidth, _borderWidth + glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            // Bottom right (22)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth - glowWidth, r.height - _borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            });
            //Bottom left (23)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth + glowWidth, r.height - _borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            });

            // Inner Glow Indices
            // Top
            indices.Add(16);
            indices.Add(17);
            indices.Add(21);
            indices.Add(21);
            indices.Add(20);
            indices.Add(16);
            // Right
            indices.Add(17);
            indices.Add(18);
            indices.Add(22);
            indices.Add(22);
            indices.Add(21);
            indices.Add(17);
            // Bottom
            indices.Add(18);
            indices.Add(19);
            indices.Add(23);
            indices.Add(23);
            indices.Add(22);
            indices.Add(18);
            //Left
            indices.Add(19);
            indices.Add(16);
            indices.Add(20);
            indices.Add(20);
            indices.Add(23);
            indices.Add(19);
        }
    }
}