#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    public sealed class SimpleListControl<TControl, TModel> : IDisposable
        where TControl : class, IBindable<TModel> {
        sealed class SimpleListElements : IReadOnlyList<VisualElement> {
            readonly SimpleListControl<TControl, TModel> control;
            readonly SimpleListView list;

            public SimpleListElements(SimpleListControl<TControl, TModel> control, SimpleListView list) {
                this.control = control;
                this.list = list;
            }

            public VisualElement this[int index] => list.elementsPerSection == 0
                ? list[index]
                : list.items.ElementAt(index);

            public int Count => control.models.Count;

            public IEnumerator<VisualElement> GetEnumerator() => list.items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => list.items.GetEnumerator();
        }

        sealed class SimpleListControls : IReadOnlyList<TControl> {
            readonly SimpleListElements elements;

            public SimpleListControls(SimpleListElements elements) {
                this.elements = elements;
            }

            public TControl this[int index] => elements[index].userData as TControl ?? throw new NullReferenceException();

            public int Count => elements.Count;

            public IEnumerator<TControl> GetEnumerator() {
                foreach (var e in elements) {
                    yield return e.userData as TControl ?? throw new NullReferenceException();
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                foreach (var e in elements) {
                    yield return e.userData as TControl ?? throw new NullReferenceException();
                }
            }
        }

        readonly SimpleListView list;
        readonly Func<VisualElement, TControl> instantiate;

        readonly SimpleListElements _views;
        public IReadOnlyList<VisualElement> views => _views;

        readonly List<TModel> _models = new();
        public IReadOnlyList<TModel> models => list.itemsSource as IReadOnlyList<TModel> ?? _models;

        readonly SimpleListControls _controls;
        public IReadOnlyList<TControl> controls => _controls;

        public SimpleListControl(SimpleListView list, Func<VisualElement, TControl> instantiate) {
            this.list = list;
            this.instantiate = instantiate;

            list.onInstantiateItem += HandleInstantiate;
            list.onBindItem += HandleBind;
            list.itemsSource = _models;

            _views = new(this, list);
            _controls = new(_views);
        }

        public void Dispose() {
            list.onInstantiateItem -= HandleInstantiate;
            list.onBindItem -= HandleBind;
            list.itemsSource = null;
        }

        void HandleInstantiate(VisualElement item) {
            item.userData = instantiate(item);
        }

        void HandleBind(VisualElement item, object? obj) {
            if (item.userData is TControl control && obj is TModel data) {
                control.Bind(data);
            }
        }

        public void ReplaceModels(IEnumerable<TModel> itemsSource) {
            _models.Clear();
            _models.AddRange(itemsSource);

            list.itemsSource = _models;
        }

        public void ReplaceModels(Span<TModel> itemsSource) {
            _models.Clear();
            foreach (var item in itemsSource) {
                _models.Add(item);
            }

            list.itemsSource = _models;
        }

        public void ReplaceModels(TModel[] itemsSource) {
            _models.Clear();

            list.itemsSource = itemsSource;
        }
    }
}