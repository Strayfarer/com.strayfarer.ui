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
        }

        Func<VisualElement> _instantiateItem;
        public Func<VisualElement> instantiateItem {
            get => _instantiateItem;
            set {
                if (_instantiateItem != value) {
                    _instantiateItem = value;
                    Rebuild(true);
                }
            }
        }
        public event Action<VisualElement> onInstantiateItem;
        public event Action<VisualElement, object> onBindItem;

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

        VisualTreeAsset _itemTemplate;
        [UxmlAttribute]
        public VisualTreeAsset itemTemplate {
            get => _itemTemplate;
            set {
                _itemTemplate = value;
                Rebuild(true);
            }
        }

        IList _itemsSource;
        [CreateProperty]
        public IList itemsSource {
            get => _itemsSource;
            set {
                _itemsSource = value;
                Rebuild(false);
            }
        }

        int _elementsPerSection;
        [CreateProperty]
        [UxmlAttribute]
        public int elementsPerSection {
            get => _elementsPerSection;
            set {
                if (_elementsPerSection == value) {
                    return;
                }

                _elementsPerSection = value;
                Rebuild(false);
            }
        }

        public IEnumerable<VisualElement> items => _elementsPerSection == 0
            ? Children()
            : sections.SelectMany(section => section.Children());

        int builtSize = -1;
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

            if (_itemsSource is null) {
                builtSize = -1;
                foreach (var child in items) {
                    ReturnItem(child);
                }

                foreach (var child in pool) {
                    child.RemoveFromHierarchy();
                }

                return;
            }

            var previousItems = new List<VisualElement>(items);

            foreach (var previousItem in previousItems.Skip(_itemsSource.Count)) {
                ReturnItem(previousItem);
                previousItem.RemoveFromHierarchy();
            }

            builtSize = 0;

            for (int i = 0; i < _itemsSource.Count; i++) {
                if (previousItems.ElementAtOrDefault(i) is not VisualElement element) {
                    if (InstantiateItem(out element)) {
                        element.AddToClassList($"simpleList-item");

                        try {
                            onInstantiateItem?.Invoke(element);
                        } catch (Exception e) {
                            Debug.LogException(e);
                        }
                    }
                }

                if (i == 0) {
                    element.AddToClassList("first-child");
                } else {
                    element.RemoveFromClassList("first-child");
                }

                if (i == _itemsSource.Count - 1) {
                    element.AddToClassList("last-child");
                } else {
                    element.RemoveFromClassList("last-child");
                }

                GetSectionForItem(i).Add(element);

                if (element.dataSource != _itemsSource[i]) {
                    element.dataSource = _itemsSource[i];
                    try {
                        onBindItem?.Invoke(element, _itemsSource[i]);
                    } catch (Exception e) {
                        Debug.LogException(e);
                    }
                }

                builtSize++;
            }
        }
    }
}