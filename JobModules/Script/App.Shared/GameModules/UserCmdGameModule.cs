﻿using App.Client.GameModules.Player;
 using App.Client.GameModules.SceneObject;
 using App.Shared.Audio;
 using App.Shared.FreeFramework.Free.Chicken;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Camera;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Actions;
using App.Shared.GameModules.Player.Actions.LadderPackage;
using App.Shared.GameModules.Player.Actions.Move;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.CharacterDebugPackage;
using App.Shared.GameModules.Player.CharacterBone;
using App.Shared.GameModules.Player.CharacterState;
using App.Shared.GameModules.Player.Move;
using App.Shared.GameModules.Player.Oxygen;
 using App.Shared.GameModules.SceneObject;
 using App.Shared.GameModules.Throwing;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using Assets.App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Player;
using Assets.Utils.Configuration;
using Core.BulletSimulation;
using Core.Compensation;
using Core.GameModule.Module;
using Core.GameModule.System;
using Utils.Singleton;

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
            AddSystem(new PlayerWeaponCmdGenerateSystem());

            AddSystem(new PlayerInterruptUpdateSystem());
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
            AddSystem(new PlayerRotateLimitSystem());
            AddSystem(new CameraPreUpdateSystem(contexts, motors));
            if(!SharedConfig.IsServer)
                AddSystem(new InitUploadEventSystem(contexts));
            AddSystem(new BulletSimulationSystem(contexts, compensationWorldFactory, bulletHitHandler));
            AddSystem(new PlayerAttackSystem(contexts));
            AddSystem(new MeleeAttackSystem(contexts, compensationWorldFactory, meleeHitHandler));
            AddSystem(new ThrowingSimulationSystem(contexts, compensationWorldFactory, throwingHitHandler, contexts.session.entityFactoryObject.SoundEntityFactory));
            AddSystem(new PlayerBulletGenerateSystem(contexts));
            
            AddSystem(new PlayerWeaponSwitchSystem());
            AddSystem(new PlayerWeaponDrawSystem());
            AddSystem(new PlayerWeaponUpdateSystem(contexts));
     
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSkyMoveStateUpdateSystem(contexts));
                AddSystem(new PlayerClientAudioCmdUpdateSystem());
            }
            else
            {
                AddSystem(new ServerPlayerSkyMoveStateUpdateSystem(contexts));
            }
            
            AddSystem(new CreatePlayerLifeStateDataSystem());
            
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
                AddSystem(new PlayerSpecialZoneEventSystem(contexts));
            }
            else
            {
                AddSystem(new ServerSpecialZoneEventSystem(contexts));
            }
            AddSystem(new PlayerControlledVehicleUserCmdExecuteSystem());
            AddSystem(new UpdatePlayerPositionOnVehicle(contexts));
            //AddSystem(new PlayerCameraInputSystem(contexts.player));
            AddSystem(new VehicleRideSystem(contexts));
            if(!SharedConfig.IsServer)
                AddSystem(new VehicleCameraUpdateSystem(contexts));
            AddSystem(new PlayerOxygenEnergySystem(contexts.vehicle));
            AddSystem(new PlayerCustomInputUpdateSystem());
            
            
            AddSystem(new PlayerAutoMoveSystem());
            AddSystem(new PlayerClimbActionSystem());
            AddSystem(new PlayerLadderActionSystem());
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
            
            AddSystem(new PlayerAppearanceDebugSystem(contexts.player));
            
            if (!SharedConfig.IsServer)
            {
                AddSystem(new PlayerSynchronizeToComponentSystem());
            } 
            
            AddSystem(new PlayerBuffUpdateSystem(contexts));
            AddSystem(new PlayerFallDamageSystem(contexts));
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
           
            AddSystem(new PlayerBagSwitchSystem(commonSessionObjects));
            if (!SharedConfig.IsServer)
                AddSystem(new CameraPostUpdateSystem(contexts));
            else AddSystem(new ServerPostCameraUpdateSystem(contexts));
        
          
        }
    }
}
