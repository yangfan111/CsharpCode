using Core.GameModule.Interface;
using App.Shared.Components.Player;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using App.Shared.Configuration;
using App.Shared.Player;
using Core.CharacterState;
using XmlConfig;
using Utils.Utils;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    /// <summary>
    /// 更新完位置后，计算入水高度，更新角色位置，然后更新动画
    /// </summary>
    class PlayerSpecialZoneEventSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSpecialZoneEventSystem));

        private float HeightOffset = 0.0f;
        private float HeightStandSwimOffset = 0.05f;
        // 站立进入游泳时，position为水面下1.55，之后改为位于水面下0.1f，差值1.45，预留0.05过渡
        private float SwimPositionUnderWater = 0f;
        private float AshoreDepth = 0f;
        private Contexts _contexts;

        public PlayerSpecialZoneEventSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (cmd.PredicatedOnce)
            {
                return;
            }
            
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            
            player.triggerEvent.NeedUnmountWeapon = false;
            
            if (player.IsOnVehicle() || player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return;
            }

            WaterTest(player);

        }


        private bool WaterTest(PlayerEntity player)
        {
            var state = player.stateInterface.State;
            PostureInConfig postureInConfig = state.GetCurrentPostureState();
            PostureInConfig nextPostureInConfig = state.GetNextPostureState();

            if (SingletonManager.Get<MapConfigManager>().InWater(player.position.Value))
            {
                var waterSurfaceHeight = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value);
                var inWaterDepth = waterSurfaceHeight - player.position.Value.y;
                //_logger.InfoFormat("inWaterDepth:{0}", inWaterDepth);
                float dist = 0f;
                switch (postureInConfig)
                {
                    case PostureInConfig.Swim:
                    {
                        if (nextPostureInConfig != PostureInConfig.Stand && (inWaterDepth < (AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset - HeightStandSwimOffset) || GroundTest(player, out dist)))
                        {
                            Ashore(player, AshoreDepth);
                            //_logger.InfoFormat("in water, swim to stand, inWaterDepth:{0}, thread:{1}", inWaterDepth, AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset);
                        }
                        break;
                    }
                    case PostureInConfig.Dive:
                    {
                        var groundTest = GroundTest(player, out dist);
                        if (inWaterDepth < (AnimatorParametersHash.FirstPersonStandCameraHeight  + HeightOffset) && !groundTest && nextPostureInConfig != PostureInConfig.Stand && nextPostureInConfig != PostureInConfig.Swim)
                        {
                            Swim(player);
                            //_logger.InfoFormat("Dive to swim, inWaterDepth:{0}, thread:{1}, new In water depth:{2}", inWaterDepth, AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset, SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) - player.position.Value.y);
                        }
                        else if ((inWaterDepth < (AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset 
                                                  - HeightStandSwimOffset) || groundTest) && nextPostureInConfig != PostureInConfig.Stand)
                        {
                            //Ashore(player, CompareUtility.IsApproximatelyEqual(0f, dist) ? 0 : waterSurfaceHeight - dist);
                            Ashore(player, groundTest ? inWaterDepth - dist:AshoreDepth);
                            //_logger.InfoFormat("in Dive, Dive to stand, inWaterDepth:{0}, thread:{1}", inWaterDepth, AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset);
                        }
                        break;
                    }
                    case PostureInConfig.Jump:
                    {
                        // 在水里从载具中跳出，或者高处跳水
                        if ((inWaterDepth > AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset) && nextPostureInConfig != PostureInConfig.Dive)
                        {
                            Dive(player);
                        }
                        break;
                    }
                    case PostureInConfig.Stand:
                    {
                        if (inWaterDepth > (AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset) && nextPostureInConfig != PostureInConfig.Swim)
                        {
                            Swim(player);
                            //_logger.InfoFormat("stand to swim, inWaterDepth:{0}, thread:{1}, new In water depth:{2}", inWaterDepth, AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset, SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) - player.position.Value.y);
                        }
                        break;
                    }
                    case PostureInConfig.Crouch:
                    {
                        if (inWaterDepth > AnimatorParametersHash.FirstPersonCrouchCameraHeight && nextPostureInConfig != PostureInConfig.Stand)
                        {
                            state.Stand();
                        }
                        break;
                    }
                    case PostureInConfig.Prone:
                    {
                        if (inWaterDepth > AnimatorParametersHash.FirstPersonProneCameraHeight && nextPostureInConfig != PostureInConfig.Crouch)
                        {
                            state.Crouch();
                        }
                            break;
                    }

                }
            }
            else
            {
                if (postureInConfig == PostureInConfig.Swim || postureInConfig == PostureInConfig.Dive)
                {
                    Ashore(player, SwimPositionUnderWater);
                    //_logger.InfoFormat("out of  water, swim to stand, postureInConfig:{0}", postureInConfig);

                }
            }

            return true;
        }

        private static readonly float DisOffset = 0.05F;
        
        private bool GroundTest(PlayerEntity playerEntity, out float dist)
        {
            bool ret = false;
            dist = 0f;
            var controller = playerEntity.characterContoller.Value;
            var radius = controller.radius;
            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderLayer(gameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.User));
            var playerPos = playerEntity.position.Value;
            var startPoint = new Vector3(playerPos.x,SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(playerPos),playerPos.z );
            // a shift lift up
            RaycastHit outHit;
            //DebugDraw.DebugWireSphere(startPoint + new Vector3(0,-(AnimatorParametersHash.FirstPersonStandCameraHeight - radius + DisOffset),0) , radius, 0 , false);
            if (Physics.SphereCast(startPoint, radius, Vector3.down, out outHit, AnimatorParametersHash.FirstPersonStandCameraHeight - radius + DisOffset, UnityLayers.AllCollidableLayerMask))
            {
                ret = true;
                dist = outHit.distance + radius;
            }
            IntersectionDetectTool.SetColliderLayer(gameObject, prevLayer);
            //_logger.InfoFormat("GroundTest : {0}", ret);
            return ret;
        }
        
        private void Swim(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;
            syncTransform.position = new Vector3(player.position.Value.x,
                                                    SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) - (AnimatorParametersHash.FirstPersonStandCameraHeight + HeightOffset),
                                                 player.position.Value.z);
            player.position.Value = syncTransform.position;
          
            PlayerMoveSystem.SyncUpdateComponentPos(player, syncTransform.position);
            player.stateInterface.State.Swim();
            player.triggerEvent.NeedUnmountWeapon = true;
            player.WeaponController().ForceUnArmHeldWeapon();

            //_logger.InfoFormat("swim ashore pos:{0}",player.position.Value.ToStringExt());
        }

        private void Ashore(PlayerEntity player, float inWaterDepth)
        {
            var syncTransform = player.RootGo().transform;
            syncTransform.position = new Vector3(player.position.Value.x,
                                                 player.position.Value.y + inWaterDepth - SwimPositionUnderWater,
                                                 player.position.Value.z);
            PlayerMoveSystem.SyncUpdateComponentPos(player, syncTransform.position);
            //_logger.InfoFormat("after ashore pos:{0}",player.position.Value.ToStringExt());
            player.stateInterface.State.Ashore();
        }

        private void Dive(PlayerEntity player)
        {
            player.stateInterface.State.Dive();
            player.triggerEvent.NeedUnmountWeapon = true;
            player.WeaponController().ForceUnArmHeldWeapon();
        }
    }
}
