using BenchRando.IC;
using BenchRando.Rando;
using Modding;

namespace BenchRando
{

    public class BenchRandoMod : Mod, IGlobalSettings<GlobalSettings>
    {
        public static BenchRandoMod Instance { get; private set; }
        public static GlobalSettings GS { get; private set; } = new();

        public BenchRandoMod() : base("BenchRando")
        {
            Instance = this;
        }

        public override void Initialize()
        {
            DefineICRefs();

            if (ModHooks.GetMod("Randomizer 4") is Mod)
            {
                RandoInterop.Setup();
            }
        }

        public static void DefineICRefs()
        {
            Container.DefineContainer<BenchContainer>();
            BRData.Setup();
            Finder.GetItemOverride += args =>
            {
                if (BRData.BenchLookup.TryGetValue(args.ItemName, out BenchDef def))
                {
                    args.Current = def.GetICItem();
                }
            };
            Finder.GetLocationOverride += args =>
            {
                if (BRData.BenchLookup.TryGetValue(args.LocationName, out BenchDef def))
                {
                    args.Current = def.GetICLocation();
                }
            };

            LanguageData.Load();
            Events.OnItemChangerHook += LanguageData.Hook;
            Events.OnItemChangerUnhook += LanguageData.Unhook;
        }

        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new();
        }

        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GS ?? new();
        }

        public static string Version { get; }
        public override string GetVersion()
        {
            return Version;
        }

        static BenchRandoMod()
        {
            Version v = typeof(BenchRandoMod).Assembly.GetName().Version;
            Version = $"{v.Major}.{v.Minor}.{v.Build}";

            // preload is required, for safely loading IC saves
            Benchwarp.Benchwarp.GS.NoPreload = false;
        }
    }
}
