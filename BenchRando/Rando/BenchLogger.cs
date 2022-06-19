using RandomizerMod.Logging;

namespace BenchRando.Rando
{
    public class BenchLogger : RandoLogger
    {
        private class SerializationData
        {
            public List<string> NonrandomizedBenches;
            public List<string> RandomizedBenches;
        }

        public override void Log(LogArguments args)
        {
            LogManager.Write(DoLog, "benches.json");
        }

        public void DoLog(TextWriter tw)
        {
            JsonUtil.Serialize(new SerializationData
            {
                NonrandomizedBenches = RandoInterop.NonrandomizedBenches,
                RandomizedBenches = RandoInterop.RandomizedBenches,
            }, tw);
            RandoInterop.Clear(); // RandoInterop data should no longer be needed once logging is finished
        }
    }
}
