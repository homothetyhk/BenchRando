using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;

namespace BenchRando.Rando.SettingsInterop
{
    internal static class RandoSettingsManagerInterop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SimpleSettingsProxy<BenchRandomizationSettings>(
                BenchRandoMod.Instance,
                (s) => ConnectionMenu.Instance.brsMEF.SetMenuValues(s ?? new()),
                () => RandoInterop.IsEnabled() ? BenchRandoMod.GS.BenchRandomizationSettings : null
                ));
        }
    }
}
