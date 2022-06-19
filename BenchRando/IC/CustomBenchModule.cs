using Benchwarp;
using Newtonsoft.Json;
using Module = ItemChanger.Modules.Module;

namespace BenchRando.IC
{
    public class CustomBenchModule : Module
    {
        [JsonProperty] private Dictionary<string, BenchDeployerGroup> injectedBenches = new();
        private bool loaded;

        public override void Initialize()
        {
            Benchwarp.Events.BenchInjectors += InjectBenches;
            Benchwarp.Events.OnBenchwarp += HandleUnlockAll;
            foreach (BenchDeployerGroup bdg in injectedBenches.Values)
            {
                ItemChanger.Events.AddSceneChangeEdit(bdg.BenchDeployer.SceneName, bdg.BenchDeployer.OnSceneChange);
                foreach (IDeployer deployer in bdg.ExtraDeployers)
                {
                    ItemChanger.Events.AddSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                }
            }
            loaded = true;
        }

        public override void Unload()
        {
            Benchwarp.Events.BenchInjectors -= InjectBenches;
            Benchwarp.Events.OnBenchwarp -= HandleUnlockAll;
            foreach (BenchDeployerGroup bdg in injectedBenches.Values)
            {
                ItemChanger.Events.RemoveSceneChangeEdit(bdg.BenchDeployer.SceneName, bdg.BenchDeployer.OnSceneChange);
                foreach (IDeployer deployer in bdg.ExtraDeployers)
                {
                    ItemChanger.Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                }
            }
            loaded = false;
        }

        private void HandleUnlockAll()
        {
            BenchKey key = new(PlayerData.instance.GetString(nameof(PlayerData.respawnScene)), PlayerData.instance.GetString(nameof(PlayerData.respawnMarkerName)));

            if (Benchwarp.Benchwarp.GS.UnlockAllBenches
                && !Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(key)
                && injectedBenches.Values.FirstOrDefault(b => b.BenchwarpInfo.ToBenchKey() == key) is BenchDeployerGroup bdg)
            {
                if (bdg.UnlockAllActions != null)
                {
                    foreach (IWritableBool wb in bdg.UnlockAllActions)
                    {
                        wb.Value = true;
                    }
                }
            }
        }

        private IEnumerable<Bench> InjectBenches()
        {
            return injectedBenches.Values.Select(bdg => bdg.BenchwarpInfo);
        }

        public void AddBench(string key, BenchDeployerGroup bdg)
        {
            injectedBenches.Add(key, bdg);
            if (loaded)
            {
                ItemChanger.Events.AddSceneChangeEdit(bdg.BenchDeployer.SceneName, bdg.BenchDeployer.OnSceneChange);
                foreach (IDeployer deployer in bdg.ExtraDeployers)
                {
                    ItemChanger.Events.AddSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                }
            }
            Bench.RefreshBenchList();
        }

        public void RemoveBench(string key)
        {
            if (loaded && injectedBenches.TryGetValue(key, out BenchDeployerGroup bdg))
            {
                ItemChanger.Events.RemoveSceneChangeEdit(bdg.BenchDeployer.SceneName, bdg.BenchDeployer.OnSceneChange);
                foreach (IDeployer deployer in bdg.ExtraDeployers)
                {
                    ItemChanger.Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                }
            }
            injectedBenches.Remove(key);
            Bench.RefreshBenchList();
        }
    }
}
