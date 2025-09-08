using Unity.Properties;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    [MovedFrom("Retropair.UXML")]
    sealed partial class PageButton : Button {
        [Header(nameof(PageButton))]
        [UxmlAttribute]
        [CreateProperty]
        [PageIndex]
        internal int pageIndex { get; set; } = 0;

        PageView _pageView;

        public PageButton() {
            AddToClassList("page-button");
            clicked += OnClicked;
        }

        void OnClicked() {
            SwitchToAttachedPage();
        }

        void SwitchToAttachedPage() {
            // Search for next PageView in hierarchy
            _pageView ??= GetFirstAncestorOfType<PageView>();
            if (_pageView != null) {
                _pageView.SwitchToPage(pageIndex);
            } else {
                Debug.LogWarning($"[PageButton] No PageView found in hierarchy for Button '{name}'");
            }
        }
    }
}