using System;
using System.Collections.Generic;
using App.Shared.FreeFramework.Free.Chicken;
using Core;
using Core.Utils;
using Entitas.Utils;
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
            ResisterHoldWeapon();
        }
        private void ResisterHoldWeapon()
        {
            var handler = new HoldWeaponHandler(playerEntity);
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Drive, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Swim, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.Climb, EInterruptCmdType.InterruptAndRollback));
         


            interruptHandlers[(int)EInterruptType.HoldWeapon] = handler;
        }
        private void ResisterGunSight()
        {
            var handler = new SightInterruptHandler(playerEntity);
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.OpenUI, EInterruptCmdType.InterruptSimple));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.WeaponRotState, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.ProneMove, EInterruptCmdType.InterruptAndRollback));
            handler.ResisterEmitter(new InterruptEmitter(EPlayerState.PostureTrans, EInterruptCmdType.InterruptAndRollback));
            interruptHandlers[(int)EInterruptType.GunSight] = handler;
        }
        LoggerAdapter logger = new LoggerAdapter("State");
        public void DoRunTimeInterrupt(HashSet<EPlayerState> states, bool cmdIsInterrupt)
        {
        
            foreach (var handler in interruptHandlers)
            {
              
                if (handler != null)
                    handler.Update(states);
            }

            if (cmdIsInterrupt)
            {
                InterruptCharactor();
            }
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