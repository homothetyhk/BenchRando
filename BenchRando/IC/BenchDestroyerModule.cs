using Benchwarp;
using ItemChanger.Deployers;
using ItemChanger.Modules;
using Newtonsoft.Json;

namespace BenchRando.IC
{
    public class BenchDestroyerModule : Module
    {
        [JsonProperty] private HashSet<BenchKey> destroyedBenches = new();
        private Dictionary<BenchKey, List<ObjectDestroyer>> benchDestroyers = new();
        private bool loaded;

        public override void Initialize()
        {
            Benchwarp.Events.BenchSuppressors += ShouldSuppressBench;
            foreach (BenchKey key in destroyedBenches)
            {
                if (!benchDestroyers.TryGetValue(key, out List<ObjectDestroyer> deployers))
                {
                    benchDestroyers.Add(key, deployers = BuildDestroyerList(key));
                }
                foreach (IDeployer deployer in deployers)
                {
                    ItemChanger.Events.AddSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                }
            }
            On.PlayerData.SetBenchRespawn_RespawnMarker_string_int += OnSetBenchRespawn1;
            On.PlayerData.SetBenchRespawn_string_string_bool += OnSetBenchRespawn2;
            On.PlayerData.SetBenchRespawn_string_string_int_bool += OnSetBenchRespawn3;
            ItemChanger.Events.AddFsmEdit(SceneNames.Room_Colosseum_Bronze, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            ItemChanger.Events.AddFsmEdit(SceneNames.Room_Colosseum_Silver, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            ItemChanger.Events.AddFsmEdit(SceneNames.Room_Colosseum_Gold, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            loaded = true;
        }

        public override void Unload()
        {
            Benchwarp.Events.BenchSuppressors -= ShouldSuppressBench;
            foreach (BenchKey key in destroyedBenches)
            {
                if (benchDestroyers.TryGetValue(key, out List<ObjectDestroyer> deployers))
                {
                    foreach (IDeployer deployer in deployers)
                    {
                        ItemChanger.Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                    }
                }
            }
            On.PlayerData.SetBenchRespawn_RespawnMarker_string_int -= OnSetBenchRespawn1;
            On.PlayerData.SetBenchRespawn_string_string_bool -= OnSetBenchRespawn2;
            On.PlayerData.SetBenchRespawn_string_string_int_bool -= OnSetBenchRespawn3;
            ItemChanger.Events.RemoveFsmEdit(SceneNames.Room_Colosseum_Bronze, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            ItemChanger.Events.RemoveFsmEdit(SceneNames.Room_Colosseum_Silver, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            ItemChanger.Events.RemoveFsmEdit(SceneNames.Room_Colosseum_Gold, new("Colosseum Manager", "Manager"), RemoveColoSetRespawn);
            loaded = false;
        }

        

        public void DestroyBench(BenchKey key)
        {
            destroyedBenches.Add(key);
            if (!benchDestroyers.ContainsKey(key))
            {
                List<ObjectDestroyer> deployers = BuildDestroyerList(key);
                benchDestroyers.Add(key, deployers);
                if (loaded)
                {
                    foreach (IDeployer deployer in deployers)
                    {
                        ItemChanger.Events.AddSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                    }
                }
            }

            Bench.RefreshBenchList();
        }

        public void UndestroyBench(BenchKey key)
        {
            destroyedBenches.Remove(key);
            if (benchDestroyers.TryGetValue(key, out List<ObjectDestroyer> deployers))
            {
                benchDestroyers.Remove(key);
                if (loaded)
                {
                    foreach (IDeployer deployer in deployers)
                    {
                        ItemChanger.Events.RemoveSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
                    }
                }
            }
            Bench.RefreshBenchList();
        }

        private static List<ObjectDestroyer> BuildDestroyerList(BenchKey key)
        {
            Bench b = Bench.baseBenches.FirstOrDefault(b => b.ToBenchKey() == key);
            if (b is null) return new(0);

            List<ObjectDestroyer> deployers = new(1);
            deployers.Add(new ObjectDestroyer
            {
                MatchType = ObjectDestroyer.NameMatchType.FirstName,
                ObjectName = b.respawnMarker,
                SceneName = b.sceneName,
            });
            if (key == new BenchKey(SceneNames.Deepnest_East_13, "RestBench"))
            {
                deployers.Add(new ObjectDestroyer
                {
                    MatchType = ObjectDestroyer.NameMatchType.FirstName,
                    ObjectName = "outskirts__0003_camp",
                    SceneName = b.sceneName,
                });
            }
            else if (key == new BenchKey(SceneNames.Fungus1_24, "RestBench"))
            {
                deployers.Add(new ObjectDestroyer
                {
                    MatchType = ObjectDestroyer.NameMatchType.FirstName,
                    ObjectName = "guardian_bench",
                    SceneName = b.sceneName,
                });
            }
            else if (key == new BenchKey(SceneNames.Mines_18, "RestBench"))
            {
                deployers.Add(new ObjectDestroyer
                {
                    MatchType = ObjectDestroyer.NameMatchType.Path,
                    ObjectName = "Dummy Bench",
                    SceneName = SceneNames.Mines_18,
                });
            }
            else if (key == new BenchKey(SceneNames.Ruins1_31, "RestBench")
                || key == new BenchKey(SceneNames.Fungus3_50, "RestBench")
                || key == new BenchKey(SceneNames.Abyss_18, "RestBench"))
            {
                deployers.Add(new ObjectDestroyer
                {
                    MatchType = ObjectDestroyer.NameMatchType.Path,
                    ObjectName = "Toll Machine Bench",
                    SceneName = key.SceneName,
                });
            }

            return deployers;
        }

        private bool ShouldSuppressBench(Bench bench)
        {
            return destroyedBenches.Contains(bench.ToBenchKey());
        }

        private void OnSetBenchRespawn1(On.PlayerData.orig_SetBenchRespawn_RespawnMarker_string_int orig, PlayerData self, RespawnMarker spawnMarker, string sceneName, int spawnType)
        {
            if (spawnMarker == null) return;
            BenchKey key = new(sceneName, spawnMarker.name);
            if (destroyedBenches.Contains(key)) return;
            orig(self, spawnMarker, sceneName, spawnType);
        }

        private void OnSetBenchRespawn2(On.PlayerData.orig_SetBenchRespawn_string_string_bool orig, PlayerData self, string spawnMarker, string sceneName, bool facingRight)
        {
            BenchKey key = new(sceneName, spawnMarker);
            if (destroyedBenches.Contains(key)) return;
            orig(self, spawnMarker, sceneName, facingRight);
        }

        private void OnSetBenchRespawn3(On.PlayerData.orig_SetBenchRespawn_string_string_int_bool orig, PlayerData self, string spawnMarker, string sceneName, int spawnType, bool facingRight)
        {
            BenchKey key = new(sceneName, spawnMarker);
            if (destroyedBenches.Contains(key)) return;
            orig(self, spawnMarker, sceneName, spawnType, facingRight);
        }

        private void RemoveColoSetRespawn(PlayMakerFSM fsm)
        {
            if (!destroyedBenches.Contains(new(SceneNames.Room_Colosseum_02, "RestBench"))) return;
            fsm.GetState("Set Respawn")?.ClearActions();
        }
    }
}
