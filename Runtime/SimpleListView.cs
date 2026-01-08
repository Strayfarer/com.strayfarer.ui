#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    [MovedFrom(true, "Retropair.UXML", "Retropair")]
    public sealed partial class SimpleListView : BindableElement {

        public SimpleListView() {
            AddToClassList("simpleList-root");
            AddToClassList("simple-list");
        }

        Func<VisualElement>? _instantiateItem;
        public Func<VisualElement>? instantiateItem {
            get => _instantiateItem;
            set {
                if (_instantiateItem != value) {
                    _instantiateItem = value;
                    Rebuild(true);
                }
            }
        }
        public event Action<VisualElement>? onInstantiateItem;
        public event Action<VisualElement, object?>? onBindItem;

        readonly Stack<VisualElement> pool = new();

        bool InstantiateItem(out VisualElement item) {
            if (pool.TryPop(out item)) {
                return false;
            }

            if (_instantiateItem is not null) {
                item = _instantiateItem();
                return true;
            }

            if (_itemTemplate) {
                item = _itemTemplate.Instantiate();
                return true;
            }

            item = new VisualElement();
            return true;
        }

        void ReturnItem(VisualElement element) {
            pool.Push(element);
        }

        VisualTreeAsset _itemTemplate = null!;
        [UxmlAttribute]
        public VisualTreeAsset itemTemplate {
            get => _itemTemplate;
            set {
                if (_itemTemplate != value) {
                    _itemTemplate = value;
                    Rebuild(true);
                }
            }
        }

        IList? _itemsSource;
        [CreateProperty]
        public IList? itemsSource {
            get => _itemsSource;
            set {
                if (_itemsSource != value) {
                    _itemsSource = value;
                    Rebuild(false);
                }
            }
        }

        int _defaultNumberOfItems;
        [Min(0)]
        [UxmlAttribute]
        [Tooltip("Draw this many items when no itemsSource is assigned at runtime.")]
        public int defaultNumberOfItems {
            get => _defaultNumberOfItems;
            set {
                if (value < 0) {
                    value = 0;
                }

                if (_defaultNumberOfItems != value) {
                    _defaultNumberOfItems = value;
                    if (_itemsSource is null) {
                        Rebuild(false);
                    }
                }
            }
        }

        int _elementsPerSection;
        [Min(0)]
        [CreateProperty]
        [UxmlAttribute]
        public int elementsPerSection {
            get => _elementsPerSection;
            set {
                if (value < 0) {
                    value = 0;
                }

                if (_elementsPerSection != value) {
                    _elementsPerSection = value;
                    Rebuild(true);
                }
            }
        }

        public IEnumerable<VisualElement> items => _elementsPerSection == 0
            ? Children()
            : sections.SelectMany(section => section.Children());

        readonly List<VisualElement> sections = new();
        VisualElement GetSectionForItem(int itemIndex) {
            if (_elementsPerSection == 0) {
                return this;
            }

            int sectionIndex = itemIndex / _elementsPerSection;
            for (int i = sections.Count; i < sectionIndex + 1; i++) {
                var section = new VisualElement();
                section.AddToClassList($"simpleList-section");
                section.AddToClassList($"simpleList-section-{i}");
                section.AddToClassList($"simple-list__section");
                section.AddToClassList($"simple-list__section--{i}");
                Add(section);
                sections.Add(section);
            }

            return sections[sectionIndex];
        }

        public void Rebuild(bool discardItems = false) {
            if (discardItems) {
                pool.Clear();
                sections.Clear();
                Clear();
            }

            if (_itemsSource is not { Count: int itemCount }) {
                itemCount = defaultNumberOfItems;
            }

            var previousItems = new List<VisualElement>(items);

            foreach (var previousItem in previousItems.Skip(itemCount)) {
                ReturnItem(previousItem);
                previousItem.RemoveFromHierarchy();
            }

            for (int i = 0; i < itemCount; i++) {
                if (previousItems.ElementAtOrDefault(i) is not VisualElement element) {
                    if (InstantiateItem(out element)) {
                        element.AddToClassList($"simpleList-item");
                        element.AddToClassList($"simple-list__item");

                        try {
                            onInstantiateItem?.Invoke(element);
                        } catch (Exception e) {
                            Debug.LogException(e);
                        }
                    }
                }

                if (i == 0) {
                    element.AddToClassList("first-child");
                    element.AddToClassList($"simple-list__item--first-child");
                } else {
                    element.RemoveFromClassList("first-child");
                    element.RemoveFromClassList($"simple-list__item--first-child");
                }

                if (i == itemCount - 1) {
                    element.AddToClassList("last-child");
                    element.AddToClassList($"simple-list__item--last-child");
                } else {
                    element.RemoveFromClassList("last-child");
                    element.RemoveFromClassList($"simple-list__item--last-child");
                }

                GetSectionForItem(i).Add(element);

                object? data = _itemsSource is null
                    ? i
                    : _itemsSource[i];
                if (element.dataSource != data) {
                    element.dataSource = data;
                    try {
                        onBindItem?.Invoke(element, data);
                    } catch (Exception e) {
                        Debug.LogException(e);
                    }
                }
            }
        }
    }
}