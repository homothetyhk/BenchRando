using Modding;
using MonoMod.ModInterop;
#pragma warning disable CS0649

namespace BenchRando.Rando.SettingsInterop
{
    [ModImportName("RandoSettingsManager")]
    internal static class RandoSettingsManagerInterop
    {
        public static Action<Mod, Type, Delegate, Delegate> RegisterConnectionSimple;
        public static void Hook() => RegisterConnectionSimple?.Invoke(BenchRandoMod.Instance, typeof(BenchRandomizationSettings), ReceiveSettings, ProvideSettings);
        private static void ReceiveSettings(BenchRandomizationSettings s) => ConnectionMenu.Instance.brsMEF.SetMenuValues(s ?? new());
        private static BenchRandomizationSettings ProvideSettings() => RandoInterop.IsEnabled() ? BenchRandoMod.GS.BenchRandomizationSettings : null;
        static RandoSettingsManagerInterop() => typeof(RandoSettingsManagerInterop).ModInterop();
    }
}
