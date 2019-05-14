using System.Collections.Generic;
using System.Text;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerStateInteractController : ModuleLogicActivator<PlayerStateInteractController>,
                                                 IPlayerStateFiltedInputMgr,
                                                 IPlayerStateColltector, IPlayerStateInterrupter
    {
        private PlayerStateRuntimeInterrupter stateInterrupter;
        private PlayerStateCollector          stateCollector;
        private PlayerStateFiltedInputMgr     inputManager;

        public void Initialize(PlayerEntity playerEntity)
        {
            stateCollector   = new PlayerStateCollector(playerEntity);
            stateInterrupter = new PlayerStateRuntimeInterrupter(playerEntity);
            //   playerStateCollectorPool.AddStateCollector(player.entityKey.Value, stateCollector); 
            inputManager = new PlayerStateFiltedInputMgr(stateCollector);
        }


        public void DoRunTimeInterrupt(IUserCmd cmd)
        {
            var states = stateCollector.GetCurrStates(EPlayerStateCollectType.UseCacheAddation);
#if UNITY_EDITOR
            if(GlobalConst.EnableStateLog)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var s in states)
                {
                    stringBuilder.Append(s);
                    stringBuilder.Append('|');
                }
                DebugUtil.MyLog(stringBuilder);
            }
#endif
            stateInterrupter.DoRunTimeInterrupt(states, cmd);
        }

        public void InterruptCharactor()
        {
            stateInterrupter.InterruptCharactor();
        }

        public bool IsInterrupted(EInterruptType interruptType)
        {
            return stateInterrupter.IsInterrupted(interruptType);
        }

        public HashSet<EPlayerState> GetCurrStates(
            EPlayerStateCollectType collectType = EPlayerStateCollectType.UseCache)
        {
            return stateCollector.GetCurrStates();
        }


        public IFilteredInput EmptyInput
        {
            get { return inputManager.EmptyInput; }
        }

        public IFilteredInput UserInput
        {
            get { return inputManager.UserInput; }
        }

        public IFilteredInput ApplyUserCmd(IUserCmd userCmd)
        {
            return inputManager.ApplyUserCmd(userCmd);
        }
    }
}