namespace BenchRando.Rando
{
    public enum ItemRandoMode
    {
        None,
        WarpUnlocks,
        RestAndWarpUnlocks,
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

        [MenuChanger.Attributes.MenuRange(-1, 99)]
        public int BenchGroup = -1;
    }
}
