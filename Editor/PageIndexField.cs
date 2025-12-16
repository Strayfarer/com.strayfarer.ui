#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Editor {
    public class PageIndexField : PopupField<int> {
        readonly SerializedProperty property;

        public PageIndexField(SerializedProperty property) : base() {
            this.property = property;

            label = property.displayName;
            choices = new(Enumerable.Range(0, GetPagesCount()));

            formatSelectedValueCallback = GetPageName;
            formatListItemCallback = GetPageName;

            this.BindProperty(property);
            this.AddFieldAlignmentToClassList();
            this.AddKebabToClassList(nameof(PageIndexField));
        }

        IEnumerable<SerializedProperty> pageProperties => property.TryGetUxmlAncestorOrSelf<PageView>(out var currentPageView)
            ? currentPageView.GetUxmlChildElements()
            : Enumerable.Empty<SerializedProperty>();

        string GetPageName(int index) {
            var pageProperty = pageProperties
                .ElementAtOrDefault(index);

            if (pageProperty is null) {
                return $"ERROR: Page {index} not found";
            }

            return $"{index}: {pageProperty.GetUxmlDisplayName().Replace("#", "\u200B#")}";
        }

        int GetPagesCount() {
            return pageProperties.Count();
        }
    }
}