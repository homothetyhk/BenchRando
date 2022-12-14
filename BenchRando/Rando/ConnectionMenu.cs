using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace BenchRando.Rando
{
    internal class ConnectionMenu
    {
        public static ConnectionMenu Instance { get; private set; }

        public static void Setup()
        {
            RandomizerMenuAPI.AddMenuPage(OnRandomizerMenuConstruction, TryGetMenuButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        public static void OnRandomizerMenuConstruction(MenuPage page)
        {
            Instance = new(page);
        }

        public static bool TryGetMenuButton(MenuPage page, out SmallButton button)
        {
            button = Instance.entryButton;
            return true;
        }

        public SmallButton entryButton;
        public MenuPage mainPage;
        public MenuElementFactory<BenchRandomizationSettings> brsMEF;
        public VerticalItemPanel brsVIP;
        public SmallButton defaultSettingsButton;

        private ConnectionMenu(MenuPage landingPage)
        {
            mainPage = new("BenchRando Main Page", landingPage);
            entryButton = new(landingPage, Localize("Bench Rando"));
            entryButton.AddHideAndShowEvent(mainPage);

            brsMEF = new(mainPage, BenchRandoMod.GS.BenchRandomizationSettings);
            Localize(brsMEF);

            IMenuElement[] slider1Contents = new IMenuElement[]
            {
                brsMEF.ElementLookup[nameof(BenchRandomizationSettings.MinimumBenchCount)], brsMEF.ElementLookup[nameof(BenchRandomizationSettings.MaximumBenchCount)]
            };

            IMenuElement[] slider2Contents = new IMenuElement[]
            {
                brsMEF.ElementLookup[nameof(BenchRandomizationSettings.MinimumBenchesPerArea)], brsMEF.ElementLookup[nameof(BenchRandomizationSettings.MaximumBenchesPerArea)]
            };

            defaultSettingsButton = new(mainPage, "Restore Default Settings");
            defaultSettingsButton.OnClick += () =>
            {
                brsMEF.SetMenuValues(new());
            };

            GridItemPanel slider1 = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, 2, SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_MEDIUM, false, slider1Contents);
            GridItemPanel slider2 = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, 2, SpaceParameters.VSPACE_MEDIUM, SpaceParameters.HSPACE_MEDIUM, false, slider2Contents);

            brsVIP = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM, true,
                brsMEF.Elements
                .Cast<IMenuElement>()
                .Except(slider1Contents)
                .Except(slider2Contents)
                .Except(brsMEF.ElementLookup[nameof(BenchRandomizationSettings.BenchGroup)].Yield<IMenuElement>())
                .Append(slider1)
                .Append(slider2)
                .Append(brsMEF.ElementLookup[nameof(BenchRandomizationSettings.BenchGroup)])
                .Append(defaultSettingsButton)
                .ToArray());
        }
    }

}
