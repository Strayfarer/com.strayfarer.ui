using System;
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
    }
}
