#nullable enable
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Strayfarer.UI.Runtime {
    sealed class GlowingBorderBenchmarks {
        const int WARMUP_COUNT = 100;
        const int MEASUREMENT_COUNT = 100;
        const int ITERATIONS = 10;

        [Test, Performance]
        public void B00_Constructor() {
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
