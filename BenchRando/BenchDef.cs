using Benchwarp;
using GlobalEnums;
using RandomizerCore.Logic;
using BenchRando.IC;
using BenchRando.Rando;

namespace BenchRando
{
    public record BenchDef
    {
        public string Name { get; init; }
        public string SceneName { get; init; }
        public string BenchAreaName { get; init; }
        public string BenchMenuName { get; init; }
        public MapZone MapZone { get; init; }
        public string DefaultStyle { get; init; } = "Right";
        public bool IsBaseBench { get; init; }
        public bool IsRandomizable { get; init; }
        public float X { get; init; }
        public float Y { get; init; }
        public FlingType FlingType { get; init; }
        public bool DreamGateRestricted { get; init; }
        public string Logic { get; init; }
        public RawLogicDef[] LogicOverrides { get; init; }
        public IDeployer[] ExtraDeployers { get; init; } = Array.Empty<IDeployer>();
        public IWritableBool[] ExtraUnlockActions { get; init; } = Array.Empty<IWritableBool>();

        public virtual string GetRespawnMarkerName()
        {
            return IsBaseBench
                ? Bench.baseBenches.First(b => b.sceneName == SceneName).respawnMarker
                : BenchDeployer.GetRespawnMarkerName(SceneName, X, Y);
        }

        public virtual string GetTermName()
        {
            return "Bench_Item_Term-" + Name.Substring(6);
        }

        public virtual string GetWaypointName()
        {
            return Name;
        }

        /// <summary>
        /// Creates a Bench from the BenchDef's data if not a base bench. Otherwise, retrieves the corresponding Benchwarp Bench definition.
        /// </summary>
        /// <returns></returns>
        public virtual Bench GetBenchwarpInfo()
        {
            if (!IsBaseBench)
            {
                return new Bench(BenchMenuName, BenchAreaName, SceneName, GetRespawnMarkerName(), 1, MapZone, DefaultStyle, default);
            }
            else return Bench.baseBenches.First(b => b.sceneName == SceneName);
        }

        /// <summary>
        /// Creates a BenchDeployerGroup from the BenchDef's data if not a base bench.
        /// </summary>
        /// <exception cref="NotImplementedException">The method is not overriden and IsBaseBench is true.</exception>
        public virtual BenchDeployerGroup GetBenchDeployerGroup()
        {
            if (!IsBaseBench)
            {
                return new()
                {
                    BenchDeployer = new()
                    {
                        SceneName = SceneName,
                        X = X,
                        Y = Y,
                        NearStyle = DefaultStyle,
                        FarStyle = DefaultStyle,
                    },
                    BenchwarpInfo = GetBenchwarpInfo(),
                    ExtraDeployers = ExtraDeployers.ToList(),
                    UnlockAllActions = ExtraUnlockActions.Select(wb => (IWritableBool)wb.Clone()).ToList(),
                };
            }
            else throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a Rando ItemDef from the BenchDef's data.
        /// </summary>
        public virtual BenchItemDef GetRMItemDef()
        {
            return new()
            {
                Name = Name,
                PriceCap = 500,
                MajorItem = false,
                Pool = "Benches"
            };
        }

        private static readonly Dictionary<string, string> randoSceneReductions = new()
        {
            [SceneNames.Room_Final_Boss_Atrium] = SceneNames.Room_temple,
            [SceneNames.GG_Atrium] = SceneNames.GG_Waterways,
            [SceneNames.GG_Atrium_Roof] = SceneNames.GG_Waterways,
            [SceneNames.GG_Workshop] = SceneNames.GG_Waterways,
            [SceneNames.Room_Tram] = SceneNames.Abyss_03_b,
            [SceneNames.Room_Tram_RG] = SceneNames.Crossroads_46,
        };

        /// <summary>
        /// Creates a Rando LocationDef from the BenchDef's data. The LocationDef has an additional property for the BenchArea of the bench.
        /// <br/>Attempts to project the BenchDef's scene to the nearest scene known to rando, if necessary.
        /// </summary>
        public virtual BenchLocationDef GetRMLocationDef()
        {
            return new()
            {
                Name = Name,
                SceneName = randoSceneReductions.TryGetValue(SceneName, out string altSceneName) ? altSceneName : SceneName,
                BenchArea = BenchAreaName,
                AdditionalProgressionPenalty = false,
                FlexibleCount = false,
            };
        }

        /// <summary>
        /// Creates an AbstractItem from the BenchDef's data.
        /// </summary>
        public virtual AbstractItem GetICItem()
        {
            return new BenchItem
            {
                name = Name,
                BenchKey = new(SceneName, GetRespawnMarkerName()),
                UIDef = new ItemChanger.UIDefs.MsgUIDef
                {
                    name = new LanguageString("UI", $"BENCHNAME.{Name}"),
                    shopDesc = new LanguageString("UI", "BENCH_SHOP_DESC"),
                    sprite = new ItemChangerSprite("ShopIcons.BenchPin"),
                },
                tags = ExtraUnlockActions.Select(wb => (Tag)new ItemChanger.Tags.SetIBoolOnGiveTag { Bool = wb })
                .Prepend(new ItemChanger.Tags.InteropTag
                {
                    Message = "RandoSupplementalMetadata",
                    Properties = new()
                    {
                        { "ModSource", "BenchRando" },
                        { "PoolGroup", "Benches" },
                    },
                })
                .ToList(),
            };
        }

        /// <summary>
        /// Creates an AbstractLocation from the BenchDef's data.
        /// </summary>
        public virtual AbstractLocation GetICLocation()
        {
            return new ItemChanger.Locations.ExistingFsmContainerLocation
            {
                containerType = "Bench",
                fsmName = "Bench Control",
                sceneName = SceneName,
                nonreplaceable = true,
                objectName = GetRespawnMarkerName(),
                flingType = FlingType,
                name = Name,
                tags = new()
                {
                    new ItemChanger.Tags.InteropTag
                    {
                        Message = "RandoSupplementalMetadata",
                        Properties = new()
                        {
                            { "ModSource", "BenchRando" },
                            { "PoolGroup", "Benches" },
                            { "VanillaItem", Name },
                        },
                    },
                }
            };
        }
    }
}
