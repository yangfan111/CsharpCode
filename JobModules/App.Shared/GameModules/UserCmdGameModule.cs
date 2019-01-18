using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Camera;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Actions;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.CharacterBone;
using App.Shared.GameModules.Player.CharacterState;
using App.Shared.GameModules.Player.Oxygen;
using App.Shared.GameModules.Throwing;
using App.Shared.GameModules.Vehicle;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Player;
using Core;
using Core.BulletSimulation;
using Core.Compensation;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.IFactory;

namespace App.Shared.GameModules
{
    public class UserCmdGameModule : GameModule
    {
        public UserCmdGameModule(Contexts contexts,
            ICompensationWorldFactory compensationWorldFactory,
            IBulletHitHandler bulletHitHandler,
            IMeleeHitHandler meleeHitHandler,
            IThrowingHitHandler throwingHitHandler,
            ICommonSessionObjects commonSessionObjects,
            Motors motors)
        {
            if (SharedConfig.IsServer)
            {
                AddSystem(new PlayerSyncStageSystem(contexts));
            }
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSynchronizeFromComponentSystem());
            }
            else
            {
                AddSystem(new ServerSynchronizeFromComponentSystem());
            }
            AddSystem(new PlayerClientTimeSystem());
            AddSystem(new PlayerGrenadeInventorySyncSystem());
            //AddSystem(new CameraUpdateArchorSystem(contexts));
            AddSystem(new PlayerRotateLimitSystem());
            AddSystem(new CameraPreUpdateSystem(contexts.vehicle,contexts.freeMove,motors));
            AddSystem(new BulletSimulationSystem(contexts, compensationWorldFactory, bulletHitHandler));
            AddSystem(new PlayerAttackSystem());
            AddSystem(new MeleeAttackSystem(contexts, compensationWorldFactory, meleeHitHandler));
            AddSystem(new ThrowingSimulationSystem(contexts, compensationWorldFactory, throwingHitHandler, contexts.session.entityFactoryObject.SoundEntityFactory));
            
            AddSystem(new PlayerWeaponSwitchSystem());
            AddSystem(new PlayerWeaponDrawSystem());
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSkyMoveStateUpdateSystem(contexts));
            }
            else
            {
                AddSystem(new ServerPlayerSkyMoveStateUpdateSystem(contexts));
            }

//            AddSystem(new PlayerMoveByRootMotionSystem());
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerMoveSystem(contexts));
            }
            else
            {
                AddSystem(new ServerMoveSystem(contexts));
            }

            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSpecialZoneEventSystem());
            }
            AddSystem(new PlayerControlledVehicleUserCmdExecuteSystem());
            AddSystem(new UpdatePlayerPositionOnVehicle(contexts));
            //AddSystem(new PlayerCameraInputSystem(contexts.player));
            AddSystem(new VehicleRideSystem(contexts));
            if(!SharedConfig.IsServer)
                AddSystem(new VehicleCameraUpdateSystem(contexts));
            AddSystem(new PlayerOxygenEnergySystem(contexts.vehicle));
            AddSystem(new PlayerCustomInputUpdateSystem());
            AddSystem(new GenericActionSystem());
            if(!SharedConfig.IsServer)
                AddSystem(new PlayerStateUpdateSystem(contexts));
            else
            {
                AddSystem(new PlayerServerStateUpdateSystem(contexts));
            }

            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerMoveByRootMotionSystem());
            }
            else
            {
                AddSystem(new ServerMoveByRootMotionSystem());
            }
            AddSystem(new PlayerFirstAppearanceUpdateSystem());
            AddSystem(new PlayerAppearanceUpdateSystem());
            if(!SharedConfig.IsServer)
                AddSystem(new PlayerCharacterBoneUpdateSystem());
            else
            {
                AddSystem(new ServerCharacterBoneUpdateSystem());
            }
            AddSystem(new PlayerHoldBreathSystem());
            AddSystem(new PlayerAvatarSystem());
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSynchronizeToComponentSystem());
            }
            
            
            AddSystem(new PlayerBuffUpdateSystem());
            AddSystem(new PlayerFallDamageSystem());
            AddSystem(new PlayerSaveSystem(contexts));
            AddSystem(new PlayerStatisticsSystem());
            AddSystem(new CameraUpdateArchorSystem(contexts));
            if (!SharedConfig.IsServer)
            {
              
               
                AddSystem(new CameraUpdateSystem(contexts, motors));
            }
            else
            {
                AddSystem(new ServerCameraUpdateSystem(contexts,motors));
            }
            AddSystem(new PlayerStateTipSystem(contexts));
           
            AddSystem(new PlayerWeaponLogicRefreshSystem(contexts));
            AddSystem(new PlayerActionInterruptSystem());
            AddSystem(new PlayerBagSwitchSystem(commonSessionObjects));
            if (!SharedConfig.IsServer)
                AddSystem(new CameraPostUpdateSystem(contexts.player, contexts.vehicle, contexts.freeMove));
            else AddSystem(new ServerPostCameraUpdateSystem(contexts));
            
        }
    }
}