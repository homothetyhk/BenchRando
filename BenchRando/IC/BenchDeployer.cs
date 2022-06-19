using Benchwarp;

namespace BenchRando.IC
{
    public record BenchDeployer : Deployer
    {
        public static string GetRespawnMarkerName(string sceneName, float x, float y)
        {
            return $"BenchDeployer-{sceneName}-({(int)x},{(int)y})";
        }

        public string NearStyle { get; init; }
        public string FarStyle { get; init; }
        public override GameObject Instantiate()
        {
            return ObjectCache.GetNewBench();
        }

        public override GameObject Deploy()
        {
            GameObject bench = Instantiate();
            bench.name = GetRespawnMarkerName(SceneName, X, Y);

            if (BenchStyle.IsValidStyle(FarStyle) && BenchStyle.GetStyle(FarStyle) is BenchStyle farStyle)
            {
                farStyle.ApplyDefaultSprite(bench);
            }
            else
            {
                BenchStyle.GetStyle("Right").ApplyDefaultSprite(bench);
            }

            if (BenchStyle.IsValidStyle(NearStyle) && BenchStyle.GetStyle(NearStyle) is BenchStyle nearStyle)
            {
                nearStyle.ApplyFsmAndPositionChanges(bench, new(X, Y, 0.02f));
                nearStyle.ApplyLitSprite(bench);
            }
            else
            {
                BenchStyle.GetStyle("Right").ApplyFsmAndPositionChanges(bench, new(X, Y, 0.02f));
                BenchStyle.GetStyle("Right").ApplyLitSprite(bench);
            }
            bench.SetActive(true);

            return bench;
        }
    }

}
