using Modding;
using Newtonsoft.Json;

namespace BenchRando.IC
{
    public record LanguageData
    {
        public string Sheet { get; init; }
        public string Key { get; init; }
        public string Value { get; init; }

        private static readonly Dictionary<LanguageKey, string> _languageStrings = new();


        internal static void Load()
        {
            JsonSerializer js = new()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };
            using Stream s = typeof(LanguageData).Assembly.GetManifestResourceStream("BenchRando.Resources.language.json");
            using StreamReader sr = new(s);
            using JsonTextReader jtr = new(sr);

            List<LanguageData> rawLanguageEntries = js.Deserialize<List<LanguageData>>(jtr);
            foreach (LanguageData entry in rawLanguageEntries)
            {
                LanguageKey key = new(entry.Sheet, entry.Key);
                _languageStrings.Add(key, entry.Value);
            }
        }

        internal static void Hook() => ModHooks.LanguageGetHook += OverrideLanguageString;
        internal static void Unhook() => ModHooks.LanguageGetHook -= OverrideLanguageString;

        private static string OverrideLanguageString(string key, string sheetTitle, string orig)
        {
            // If orig has already been overridden, then it was probably an ItemChanger language override
            if (orig != Language.Language.GetInternal(key, sheetTitle)) return orig;

            LanguageKey obj = new(sheetTitle, key);
            return _languageStrings.TryGetValue(obj, out string overrideValue) ? overrideValue : orig;
        }
    }
}
