#nullable enable
using System;
using System.Collections;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Slothsoft.TestRunner;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Runtime {
    [TestOf(typeof(GlowingBorder))]
    [TestFixture]
    sealed class GlowingBorderTests {
        const string STYLESHEET = "Packages/com.strayfarer.ui/Tests/Assets/USS_Benchmarking.uss";

        TestUIHarness<GlowingBorder> test = null!;
        GlowingBorder sut => test.sut;

        [SetUp]
        public void SetUpSuT() {
            test = new();
            test.sut.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET));
            test.sut.AddToClassList("sut");
        }

        [TearDown]
        public void TearDownSuT() {
            test.Dispose();
        }

        public static readonly string[] allClassCombinations = new[] {
            "with-radius",
            "with-glow",
            "with-inner",
        };

        [UnityTest]
        public IEnumerator T00_WhenAddNewStyle_ThenRepaint([ValueSource(nameof(allClassCombinations))] string className) {
            var expected = Substitute.For<Action>();

            yield return null;

            sut.generateVisualContent += _ => expected();

            test.sut.AddToClassList(className);

            yield return null;

            expected.Received(1).Invoke();
        }

        [UnityTest]
        public IEnumerator T01_WhenRemoveOldStyle_ThenRepaint([ValueSource(nameof(allClassCombinations))] string className) {
            var expected = Substitute.For<Action>();

            yield return null;

            test.sut.AddToClassList(className);

            yield return null;

            sut.generateVisualContent += _ => expected();

            test.sut.RemoveFromClassList(className);

            yield return null;

            expected.Received(1).Invoke();
        }

        [UnityTest]
        public IEnumerator T02_WhenNoChangeInStyle_ThenNoRepaint([ValueSource(nameof(allClassCombinations))] string className) {
            var expected = Substitute.For<Action>();

            yield return null;

            test.sut.AddToClassList(className);

            yield return null;

            sut.generateVisualContent += _ => expected();

            test.sut.AddToClassList("duplicate");

            yield return null;

            expected.DidNotReceive().Invoke();
        }
    }
}
