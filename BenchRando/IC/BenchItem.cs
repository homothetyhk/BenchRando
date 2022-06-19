using Benchwarp;

namespace BenchRando.IC
{
    public class BenchItem : AbstractItem
    {
        public BenchKey BenchKey;

        protected override void OnLoad()
        {
            if (!WasEverObtained())
            {
                Benchwarp.Benchwarp.LS.lockedBenches.Add(BenchKey);
            }
        }

        public override void GiveImmediate(GiveInfo info)
        {
            Benchwarp.Benchwarp.LS.lockedBenches.Remove(BenchKey);
            Benchwarp.Benchwarp.LS.visitedBenchScenes.Add(BenchKey);
        }

        public override bool Redundant()
        {
            return Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(BenchKey);
        }
    }
}
