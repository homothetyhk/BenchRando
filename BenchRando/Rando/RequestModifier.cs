using BenchRando.IC;
using RandomizerCore.Randomization;
using RandomizerMod.RC;

namespace BenchRando.Rando
{
    public static class RequestModifier
    {
        public static void Setup()
        {
            // Define the bench group, add the group matcher, and define item and location defs.
            RequestBuilder.OnUpdate.Subscribe(-495f, SetupRefs);

            // Add benches to the pool shortly after the rest of the items; they will be
            // added to the correct group if necessary because of the matcher we added in SetupRefs.
            // we will not bother to put benches in vanilla; they have no progression effect there.
            RequestBuilder.OnUpdate.Subscribe(0.6f, AddBenches);

            // The deranged constraint must be applied separately
            RequestBuilder.OnUpdate.Subscribe(102f, DerangeBenches);
        }

        private static void SetupRefs(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.IsEnabled()) return;

            // We make refs for all randomized or nonrandomized benches in use.
            // No refs for benches which were not selected for the seed.
            foreach (string s in RandoInterop.Benches)
            {
                BenchDef def = BRData.BenchLookup[s];

                rb.EditItemRequest(def.Name, info =>
                {
                    info.getItemDef = () => def.GetRMItemDef();
                });
                rb.EditLocationRequest(def.Name, info =>
                {
                    info.getLocationDef = () => def.GetRMLocationDef();
                    info.onPlacementFetch += (f, r, p) =>
                    {
                        if (RandoInterop.Settings.RandomizedItems == ItemRandoMode.RestAndWarpUnlocks)
                        {
                            p.GetOrAddTag<IC.RequireUnlockToRestTag>();
                        }
                    };
                });
            }

            if (rb.gs.SplitGroupSettings.RandomizeOnStart && RandoInterop.Settings.BenchGroup >= 0 && RandoInterop.Settings.BenchGroup <= 2)
            {
                RandoInterop.Settings.BenchGroup = rb.rng.Next(3);
            }
            // If the value is 0 or -1, then benches will be placed in the main item group by default, so we don't need a resolver.
            if (RandoInterop.Settings.BenchGroup > 0)
            {
                ItemGroupBuilder benchGroup = null;
                string label = RBConsts.SplitGroupPrefix + RandoInterop.Settings.BenchGroup;
                foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
                {
                    if (igb.label == label)
                    {
                        benchGroup = igb;
                        break;
                    }
                }
                benchGroup ??= rb.MainItemStage.AddItemGroup(label);

                rb.OnGetGroupFor.Subscribe(0.01f, ResolveBenchGroup);

                bool ResolveBenchGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
                {
                    if (type == RequestBuilder.ElementType.Transition)
                    {
                        gb = default;
                        return false;
                    }

                    if (!BRData.IsBenchName(item))
                    {
                        gb = default;
                        return false;
                    }

                    gb = benchGroup;
                    return true;
                }
            }
        }

        private static void AddBenches(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.IsEnabled()) return;

            if (RandoInterop.RandomizedBenches is List<string> benchPool)
            {
                List<string> removedBenches = RandoInterop.NonrandomizedBenches ??= new();

                if (rb.gs.LongLocationSettings.WhitePalaceRando == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludeWhitePalace)
                {
                    for (int i = 0; i < benchPool.Count; i++)
                    {
                        if (rb.TryGetLocationDef(benchPool[i], out RandomizerMod.RandomizerData.LocationDef ldef) && ldef.MapArea == "White Palace")
                        {
                            removedBenches.Add(benchPool[i]);
                            benchPool.RemoveAt(i--);
                        }
                    }
                }
                else if (rb.gs.LongLocationSettings.WhitePalaceRando == RandomizerMod.Settings.LongLocationSettings.WPSetting.ExcludePathOfPain)
                {
                    for (int i = 0; i < benchPool.Count; i++)
                    {
                        if (rb.TryGetLocationDef(benchPool[i], out RandomizerMod.RandomizerData.LocationDef ldef) && ldef.TitledArea == "Path of Pain")
                        {
                            removedBenches.Add(benchPool[i]);
                            benchPool.RemoveAt(i--);
                        }
                    }
                }

                // These will get added to the bench group because of the GroupResolver we applied.
                foreach (string bench in benchPool)
                {
                    rb.AddItemByName(bench);
                    rb.AddLocationByName(bench);
                }
            }
            if (RandoInterop.NonrandomizedBenches != null)
            {
                // Maintaining vanilla is important for Can_Bench
                foreach (string bench in RandoInterop.NonrandomizedBenches)
                {
                    rb.AddToVanilla(bench, bench);
                }
            }
            
        }

        private static void DerangeBenches(RequestBuilder rb)
        {
            if (!RandoInterop.Settings.IsEnabled()) return;
            if (!rb.gs.CursedSettings.Deranged) return;

            foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
            {
                if (igb.strategy is DefaultGroupPlacementStrategy dgps)
                {
                    dgps.Constraints += (x, y) => !(BRData.IsBenchName(x.Name) && x.Name == y.Name);
                }
            }
        }
    }
}
