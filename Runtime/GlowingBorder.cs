using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace Strayfarer.UI {
    [UxmlElement]
    public sealed partial class GlowingBorder : VisualElement {
        [Header(nameof(PageButton))]
        [UxmlAttribute]
        internal Color outerColor {
            get => _outerColor ?? Color.clear;
            set { _outerColor = value; MarkDirtyRepaint(); }
        }
        Color? _outerColor = Color.black;

        [UxmlAttribute]
        internal Color innerColor {
            get => _innerColor ?? Color.clear;
            set { _innerColor = value; MarkDirtyRepaint(); }
        }
        Color? _innerColor = Color.gray;

        //[UxmlAttribute]
        //internal float borderWidth {
        //    get => _borderWidth;
        //    set {
        //        _borderWidth = value;
        //        MarkDirtyRepaint();
        //    }
        //}
        float _borderWidth;

        [UxmlAttribute, Range(0, 50)]
        internal float outerWidthPercent {
            get => _outerWidthPercent;
            set { _outerWidthPercent = value; MarkDirtyRepaint(); }
        }
        float _outerWidthPercent;

        //float cachedBorderWidth;
        //Color cachedBorderColor;

        public GlowingBorder() {
            //RegisterCallback<AttachToPanelEvent>(OnAttach);
            //RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            //generateVisualContent += OnGenerateVisualContent;
            //RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolved);
            generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
        }

        //void OnGeometryChanged(GeometryChangedEvent evt) {
        //    CacheBorderWidth();
        //}

        //void OnAttach(AttachToPanelEvent evt) {
        //    CacheBorderWidth();
        //    DeactivateNativeBorder();
        //}

        //void CacheBorderWidth() {
        //    cachedBorderWidth = style.borderBottomWidth.keyword == StyleKeyword.Undefined
        //        ? 0
        //        : style.borderBottomWidth.value;

        //    if (style.borderBottomColor.keyword != StyleKeyword.Undefined) {
        //        cachedBorderColor = style.borderBottomColor.value;
        //    }
        //    MarkDirtyRepaint();
        //}

        //void DeactivateNativeBorder() {
        //    // Deactivate native border
        //    style.borderBottomWidth = 0;
        //    style.borderLeftWidth = 0;
        //    style.borderRightWidth = 0;
        //    style.borderTopWidth = 0;

        //    style.borderBottomColor = Color.clear;
        //    style.borderLeftColor = Color.clear;
        //    style.borderRightColor = Color.clear;
        //    style.borderTopColor = Color.clear;
        //}

        void OnGenerateVisualContent(MeshGenerationContext context) {
            //_borderWidth = cachedBorderWidth > 0 ? cachedBorderWidth : _borderWidth;
            //_outerColor = cachedBorderColor;

            //if (_borderWidth <= 0) {
            //    Debug.Log("Border is 0, don't draw...");
            //    return;
            //}
            _borderWidth = (float)(context.visualElement?.resolvedStyle.borderBottomWidth);
            //DeactivateNativeBorder();

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
            var r = contentRect;

            // ----- Outer Border ----
            // Top left
            verts.Add(new Vertex {
                position = new Vector3(0, 0, Vertex.nearZ),
                tint = outerColor,
            });
            // Top right
            verts.Add(new Vertex {
                position = new Vector3(r.width, 0, Vertex.nearZ),
                tint = outerColor,
            });
            // Bottom right
            verts.Add(new Vertex {
                position = new Vector3(r.width, r.height, Vertex.nearZ),
                tint = outerColor,
            });
            //Bottom left
            verts.Add(new Vertex {
                position = new Vector3(0, r.height, Vertex.nearZ),
                tint = outerColor,
            });
            // Top left (inner)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth, _borderWidth, Vertex.nearZ),
                tint = outerColor,
            });
            // Top right (inner)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth, _borderWidth, Vertex.nearZ),
                tint = outerColor,
            });
            // Bottom right (inner)
            verts.Add(new Vertex {
                position = new Vector3(r.width - _borderWidth, r.height - _borderWidth, Vertex.nearZ),
                tint = outerColor,
            });
            //Bottom left (inner)
            verts.Add(new Vertex {
                position = new Vector3(_borderWidth, r.height - _borderWidth, Vertex.nearZ),
                tint = outerColor,
            });

            // Outer border indices
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
            indices.Add(8);
            indices.Add(9);
            indices.Add(13);
            indices.Add(13);
            indices.Add(12);
            indices.Add(8);
            // Right
            indices.Add(9);
            indices.Add(10);
            indices.Add(14);
            indices.Add(14);
            indices.Add(13);
            indices.Add(9);
            // Bottom
            indices.Add(10);
            indices.Add(11);
            indices.Add(15);
            indices.Add(15);
            indices.Add(14);
            indices.Add(10);
            //Left
            indices.Add(11);
            indices.Add(8);
            indices.Add(12);
            indices.Add(12);
            indices.Add(15);
            indices.Add(11);
        }
    }
}