#nullable enable
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Strayfarer.UI.Runtime {
    sealed class GlowingBorderBenchmarks {
        const int WARMUP_COUNT = 100;
        const int MEASUREMENT_COUNT = 100;
        const int ITERATIONS = 10;

        static bool hasGraphicsDevice => SystemInfo.graphicsDeviceType is not GraphicsDeviceType.Null;

        [Test, Performance, Explicit("Constructor benchmark is graphics-environment sensitive and should not run in normal CI test sweeps.")]
        public void B00_Constructor() {
            if (!hasGraphicsDevice) {
                Assert.Ignore($"No graphics device available: {SystemInfo.graphicsDeviceType}");
                return;
            }

            Measure
                .Method(() => {
                    var sut = new GlowingBorder();
                })
                .GC()
                .WarmupCount(WARMUP_COUNT)
                .MeasurementCount(MEASUREMENT_COUNT)
                .IterationsPerMeasurement(ITERATIONS)
                .Run();
        }
    }
}
