using Core.Attack;
using Core.Compensation;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Attack
{
    public class BulletSimulationSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletSimulationSystem));
        private readonly BulletHitSimulator _bulletSimulator;
        private readonly BulletEntityCollector _bulletEntityCollector;

        public BulletSimulationSystem(Contexts contexts, ICompensationWorldFactory compensationWorldFactory,
                                      IBulletHitHandler bulletHitHandler)
        {
            _bulletEntityCollector = new BulletEntityCollector(contexts.bullet, contexts.player);
            int layerMask = BulletLayers.GetBulletLayerMask();
            _bulletSimulator = new BulletHitSimulator(layerMask, compensationWorldFactory, bulletHitHandler,
                                                SharedConfig.BulletSimulationIntervalTime);
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            _bulletEntityCollector.BulletOwner = owner.OwnerEntityKey;
            _bulletSimulator.Update(cmd.RenderTime, cmd.Seq, _bulletEntityCollector.GetAllPlayerBulletAgents());
        }
    }
}