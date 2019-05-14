using System.Collections.Generic;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{


    public class PlayerStateRuntimeInterrupter
    {
        private InterruptEventHandler[] interruptHandlers;

        private PlayerEntity playerEntity;

        public PlayerStateRuntimeInterrupter(PlayerEntity playerEntity)
        {
            this.playerEntity = playerEntity;
            interruptHandlers = new InterruptEventHandler[(int)EInterruptType.Count];
            ResisterGunSight();
           // ResisterPullBolt();
            ResisterHoldWeapon();
        }
        private void ResisterHoldWeapon()
        {
            var handler = new HoldWeaponHandler(playerEntity);
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Dying, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Drive, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Swim, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Climb, EInterruptCmdType.InterruptAndRollback));
         


            interruptHandlers[(int)EInterruptType.HoldWeapon] = handler;
        }

        private void ResisterPullBolt()
        {
//            var handler = new PullboltHandler(playerEntity);
//            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.PullBoltInterrupt, EInterruptCmdType.InterruptSimple));
//            interruptHandlers[(int)EInterruptType.Pullbolt] = handler;
        }
        private void ResisterGunSight()
        {
            var handler = new SightInterruptHandler(playerEntity);
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.OpenUI, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Reload, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.SpecialReload, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.PaintDisc, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.SwitchWeapon, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.WeaponRotState, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.ProneMove, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.PostureTrans, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.PullBolt, EInterruptCmdType.InterruptAndRollback));

            
            interruptHandlers[(int)EInterruptType.GunSight] = handler;
        }
        LoggerAdapter logger = new LoggerAdapter("State");
        public void DoRunTimeInterrupt(HashSet<EPlayerState> states, IUserCmd cmd)
        {
        
            foreach (var handler in interruptHandlers)
            {
              
                if (handler != null)
                    handler.Update(states);
            }

            if (cmd.IsInterrupt)
            {
                InterruptCharactor();
            }
        }

        public bool IsInterrupted(EInterruptType interruptType)
        {
          return  interruptHandlers[(int)interruptType].IsInterrupted();
        }

        public void InterruptCharactor()
        {
            var RelatedCharState = playerEntity.stateInterface.State;
            RelatedCharState.InterruptAction();
            RelatedCharState.InterruptSwitchWeapon();
            RelatedCharState.ForceBreakSpecialReload(null);
            RelatedCharState.ForceFinishGrenadeThrow();
            PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, playerEntity.gamePlay);
            if (playerEntity.hasThrowingAction)
                playerEntity.throwingAction.ActionInfo.ClearState();
        }

    }
}