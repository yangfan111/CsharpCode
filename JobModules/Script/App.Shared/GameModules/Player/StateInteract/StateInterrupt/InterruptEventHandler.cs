using System;
using System.Collections.Generic;
using Core;
using Core.Utils;
using XmlConfig;
using App.Shared.Components.Player;

namespace App.Shared.GameModules.Player
{
    public class InterruptEventHandler
    {
        protected Action                            interruptAction;
        protected Action                            recoverAction;
        public  Func<bool>                        filterFunc;
        protected Func<HashSet<EPlayerState>, bool> recoverFunc;
        List<InterruptEmitter> emitterList = new List<InterruptEmitter>();

        private EInterruptState interruptState;
        //        private EInterruptCmdType cmdType;
        private PlayerEntity player;

        public InterruptEventHandler(PlayerEntity playerEntity)
        {
            player = playerEntity;
            interruptState = EInterruptState.Closed;
        }

        public bool IsInterrupted()
        {
            return interruptState == EInterruptState.WaitRecover;
        }
        public void ResisterEmitter(InterruptEmitter emitter)
        {
            emitter.SetHanlder(this);
            emitterList.Add(emitter);
        }
        private void Trigger(HashSet<EPlayerState> states)
        {
            if (!filterFunc())
                return;
            foreach(var emitter in emitterList)
            {
                if (emitter.Trigger(states))
                    break;
            }
        }
        public void Update(HashSet<EPlayerState> states)
        {
            Trigger(states);
            if (interruptState == EInterruptState.Closed)
                return;
            if (states.Contains(EPlayerState.Dead)/* || states.Contains(EPlayerState.Dying)*/)
            {
                interruptState = EInterruptState.Closed;
                recoverFunc    = null;
                return;
            }

            switch (interruptState)
            {
                case EInterruptState.WaitInterrupt:
                    Interrupt();
                    if (recoverFunc == null)
                        interruptState = EInterruptState.Closed;
                    else
                        interruptState = EInterruptState.WaitRecover;
                    break;
                case EInterruptState.WaitRecover:
                    if (player.hasGamePlay &&
                        (player.gamePlay.JobAttribute == (int)EJobAttribute.EJob_Matrix ||
                        player.gamePlay.JobAttribute == (int)EJobAttribute.EJob_Variant)) {
                        interruptState = EInterruptState.Closed;
                        return;
                    }

                    if (recoverFunc(states))
                    {
                        Recover();
                        interruptState = EInterruptState.Closed;
                    }
                    break;
            }
        }

        public void Interrupt(Func<HashSet<EPlayerState>, bool> recoverFunc = null)
        {
            interruptState   = EInterruptState.WaitInterrupt;
            this.recoverFunc = recoverFunc;
        }

        private void Interrupt()
        {
            DebugUtil.MyLog("Interrupt");
            interruptAction();
        }
        private void Recover()
        {
            DebugUtil.MyLog("Recover");

            if (!filterFunc() && recoverAction != null)
                recoverAction();
        }
    }

    public class SightInterruptHandler : InterruptEventHandler
    {
        public SightInterruptHandler(PlayerEntity playerEntity) : base(playerEntity)
        {
            
            interruptAction = () => playerEntity.StateInteractController().UserInput.SetInput(EPlayerInput.IsCameraFocus, true);
            recoverAction   = () => playerEntity.StateInteractController().UserInput.SetInput(EPlayerInput.IsCameraFocus, true);
            filterFunc      = playerEntity.cameraStateNew.IsAiming;
        }

       
    }
    public class HoldWeaponHandler : InterruptEventHandler
    {
        private EWeaponSlotType recoveredSlotType;
        public HoldWeaponHandler (PlayerEntity playerEntity) : base(playerEntity)
        {
            var weaponController = playerEntity.WeaponController();
            interruptAction = () =>  recoveredSlotType = weaponController.UnArmWeapon(false);
            recoverAction   = () =>  weaponController.ArmWeapon(recoveredSlotType,true);
            filterFunc      = () =>  !weaponController.IsHeldSlotEmpty;
        }

       
    }
//    public class PullboltHandler : InterruptEventHandler
//    {
//        public PullboltHandler (PlayerEntity playerEntity) : base(playerEntity)
//        {
//            var weaponController = playerEntity.WeaponController();
//            interruptAction = () =>  weaponController.HeldWeaponAgent.InterruptPullBolt();
////            recoverAction   = () => playerEntity.StateInteractController().UserInput.SetInput(EPlayerInput.IsCameraFocus, true);
//            filterFunc = () => weaponController.HeldWeaponAgent.RunTimeComponent.IsPullingBolt;
//        }
//
//       
//    }
//    
    
//    public class PullBoltHandler : InterruptEventHandler
//    {
//        private EWeaponSlotType recoveredSlotType;
//        public PullBoltHandler (PlayerEntity playerEntity) : base(playerEntity)
//        {
//            var weaponController = playerEntity.WeaponController();
//            interruptAction = () =>  recoveredSlotType = weaponController.UnArmWeapon(false);
//            recoverAction   = () =>  weaponController.ArmWeapon(recoveredSlotType,true);
//            filterFunc      = () =>  !weaponController.IsHeldSlotEmpty;
//        }
//
//       
//    }
}