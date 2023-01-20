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

        private const string INCLUDEBENCHWARPSELECT = "INCLUDEBENCHWARPSELECT";

        public static void Setup()
        {
            RCData.RuntimeLogicOverride.Subscribe(0.2f, ModifyLMB);
            RCData.RuntimeLogicOverride.Subscribe(100f, LateModifyLMB);
        }

        public static void ModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            RandoInterop.Clear();
            if (!RandoInterop.IsEnabled()) return;
            RandoInterop.Initialize(gs.Seed);

            lmb.GetOrAddTerm("BENCHRANDO"); // for use by consumers in coalescing expressions to detect BR I guess?

            if (RandoInterop.LS.Settings.RandomizedItems == ItemRandoMode.WarpUnlocks
                || RandoInterop.LS.Settings.RandomizedItems == ItemRandoMode.RestAndWarpUnlocks)
            {
                lmb.DoMacroEdit(new(INCLUDEBENCHWARPSELECT, "TRUE")); // we won't insert this macro into BR logic, but since we rely on select warps in logic we may as well properly update it
            }

            foreach (string s in RandoInterop.LS.Benches)
            {
                lmb.AddItem(new BoolItem(s, lmb.GetOrAddTerm(BRData.BenchLookup[s].GetTermName())));
            }

            foreach (string s in RandoInterop.LS.Benches)
            {
                BenchDef b = BRData.BenchLookup[s];
                lmb.AddWaypoint(new(b.GetWaypointName(), b.Logic));
                if (!b.IsBaseBench)
                {
                    foreach (RawLogicDef l in b.LogicOverrides)
                    {
                        lmb.DoLogicEdit(l);
                    }
                }
            }

            // We rebuild the Can_Bench waypoint to use the new benches
            // This the ability to rest at any bench
            LogicClauseBuilder canBench = new(ConstToken.False);
            foreach (string s in RandoInterop.LS.Benches)
            {
                canBench.OrWith(BRData.BenchLookup[s].GetWaypointName(), lmb.LP);
            }
            LogicClauseBuilder canWarpToDGBench;
            LogicClauseBuilder canWarpToBench;
            const string Can_Bench = "Can_Bench";
            const string Can_Warp_To_DG_Bench = "Can_Warp_To_DG_Bench";
            const string Can_Warp_To_Bench = "Can_Warp_To_Bench";

            switch (RandoInterop.LS.Settings.RandomizedItems)
            {
                case ItemRandoMode.WarpUnlocks:
                case ItemRandoMode.RestAndWarpUnlocks:
                    canWarpToDGBench = new(ConstToken.False);
                    canWarpToBench = new(Can_Warp_To_DG_Bench, lmb.LP);
                    foreach (string s in RandoInterop.LS.RandomizedBenches)
                    {
                        BenchDef def = BRData.BenchLookup[s];
                        if (!def.DreamGateRestricted) canWarpToDGBench.OrWith(def.GetTermName(), lmb.LP);
                        else canWarpToBench.OrWith(def.GetTermName(), lmb.LP);
                    }
                    foreach (string s in RandoInterop.LS.NonrandomizedBenches)
                    {
                        BenchDef def = BRData.BenchLookup[s];
                        if (!def.DreamGateRestricted) canWarpToDGBench.OrWith(def.GetWaypointName(), lmb.LP);
                        else canWarpToBench.OrWith(def.GetWaypointName(), lmb.LP);
                    }
                    break;
                case ItemRandoMode.None:
                case ItemRandoMode.RestUnlocks:
                default:
                    canWarpToDGBench = lmb.LP.ParseInfixToBuilder(INCLUDEBENCHWARPSELECT);
                    canWarpToBench = lmb.LP.ParseInfixToBuilder(INCLUDEBENCHWARPSELECT + " + " + Can_Warp_To_DG_Bench);
                    foreach (string s in RandoInterop.LS.Benches)
                    {
                        BenchDef def = BRData.BenchLookup[s];
                        if (!def.DreamGateRestricted) canWarpToDGBench.OrWith(def.GetWaypointName(), lmb.LP);
                        else canWarpToBench.OrWith(def.GetWaypointName(), lmb.LP);
                    }
                    break;
            }
            lmb.LogicLookup[Can_Bench] = new(canBench);
            lmb.LogicLookup[Can_Warp_To_DG_Bench] = new(canWarpToDGBench);
            lmb.LogicLookup[Can_Warp_To_Bench] = new(canWarpToBench);
        }

        public static void LateModifyLMB(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!RandoInterop.IsEnabled()) return;

            foreach (string b in RandoInterop.LS.RandomizedBenches)
            {
                EditWaypoint(lmb, BRData.BenchLookup[b]);
            }
            foreach (string b in RandoInterop.LS.NonrandomizedBenches)
            {
                EditNonrandomizedWaypoint(lmb, BRData.BenchLookup[b]);
            }

            if (RandoInterop.LS.Settings.RandomizeBenchSpots)
            {
                foreach (BenchDef def in BRData.BenchLookup.Values.Where(b => b.IsBaseBench && !RandoInterop.LS.Benches.Contains(b.Name)))
                {
                    lmb.Waypoints.Remove(def.GetWaypointName());
                    lmb.DoLogicEdit(new(def.Name, "NONE"));
                    foreach (RawLogicDef edit in def.LogicOverrides)
                    {
                        lmb.DoSubst(new(edit.name, def.GetWaypointName(), "NONE"));
                    }
                }
            }

            // If vanilla benches don't exist, we remove logic that assumes benches are available for nonterminal shade skips and charm usage
            if (RandoInterop.LS.Settings.RandomizeBenchSpots
                || RandoInterop.LS.Settings.RandomizedItems == ItemRandoMode.RestAndWarpUnlocks
                || RandoInterop.LS.Settings.RandomizedItems == ItemRandoMode.RestUnlocks)
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

        private static void EditWaypoint(LogicManagerBuilder lmb, BenchDef def)
        { 
            string name = def.GetWaypointName();
            LogicClauseBuilder lcb = new(lmb.LogicLookup[name]);
            lcb.Subst(lmb.LP.GetTermToken("$BENCHRESET"), ConstToken.True);
            lcb.Subst(lmb.LP.GetTermToken("$WARPTOBENCH"), ConstToken.False);
            switch (RandoInterop.LS.Settings.RandomizedItems)
            {
                case ItemRandoMode.None:
                    lcb.AndWith("$BENCHRESET", lmb.LP);
                    break;
                case ItemRandoMode.WarpUnlocks:
                    lcb.AndWith("$BENCHRESET", lmb.LP);
                    lcb.OrWith($"Start_State + $WARPTOBENCH + {def.GetTermName()}", lmb.LP);
                    break;
                case ItemRandoMode.RestAndWarpUnlocks:
                    lcb.AndWith($"({def.GetTermName()} + $BENCHRESET | ANY)", lmb.LP);
                    lcb.OrWith($"Start_State + $WARPTOBENCH + {def.GetTermName()}", lmb.LP);
                    break;
                case ItemRandoMode.RestUnlocks:
                    lcb.AndWith($"({def.GetTermName()} + $BENCHRESET | ANY)", lmb.LP);
                    break;
            }
            lmb.LogicLookup[name] = new(lcb);
        }

        private static void EditNonrandomizedWaypoint(LogicManagerBuilder lmb, BenchDef def)
        {
            string name = def.GetWaypointName();
            LogicClauseBuilder lcb = new(lmb.LogicLookup[name]);
            lcb.Subst(lmb.LP.GetTermToken("$BENCHRESET"), ConstToken.True);
            lcb.AndWith("$BENCHRESET", lmb.LP);
            lmb.LogicLookup[name] = new(lcb);
        }
    }
}
