#nullable enable
using System;
using System.Collections;
using NUnit.Framework;
using Slothsoft.TestRunner;
using Unity.PerformanceTesting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Runtime {
    [TestOf(typeof(FlexSprite))]
    [TestFixture(typeof(FlexSprite))]
    [TestFixture(typeof(FlexVectorImage))]
    [TestFixture(typeof(Image), "sprite")]
    [TestFixture(typeof(Image), "texture")]
    [TestFixture(typeof(Image), "vectorImage")]
    sealed class ImageBenchmarks<T> where T : Image, new() {
        const string TEST_PNG = "Packages/com.strayfarer.ui/Tests/Assets/TEST_NineSlice.png";
        static Texture TestTexture => AssetDatabase.LoadAssetAtPath<Texture>(TEST_PNG);
        static Sprite TestSprite => AssetDatabase.LoadAssetAtPath<Sprite>(TEST_PNG);

        const string TEST_SVG = "Packages/com.strayfarer.ui/Tests/Assets/TEST_VectorImage.svg";
        static VectorImage TestVector => AssetDatabase.LoadAssetAtPath<VectorImage>(TEST_SVG);

        const string STYLESHEET = "Packages/com.strayfarer.ui/Tests/Assets/USS_Benchmarking.uss";
        static StyleSheet TestStyleSheet => AssetDatabase.LoadAssetAtPath<StyleSheet>(STYLESHEET);

        const int WARMUP_COUNT = 60;
        const int MEASUREMENT_COUNT = 180;

        TestUIHarness<T> test = null!;
        T sut => test.sut;

        readonly string mode;
        public ImageBenchmarks() : this(string.Empty) {
        }
        public ImageBenchmarks(string mode) {
            this.mode = mode;
        }

        [SetUp]
        public void SetUpSuT() {
            test = new();
            test.sut.styleSheets.Add(TestStyleSheet);

            switch (sut) {
                case FlexSprite flexSprite:
                    flexSprite.sprite = TestSprite;
                    break;
                case FlexVectorImage flexVectorImage:
                    flexVectorImage.vectorImage = TestVector;
                    break;
                default:
                    switch (mode) {
                        case "sprite":
                            sut.sprite = TestSprite;
                            break;
                        case "texture":
                            sut.image = TestTexture;
                            break;
                        case "vectorImage":
                            sut.vectorImage = TestVector;
                            break;
                    }

                    break;
            }
        }

        [TearDown]
        public void TearDownSuT() {
            test.Dispose();
        }

        public static readonly string[] allClassCombinations = new[] {
            "sut",
        };

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
