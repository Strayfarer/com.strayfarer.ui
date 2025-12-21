#nullable enable
using System.Collections;
using NUnit.Framework;
using Slothsoft.TestRunner;
using Unity.PerformanceTesting;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Strayfarer.UI.Runtime {
    sealed class GlowingBorderBenchmarks {

        sealed class DirtyMarker : MonoBehaviour {
            public VisualElement? elementToMarkDirty;
            void Update() {
                elementToMarkDirty?.MarkDirtyRepaint();
            }
        }

        const int WARMUP_COUNT = 100;
        const int MEASUREMENT_COUNT = 100;

        TestGameObject<UIDocument> test = null!;
        DirtyMarker dirtyMarker = null!;

        GlowingBorder? sut = null;

        [SetUp]
        public void SetUpSuT() {
            sut = new();

            var settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.themeStyleSheet = ScriptableObject.CreateInstance<ThemeStyleSheet>();

            test = new();
            dirtyMarker = test.gameObject.AddComponent<DirtyMarker>();
            test.sut.panelSettings = settings;
            test.sut.rootVisualElement.Add(sut);
        }

        [TearDown]
        public void TearDownSuT() {
            sut = null;

            test.Dispose();
        }

        [UnityTest, Performance]
        public IEnumerator B00_DrawOnce() {
            yield return Measure
                .Frames()
                .WarmupCount(WARMUP_COUNT)
                .MeasurementCount(MEASUREMENT_COUNT)
                .Run();
        }

        [UnityTest, Performance]
        public IEnumerator B01_DrawEveryFrame() {
            dirtyMarker.elementToMarkDirty = sut;

            yield return Measure
                .Frames()
                .WarmupCount(WARMUP_COUNT)
                .MeasurementCount(MEASUREMENT_COUNT)
                .Run();
        }

        static readonly SampleGroup gcGroup = new("GC Allocated In Frame", SampleUnit.Byte);

        [UnityTest, Performance]
        public IEnumerator B00_DrawOnce_GC() {
            using var gc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
            gc.Reset();

            for (int i = 0; i < WARMUP_COUNT; i++) {
                yield return null;
            }

            for (int i = 0; i < MEASUREMENT_COUNT; i++) {
                gc.Start();
                yield return null;
                gc.Stop();
                Measure.Custom(gcGroup, gc.CurrentValue);
                gc.Reset();
            }
        }

        [UnityTest, Performance]
        public IEnumerator B01_DrawEveryFrame_GC() {
            dirtyMarker.elementToMarkDirty = sut;

            using var gc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
            gc.Reset();

            for (int i = 0; i < WARMUP_COUNT; i++) {
                yield return null;
            }

            for (int i = 0; i < MEASUREMENT_COUNT; i++) {
                gc.Start();
                yield return null;
                gc.Stop();
                Measure.Custom(gcGroup, gc.CurrentValue);
                gc.Reset();
            }
        }
    }
}
