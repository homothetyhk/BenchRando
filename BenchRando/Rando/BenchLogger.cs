using RandomizerMod.Logging;

namespace BenchRando.Rando
{
    public class BenchLogger : RandoLogger
    {
        public override void Log(LogArguments args)
        {
            LogManager.Write(tw => DoLog(tw, args), "BenchSpoiler.json");
        }

        public void DoLog(TextWriter tw, LogArguments args)
        {
            if (args.TryGetBRLocalSettings(out LocalSettings ls))
            {
                JsonUtil.Serialize(ls, tw);
            }
        }
    }
}
