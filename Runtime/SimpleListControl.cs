#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    public sealed class SimpleListControl<TControl, TModel> : IDisposable
        where TControl : class, IBindable<TModel> {
        sealed class SimpleListControls : IReadOnlyList<TControl> {
            readonly IReadOnlyList<VisualElement> elements;

            public SimpleListControls(IReadOnlyList<VisualElement> elements) {
                this.elements = elements;
            }

            public TControl this[int index] => elements[index].userData as TControl ?? throw new NullReferenceException();

            public int Count => elements.Count;

            public IEnumerator<TControl> GetEnumerator() {
                foreach (var e in elements) {
                    yield return e.userData as TControl ?? throw new NullReferenceException();
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        readonly Func<VisualElement, TControl> instantiate;

        readonly SimpleListView list;
        public IReadOnlyList<VisualElement> views => list.items;

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

            _controls = new(list.items);
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