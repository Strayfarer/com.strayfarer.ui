using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Strayfarer.UI {
    [UxmlElement]
    public sealed partial class GlowingBorder : VisualElement {
        [Header(nameof(PageButton))]

        Color innerBorderColor = Color.gray;
        Color glowColor = Color.white;
        Color clearColor = Color.clear;
        float outerBorderWidthPercent;
        float borderWidth;
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
                this.innerBorderColor = innerBorderColor;
            }
            if (evt.customStyle.TryGetValue(k_glowColorProperty, out var glowBorderColor)) {
                glowColor = glowBorderColor;
            }
            if (evt.customStyle.TryGetValue(k_outerBorderWidthPercentProperty, out float outerBorderWidthPercent)) {
                this.outerBorderWidthPercent = outerBorderWidthPercent;
            }
            if (evt.customStyle.TryGetValue(k_glowBorderWidthProperty, out float glowBorderWidth)) {
                glowWidth = glowBorderWidth;
            }
        }

        void OnGenerateVisualContent(MeshGenerationContext context) {
            borderWidth = resolvedStyle.borderBottomWidth;
            CalculateGlowingFrame(out var verts, out ushort[] indices);
            var mwd = context.Allocate(verts.Length, indices.Length);
            mwd.SetAllVertices(verts);
            mwd.SetAllIndices(indices);

            Array.Clear(verts, 0, verts.Length);
            Array.Clear(indices, 0, indices.Length);
        }

        void CalculateGlowingFrame(out Vertex[] verts, out ushort[] indices) {

            CreateInnerBorder(out var listOfInnerBorderTris, localBound);
            CreateGlow(out var listOfGlowTris, localBound);
            var listOfTris = listOfInnerBorderTris;
            listOfTris.AddRange(listOfGlowTris);

            var triUtil = new TriUtil(listOfTris);
            triUtil.CalculateVertsAndIndices(out verts, out indices);
        }

        void CreateInnerBorder(out List<Tri> tris, Rect r) {
            tris = new List<Tri>();
            // ----- Inner Border ----
            float outerWidth = outerBorderWidthPercent / 100 * borderWidth;
            float widthDelta = borderWidth - outerWidth;
            // Top left
            var topLeft = new Vertex {
                position = new Vector3(outerWidth, outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var topRight = new Vertex {
                position = new Vector3(r.width - outerWidth, outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var bottomRight = new Vertex {
                position = new Vector3(r.width - outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            //Bottom left
            var bottomLeft = new Vertex {
                position = new Vector3(outerWidth, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // Top left (inner)
            var topLeftInner = new Vertex {
                position = new Vector3(widthDelta, widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right (inner)
            var topRightInner = new Vertex {
                position = new Vector3(r.width - widthDelta, widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right (inner)
            var bottomRightInner = new Vertex {
                position = new Vector3(r.width - widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            //Bottom left (inner)
            var bottomLeftInner = new Vertex {
                position = new Vector3(widthDelta, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top
            tris.Add(new Tri(topLeft, topRight, topRightInner));
            tris.Add(new Tri(topRightInner, topLeftInner, topLeft));

            // Right
            tris.Add(new Tri(topRight, bottomRight, bottomRightInner));
            tris.Add(new Tri(bottomRightInner, topRightInner, topRight));

            // Bottom
            tris.Add(new Tri(bottomRight, bottomLeft, bottomLeftInner));
            tris.Add(new Tri(bottomLeftInner, bottomRightInner, bottomRight));

            // Left
            tris.Add(new Tri(bottomLeft, topLeft, topLeftInner));
            tris.Add(new Tri(topLeftInner, bottomLeftInner, bottomLeft));
        }

        void CreateGlow(out List<Tri> tris, Rect r) {
            // ----- Glowing Border -----

            tris = new List<Tri>();

            // Outer Glow Vertices
            // Top left
            var topLeft = new Vertex {
                position = new Vector3(-glowWidth, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var topRight = new Vertex {
                position = new Vector3(r.width + glowWidth, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var bottomRight = new Vertex {
                position = new Vector3(r.width + glowWidth, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            //Bottom left
            var bottomLeft = new Vertex {
                position = new Vector3(-glowWidth, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };

            // Base Box Vertices
            // Top left
            var topLeftInner = new Vertex {
                position = new Vector3(0, 0, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var topRightInner = new Vertex {
                position = new Vector3(r.width, 0, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var bottomRightInner = new Vertex {
                position = new Vector3(r.width, r.height, Vertex.nearZ),
                tint = glowColor,
            };
            //Bottom left
            var bottomLeftInner = new Vertex {
                position = new Vector3(0, r.height, Vertex.nearZ),
                tint = glowColor,
            };

            // Top
            tris.Add(new Tri(topLeft, topRight, topRightInner));
            tris.Add(new Tri(topRightInner, topLeftInner, topLeft));

            // Right
            tris.Add(new Tri(topRight, bottomRight, bottomRightInner));
            tris.Add(new Tri(bottomRightInner, topRightInner, topRight));

            // Bottom
            tris.Add(new Tri(bottomRight, bottomLeft, bottomLeftInner));
            tris.Add(new Tri(bottomLeftInner, bottomRightInner, bottomRight));

            // Left
            tris.Add(new Tri(bottomLeft, topLeft, topLeftInner));
            tris.Add(new Tri(topLeftInner, bottomLeftInner, bottomLeft));

            // Inner Border Box
            // Top left
            var innerGlowTopLeft = new Vertex {
                position = new Vector3(borderWidth, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var innerGlowTopRight = new Vertex {
                position = new Vector3(r.width - borderWidth, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var innerGlowBottomRight = new Vertex {
                position = new Vector3(r.width - borderWidth, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            //Bottom left
            var innerGlowBottomLeft = new Vertex {
                position = new Vector3(borderWidth, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };

            // Inner Glow Vertices
            // Top left
            var innerGlowTopLeftInner = new Vertex {
                position = new Vector3(borderWidth + glowWidth, borderWidth + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var innerGlowTopRightInner = new Vertex {
                position = new Vector3(r.width - borderWidth - glowWidth, borderWidth + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var innerGlowBottomRightInner = new Vertex {
                position = new Vector3(r.width - borderWidth - glowWidth, r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            //Bottom left
            var innerGlowBottomLeftInner = new Vertex {
                position = new Vector3(borderWidth + glowWidth, r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            };

            // Top
            tris.Add(new Tri(innerGlowTopLeft, innerGlowTopRight, innerGlowTopRightInner));
            tris.Add(new Tri(innerGlowTopRightInner, innerGlowTopLeftInner, innerGlowTopLeft));

            // Right
            tris.Add(new Tri(innerGlowTopRight, innerGlowBottomRight, innerGlowBottomRightInner));
            tris.Add(new Tri(innerGlowBottomRightInner, innerGlowTopRightInner, innerGlowTopRight));

            // Bottom
            tris.Add(new Tri(innerGlowBottomRight, innerGlowBottomLeft, innerGlowBottomLeftInner));
            tris.Add(new Tri(innerGlowBottomLeftInner, innerGlowBottomRightInner, innerGlowBottomRight));

            // Left
            tris.Add(new Tri(innerGlowBottomLeft, innerGlowTopLeft, innerGlowTopLeftInner));
            tris.Add(new Tri(innerGlowTopLeftInner, innerGlowBottomLeftInner, innerGlowBottomLeft));
        }
    }
}