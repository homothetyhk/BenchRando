using RandomizerMod.Logging;

namespace BenchRando.Rando
{
    public class BenchLogger : RandoLogger
    {
        public override void Log(LogArguments args)
        {
            LogManager.Write(DoLog, "BenchSpoiler.json");
        }

        public void DoLog(TextWriter tw)
        {
            JsonUtil.Serialize(RandoInterop.LS, tw);
            RandoInterop.Clear(); // RandoInterop data should no longer be needed once logging is finished
        }
    }
}
