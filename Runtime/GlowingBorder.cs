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
        float borderRadius;
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
            borderRadius = resolvedStyle.borderTopLeftRadius;
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
            float borderRadiusDelta = borderRadius - outerWidth;
            float maxOuter = Math.Max(borderRadius, outerWidth);
            float maxInner = Math.Max(borderRadius, widthDelta);

            // --- Top ---
            // Top left
            var tTopLeft = new Vertex {
                position = new Vector3(maxOuter, outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var tTopRight = new Vertex {
                position = new Vector3(r.width - maxOuter, outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var tBottomRight = new Vertex {
                position = new Vector3(r.width - maxInner, widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var tBottomLeft = new Vertex {
                position = new Vector3(maxInner, widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Bottom ---
            // Top left
            var bTopLeft = new Vertex {
                position = new Vector3(maxInner, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var bTopRight = new Vertex {
                position = new Vector3(r.width - maxInner, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var bBottomRight = new Vertex {
                position = new Vector3(r.width - maxOuter, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            //Bottom left
            var bBottomLeft = new Vertex {
                position = new Vector3(maxOuter, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Left ---
            // Top left
            var lTopLeft = new Vertex {
                position = new Vector3(outerWidth, maxOuter, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var lTopRight = new Vertex {
                position = new Vector3(widthDelta, maxInner, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var lBottomRight = new Vertex {
                position = new Vector3(widthDelta, r.height - maxInner, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var lBottomLeft = new Vertex {
                position = new Vector3(outerWidth, r.height - maxOuter, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Right ---

            // Top left
            var rTopLeft = new Vertex {
                position = new Vector3(r.width - widthDelta, maxInner, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var rTopRight = new Vertex {
                position = new Vector3(r.width - outerWidth, maxOuter, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var rBottomRight = new Vertex {
                position = new Vector3(r.width - outerWidth, r.height - maxOuter, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var rBottomLeft = new Vertex {
                position = new Vector3(r.width - widthDelta, r.height - maxInner, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // Top
            tris.Add(new Tri(tTopLeft, tTopRight, tBottomRight));
            tris.Add(new Tri(tBottomRight, tBottomLeft, tTopLeft));

            // Right
            tris.Add(new Tri(rTopRight, rBottomRight, rBottomLeft));
            tris.Add(new Tri(rBottomLeft, rTopLeft, rTopRight));

            // Bottom
            tris.Add(new Tri(bBottomRight, bBottomLeft, bTopLeft));
            tris.Add(new Tri(bTopLeft, bTopRight, bBottomRight));

            // Left
            tris.Add(new Tri(lBottomLeft, lTopLeft, lTopRight));
            tris.Add(new Tri(lTopRight, lBottomRight, lBottomLeft));

            int sections = 20;

            var center = new Vector3(borderRadius, borderRadius, Vertex.nearZ);
            float sectionDegrees = Vector3.Angle(tTopLeft.position - center, lTopLeft.position - center) / sections;

            // Top left
            CreateCorner(tTopLeft, lTopLeft, tBottomLeft, lTopRight, center, sections, sectionDegrees, widthDelta, out var topLeftTris);
            tris.AddRange(topLeftTris);

            // Top right
            center = new Vector3(r.width - borderRadius, borderRadius, Vertex.nearZ);
            CreateCorner(rTopRight, tTopRight, rTopLeft, tBottomRight, center, sections, sectionDegrees, widthDelta, out var topRightTris);
            tris.AddRange(topRightTris);

            // Bottom right
            center = new Vector3(r.width - borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(bBottomRight, rBottomRight, bTopRight, rBottomLeft, center, sections, sectionDegrees, widthDelta, out var bottomRightTris);
            tris.AddRange(bottomRightTris);

            // Bottom left
            center = new Vector3(borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(lBottomLeft, bBottomLeft, lBottomRight, bTopLeft, center, sections, sectionDegrees, widthDelta, out var bottomLeftTris);
            tris.AddRange(bottomLeftTris);
        }


        void CreateCorner(Vertex outerStart, Vertex outerEnd, Vertex cornerStart, Vertex cornerEnd, Vector3 center, int sections, float sectionDegrees, float widthDelta, out List<Tri> tris) {
            tris = new List<Tri>();
            var currentStart = outerStart;
            var currentInnerCorner = cornerStart;
            for (int i = 0; i < sections; i++) {
                if (i == sections - 1) {
                    tris.Add(new Tri(currentStart, cornerEnd, outerEnd));
                    if (borderRadius > widthDelta) {
                        tris.Add(new Tri(currentStart, currentInnerCorner, cornerEnd));
                    }

                } else {
                    CreateSectionPoint(center, sectionDegrees, currentStart, out var outerSectionPoint);
                    var innerSectionPoint = currentInnerCorner;
                    if (borderRadius > widthDelta) {
                        CreateSectionPoint(center, sectionDegrees, currentInnerCorner, out innerSectionPoint);
                        tris.Add(new Tri(currentStart, currentInnerCorner, innerSectionPoint));
                        currentInnerCorner = innerSectionPoint;
                    }
                    tris.Add(new Tri(currentStart, innerSectionPoint, outerSectionPoint));
                    currentStart = outerSectionPoint;
                }
            }
        }

        void CreateSectionPoint(Vector3 center, float sectionDegrees, Vertex currentStart, out Vertex sectionPoint) {
            var startingPoint = currentStart.position;
            var rotation = Quaternion.AngleAxis(sectionDegrees, Vector3.back);
            var shiftedPoint = startingPoint - center;
            startingPoint = (rotation * shiftedPoint) + center;

            sectionPoint = new Vertex {
                position = new Vector3(startingPoint.x, startingPoint.y, Vertex.nearZ),
                tint = innerBorderColor,
            };
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