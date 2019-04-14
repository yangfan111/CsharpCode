//using System;
//using System.Collections.Generic;
//using Core;
//using App.Shared.Components.Weapon;
//using App.Shared.GameModules.Weapon;
//using XmlConfig;
//
//namespace App.Shared.GameModules.Player
//{
//    internal abstract class InterruptHandler
//    {
//        protected InterruptAction interruptAction;
//        protected InterruptAction recoverAction;
//        protected InterruptAction resetInterruptAction;
//        protected InterruptAction resetRecoverAction;
//
//
//        internal InterruptRecoverChecker interruptRecoverChecker;
//        private  bool                    alreadyInterrupted;
//
//        /// <summary>
//        /// return whether do interrupt sucess
//        /// </summary>
//        /// <returns></returns>
//        internal bool DoInterrupt()
//        {
//            if (!alreadyInterrupted)
//            {
//                alreadyInterrupted = true;
//                interruptAction();
//                return true;
//            }
//
//            return false;
//        }
//
//        internal void DoRecover()
//        {
//            recoverAction();
//        }
//
//        internal virtual bool IsInterruptSuceed(PlayerEntity playerEntity)
//        {
//            return true;
//        }
//
//        internal virtual bool IsRecoverSuceed(PlayerEntity playerEntity)
//        {
//            return true;
//        }
//
//        internal void ResetInterrupt()
//        {
//            if (resetInterruptAction != null)
//                resetInterruptAction();
//        }
//
//        internal void Reset()
//        {
//            alreadyInterrupted      = false;
//            interruptRecoverChecker = null;
//            if (resetInterruptAction != null)
//                resetInterruptAction();
//            if (resetRecoverAction != null)
//                resetRecoverAction();
//        }
//
//
//        internal bool VertifyRecoverState(PlayerEntity player)
//        {
//            if (interruptRecoverChecker != null)
//            {
//                return interruptRecoverChecker(player);
//            }
//
//            return true;
//        }
//    }
//
//    internal class GunSightInterruptHandler : InterruptHandler
//    {
//        internal GunSightInterruptHandler(PlayerWeaponController controller)
//        {
//            interruptAction      = controller.CancelSight;
//            recoverAction        = controller.SetSight;
//            resetInterruptAction = () => controller.HeldWeaponAgent.ClientSyncComponent.IsInterruptSightView = false;
//            resetRecoverAction   = () => controller.HeldWeaponAgent.ClientSyncComponent.IsRecoverSightView   = false;
//        }
//
////        internal override bool IsInterruptSuceed(PlayerEntity playerEntity)
////        {
////            return WeaponInterruptUtil.CheckInterruptSightViewSuceed(playerEntity);
////        }
////
////        internal override bool IsRecoverSuceed(PlayerEntity playerEntity)
////        {
////            return WeaponInterruptUtil.CheckRecoverSightViewSuceed(playerEntity);
////        }
//    }
//
//    internal class HoldWeaponInterruptHandler : InterruptHandler
//    {
//        private EWeaponSlotType recoveredSlotType;
//
//        internal HoldWeaponInterruptHandler(PlayerWeaponController controller)
//        {
//            interruptAction = () => recoveredSlotType = controller.UnArmWeapon(false);
//            recoverAction = () =>
//            {
//                controller.ArmWeapon(recoveredSlotType, true);
//                // DebugUtil.MyLog("TryArmWeaponImmediately:" + recoveredSlotType);
//            };
//        }
//    }
//
//    internal delegate bool InterruptRecoverChecker(PlayerEntity playerEntity);
//
//    internal delegate void InterruptAction();
//
//    internal class PullboltInterruptHandler : InterruptHandler
//    {
//        internal PullboltInterruptHandler(PlayerWeaponController controller)
//        {
//        }
//    }
//
//    internal class InterruptUpdateHandler
//    {
//        private InterruptHandler[] interruptHandlers;
//
//        internal InterruptUpdateHandler(PlayerWeaponController controller)
//        {
//            interruptHandlers =
//                new InterruptHandler[(int) EInterruptType.Count];
//            interruptHandlers[(int) EInterruptType.GunSight]   = new GunSightInterruptHandler(controller);
//            interruptHandlers[(int) EInterruptType.HoldWeapon] = new HoldWeaponInterruptHandler(controller);
//            interruptHandlers[(int) EInterruptType.Pullbolt]   = new PullboltInterruptHandler(controller);
//        }
//
//
//        internal void AddInterrupt(EInterruptType              interruptType,
//                                   PlayerWeaponClientUpdateComponent interruptComponent)
//        {
//            //   interruptHandlers[(int) interruptType].interruptRecoverChecker = null;
//            interruptComponent[interruptType] = WeaponUtil.CreateInterrupt(EInterruptCmdType.InterruptSimple);
//        }
//
//        internal void AddInterrupt(EInterruptType              interruptType,
//                                   Func<HashSet<EPlayerState>, bool> recoverChecker,
//                                   PlayerWeaponClientUpdateComponent interruptComponent,
//                                   bool                              interruptImmediately = true)
//        {
//            interruptComponent[interruptType] = WeaponUtil.CreateInterrupt(EInterruptCmdType.InterruptAndRollback);
//            var handler = interruptHandlers[(int) interruptType];
//            handler.interruptRecoverChecker = recoverChecker;
//            if (interruptImmediately)
//                handler.DoInterrupt();
//
//            //var remoteData = interruptComponent[interruptType];
//        }
//
//
//        internal void Update(PlayerWeaponClientUpdateComponent component, HashSet<EPlayerState> playerStates)
//        {
//            if(playerStates.Contains(EPlayerState.Dead)||playerStates.Contains(EPlayerState.Dying))
//            {
//                component.Clear();
//                return;
//            }
//            InterruptHandler handler;
//            InterruptData    data;
//            for (EInterruptType i = EInterruptType.GunSight; i < EInterruptType.Count; i++)
//            {
//                data    = component[i];
//                handler = interruptHandlers[(int) i];
//                if (!data.hasValue) continue;
//                switch ((EInterruptState) data.state)
//                {
//                    case EInterruptState.WaitInterrupt:
//                        handler.DoInterrupt();
//                        data.state += 1; //(byte)EInterruptState.Interrupted;
//                        break;
//                    case EInterruptState.Interrupted:
//                        if (handler.IsInterruptSuceed(player))
//                        {
//                            if (data.cmdType == (byte) EInterruptCmdType.InterruptSimple)
//                            {
//                                data.Reset();
//                                handler.Reset();
//                            }
//                            else
//                            {
//                                data.state += 1; //(byte)EInterruptState.InterruptedSuceed;
//                                handler.ResetInterrupt();
//                            }
//                        }
//
//                        break;
//                    case EInterruptState.WaitRecover:
//                        if (handler.VertifyRecoverState(player))
//                        {
//                            data.state += 1; //(byte)EInterruptState.ForRecover;
//                        }
//
//                        break;
//                    case EInterruptState.ForceRecover:
//                        handler.DoRecover();
//                        data.state += 1;
//                        break;
//                    case EInterruptState.Recovered:
//                        if (handler.IsRecoverSuceed(player))
//                        {
//                            data.Reset();
//                            handler.Reset();
//                        }
//
//                        break;
//                }
//
//                player.playerWeaponClientUpdate[i] = data;
//            }
//        }
//    }
//}