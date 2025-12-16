#nullable enable
using UnityEngine;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public sealed class Heading : Label {
        public Heading(string text) : base(text) {
            style.unityFontStyleAndWeight = FontStyle.Bold;
            style.marginBottom = 4;
        }
    }
}
