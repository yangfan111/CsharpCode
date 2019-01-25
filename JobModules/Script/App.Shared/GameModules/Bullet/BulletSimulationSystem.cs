using Core.BulletSimulation;
using Core.Compensation;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Bullet
{
    public class BulletSimulationSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletSimulationSystem));
        private readonly BulletHitSimulator _simulator;
        private BulletEntityCollector _bulletEntityCollector;

        public BulletSimulationSystem(
            Contexts contexts, 
            ICompensationWorldFactory compensationWorldFactory,
            IBulletHitHandler bulletHitHandler)
        {
            _bulletEntityCollector =
                new BulletEntityCollector(contexts.bullet, contexts.player);

            var layerMask = BulletLayers.GetBulletLayerMask();
                        
            _simulator = new BulletHitSimulator(layerMask, _bulletEntityCollector, compensationWorldFactory,
                bulletHitHandler, SharedConfig.BulletSimulationIntervalTime);
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            _bulletEntityCollector.OwnerEntityKey = owner.OwnerEntityKey;
            _simulator.Update(cmd.RenderTime, cmd.Seq);
        }
    }
}