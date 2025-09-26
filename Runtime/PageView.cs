using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    [MovedFrom(true, "Retropair.UXML", "Retropair")]
    public sealed partial class PageView : VisualElement {
        // Index of the active page
        int _activeIndex = 0;

        [Header("PageView")]
        [UxmlAttribute]
        [CreateProperty]
        [PageIndex]
        internal int activeIndex {
            get => _activeIndex;
            set => SwitchToPageUnchecked(value);
        }

        public PageView() {
            AddToClassList("page-view");
            RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);
        }

        /// <summary>
        /// Shows the active page and hides the others via the page index.
        /// </summary>
        public void SwitchToPage(int index) {
            if (index < 0 || index >= childCount) {
                Debug.LogError($"PageView does not contain a page with index: {index}");
                return;
            }

            SwitchToPageUnchecked(index);
        }

        /// <summary>
        /// Shows the active page and hides the others via the page name attribute.
        /// </summary>
        public void SwitchToPage(string name) {
            var child = Children().FirstOrDefault(c => c.name == name);
            if (child == null) {
                Debug.LogError($"PageView does not contain a page with name: {name}");
                return;
            }

            for (int i = 0; i < childCount; i++) {
                this[i].style.display = (this[i] == child) ? DisplayStyle.Flex : DisplayStyle.None;
            }

            _activeIndex = IndexOf(child);
        }

        void OnAttachedToPanel(AttachToPanelEvent e) {
            // Show only the active page on first attach
            SwitchToPageUnchecked(_activeIndex);
        }

        void SwitchToPageUnchecked(int index) {
            _activeIndex = index;
            for (int i = 0; i < childCount; i++) {
                this[i].style.display = (i == index) ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }
    }
}
