using System.Collections.ObjectModel;

namespace BenchRando
{
    public static class BRData
    {
        /// <summary>
        /// The bench data used by BenchRando for randomization. Can be modified by other mods.
        /// Is null outside of randomization.
        /// </summary>
        public static ReadOnlyDictionary<string, BenchDef> BenchLookup { get; private set; }
        /// <summary>
        /// The bench data embedded in BenchRando, which seeds ExtendedBenchLookup prior to ModifyBenchList.
        /// This property can always be safely used.
        /// </summary>
        public static ReadOnlyDictionary<string, BenchDef> EmbeddedBenchData { get; }

        /// <summary>
        /// Invoked each time BRData is set up at the start of randomization.
        /// </summary>
        public static event Action<Dictionary<string, BenchDef>> ModifyBenchList;

        /// <summary>
        /// Returns the values of ExtendedBenchLookup which are not randomizable.
        /// </summary>
        public static IEnumerable<BenchDef> GetNonRandomizableBenches()
        {
            return BenchLookup.Values.Where(b => !b.IsRandomizable);
        }

        /// <summary>
        /// Returns true if key is not null and is in ExtendedBenchLookup.
        /// </summary>
        public static bool IsBenchName(string key)
        {
            return key is not null && BenchLookup.ContainsKey(key);
        }

        public static void Setup()
        {
            Dictionary<string, BenchDef> extendedBenchLookup = new(EmbeddedBenchData);
            try
            {
                ModifyBenchList?.Invoke(extendedBenchLookup);
            }
            catch (Exception e)
            {
                BenchRandoMod.Instance.LogError($"Error invoking ModifyBenchList:\n{e}");
                throw;
            }

            BenchLookup = new(extendedBenchLookup);

            BenchRandoMod.Instance.Log($"Base count: " + BenchLookup.Values.Count(b => b.IsBaseBench));
            BenchRandoMod.Instance.Log($"Extended count: " + BenchLookup.Count);
            foreach (var g in BenchLookup.Values.GroupBy(g => g.BenchAreaName))
            {
                BenchRandoMod.Instance.Log($"{g.Key}: {g.Count()}");
            }
        }

        /// <summary>
        /// Nulls BenchLookup.
        /// </summary>
        public static void Reset()
        {
            BenchLookup = null;
        }

        private static void WriteBenchNames()
        {
            using StreamWriter sw = new(File.OpenWrite(Path.Combine(Path.GetDirectoryName(typeof(BRData).Assembly.Location), "BenchNames.cs")));
            sw.WriteLine("namespace BenchRando");
            sw.WriteLine("{");
            sw.WriteLine("    public static class BenchNames");
            sw.WriteLine("    {");
            foreach (string s in EmbeddedBenchData.Keys)
            {
                sw.WriteLine($"        public const string {s.Replace("\'", "").Replace('-', '_')} = \"{s}\";");
            }
            sw.WriteLine("    }");
            sw.WriteLine("}");
        }

        static BRData()
        {
            EmbeddedBenchData = new(JsonUtil.Deserialize<Dictionary<string, BenchDef>>("BenchRando.Resources.benches.json"));
        }

    }
}
