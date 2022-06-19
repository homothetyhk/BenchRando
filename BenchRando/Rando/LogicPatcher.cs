using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringLogic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace BenchRando.Rando
{
    internal static class LogicPatcher
    {
        private static readonly string[] brokenShadeSkipMacros = new[]
        {
            "ITEMSHADESKIPS",
            "MAPAREASHADESKIPS",
            "AREASHADESKIPS",
            "ROOMSHADESKIPS",
        };

        private const string SAFE_SHADESKIP_MACRO = "DGSHADESKIPS";

        private static readonly string[] brokenBenchMacros = new[]
        {
            "ITEMBENCH",
            "MAPAREABENCH",
            "AREABENCH",
            "ROOMBENCH",
        };

        private const string SAFE_BENCH_MACRO = "DGBENCH";

        public static void Setup()
        {
            RCData.RuntimeLogicOverride.Subscribe(0.2f, ModifyLMB);
        }

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.Settings.IsEnabled()) return;
            
            RandoInterop.SetBenches(gs.Seed);

            // It is safest to make terms and items for all benches in use, whether they are indicated to be randomized or not
            foreach (string s in RandoInterop.Benches)
            {
                lmb.AddItem(new BoolItem(s, lmb.GetOrAddTerm(s)));
            }
            foreach (string s in RandoInterop.Benches)
            {
                BenchDef bench = BRData.BenchLookup[s];
                foreach (RawLogicDef def in bench.LogicOverrides)
                {
                    lmb.DoLogicEdit(def);
                }
                lmb.AddLogicDef(new(s, bench.Logic));
            }

            // We rebuild the Can_Bench waypoint to use the new benches
            // This the ability to rest at any bench
            LogicClauseBuilder canBench = new(ConstToken.False);
            foreach (string s in RandoInterop.Benches)
            {
                canBench.OrWith(lmb.LP.GetTermToken(s));
                if (RandoInterop.Settings.RandomizedItems != ItemRandoMode.RestAndWarpUnlocks)
                {
                    canBench.OrWith(lmb.LP.GetTermToken("*" + s));
                }
            }
            lmb.LogicLookup["Can_Bench"] = new(canBench);

            // If vanilla benches don't exist, we remove logic that assumes benches are available for nonterminal shade skips and charm usage
            if (RandoInterop.Settings.RandomizeBenchSpots || RandoInterop.Settings.RandomizedItems == ItemRandoMode.RestAndWarpUnlocks)
            {
                foreach (string m in brokenShadeSkipMacros)
                {
                    lmb.DoMacroEdit(new(m, SAFE_SHADESKIP_MACRO));
                }
                foreach (string m in brokenBenchMacros)
                {
                    lmb.DoMacroEdit(new(m, SAFE_BENCH_MACRO));
                }
            }
        }
    }
}
