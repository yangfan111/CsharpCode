

using App.Shared;
using App.Shared.Components.Player;
using App.Shared.EntityFactory;
using Assets.Sources;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Client.GameModules.Player
{
    class RobotUserCmdProviderInitSystem:IUserCmdExecuteSystem
    {
    
        private Contexts _contexts;
        public RobotUserCmdProviderInitSystem(Contexts contexts)
        {
          
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (!SharedConfig.IsRobot) return;
            var player = owner.OwnerEntity as PlayerEntity;
            if (player.hasRobot) return;
            var userCmdGenerator = _contexts.session.clientSessionObjects.UserCmdGenerator;
            if (userCmdGenerator is IRobotUserCmdProviderContainer)
            {
                PlayerEntityFactory.CreateRobotPlayerEntity(_contexts, player, new DummyRobotConfig(), (userCmdGenerator as IRobotUserCmdProviderContainer).RobotUserCmdProvider,userCmdGenerator);
            }
        }
    }
}
