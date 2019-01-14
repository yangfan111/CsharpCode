using Core.GameModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.CharacterBone;
using Core.Prediction.UserPrediction.Cmd;
using App.Shared.GameModules.Player.CharacterState;
using Core.Utils;
using Utils.Singleton;

namespace App.Shared.GameModules.Player
{
    class PlayerSynchronizeToComponentSystem : IUserCmdExecuteSystem
    {
        void IUserCmdExecuteSystem.ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;

            var stateManager = playerEntity.stateInterface.State;

            ComponentSynchronizer.SyncToStateComponent(playerEntity.state, stateManager);
            ComponentSynchronizer.SyncToStateInterVarComponent(playerEntity.stateInterVar, stateManager);
            
        }
    }

    class PlayerSynchronizeFromComponentSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerSynchronizeFromComponentSystem));
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;

            var stateManager = playerEntity.stateInterface.State;

            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SyncStateVar);

            ComponentSynchronizer.SyncFromStateComponent(playerEntity.state, stateManager);
            ComponentSynchronizer.SyncFromStateInterVarComponent(playerEntity.stateInterVar, stateManager);
            
            ComponentSynchronizer.SyncToStateInterVarBeforeComponent(playerEntity.stateInterVarBefore, stateManager );
            ComponentSynchronizer.SyncToStateBeforeComponent(playerEntity.stateBefore, stateManager );
            
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SyncStateVar);
            
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.SyncFirePosition);

            //同步第一人称枪口位置
            CharacterBoneSynchronizer.SyncToFirePositionComponent(playerEntity.firePosition, playerEntity);
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.SyncFirePosition);

            //Logger.InfoFormat("PredictedAppearanceComponent:{0}, seq:{1}", playerEntity.predictedAppearance.ToString(), cmd.Seq);
        }
    }
    
    class ServerSynchronizeFromComponentSystem : IUserCmdExecuteSystem
    {
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;

            var stateManager = playerEntity.stateInterface.State;

            ComponentSynchronizer.SyncFromStateBeforeComponent(playerEntity.stateBefore, stateManager);
            ComponentSynchronizer.SyncFromStateInterVarBeforeComponent(playerEntity.stateInterVarBefore, stateManager);
        }

//        public void BeforeExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
//        {
//            ExecuteUserCmd(owner, cmd);
//        }
    }
}
