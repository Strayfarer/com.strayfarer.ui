#nullable enable
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
        float borderRadiusHor;
        float borderRadiusVert;
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
            borderRadiusHor = borderRadius > localBound.width / 2 ? localBound.width / 2 : borderRadius;
            borderRadiusVert = borderRadius > localBound.height / 2 ? localBound.height / 2 : borderRadius;

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
            float outerRadiusHor = Math.Max(borderRadiusHor, outerWidth);
            float innerRadiusHor = Math.Max(borderRadiusHor, widthDelta);
            float outerRadiusVert = Math.Max(borderRadiusVert, outerWidth);
            float innerRadiusVert = Math.Max(borderRadiusVert, widthDelta);

            float halfWidth = r.width / 2;
            float halfHeight = r.height / 2;

            // --- Top ---
            // Top left
            var tTopLeft = new Vertex {
                position = new Vector3(outerRadiusHor < halfWidth ? outerRadiusHor : halfWidth, outerWidth < halfHeight ? outerWidth : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var tTopRight = new Vertex {
                position = new Vector3((r.width - outerRadiusHor) > halfWidth ? (r.width - outerRadiusHor) : halfWidth, outerWidth < halfHeight ? outerWidth : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var tBottomRight = new Vertex {
                position = new Vector3((r.width - innerRadiusHor) > halfWidth ? (r.width - innerRadiusHor) : halfWidth, widthDelta < halfHeight ? widthDelta : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var tBottomLeft = new Vertex {
                position = new Vector3(innerRadiusHor < halfWidth ? innerRadiusHor : halfWidth, widthDelta < halfHeight ? widthDelta : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Right ---
            // Top left
            var rTopLeft = new Vertex {
                position = new Vector3((r.width - widthDelta) > halfWidth ? (r.width - widthDelta) : halfWidth, innerRadiusVert < halfHeight ? innerRadiusHor : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var rTopRight = new Vertex {
                position = new Vector3((r.width - outerWidth) > halfWidth ? (r.width - outerWidth) : halfWidth, outerRadiusVert < halfHeight ? outerRadiusHor : halfHeight, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var rBottomRight = new Vertex {
                position = new Vector3(r.width - outerWidth, r.height - outerRadiusVert, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var rBottomLeft = new Vertex {
                position = new Vector3(r.width - widthDelta, r.height - innerRadiusVert, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Bottom ---
            // Top left
            var bTopLeft = new Vertex {
                position = new Vector3(innerRadiusHor, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var bTopRight = new Vertex {
                position = new Vector3(r.width - innerRadiusHor, r.height - widthDelta, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var bBottomRight = new Vertex {
                position = new Vector3(r.width - outerRadiusHor, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };
            //Bottom left
            var bBottomLeft = new Vertex {
                position = new Vector3(outerRadiusHor, r.height - outerWidth, Vertex.nearZ),
                tint = innerBorderColor,
            };

            // --- Left ---
            // Top left
            var lTopLeft = new Vertex {
                position = new Vector3(outerWidth, outerRadiusVert, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Top right
            var lTopRight = new Vertex {
                position = new Vector3(widthDelta, innerRadiusVert, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom right
            var lBottomRight = new Vertex {
                position = new Vector3(widthDelta, r.height - innerRadiusVert, Vertex.nearZ),
                tint = innerBorderColor,
            };
            // Bottom left
            var lBottomLeft = new Vertex {
                position = new Vector3(outerWidth, r.height - outerRadiusVert, Vertex.nearZ),
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

            #region Region: corners
            // --------- CORNERS ---------------
            //int sections = Mathf.Min((int)((borderRadius / 10 * 5) + 5), 15);
            int sections = 6;

            var center = new Vector3(borderRadius, borderRadius, Vertex.nearZ);
            float angle = Vector3.Angle(tTopLeft.position - center, lTopLeft.position - center) > 90 ? 90 : Vector3.Angle(tTopLeft.position - center, lTopLeft.position - center);
            float sectionDegrees = angle / sections;

            // Top left
            CreateCorner(tTopLeft, lTopLeft, tBottomLeft, lTopRight, center, sections, sectionDegrees, widthDelta, innerBorderColor, innerBorderColor, out var topLeftTris);
            tris.AddRange(topLeftTris);

            // Top right
            center = new Vector3(r.width - borderRadius, borderRadius, Vertex.nearZ);
            CreateCorner(rTopRight, tTopRight, rTopLeft, tBottomRight, center, sections, sectionDegrees, widthDelta, innerBorderColor, innerBorderColor, out var topRightTris);
            tris.AddRange(topRightTris);

            // Bottom right
            center = new Vector3(r.width - borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(bBottomRight, rBottomRight, bTopRight, rBottomLeft, center, sections, sectionDegrees, widthDelta, innerBorderColor, innerBorderColor, out var bottomRightTris);
            tris.AddRange(bottomRightTris);

            // Bottom left
            center = new Vector3(borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(lBottomLeft, bBottomLeft, lBottomRight, bTopLeft, center, sections, sectionDegrees, widthDelta, innerBorderColor, innerBorderColor, out var bottomLeftTris);
            tris.AddRange(bottomLeftTris);
            #endregion
        }


        void CreateCorner(Vertex outerStart, Vertex outerEnd, Vertex cornerStart, Vertex cornerEnd, Vector3 center, int sections, float sectionDegrees, float width, Color outerSectionColor, Color innerSectionColor, out List<Tri> tris) {
            tris = new List<Tri>();
            var currentStart = outerStart;
            var currentInnerCorner = cornerStart;
            for (int i = 0; i < sections; i++) {
                if (i == sections - 1) {
                    tris.Add(new Tri(currentStart, cornerEnd, outerEnd));
                    if (borderRadius > width) {
                        tris.Add(new Tri(currentStart, currentInnerCorner, cornerEnd));
                    }

                } else {
                    CreateSectionPoint(center, sectionDegrees, currentStart, outerSectionColor, out var outerSectionPoint);
                    var innerSectionPoint = currentInnerCorner;
                    if (borderRadius > width) {
                        CreateSectionPoint(center, sectionDegrees, currentInnerCorner, innerSectionColor, out innerSectionPoint);
                        tris.Add(new Tri(currentStart, currentInnerCorner, innerSectionPoint));
                        currentInnerCorner = innerSectionPoint;
                    }
                    tris.Add(new Tri(currentStart, innerSectionPoint, outerSectionPoint));
                    currentStart = outerSectionPoint;
                }
            }
        }

        void CreateSectionPoint(Vector3 center, float sectionDegrees, Vertex currentStart, Color sectionColor, out Vertex sectionPoint) {
            var startingPoint = currentStart.position;
            var rotation = Quaternion.AngleAxis(sectionDegrees, Vector3.back);
            var shiftedPoint = startingPoint - center;
            startingPoint = (rotation * shiftedPoint) + center;

            sectionPoint = new Vertex {
                position = new Vector3(startingPoint.x, startingPoint.y, Vertex.nearZ),
                tint = sectionColor,
            };
        }

        void CreateGlow(out List<Tri> tris, Rect r) {
            // ----- Glowing Border -----

            tris = new List<Tri>();

            float halfWidth = r.width / 2;
            float halfHeight = r.height / 2;

            #region Region: outer glow
            // Outer Glow Vertices
            // --- Top ---
            // Top left
            var tTopLeft = new Vertex {
                position = new Vector3(borderRadiusHor, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var tTopRight = new Vertex {
                position = new Vector3(r.width - borderRadiusHor, -glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var tBottomRight = new Vertex {
                position = new Vector3(r.width - borderRadiusHor, 0, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom left
            var tBottomLeft = new Vertex {
                position = new Vector3(borderRadiusHor, 0, Vertex.nearZ),
                tint = glowColor,
            };

            // --- Right ---
            // Top left
            var rTopLeft = new Vertex {
                position = new Vector3(r.width, borderRadiusHor, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var rTopRight = new Vertex {
                position = new Vector3(r.width + glowWidth, borderRadiusHor, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var rBottomRight = new Vertex {
                position = new Vector3(r.width + glowWidth, r.height - borderRadiusHor, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom left
            var rBottomLeft = new Vertex {
                position = new Vector3(r.width, r.height - borderRadiusHor, Vertex.nearZ),
                tint = glowColor,
            };

            // --- Bottom ---
            // Top left
            var bTopLeft = new Vertex {
                position = new Vector3(borderRadiusHor, r.height, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var bTopRight = new Vertex {
                position = new Vector3(r.width - borderRadiusHor, r.height, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var bBottomRight = new Vertex {
                position = new Vector3(r.width - borderRadiusHor, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            //Bottom left
            var bBottomLeft = new Vertex {
                position = new Vector3(borderRadiusHor, r.height + glowWidth, Vertex.nearZ),
                tint = clearColor,
            };

            // --- Left ---
            // Top left
            var lTopLeft = new Vertex {
                position = new Vector3(-glowWidth, borderRadiusHor, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var lTopRight = new Vertex {
                position = new Vector3(0, borderRadiusHor, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var lBottomRight = new Vertex {
                position = new Vector3(0, r.height - borderRadius, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom left
            var lBottomLeft = new Vertex {
                position = new Vector3(-glowWidth, r.height - borderRadius, Vertex.nearZ),
                tint = clearColor,
            };

            // Top
            tris.Add(new Tri(tTopLeft, tTopRight, tBottomRight));
            tris.Add(new Tri(tBottomRight, tBottomLeft, tTopLeft));

            // Right
            tris.Add(new Tri(rTopLeft, rTopRight, rBottomRight));
            tris.Add(new Tri(rBottomRight, rBottomLeft, rTopLeft));

            // Bottom
            tris.Add(new Tri(bTopLeft, bTopRight, bBottomRight));
            tris.Add(new Tri(bBottomRight, bBottomLeft, bTopLeft));

            // Left
            tris.Add(new Tri(lTopLeft, lTopRight, lBottomRight));
            tris.Add(new Tri(lBottomRight, lBottomLeft, lTopLeft));

            #endregion

            #region Region: inner glow
            // -------- Inner Border Box ---------
            //var maxRadius = Mathf.Max(borderRadius, bord)
            bool smolGlow = borderRadius - borderWidth > glowWidth;
            // Top
            // Top left
            float wideDistance = borderWidth + glowWidth;
            var tTopLeftInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? borderRadius : wideDistance, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var tTopRightInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? r.width - borderRadius : r.width - borderWidth - glowWidth, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var tBottomRightInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? r.width - borderRadius : r.width - borderWidth - glowWidth, wideDistance, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom left
            var tBottomLeftInnerGlow = new Vertex {
                position = new Vector3(borderRadius - borderWidth > glowWidth ? borderRadius : wideDistance, wideDistance, Vertex.nearZ),
                tint = clearColor,
            };

            // Right
            // Top left
            var rTopLeftInnerGlow = new Vertex {
                position = new Vector3(r.width - borderWidth - glowWidth, smolGlow ? borderRadius : wideDistance, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var rTopRightInnerGlow = new Vertex {
                position = new Vector3(r.width - borderWidth, smolGlow ? borderRadius : wideDistance, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom right
            var rBottomRightInnerGlow = new Vertex {
                position = new Vector3(r.width - borderWidth, smolGlow ? r.height - borderRadius : r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom left
            var rBottomLeftInnerGlow = new Vertex {
                position = new Vector3(r.width - borderWidth - glowWidth, smolGlow ? r.height - borderRadius : r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            };

            // Bottom
            // Top left
            var bTopLeftInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? borderRadius : wideDistance, r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Top right
            var bTopRightInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? r.width - borderRadius : r.width - borderWidth -glowWidth, r.height - borderWidth - glowWidth, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var bBottomRightInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? r.width - borderRadius : r.width - borderWidth - glowWidth, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            // Bottom left
            var bBottomLeftInnerGlow = new Vertex {
                position = new Vector3(smolGlow ? borderRadius : wideDistance, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };

            // Left
            // Top left
            var lTopLeftInnerGlow = new Vertex {
                position = new Vector3(borderWidth, smolGlow ? borderRadius : wideDistance, Vertex.nearZ),
                tint = glowColor,
            };
            // Top right
            var lTopRightInnerGlow = new Vertex {
                position = new Vector3(wideDistance, smolGlow ? borderRadius : wideDistance, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom right
            var lBottomRightInnerGlow = new Vertex {
                position = new Vector3(wideDistance, smolGlow ? r.height - borderRadius : r.height - wideDistance, Vertex.nearZ),
                tint = clearColor,
            };
            // Bottom left
            var lBottomLeftInnerGlow = new Vertex {
                position = new Vector3(borderWidth, smolGlow ? r.height - borderRadius : r.height - wideDistance, Vertex.nearZ),
                tint = glowColor,
            };

            // Top
            tris.Add(new Tri(tTopLeftInnerGlow, tTopRightInnerGlow, tBottomRightInnerGlow));
            tris.Add(new Tri(tBottomRightInnerGlow, tBottomLeftInnerGlow, tTopLeftInnerGlow));

            // Right
            tris.Add(new Tri(rTopLeftInnerGlow, rTopRightInnerGlow, rBottomRightInnerGlow));
            tris.Add(new Tri(rBottomRightInnerGlow, rBottomLeftInnerGlow, rTopLeftInnerGlow));

            // Bottom
            tris.Add(new Tri(bTopLeftInnerGlow, bTopRightInnerGlow, bBottomRightInnerGlow));
            tris.Add(new Tri(bBottomRightInnerGlow, bBottomLeftInnerGlow, bTopLeftInnerGlow));

            // Left
            tris.Add(new Tri(lTopLeftInnerGlow, lTopRightInnerGlow, lBottomRightInnerGlow));
            tris.Add(new Tri(lBottomRightInnerGlow, lBottomLeftInnerGlow, lTopLeftInnerGlow));
            #endregion

            #region Region: corners
            // --------- CORNERS ---------------
            int sections = 6;

            var center = new Vector3(borderRadius, borderRadius, Vertex.nearZ);
            float angle = Vector3.Angle(tTopLeft.position - center, lTopLeft.position - center) > 90 ? 90 : Vector3.Angle(tTopLeft.position - center, lTopLeft.position - center);
            float sectionDegrees = angle / sections;

            // ---- Outer ----
            // Top left
            CreateCorner(tTopLeft, lTopLeft, tBottomLeft, lTopRight, center, sections, sectionDegrees, 0, clearColor, glowColor, out var topLeftTris);
            tris.AddRange(topLeftTris);

            // Top right
            center = new Vector3(r.width - borderRadius, borderRadius, Vertex.nearZ);
            CreateCorner(rTopRight, tTopRight, rTopLeft, tBottomRight, center, sections, sectionDegrees, 0, clearColor, glowColor, out var topRightTris);
            tris.AddRange(topRightTris);

            // Bottom right
            center = new Vector3(r.width - borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(bBottomRight, rBottomRight, bTopRight, rBottomLeft, center, sections, sectionDegrees, 0, clearColor, glowColor, out var bottomRightTris);
            tris.AddRange(bottomRightTris);

            // Bottom left
            center = new Vector3(borderRadius, r.height - borderRadius, Vertex.nearZ);
            CreateCorner(lBottomLeft, bBottomLeft, lBottomRight, bTopLeft, center, sections, sectionDegrees, 0, clearColor, glowColor, out var bottomLeftTris);
            tris.AddRange(bottomLeftTris);

            // ---- Inner ----
            // Corner point
            var corner = new Vertex {
                position = new Vector3(wideDistance, wideDistance, Vertex.nearZ),
                tint = clearColor,
            };

            // Top left
            var tlTop = new Vertex {
                position = new Vector3(borderRadius, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            var tlBottom = new Vertex {
                position = new Vector3(borderWidth, borderRadius, Vertex.nearZ),
                tint = glowColor,
            };
            center = new Vector3(borderRadius, borderRadius, Vertex.nearZ);
            CreateCorner(tlTop, tlBottom, corner, corner, center, sections, sectionDegrees, borderRadius, glowColor, clearColor, out var topLeftTrisInner);
            tris.AddRange(topLeftTrisInner);

            tris.Add(new Tri(tTopLeftInnerGlow, corner, tlTop));
            tris.Add(new Tri(tlBottom, corner, lTopLeftInnerGlow));

            // Top right
            var trTop = new Vertex {
                position = new Vector3(r.width - borderRadius, borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            var trBottom = new Vertex {
                position = new Vector3(r.width - borderWidth, borderRadius, Vertex.nearZ),
                tint = glowColor,
            };
            center = new Vector3(r.width - borderRadius, borderRadius, Vertex.nearZ);
            corner.position = new Vector3(r.width - wideDistance, wideDistance, Vertex.nearZ);
            CreateCorner(trBottom, trTop, corner, corner, center, sections, sectionDegrees, borderRadius, glowColor, clearColor, out var topRightTrisInner);
            tris.AddRange(topRightTrisInner);

            tris.Add(new Tri(trTop, corner, tTopRightInnerGlow));
            tris.Add(new Tri(rTopRightInnerGlow, corner, trBottom));

            // Bottom right
            var brTop = new Vertex {
                position = new Vector3(r.width - borderWidth, r.height - borderRadius, Vertex.nearZ),
                tint = glowColor,
            };
            var brBottom = new Vertex {
                position = new Vector3(r.width - borderRadius, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            center = new Vector3(r.width - borderRadius, r.height - borderRadius, Vertex.nearZ);
            corner.position = new Vector3(r.width - wideDistance, r.height - wideDistance, Vertex.nearZ);
            CreateCorner(brBottom, brTop, corner, corner, center, sections, sectionDegrees, borderRadius, glowColor, clearColor, out var bottomRightTrisInner);
            tris.AddRange(bottomRightTrisInner);

            tris.Add(new Tri(brTop, corner, rBottomRightInnerGlow));
            tris.Add(new Tri(bBottomRightInnerGlow, corner, brBottom));

            // Bottom left
            var blTop = new Vertex {
                position = new Vector3(borderWidth, r.height - borderRadius, Vertex.nearZ),
                tint = glowColor,
            };
            var blBottom = new Vertex {
                position = new Vector3(borderRadius, r.height - borderWidth, Vertex.nearZ),
                tint = glowColor,
            };
            center = new Vector3(borderRadius, r.height - borderRadius, Vertex.nearZ);
            corner.position = new Vector3(wideDistance, r.height - wideDistance, Vertex.nearZ);
            CreateCorner(blTop, blBottom, corner, corner, center, sections, sectionDegrees, borderRadius, glowColor, clearColor, out var bottomLeftTrisInner);
            tris.AddRange(bottomLeftTrisInner);

            tris.Add(new Tri(blBottom, corner, bBottomLeftInnerGlow));
            tris.Add(new Tri(lBottomLeftInnerGlow, corner, blTop));
            #endregion
        }
    }
}