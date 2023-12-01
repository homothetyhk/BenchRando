﻿using MenuChanger;
using Modding;
using Newtonsoft.Json;
using RandomizerMod.Logging;
using RandomizerMod.RC;

namespace BenchRando.Rando
{
    public static class RandoInterop
    {
        public static LocalSettings LS;

        /// <summary>
        /// Hooks various modifiers and loggers to allow BR to modify randomization.
        /// </summary>
        public static void Setup()
        {
            ConnectionMenu.Setup();
            LogicPatcher.Setup();
            RequestModifier.Setup();
            RandoController.OnExportCompleted += Finish;
            RandoController.OnCreateLogArguments += AddLocalSettingsToLogArguments;
            SettingsLog.AfterLogSettings += OnLogSettings;
            LogManager.AddLogger(new BenchLogger());
            CondensedSpoilerLogger.AddCategory("Benches", args => true, new(BRData.EmbeddedBenchData.Keys));
            SettingsInterop.RandoSettingsManagerInterop.Hook();
            MenuChangerMod.OnExitMainMenu += Clear;
        }

        internal const string BenchRandoSettingsKey = "BenchRandoSettings";

        /// <summary>
        /// Disposes local settings once no longer needed to prevent accidental access.
        /// </summary>
        public static void Clear()
        {
            LS = null;
        }

        /// <summary>
        /// Returns true when bench spot randomization or bench item randomization is active.
        /// </summary>
        public static bool IsEnabled()
        {
            return BenchRandoMod.GS.BenchRandomizationSettings.IsEnabled();
        }

        /// <summary>
        /// Called during LogicPatcher to setup BRData and LS prior to injecting logic and randomization.
        /// <br/>The constructor of LS handles randomizing bench spots and styles, using the BRData bench list and the parameter seed.
        /// </summary>
        public static void Initialize(int seed)
        {
            BRData.Reset();
            BRData.Setup();
            LS = new(seed);
        }

        /// <summary>
        /// Creates various BR modules during Export.
        /// </summary>
        public static void Finish(RandoController rc)
        {
            if (!IsEnabled()) return;

            ItemChangerMod.Modules.GetOrAdd<IC.BRLocalSettingsModule>().LS = LS;
            ItemChangerMod.Modules.GetOrAdd<ItemChanger.Modules.PlayerDataEditModule>()
                .AddPDEdit(nameof(PlayerData.charmBenchMsg), true);

            if (LS.Settings.RandomizeBenchSpots)
            {
                IC.BenchDestroyerModule bdm = ItemChangerMod.Modules.GetOrAdd<IC.BenchDestroyerModule>();
                foreach (string s in BRData.BenchLookup.Values.Where(b => b.IsBaseBench).Select(b => b.Name).Except(LS.Benches))
                {
                    bdm.DestroyBench(BRData.BenchLookup[s].GetBenchwarpInfo().ToBenchKey());
                }
                IC.CustomBenchModule cbm = ItemChangerMod.Modules.GetOrAdd<IC.CustomBenchModule>();
                foreach (BenchDef def in LS.Benches.Select(b => BRData.BenchLookup[b]).Where(b => !b.IsBaseBench))
                {
                    IC.BenchDeployerGroup bdg = def.GetBenchDeployerGroup();
                    if (LS.ModifiedNearStyles != null && LS.ModifiedNearStyles.TryGetValue(def.Name, out string nearStyle))
                    {
                        bdg.BenchDeployer = bdg.BenchDeployer with { NearStyle = nearStyle };
                    }
                    if (LS.ModifiedFarStyles != null && LS.ModifiedFarStyles.TryGetValue(def.Name, out string farStyle))
                    {
                        bdg.BenchDeployer = bdg.BenchDeployer with { FarStyle = farStyle };
                    }
                    cbm.AddBench(def.Name, bdg);
                }
            }
        }

        private static void AddLocalSettingsToLogArguments(LogArguments args)
        {
            args.properties[BenchRandoSettingsKey] = LS;
        }

        private static void OnLogSettings(LogArguments args, TextWriter tw)
        {
            if (args.TryGetBRLocalSettings(out LocalSettings ls) && ls.Settings is BenchRandomizationSettings randoSettings)
            {
                tw.WriteLine("Logging BenchRando BenchRandomizationSettings:");
                using JsonTextWriter jtw = new(tw) { CloseOutput = false };
                JsonUtil._js.Serialize(jtw, randoSettings);
                tw.WriteLine();
            }
        }

        internal static bool TryGetBRLocalSettings(this LogArguments args, out LocalSettings ls)
        {
            ls = null;

            if (args is null) return false;

            if (!args.properties.TryGetValue(BenchRandoSettingsKey, out object objLs)) return false;

            ls = objLs as LocalSettings;
            return ls is not null;
        }
    }
}
