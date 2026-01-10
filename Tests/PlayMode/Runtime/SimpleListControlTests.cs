#nullable enable
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Slothsoft.TestRunner;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [TestOf(typeof(SimpleListControl<Control, Data>))]
    [TestFixture(true)]
    [TestFixture(false)]
    sealed class SimpleListControlTests {
        sealed record Data {
            public string text = string.Empty;

            public Data(string text) => this.text = text;
        }

        sealed class Control : IBindable<Data> {
            readonly Label label = new();

            public Control(VisualElement root) {
                root.Add(label);
            }

            public void Bind(Data data) => label.text = data.text;
        }

        readonly bool usePanel;

        public SimpleListControlTests(bool usePanel) {
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

        SimpleListControl<Control, Data> CreateSut() {
            var view = new SimpleListView();

            if (test is not null) {
                test.sut.rootVisualElement.Add(view);
            }

            return new(view, root => new Control(root));
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetModels() {
            var sut = CreateSut();

            var models = new Data[] { new("a"), new("b") };

            sut.ReplaceModels(models);

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.models, Is.EqualTo(models));
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetModelsAsEnumerable() {
            var sut = CreateSut();

            var models = new Data[] { new("a"), new("b") };

            sut.ReplaceModels(models);

            if (usePanel) {
                yield return null;
            }

            Assert.That(new List<Data>(sut.models), Is.EqualTo(models));
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetViews() {
            var sut = CreateSut();

            sut.ReplaceModels(new Data[] { new("a"), new("b") });

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.views.Count, Is.EqualTo(2));
            Assert.That(sut.views[0], Is.InstanceOf<VisualElement>());
            Assert.That(sut.views[1], Is.InstanceOf<VisualElement>());
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetViewsAsEnumerable() {
            var sut = CreateSut();

            var models = new Data[] { new("a"), new("b") };

            sut.ReplaceModels(models);

            if (usePanel) {
                yield return null;
            }

            Assert.That(new List<VisualElement>(sut.views), Is.EqualTo(sut.views));
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetControls() {
            var sut = CreateSut();

            sut.ReplaceModels(new Data[] { new("a"), new("b") });

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.controls.Count, Is.EqualTo(2));
            Assert.That(sut.controls[0], Is.InstanceOf<Control>());
            Assert.That(sut.controls[1], Is.InstanceOf<Control>());
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetControlsAsEnumerable() {
            var sut = CreateSut();

            var models = new Data[] { new("a"), new("b") };

            sut.ReplaceModels(models);

            if (usePanel) {
                yield return null;
            }

            Assert.That(new List<Control>(sut.controls), Is.EqualTo(sut.controls));
        }

        [UnityTest]
        public IEnumerator GivenNoTemplate_WhenReplaceModels_ThenSetLabels() {
            var sut = CreateSut();

            sut.ReplaceModels(new Data[] { new("a"), new("b") });

            if (usePanel) {
                yield return null;
            }

            Assert.That(sut.views[0].Q<Label>(), Has.Property(nameof(Label.text)).EqualTo("a"));
            Assert.That(sut.views[1].Q<Label>(), Has.Property(nameof(Label.text)).EqualTo("b"));
        }
    }
}
