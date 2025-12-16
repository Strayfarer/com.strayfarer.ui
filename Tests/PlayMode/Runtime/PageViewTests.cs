#nullable enable
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Slothsoft.TestRunner;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI {
    [TestFixture(TestOf = typeof(PageView))]
    sealed class PageViewTests {
        TestGameObject<UIDocument> test = null!;
        UIDocument document => test.sut;
        PageView sut = null!;

        [SetUp]
        public void SetUpSuT() {
            test = new();
            sut = new();
            document.rootVisualElement.Add(sut);
        }

        [TearDown]
        public void TearDownSuT() {
            test.Dispose();
        }

        [UnityTest]
        public IEnumerator GivenNoPages_WhenDoNothing_ThenDoNotError() {
            document.rootVisualElement.Add(new PageView());
            yield return null;
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPages_WhenSetPageIndex_ThenSetDisplayStyle(int index) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(_ => new VisualElement())
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.activeIndex = index;

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPages_WhenSwitchToPageIndex_ThenSetDisplayStyle(int index) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(_ => new VisualElement())
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.SwitchToPage(index);

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPages_WhenSwitchToPageName_ThenSetDisplayStyle(int index) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(i => new VisualElement() { name = $"page {i}" })
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.SwitchToPage($"page {index}");

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenPagesWithChildren_WhenSwitchToPageName_ThenIgnoreDescendants(int index) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(i => new VisualElement() { name = $"page {i}" })
                .ToList();

            foreach (var page in pages) {
                page.Add(new VisualElement() { name = "page 1" });
                sut.Add(page);
            }

            sut.SwitchToPage($"page {index}");

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        [Test]
        public void GivenPagesWithDuplicateNames_WhenSwitchToPageName_ThenPickFirst() {
            var pages = Enumerable
                .Range(0, 3)
                .Select(i => new VisualElement() { name = $"page" })
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.SwitchToPage($"page");

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == 0);
            }
        }

        [TestCase(0, "missing")]
        [TestCase(1, "page")]
        public void GivenPages_WhenSwitchToPageButMissingName_ThenErrorAndDoNothing(int index, string test) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(i => new VisualElement() { name = $"page {i}" })
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.activeIndex = index;

            LogAssert.Expect(LogType.Error, $"PageView does not contain a page with name: {test}");

            sut.SwitchToPage(test);

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        [TestCase(0, 3)]
        [TestCase(1, -1)]
        public void GivenPages_WhenSwitchToPageButMissingIndex_ThenErrorAndDoNothing(int index, int test) {
            var pages = Enumerable
                .Range(0, 3)
                .Select(i => new VisualElement() { name = $"page {i}" })
                .ToList();

            foreach (var page in pages) {
                sut.Add(page);
            }

            sut.activeIndex = index;

            LogAssert.Expect(LogType.Error, $"PageView does not contain a page with index: {test}");

            sut.SwitchToPage(test);

            for (int i = 0; i < pages.Count; i++) {
                AssertVisibility(pages[i], i == index);
            }
        }

        void AssertVisibility(VisualElement page, bool shouldBeVisible) {
            var expected = shouldBeVisible
                ? DisplayStyle.Flex
                : DisplayStyle.None;
            Assert.That(page.style.display.value, Is.EqualTo(expected));
        }
    }
}
