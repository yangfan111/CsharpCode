using System;
using App.Shared.Components.Ui;
using App.Shared.GameModules.Camera.Utils;
using Core.CharacterState.Action;
using Core.CharacterState.Posture;
using Core.EntityComponent;
using Core.WeaponLogic;
using Entitas;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;
using Core.Attack;
using Core;
using Core.WeaponLogic.Throwing;
using Assets.Utils.Configuration;
using Core.CameraControl.NewMotor;
using Utils.Appearance;
using Core.Common;
using Core.Free;
using Core.Utils;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponStateAdapter : IPlayerWeaponState, ISpeedProvider
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponStateAdapter));
        private PlayerEntity _playerEntity;
        private ICharacterAction _action;
        ICharacterPosture _characterPosture;
        private UiContext _uiContext;

        public PlayerWeaponStateAdapter(PlayerEntity playerEntity, ICharacterAction action, ICharacterPosture characterPosture, UiContext uiContext)
        {
            _playerEntity = playerEntity;
            _action = action;
            _characterPosture = characterPosture;
            _uiContext = uiContext;
        }

        public Entity Owner
        {
            get { return _playerEntity; }
        }
        public EntityKey Key
        {
            get { return _playerEntity.entityKey.Value; }
        }
        public int ClientTime
        {
            get { return _playerEntity.time.ClientTime; }
        }

        public float ViewPitch
        {
            get { return _playerEntity.orientation.Pitch; }
        }
        public float ViewYaw
        {
            get { return _playerEntity.orientation.Yaw; }
        }

        public Vector3 BulletEmitPosition
        {
            get { return PlayerEntityUtility.GetCameraBulletEmitPosition(_playerEntity); }
        }

        public bool IsReload
        {
            get { return _playerEntity.playerWeaponState.Reloading; }
        }

        public int CurrentWeapon
        {
            get { return _playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponId; }
        }

        public float HorizontalVeocity
        {
            get
            {
                return _playerEntity.playerMove.HorizontalVelocity;
            }
        }

        public float GetBaseSpeed()
        {
            return _playerEntity.weaponLogic.Weapon.GetBaseSpeed();
        }

        public float GetDefaultSpeed()
        {
            return _playerEntity.playerMove.DefaultSpeed;
        }

        public bool IsAir
        {
            get { return !_playerEntity.playerMove.IsGround; }
        }

        public int BurstShootCount
        {
            get { return _playerEntity.playerWeaponState.BurstShootCount; }
            set { _playerEntity.playerWeaponState.BurstShootCount = value; }
        }

        public int SpecialReloadCount
        {
            get
            {
                return _playerEntity.weaponLogic.Weapon.GetSpecialReloadCount();
            }
        }

        public bool CanCameraFocus()
        {
            return _playerEntity.weaponLogic.Weapon.CanCameraFocus();
        }

        public bool IsDuckOrProneState
        {
            get
            {
                return _characterPosture.GetCurrentPostureState() == PostureInConfig.Crouch
                  || _characterPosture.GetCurrentPostureState() == PostureInConfig.Prone;
            }
        }

        public bool IsAiming
        {
            get { return _playerEntity.IsCameraGunSight(); }
        }

        public bool IsProne
        {
            get { return _characterPosture.GetCurrentPostureState() == PostureInConfig.Prone; }
        }
        public float NegPunchPitch
        {
            get { return _playerEntity.orientation.NegPunchPitch; }
            set { _playerEntity.orientation.NegPunchPitch = value; }
        }


        public float NegPunchYaw
        {
            get { return _playerEntity.orientation.NegPunchYaw; }
            set { _playerEntity.orientation.NegPunchYaw = value; }
        }
        public int PunchDecayCdTime
        {
            get { return _playerEntity.playerWeaponState.PunchDecayCdTime; }
            set { _playerEntity.playerWeaponState.PunchDecayCdTime = value; }
        }
        public bool PunchYawLeftSide
        {
            get { return _playerEntity.playerWeaponState.PunchYawLeftSide; }
            set { _playerEntity.playerWeaponState.PunchYawLeftSide = value; }
        }
        public float LastAccuracy
        {
            get { return _playerEntity.playerWeaponState.Accuracy; }
            set { _playerEntity.playerWeaponState.Accuracy = value; }
        }
        public int NextAttackTimer
        {
            get { return _playerEntity.playerWeaponState.NextAttackTimer; }
            set { _playerEntity.playerWeaponState.NextAttackTimer = value; }
        }

        public int BulletCountLimit
        {
            get
            {
                return _playerEntity.weaponLogic.Weapon.GetBulletLimit();
            }
        }

        public int ReservedBulletCount
        {
            get
            {
                var achive = _playerEntity.GetController<PlayerWeaponController>();
                if (null == achive)
                    return 0;
                return achive.GetReservedBullet();
            }
            set
            {
                var achive = _playerEntity.GetController<PlayerWeaponController>();
                if (null == achive)
                    return;
                var controller = _playerEntity.GetController<PlayerWeaponController>();
                controller.SetReservedBullet(value);
            }
        }

        public int LoadedBulletCount
        {
            get
            {
                var achive = _playerEntity.GetController<PlayerWeaponController>();
                if (null == achive)
                {
                    return 0;
                }
                return achive.CurrWeaponBullet;
            }

            set
            {
                var achive = _playerEntity.GetController<PlayerWeaponController>();
           
                if (null == achive)
                    return;
                var controller = _playerEntity.GetController<PlayerWeaponController>();
                controller.SetSlotWeaponBullet(value);
            }
        }

        public int LastFireTime
        {
            get { return _playerEntity.playerWeaponState.LastFireTime; }
            set { _playerEntity.playerWeaponState.LastFireTime = value; }
        }

        public float ReloadSpeed
        {
            get { return _playerEntity.weaponLogic.Weapon.GetReloadSpeed(); }
        }

        public int ContinuesShootCount
        {
            get { return _playerEntity.playerWeaponState.ContinuesShootCount; }
            set { _playerEntity.playerWeaponState.ContinuesShootCount = value; }
        }
        public bool ContinuesShootDecreaseNeeded
        {
            get { return _playerEntity.playerWeaponState.ContinuesShootDecreaseNeeded; }
            set { _playerEntity.playerWeaponState.ContinuesShootDecreaseNeeded = value; }
        }
        public int ContinuesShootDecreaseTimer
        {
            get { return _playerEntity.playerWeaponState.ContinuesShootDecreaseTimer; }
            set { _playerEntity.playerWeaponState.ContinuesShootDecreaseTimer = value; }
        }

        public Vector3 LastBulletDir
        {
            get { return _playerEntity.playerWeaponState.LastBulletDir; }
            set { _playerEntity.playerWeaponState.LastBulletDir = value; }
        }

        public float LastSpreadX
        {
            get { return _playerEntity.playerWeaponState.LastSpreadX; }
            set { _playerEntity.playerWeaponState.LastSpreadX = value; }
        }

        public float LastSpreadY
        {
            get { return _playerEntity.playerWeaponState.LastSpreadY; }
            set { _playerEntity.playerWeaponState.LastSpreadY = value; }
        }

        public EFireMode FireMode
        {
            get { return (EFireMode)_playerEntity.GetController<PlayerWeaponController>().CurrFireMode; }
            set { _playerEntity.GetController<PlayerWeaponController>().CurrFireMode = (int)value; }
        }

        public void ShowFireModeChangeTip(EFireMode newFireMode)
        {
            switch (newFireMode)
            {
                case EFireMode.Auto:
                    _playerEntity.tip.TipType = ETipType.FireModeToAuto;
                    break;
                case EFireMode.Burst:
                    _playerEntity.tip.TipType = ETipType.FireModeToBurst;
                    break;
                case EFireMode.Manual:
                    _playerEntity.tip.TipType = ETipType.FireModeToManual;
                    break;
            }
        }

        public void ShowFireModeUnchangeTip()
        {
            _playerEntity.tip.TipType = ETipType.FireModeLocked;
        }

        public void ShowNoBulletTip()
        {
            _playerEntity.tip.TipType = ETipType.FireWithNoBullet;
        }

        public bool IsPrevCmdFire { get; set; }
        public int ContinuousAttackTime
        {
            get { return _playerEntity.playerWeaponState.ContinuousAttackTime; }
            set { _playerEntity.playerWeaponState.ContinuousAttackTime = value; }
        }
        public int NextAttackingTimeLimit
        {
            get { return _playerEntity.playerWeaponState.NextAttackingTimeLimit; }
            set { _playerEntity.playerWeaponState.NextAttackingTimeLimit = value; }
        }
        public bool MeleeAttacking
        {
            get { return _playerEntity.playerWeaponState.MeleeAttacking; }
            set { _playerEntity.playerWeaponState.MeleeAttacking = value; }
        }

        public bool RangeAttacking
        {
            get { return _playerEntity.playerWeaponState.RangeAttacking; }
            set { _playerEntity.playerWeaponState.RangeAttacking = value; }
        }

        bool IPlayerWeaponState.IsBolted
        {
            
            get { return _playerEntity.GetController<PlayerWeaponController>().CurrBolted; }
            set { _playerEntity.GetController<PlayerWeaponController>().CurrBolted = value; }
        }

        public bool CanFire()
        {
            var action = _action.CanFire();
            var camera = _playerEntity.IsCameraCanFire();
            return action && camera;
        }

        public bool CanMeleeFire()
        {
            var action = _action.CanFire();
            var camera = _playerEntity.IsCameraCanFire();
            return action && camera;
        }

        public void OnDefaultFire(bool NeedActionDeal = false)
        {
            if (_playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                _action.SightsFire();
            }
            else
            {
                _action.Fire();
            }
            _playerEntity.weaponLogic.WeaponSound.PlaySound(EWeaponSoundType.LeftFire1);
        }

        public bool IsFiring
        {
            get
            {
                var isFiring = _playerEntity.stateInterface.State.GetActionState() == ActionInConfig.Fire;
                return isFiring;
            }
        }

        public void OnSpecialFire(bool NeedActionDeal = false)
        {
            if (_playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                _action.SpecialSightsFire(() =>
                {
                    if (NeedActionDeal)
                    {
                        _playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                    }
                });
            }
            else
            {
                _action.SpecialFire(() =>
                {
                    if (NeedActionDeal)
                    {
                        _playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                    }
                });
            }
            _playerEntity.weaponLogic.WeaponSound.PlaySound(EWeaponSoundType.LeftFire1);
            if (NeedActionDeal)
            {
                _playerEntity.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
            }
        }

        public void OnLightMeleeFire(System.Action callback)
        {
            _action.LightMeleeAttackOne(callback);
        }

        public void OnSecondLightMeleeFire(Action callback)
        {
            _action.LightMeleeAttackTwo(callback);
        }

        public void OnMeleeSpecialFire(System.Action callback)
        {
            _action.MeleeSpecialAttack(callback);
        }

        public void OnSwitchMode(CommonFireConfig common)
        {
           // _playerEntity.soundManager.Value.PlayOnce(EPlayerSoundType.ChangeMode);
            Core.Audio.GameAudioMedium.PerformOnGunModelSwitch(common,this);
            //_playerEntity.weaponLogic.WeaponSound.PlaySound(EWeaponSoundType.SwitchFireMode);
        }
        public void OnSwitchWeapon()
        {
            Core.Audio.GameAudioMedium.PerformOnGunSwitch(this);
        }
        public void BreakReloading()
        {
            _playerEntity.stateInterface.State.BreakSpecialReload();
        }

        public void ForceBreakReloading(Action callback)
        {
            _playerEntity.stateInterface.State.ForceBreakSpecialReload(callback);
        }

        public void StartMeleeAttack(int attackTime, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if (_playerEntity.hasMeleeAttackInfoSync)
            {
                _playerEntity.meleeAttackInfoSync.AttackTime = attackTime;
            }
            else
            {
                _playerEntity.AddMeleeAttackInfoSync(attackTime);
            }
            if (_playerEntity.hasMeleeAttackInfo)
            {
                _playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                _playerEntity.meleeAttackInfo.AttackConfig = config;
            }
            else
            {
                _playerEntity.AddMeleeAttackInfo();
                _playerEntity.meleeAttackInfo.AttackInfo = attackInfo;
                _playerEntity.meleeAttackInfo.AttackConfig = config;
            }
        }

        public void CancelMeleeAttack()
        {
            if (_playerEntity.hasMeleeAttackInfoSync)
            {
                _playerEntity.RemoveMeleeAttackInfoSync();
            }
            if (_playerEntity.hasMeleeAttackInfo)
            {
                _playerEntity.RemoveMeleeAttackInfo();
            }
        }

        public void ReadyThrowing()
        {
            _playerEntity.stateInterface.State.InterruptAction();
            _playerEntity.stateInterface.State.StartFarGrenadeThrow();
        }

        public void StartThrowing()
        {
            _playerEntity.stateInterface.State.FinishGrenadeThrow();
        }

        public void SwitchThrowingMode(bool isNearThrow)
        {
            if (isNearThrow)
            {
                _playerEntity.stateInterface.State.ChangeThrowDistance(0);
            }
            else
            {
                _playerEntity.stateInterface.State.ChangeThrowDistance(1);
            }
        }

        public void ForceStopThrowing()
        {
            _playerEntity.stateInterface.State.ForceFinishGrenadeThrow();
        }

        public void OnWeaponCost()
        {
            var slot = _playerEntity.weaponState.CurrentWeaponSlot;
            _playerEntity.GetController<PlayerWeaponController>().OnExpend((EWeaponSlotType)slot);
        }

        public void UnmountWeaponByAction()
        {
            _playerEntity.GetController<PlayerWeaponController>().ForceUnmountCurrWeapon();
        }

        public void EndSpecialFire()
        {
            _action.SpecialFireEnd();
        }

        public int PlaySoundOnce(int id)
        {
            return 0;
        //    return _playerEntity.soundManager.Value.PlayOnce(id);
        }

        private int _lastGrenadeId;

        public int LastGrenadeId
        {
            get { return _lastGrenadeId; }
            set { _lastGrenadeId = value; }
        }

        public bool PullBolting
        {
            get
            {
                return _playerEntity.playerWeaponState.PullBolting;
            }
            set
            {
                if (value)
                {
                    var gunSight = _playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight;
                    _playerEntity.playerWeaponState.GunSightBeforePullBolting = gunSight;
                    _playerEntity.playerWeaponState.ForceChangeGunSight = gunSight;
                }
                else
                {
                    if (_playerEntity.playerWeaponState.GunSightBeforePullBolting)
                    {
                        _playerEntity.playerWeaponState.ForceChangeGunSight = true;
                        _playerEntity.playerWeaponState.GunSightBeforePullBolting = false;
                    }
                }
                _playerEntity.playerWeaponState.PullBolting = value;
            }
        }

        public bool ResumeGunSightAfterPullBolt
        {
            get
            {
                var resume = _playerEntity.playerWeaponState.GunSightBeforePullBolting;
                return resume;
            }
        }

        public ThrowingActionInfo ThrowingActionInfo
        {
            get
            {
                if (!_playerEntity.hasThrowingAction)
                {
                    _playerEntity.AddThrowingAction();
                    if (null == _playerEntity.throwingAction.ActionInfo)
                        _playerEntity.throwingAction.ActionInfo = new ThrowingActionInfo();
                }
                return _playerEntity.throwingAction.ActionInfo;
            }
        }

        public void OnFireOnce()
        {
            _playerEntity.statisticsData.Statistics.ShootingCount += 1;
            //服务端不需要处理,uicontext为null
            if (null != _uiContext && _uiContext.hasMap)
            {
                var map = _uiContext.map;

                MiniMapTeamPlayInfo playerInfo = null;
                foreach (var player in map.TeamInfos)
                {
                    if (_playerEntity.playerInfo.PlayerId == player.PlayerId)
                        playerInfo = player;
                }

                if (null != playerInfo)
                {
                    playerInfo.IsShooting = true;
                    playerInfo.ShootingCount++;
                }
            }
        }

        public void OnGrenadeThrowingOnce()
        {
            _playerEntity.statisticsData.Statistics.UseThrowingCount++;
        }

        public void OnWeaponStateChanged()
        {
            Logger.InfoFormat("{0} BagLockedByWeaponChanged", _playerEntity.entityKey.Value);
            _playerEntity.weaponState.BagLocked = true;
        }

        public bool IsAlwaysEmptyReload
        {
            get
            {
                var curWeaponId = _playerEntity.weaponLogicInfo.WeaponId;
                return SingletonManager.Get<WeaponConfigManager>().IsSpecialType(curWeaponId, ESpecialWeaponType.ReloadEmptyAlways);
            }
        }

        public bool IsSniperFire
        {
            get
            {
                var curWeaponId = _playerEntity.weaponLogicInfo.WeaponId;
                return SingletonManager.Get<WeaponConfigManager>().IsSpecialType(curWeaponId, ESpecialWeaponType.SniperFrie);
            }
        }

        public bool IsFireHold
        {
            get
            {
                return _playerEntity.stateInterface.State.GetActionState() == ActionInConfig.SpecialFireHold;
            }
        }

        public bool IsFireEnd
        {
            get
            {
                var state = _playerEntity.stateInterface.State;
                return state.GetActionState() == ActionInConfig.SpecialFireEnd;
            }
        }

        public EBulletCaliber Caliber
        {
            get
            {
                var weapon = _playerEntity.GetController<PlayerWeaponController>().CurrSlotWeaponInfo;
                if (weapon.Id < 1)
                {
                    return EBulletCaliber.Length;
                }
                var config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weapon.Id);
                return (EBulletCaliber)config.Caliber;
            }
        }

        public float WeaponPunchPitch
        {
            get
            {
                return _playerEntity.orientation.WeaponPunchPitch;
            }
            set
            {
                _playerEntity.orientation.WeaponPunchPitch = value;
            }
        }
        public float WeaponPunchYaw
        {
            get
            {
                return _playerEntity.orientation.WeaponPunchYaw;
            }
            set
            {
                _playerEntity.orientation.WeaponPunchYaw = value;
            }
        }

        public Transform SightLocatorOnPartP1
        {
            get
            {
                return BoneMount.FindChildBoneFromCache(_playerEntity.firstPersonModel.Value, BoneName.AttachmentSight, true);
            }
        }

        public Transform SightLocatorOnWeaponP1
        {
            get
            {
                return BoneMount.FindChildBoneFromCache(_playerEntity.firstPersonModel.Value, BoneName.WeaponSight, true);
            }

        }

        public Vector3 CameraAngle
        {
            get
            {
                return _playerEntity.cameraFinalOutputNew.EulerAngle;
            }
        }

        public Vector3 CameraPosition
        {
            get
            {
                return _playerEntity.cameraFinalOutputNew.Position;
            }
        }

        public Vector3? GetSightFireViewPosition
        {
            get { return _playerEntity.firePosition.SightValid ? _playerEntity.firePosition.SightPosition : (Vector3?)null; }
        }

        public Vector3? MuzzleP3LocatorPosition
        {
            get { return _playerEntity.firePosition.MuzzleP3Valid ? _playerEntity.firePosition.MuzzleP3Position : (Vector3?)null; }
        }

        public IFreeData FreeData
        {
            get
            {
                return _playerEntity.freeData.FreeData;
            }
        }
        public GameObject CurrentWeaponGo
        {
            get
            {
                if (_playerEntity.appearanceInterface.Appearance.IsFirstPerson)
                {
                    return _playerEntity.appearanceInterface.Appearance.GetWeaponP1InHand();
                }
                else
                {
                    return _playerEntity.appearanceInterface.Appearance.GetWeaponP3InHand();
                }
            }
        }
        public bool IsThrowingStartFly
        {
            get
            {
                return _playerEntity.throwingUpdate.IsStartFly;
            }

            set
            {
                _playerEntity.throwingUpdate.IsStartFly = value;
            }
        }
    }


}

