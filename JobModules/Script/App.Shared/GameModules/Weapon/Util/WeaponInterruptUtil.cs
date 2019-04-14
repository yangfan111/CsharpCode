//using App.Shared.GameMode;
//using Assets.Utils.Configuration;
//using Assets.XmlConfig;
//using Core;
//using Core.Attack;
//using Core.Room;
//using Core.Utils;
//using System;
//using System.Collections.Generic;
//using App.Shared.Components.Player;
//using Utils.Configuration;
//using Utils.Singleton;
//using WeaponConfigNs;
//using XmlConfig;
//
//namespace App.Shared
//{
//    public static class WeaponInterruptUtil
//    {
//        public static bool CheckInterruptSightViewSuceed(PlayerEntity playerEntity)
//        {
//            return (ECameraViewMode) playerEntity.cameraStateNew.ViewNowMode != ECameraViewMode.GunSight;
//        }
//        public static bool CheckRecoverSightViewSuceed(PlayerEntity playerEntity)
//        {
//            return (ECameraViewMode) playerEntity.cameraStateNew.ViewNowMode == ECameraViewMode.GunSight;
//        }
//   
//
//        public static bool CheckPullBoltEnd(PlayerEntity playerEntity)
//        {
//            return !playerEntity.WeaponController().HeldWeaponAgent.ClientSyncComponent.IsPullingBolt;
//        }
//        public static bool CheckProneEnd(PlayerEntity playerEntity)
//        {
//            var postureState = playerEntity.stateInterface.State.GetCurrentPostureState();
//            return postureState == PostureInConfig.Prone;
//        }
//
//        public static bool CheckWeaponRotStateEnd(PlayerEntity playerEntity)
//        {
//            return !playerEntity.characterBone.IsWeaponRotState;
//        }
//        public static bool CheckProneMovementEnd(PlayerEntity playerEntity)
//        {
//            return !playerEntity.stateInterface.State.IsProneMovement();
//        }
//        
//
//        public static bool CheckVehicleOff(PlayerEntity playerEntity)
//        {
//            return !playerEntity.hasControlledVehicle || !playerEntity.controlledVehicle.IsOnVehicle;
//        }
//
//        public static bool CheckSwimEnd(PlayerEntity playerEntity)
//        {
//            var postureState = playerEntity.stateInterface.State.GetCurrentPostureState();
//            return PostureInConfig.Swim != postureState && PostureInConfig.Dive != postureState;
//        }
//
//        /// <summary>
//        /// 埋包，拆包
//        /// </summary>
//        /// <param name="playerEntity"></param>
//        /// <returns></returns>
//        public static bool CheckC4End(PlayerEntity playerEntity)
//        {
//            ActionInConfig state = playerEntity.stateInterface.State.GetActionState();
//            return state != ActionInConfig.BuriedBomb || state != ActionInConfig.DismantleBomb;
//        }
//
//        public static bool CheckClimbEnd(PlayerEntity playerEntity)
//        {
//            var postureState = playerEntity.stateInterface.State.GetCurrentPostureState();
//            return PostureInConfig.Jump != postureState && PostureInConfig.Climb != postureState;
//        }
//
//        public static bool CheckCrouchTransmitEnd(PlayerEntity playerEntity)
//        {
//            var postureState = playerEntity.stateInterface.State.GetCurrentPostureState();
//            return PostureInConfig.Crouch == postureState ;
//        }
//
//        public static bool CheckPlayerIsAlive(PlayerEntity player)
//        {
//            if (!player.hasPlayerGameState) return true;
//            switch (player.playerGameState.CurrentPlayerLifeState)
//            {
//                case PlayerLifeStateEnum.Reborn:
//                case PlayerLifeStateEnum.Dead:
//                    return false;
//            }
//
//            return true;
//        }
//    }
//}