using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [UxmlElement]
    [MovedFrom(true, "Retropair.UXML", "Retropair")]
    public sealed partial class SimpleListView : BindableElement {

        public event Action<VisualElement> onInstantiateItem;

        VisualTreeAsset _itemTemplate;
        [UxmlAttribute]
        public VisualTreeAsset itemTemplate {
            get => _itemTemplate;
            set {
                _itemTemplate = value;
                Rebuild();
            }
        }

        IList _itemsSource;
        [CreateProperty]
        public IList itemsSource {
            get => _itemsSource;
            set {
                if (_itemsSource == value) {
                    Update();
                } else {
                    _itemsSource = value;
                    Rebuild();
                }
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
                Rebuild();
            }
        }

        void Update() {
            int newSize = _itemsSource is null
                ? -1
                : _itemsSource.Count;

            if (builtSize != newSize) {
                Rebuild();
            }
        }

        int builtSize = -1;
        readonly List<VisualElement> sections = new();
        VisualElement GetSectionForElement(int elementIndex) {
            if (elementsPerSection == 0) {
                return this;
            }

            int sectionIndex = elementIndex / elementsPerSection;
            for (int i = sections.Count; i < sectionIndex + 1; i++) {
                var section = new VisualElement();
                section.AddToClassList($"simpleList-section");
                section.AddToClassList($"simpleList-section-{i}");
                Add(section);
                sections.Add(section);
            }

            return sections[sectionIndex];
        }

        VisualElement InstantiateItem() {
            var item = _itemTemplate is null
                ? new VisualElement()
                : _itemTemplate.Instantiate();
            item.AddToClassList($"simpleList-item");
            return item;
        }

        void Rebuild() {
            sections.Clear();

            Clear();

            if (_itemsSource is null) {
                builtSize = -1;
                return;
            }

            builtSize = 0;

            for (int i = 0; i < _itemsSource.Count; i++) {
                var element = InstantiateItem();
                element.dataSource = _itemsSource[i];
                GetSectionForElement(i).Add(element);

                builtSize++;

                try {
                    onInstantiateItem?.Invoke(element);
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        public SimpleListView() {
            AddToClassList("simpleList-root");
        }
    }
}