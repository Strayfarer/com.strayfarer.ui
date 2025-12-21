#nullable enable
using System;
using System.Collections;
using NUnit.Framework;
using Slothsoft.TestRunner;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Runtime {
    [TestOf(typeof(GlowingBorder))]
    [TestFixture(typeof(VisualElement))]
    [TestFixture(typeof(GlowingBorder))]
    sealed class GlowingBorderBenchmarks<T> where T : VisualElement, new() {
        const string STYLESHEET = "Packages/com.strayfarer.ui/Tests/Assets/USS_Benchmarking.uss";

        const int WARMUP_COUNT = 10;
        const int MEASUREMENT_COUNT = 60;

        TestUIHarness<T> test = null!;
        VisualElement sut => test.sut;

        [SetUp]
        public void SetUpSuT() {
            test = new();
            test.sut.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET));
        }

        [TearDown]
        public void TearDownSuT() {
            test.Dispose();
        }

        public static readonly string[] allClassCombinations = new[] {
            "sut",
            "sut with-radius",
            "sut with-glow",
            "sut with-inner",
            "sut with-radius with-glow with-inner",
        };

        [UnityTest, Performance]
        public IEnumerator B00_DrawOnce([ValueSource(nameof(allClassCombinations))] string classes) {
            Array.ForEach(classes.Split(' '), sut.AddToClassList);

            yield return new MemoryMeasurement()
                .WarmupCount(WARMUP_COUNT)
                .MeasurementCount(MEASUREMENT_COUNT)
                .RecordFrameTime()
                .Run();
        }

        [UnityTest, Performance]
        public IEnumerator B10_DrawEveryFrame([ValueSource(nameof(allClassCombinations))] string classes) {
            Array.ForEach(classes.Split(' '), sut.AddToClassList);

            return new MemoryMeasurement()
                .WarmupCount(WARMUP_COUNT)
                .MeasurementCount(MEASUREMENT_COUNT)
                .RecordFrameTime()
                .Run(sut.MarkDirtyRepaint);
        }
    }
}
