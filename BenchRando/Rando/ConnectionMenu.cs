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

        private ConnectionMenu(MenuPage landingPage)
        {
            mainPage = new("BenchRando Main Page", landingPage);
            entryButton = new(landingPage, Localize("Bench Rando"));
            entryButton.AddHideAndShowEvent(mainPage);

            brsMEF = new(mainPage, BenchRandoMod.GS.BenchRandomizationSettings);
            Localize(brsMEF);
            brsVIP = new(mainPage, SpaceParameters.TOP_CENTER_UNDER_TITLE, SpaceParameters.VSPACE_MEDIUM, true, brsMEF.Elements);
        }
    }

}
