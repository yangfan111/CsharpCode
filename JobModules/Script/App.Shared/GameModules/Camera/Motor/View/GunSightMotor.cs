using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using App.Shared;
using App.Shared.GameModules;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Weapon;
using App.Shared.Player;
using Assets.App.Shared.GameModules.Camera;
using com.wd.free.skill;
using Core.Utils;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Bone;
using Utils.Appearance.Effects;
using Utils.CharacterState;
using XmlConfig;

namespace Core.CameraControl.NewMotor.View
{
    public class GunSightMotor:AbstractCameraMotor
    {
        public GunSightMotor(Motors m):base(m)
        {
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    if (!player.hasAppearanceInterface) return;
                    if (!player.appearanceInterface.Appearance.IsFirstPerson)
                    {
                        player.appearanceInterface.Appearance.SetFirstPerson();
                        player.characterBoneInterface.CharacterBone.SetFirstPerson();
                        player.UpdateCameraArchorPostion();
                    }

                });
            
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    if (!player.hasAppearanceInterface) return;
                    var playerUtils = EffectUtility.GetEffect(player.RootGo(), "PlayerUtils");
                    if (playerUtils != null)
                        playerUtils.SetParam("GunViewBegin", (object)player.RootGo().gameObject);
                });

            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    if (!player.hasAppearanceInterface) return;
                    var speed = player.WeaponController().HeldWeaponAgent.CmrFocusSpeed;
                    player.stateInterface.State.SetSight(speed);
                    Logger.InfoFormat("Enter sight!");
                });
            
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    if (!player.hasAppearanceInterface) return;
                    player.AudioController().PlaySimpleAudio(EAudioUniqueId.SightOpen, true);
                });
            
            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    if (!player.hasAppearanceInterface) return;
                    OpenDepthOfField(player);
                });

            _motors.ActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.View, (int)ModeId,
                (player, state) =>
                {
                    var speed = player.WeaponController().HeldWeaponAgent.CmrFocusSpeed;
                    player.stateInterface.State.CancelSight(speed);
                    Logger.InfoFormat("Leave sight!");
                });
            
            _motors.ActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    var playerUtils = EffectUtility.GetEffect(player.RootGo(), "PlayerUtils");
                    if (playerUtils != null)
                        playerUtils.SetParam("GunViewEnd", (object)player.RootGo().gameObject);
                });

            _motors.ActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    player.AudioController().PlaySimpleAudio(EAudioUniqueId.SightClose, true);
                });

            _motors.ActionManager.AddAction(CameraActionType.Leave, SubCameraMotorType.View, (int) ModeId,
                (player, state) =>
                {
                    CloseDepthOfField(player);
                });
        }

        private void OpenDepthOfField(PlayerEntity player)
        {
            Logger.InfoFormat("openDepthOfField");

            if (SharedConfig.IsServer)
                return;
            
            Transform scope = BoneMount.FindChildBoneFromCache(player.firstPersonModel.Value, BoneName.ScopeLocator);
            var dofParams = player.WeaponController().HeldWeaponAgent.GetDOFParams();

            bool needDefaultSet = false;

            if (scope != null && scope.childCount != 0 )
            {
                var script = scope.GetComponentInChildren<AbstractEffectMonoBehaviour>();
                if (script != null)
                {
                    SetDof(dofParams, script);
                }
                else if( dofParams[0] == "1" )
                {
                    needDefaultSet = true;
                }
            } 
            else needDefaultSet = true;
            
            if(needDefaultSet)
            {
                var script = player.effects.GetEffect("DepthOfFieldPostEffectMask");
                if (script != null)
                {
                    var mono = script as AbstractEffectMonoBehaviour;
                    if (mono != null)
                        mono.enabled = true;
                    
                    try
                    {
                        SetDof(dofParams, mono);
                        script.SetParam("Enable", true);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("error when Enable:");
                    }
                    
                }
            }
        }

        private static void SetDof(string[] dofParams, AbstractEffectMonoBehaviour script)
        {
            for (int i = 0; i < dofParams.Length; i++)
            {
                try
                {
                    script.SetParam(i, "paramsets", dofParams[i].Replace("f", ""));
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("error when set params: :{0}={1}", dofParams[i].Replace("f", ""), i);
                }
            }
        }

        private void CloseDepthOfField(PlayerEntity player)
        {
            Logger.ErrorFormat("closeDepthOfField");
            if (SharedConfig.IsServer)
                return;
            var script = player.effects.GetEffect("DepthOfFieldPostEffectMask");
            if (script != null)
            {
                var mono = script as MonoBehaviour;
                if (mono != null)
                    mono.enabled = false;
            }
        }

        public override short ModeId
        {
            get { return (short)ECameraViewMode.GunSight;  }
        }
        public override int Order
        {
            get
            {
                return 3;
            }
        }
        

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            if (input.IsDead) return false;
            if (!input.CanWeaponGunSight) return false;
            if (state.IsFree()) return false;
            var config = input.GetPoseConfig(state.GetMainMotor().NowMode);
            if (!config.CanSwitchView) return false;
            
            if (state.ViewMode == ECameraViewMode.GunSight &&
                (input.FilteredCameraFocus || input.InterruptCameraFocus))
            {
                if(input.InterruptCameraFocus)
                {
                    if(Logger.IsDebugEnabled)
                    {
                        Logger.Debug("ForceInterruptGunSight");
                    }
                }
                return false;
            }
            if (state.ViewMode==ECameraViewMode.ThirdPerson && !input.InterruptCameraFocus &&  (input.FilteredCameraFocus ))
            {
               // DebugUtil.MyLog("Change cmr to gunsight");
                return true;
            }

            if (state.ViewMode==ECameraViewMode.FirstPerson && !input.InterruptCameraFocus && (input.FilteredCameraFocus ))
            {
                return true;
            }

            return state.ViewMode == ECameraViewMode.GunSight;
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            if (input.ChangeCamera)
            {
                subState.LastMode = (byte)(subState.LastMode==(int)ECameraViewMode.FirstPerson
                    ? ECameraViewMode.ThirdPerson
                    : ECameraViewMode.FirstPerson);
            }
            var fov = player.WeaponController().HeldWeaponAgent.GetGameFov(player.oxygenEnergyInterface.Oxygen.InShiftState);
            if(fov <= 0)
            {
                Logger.ErrorFormat("Illegal fov value {0}", fov);
                return;
            }
            output.Fov = fov;       
        }
        
        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;  
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
           
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            
        }
    }
}
