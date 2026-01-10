#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Slothsoft.TestRunner;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [TestOf(typeof(SimpleListView))]
    [TestFixture(true)]
    [TestFixture(false)]
    sealed class SimpleListViewTests {

        readonly bool usePanel;

        public SimpleListViewTests(bool usePanel) {
            this.usePanel = usePanel;
        }

        TestGameObject<UIDocument>? test;

        [SetUp]
        public void SetUpPanel() {
            if (usePanel) {
                test = new();
                test.sut.panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            }
        }

        [TearDown]
        public void TearDownPanel() {
            test?.Dispose();
        }

        SimpleListView CreateSut() {
            var sut = new SimpleListView();

            if (test is not null) {
                test.sut.rootVisualElement.Add(sut);
            }

            return sut;
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenSetItemSource_ThenCreateEmpty() {
            var sut = CreateSut();

            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.childCount, Is.EqualTo(2));

            Assert.That(sut[0], Is.InstanceOf<VisualElement>());
            Assert.That(sut[1], Is.InstanceOf<VisualElement>());
        }

        [UnityTest]
        public IEnumerator GivenOnInstantiateItem_WhenSetItemSource_ThenCall() {
            var action = Substitute.For<Action<VisualElement>>();

            var sut = CreateSut();

            sut.onInstantiateItem += action;

            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            action.Received(1).Invoke(sut[0]);
            action.Received(1).Invoke(sut[1]);
        }

        [UnityTest]
        public IEnumerator GivenOnInstantiateItem_ThenRepaint() {
            var action = Substitute.For<Action<VisualElement>>();

            var sut = CreateSut();

            sut.itemsSource = new string[] { "a", "b" };

            yield return null;

            sut.onInstantiateItem += action;

            if (usePanel) {
                yield return null;
            }

            action.Received(1).Invoke(sut[0]);
            action.Received(1).Invoke(sut[1]);
        }

        [UnityTest]
        public IEnumerator GivenInstantiateItem_WhenSetItemSource_ThenCreate() {
            var sut = CreateSut();

            sut.instantiateItem = () => new Label();
            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.items, Has.Count.EqualTo(2).And.All.InstanceOf<Label>());
        }

        [UnityTest]
        public IEnumerator GivenItemsSource_WhenSetInstantiateItem_ThenCreate() {
            var sut = CreateSut();

            sut.instantiateItem = () => new Label();
            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.items, Has.Count.EqualTo(2).And.All.InstanceOf<Label>());
        }

        [UnityTest]
        public IEnumerator GivenItemsSource_WhenSetSmaller_ThenRemove() {
            var sut = CreateSut();

            sut.itemsSource = new string[] { "a", "b" };
            sut.itemsSource = new string[] { "a" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.items, Has.Count.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator GivenItemsSource_ThenSetDataSource() {
            var sut = CreateSut();

            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.items.First(), Has.Property(nameof(sut.dataSource)).EqualTo("a"));
            Assert.That(sut.items.Last(), Has.Property(nameof(sut.dataSource)).EqualTo("b"));
        }

        [UnityTest]
        public IEnumerator GivenOnBindItem_ThenRebind() {
            var action = Substitute.For<Action<VisualElement, object?>>();

            var sut = CreateSut();

            sut.itemsSource = new string[] { "a", "b" };

            yield return null;

            sut.onBindItem += action;

            if (usePanel) {
                yield return null;
            }

            action.Received(1).Invoke(sut[0], "a");
            action.Received(1).Invoke(sut[1], "b");
        }

        [UnityTest]
        public IEnumerator GivenOnBindItem_WhenSetItemsSource_ThenCall() {
            var calls = new List<string>();

            var sut = CreateSut();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(calls, Is.EqualTo(new[] { "a", "b" }));
        }

        [UnityTest]
        public IEnumerator GivenOnBindItem_WhenSetItemsSourceAgain_ThenCallForChangedValues() {
            var calls = new List<string>();

            var sut = CreateSut();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b" };
            sut.itemsSource = new string[] { "c", "b" };

            if (usePanel) {
                Assert.That(calls, Is.Empty);

                yield return null;

                Assert.That(calls, Is.EqualTo(new[] { "c", "b" }));
            } else {
                Assert.That(calls, Is.EqualTo(new[] { "a", "b", "c" }));
            }
        }

        [UnityTest]
        public IEnumerator GivenOnBindItem_WhenSetItemsSourceAgain_ThenBindAsNeeded() {
            var calls = new List<string>();

            var sut = CreateSut();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b", "f" };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            if (usePanel) {
                Assert.That(calls, Is.Empty);

                yield return null;

                Assert.That(calls, Is.EqualTo(new[] { "c", "b", "d", "e" }));
            } else {
                Assert.That(calls, Is.EqualTo(new[] { "a", "b", "f", "c", "c", "b", "d", "e" }));
            }
        }

        [UnityTest]
        public IEnumerator GivenItemsSource_WhenSetItemsSourceAgain_ThenInstantiateAsNeeded() {
            int count = 0;

            var sut = CreateSut();
            sut.instantiateItem = () => {
                count++;
                return new VisualElement();
            };
            sut.itemsSource = new string[] { "a", "b", "f" };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(count, Is.EqualTo(4));
        }

        [UnityTest]
        public IEnumerator GivenOnInstantiate_WhenSetItemsSourceAgain_ThenInstantiateAsNeeded() {
            int count = 0;

            var sut = CreateSut();

            sut.onInstantiateItem += _ => count++;

            sut.itemsSource = new string[] { "a", "b", "f" };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            if (usePanel) {
                yield return null;
            }

            Assert.That(count, Is.EqualTo(4));
        }
    }
}
