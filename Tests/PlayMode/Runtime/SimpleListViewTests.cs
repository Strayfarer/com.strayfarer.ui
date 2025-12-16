#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [TestFixture(TestOf = typeof(SimpleListView))]
    public class SimpleListViewTests {
        [Test]
        public void GivenNoTemplate_WhenSetItemSource_ThenCreateEmpty() {
            var sut = new SimpleListView {
                itemsSource = new string[] { "a", "b" }
            };

            Assert.That(sut.childCount, Is.EqualTo(2));

            Assert.That(sut[0], Is.InstanceOf<VisualElement>());
            Assert.That(sut[1], Is.InstanceOf<VisualElement>());
        }

        [Test]
        public void GivenOnInstantiateItem_WhenSetItemSource_ThenCall() {
            var action = Substitute.For<Action<VisualElement>>();

            var sut = new SimpleListView();

            sut.onInstantiateItem += action;

            sut.itemsSource = new string[] { "a", "b" };

            action.Received(1).Invoke(sut[0]);
            action.Received(1).Invoke(sut[1]);
        }

        [Test]
        public void GivenInstantiateItem_WhenSetItemSource_ThenCreate() {
            var sut = new SimpleListView {
                instantiateItem = () => new Label(),
                itemsSource = new string[] { "a", "b" }
            };

            Assert.That(sut.items, Has.Count.EqualTo(2).And.All.InstanceOf<Label>());
        }

        [Test]
        public void GivenItemsSource_WhenSetInstantiateItem_ThenCreate() {
            var sut = new SimpleListView {
                itemsSource = new string[] { "a", "b" },
                instantiateItem = () => new Label()
            };

            Assert.That(sut.items, Has.Count.EqualTo(2).And.All.InstanceOf<Label>());
        }

        [Test]
        public void GivenItemsSource_WhenSetSmaller_ThenRemove() {
            var sut = new SimpleListView {
                itemsSource = new string[] { "a", "b", "c" }
            };

            sut.itemsSource = new string[] { "a" };

            Assert.That(sut.items, Has.Count.EqualTo(1));
        }

        [Test]
        public void GivenItemsSource_ThenSetDataSource() {
            var sut = new SimpleListView {
                itemsSource = new string[] { "a", "b" }
            };

            Assert.That(sut.items.First(), Has.Property(nameof(sut.dataSource)).EqualTo("a"));
            Assert.That(sut.items.Last(), Has.Property(nameof(sut.dataSource)).EqualTo("b"));
        }

        [Test]
        public void GivenOnBindItem_WhenSetItemsSource_ThenCall() {
            var calls = new List<string>();

            var sut = new SimpleListView();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b" };

            Assert.That(calls, Is.EqualTo(new[] { "a", "b" }));
        }

        [Test]
        public void GivenOnBindItem_WhenSetItemsSourceAgain_ThenCallForChangedValues() {
            var calls = new List<string>();

            var sut = new SimpleListView();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b" };
            sut.itemsSource = new string[] { "c", "b" };

            Assert.That(calls, Is.EqualTo(new[] { "a", "b", "c" }));
        }

        [Test]
        public void GivenOnBindItem_WhenSetItemsSourceAgain_ThenBindAsNeeded() {
            var calls = new List<string>();

            var sut = new SimpleListView();

            sut.onBindItem += (element, data) => { calls.Add(data as string ?? throw new Exception()); };

            sut.itemsSource = new string[] { "a", "b", "f" };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            Assert.That(calls, Is.EqualTo(new[] { "a", "b", "f", "c", "c", "b", "d", "e" }));
        }

        [Test]
        public void GivenItemsSource_WhenSetItemsSourceAgain_ThenInstantiateAsNeeded() {
            int count = 0;

            var sut = new SimpleListView {
                instantiateItem = () => {
                    count++;
                    return new VisualElement();
                },
                itemsSource = new string[] { "a", "b", "f" }
            };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            Assert.That(count, Is.EqualTo(4));
        }

        [Test]
        public void GivenOnInstantiate_WhenSetItemsSourceAgain_ThenInstantiateAsNeeded() {
            int count = 0;

            var sut = new SimpleListView();

            sut.onInstantiateItem += _ => count++;

            sut.itemsSource = new string[] { "a", "b", "f" };
            sut.itemsSource = new string[] { "c", "b" };
            sut.itemsSource = null;
            sut.itemsSource = new string[] { "c", "b", "d", "e" };

            Assert.That(count, Is.EqualTo(4));
        }
    }
}
