using BenchRando.Rando;
using Benchwarp;
using RandomizerCore.Extensions;
using Random = System.Random;

namespace BenchRando
{
    /// <summary>
    /// Settings representing the save-specific BenchRandoMod data.
    /// <br/>As a connection mod, BR does not implement ILocalSettings, and instead packages its settings in the BRLocalSettingsModule.
    /// </summary>
    public class LocalSettings
    {
        public BenchRandomizationSettings Settings;
        public List<string> RandomizedBenches;
        public List<string> NonrandomizedBenches;
        public List<string> Benches;
        public Dictionary<string, string> ModifiedNearStyles;
        public Dictionary<string, string> ModifiedFarStyles;

        /// <summary>
        /// Creates LocalSettings from the current GlobalSettings and BRData, randomized according to the seed parameter.
        /// </summary>
        public LocalSettings(int seed)
        {
            Settings = BenchRandoMod.GS.BenchRandomizationSettings.Clone();
            if (!Settings.RandomizeBenchSpots)
            {
                switch (Settings.RandomizedItems)
                {
                    case ItemRandoMode.WarpUnlocks:
                    case ItemRandoMode.RestAndWarpUnlocks:
                    case ItemRandoMode.RestUnlocks:
                        RandomizedBenches = new(BRData.BenchLookup.Values.Where(b => b.IsBaseBench && b.IsRandomizable).Select(b => b.Name));
                        NonrandomizedBenches = new(BRData.BenchLookup.Values.Where(b => b.IsBaseBench && !b.IsRandomizable).Select(b => b.Name));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // randomize benches so that every bench area has at least 1 bench and at most 6 benches
                // the total number of benches can vary within a narrow range

                Random rng = new(seed * 79 + 47);

                NonrandomizedBenches = new(BRData.GetNonRandomizableBenches().Select(b => b.Name));
                RandomizedBenches = new();
                List<string> newBenchReceiver = Settings.RandomizedItems != ItemRandoMode.None
                    ? RandomizedBenches : NonrandomizedBenches;

                Dictionary<string, List<string>> benchesByArea = BRData.BenchLookup.Values
                    .Where(b => b.IsRandomizable)
                    .GroupBy(b => b.BenchAreaName)
                    .ToDictionary(g => g.Key, g => g.Select(b => b.Name).ToList());
                Dictionary<string, int> countsByArea = benchesByArea.Keys.ToDictionary(k => k, k => 0);
                List<string> areas = countsByArea.Keys.ToList();
                int total = 0;
                const int MINBENCHES = 45;
                const int MAXBENCHES = 55;
                const int MAXBENCHESPERAREA = 6;

                foreach (BenchDef b in BRData.BenchLookup.Values.Where(b => !b.IsRandomizable))
                {
                    if (countsByArea.TryGetValue(b.BenchAreaName ?? string.Empty, out int value))
                    {
                        countsByArea[b.BenchAreaName] = value + 1;
                        total++;
                    }
                }
                foreach (string k in benchesByArea.Keys)
                {
                    if (countsByArea[k] == 0)
                    {
                        var l = benchesByArea[k];
                        if (l.Count == 0) continue;
                        string b = rng.PopNext(l);

                        countsByArea[k]++;
                        total++;
                        newBenchReceiver.Add(b);
                    }
                    if (countsByArea[k] >= MAXBENCHESPERAREA)
                    {
                        areas.Remove(k);
                    }
                }



                int benchTotal = rng.Next(MINBENCHES, MAXBENCHES);

                while (total < benchTotal)
                {
                    string area = rng.Next(areas);
                    List<string> l = benchesByArea[area];
                    if (l.Count == 0) { areas.Remove(area); continue; }
                    string b = rng.PopNext(l);

                    countsByArea[area]++;
                    total++;
                    newBenchReceiver.Add(b);

                    if (countsByArea[area] >= MAXBENCHESPERAREA) { areas.Remove(area); }
                }
            }

            if (NonrandomizedBenches is null || RandomizedBenches is null) return;

            Benches = NonrandomizedBenches.Concat(RandomizedBenches).ToList();

            if (Settings.NewBenchStyle != StyleRandoMode.Default)
            {
                Random rng = new(seed * 1229 + 1009);
                List<string> styleNames = BenchStyle.StyleNames.ToList();
                styleNames.Sort();
                ModifiedNearStyles = new();

                foreach (string b in Benches)
                {
                    ModifiedNearStyles.Add(b, rng.Next(styleNames));
                }
                if (Settings.NewBenchStyle == StyleRandoMode.RandomCoupled) ModifiedFarStyles = ModifiedNearStyles;
                else
                {
                    ModifiedFarStyles = new();
                    foreach (string b in Benches)
                    {
                        ModifiedFarStyles.Add(b, rng.Next(styleNames));
                    }
                }
            }
            else
            {
                ModifiedNearStyles = ModifiedFarStyles = new(0);
            }
        }
    }
}
