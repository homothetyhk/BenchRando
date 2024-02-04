using MenuChanger.Attributes;

namespace BenchRando.Rando
{
    public enum ItemRandoMode
    {
        None,
        WarpUnlocks,
        RestAndWarpUnlocks,
        RestUnlocks,
    }

    public enum StyleRandoMode
    {
        Default,
        RandomCoupled,
        RandomDecoupled
    }

    public class BenchRandomizationSettings
    {
        public bool IsEnabled() => RandomizedItems != ItemRandoMode.None || RandomizeBenchSpots;

        public ItemRandoMode RandomizedItems;
        public bool RandomizeBenchSpots;
        public StyleRandoMode NewBenchStyle;

        private const int AREACOUNT = 15;

        [DynamicBound(nameof(MaximumBenchCount), true)]
        [TriggerValidation(nameof(MinimumBenchesPerArea))]
        [MenuRange(AREACOUNT, 87)]
        public int MinimumBenchCount { get; set; } = 45;

        [DynamicBound(nameof(MinimumBenchCount), false)]
        [TriggerValidation(nameof(MaximumBenchesPerArea))]
        [MenuRange(AREACOUNT, 87)]
        public int MaximumBenchCount { get; set; } = 54;

        [DynamicBound(nameof(MinimumBenchesPerAreaUB), true)]
        [DynamicBound(nameof(MaximumBenchesPerArea), true)]
        [MenuRange(1, 3)]
        public int MinimumBenchesPerArea { get; set; } = 1;

        [DynamicBound(nameof(MinimumBenchesPerArea), false)]
        [DynamicBound(nameof(MaximumBenchesPerAreaLB), false)]
        [MenuRange(1, 6)]
        public int MaximumBenchesPerArea { get; set; } = 6;

        private int MinimumBenchesPerAreaUB => MinimumBenchCount / AREACOUNT;
        
        private int MaximumBenchesPerAreaLB => MaximumBenchCount % AREACOUNT == 0 ? MaximumBenchCount / AREACOUNT : MaximumBenchCount / AREACOUNT + 1;

        [MenuRange(-1, 99)]
        public int BenchGroup = -1;

        public BenchRandomizationSettings Clone() => (BenchRandomizationSettings)MemberwiseClone();

    }
}
