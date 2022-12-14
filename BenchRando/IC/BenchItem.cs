using BenchRando.Rando;
using Benchwarp;

namespace BenchRando.IC
{
    public class BenchItem : AbstractItem
    {
        public BenchKey BenchKey;
        private bool UnlocksWarp
        {
            get
            {
                if (ItemChangerMod.Modules.Get<BRLocalSettingsModule>() is BRLocalSettingsModule lsm && lsm.LS is LocalSettings ls)
                {
                    return ls.Settings.RandomizedItems switch
                    {
                        ItemRandoMode.RestUnlocks => false,
                        _ => true,
                    };
                }
                else
                {
                    return true;
                }
            }
        }

        protected override void OnLoad()
        {
            if (!WasEverObtained())
            {
                BenchwarpMod.LS.lockedBenches.Add(BenchKey);
            }
        }

        public override void GiveImmediate(GiveInfo info)
        {
            BenchwarpMod.LS.lockedBenches.Remove(BenchKey);
            if (UnlocksWarp)
            {
                BenchwarpMod.LS.visitedBenchScenes.Add(BenchKey);
            }
        }

        public override bool Redundant()
        {
            if (BenchwarpMod.LS.lockedBenches.Contains(BenchKey)) return false;
            if (UnlocksWarp && !BenchwarpMod.LS.visitedBenchScenes.Contains(BenchKey)) return false;
            return true;
        }
    }
}
