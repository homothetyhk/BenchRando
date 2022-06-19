using Benchwarp;

namespace BenchRando.IC
{
    public class BenchDeployerGroup
    {
        public BenchDeployer BenchDeployer;
        public Bench BenchwarpInfo;
        public List<IDeployer> ExtraDeployers = new(0);
        public List<IWritableBool> UnlockAllActions = new(0);
    }
}
