using Core.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using UnityEngine;

namespace Core.CharacterState
{
    public class AnimatorParametersHash
    {
        private static AnimatorParametersHash _instance;

        public static AnimatorParametersHash Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AnimatorParametersHash();
                }
                return _instance;
            }
        }

        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(AnimatorParametersHash));

        public const int PeekTime = 150;
        public const float Epsilon = 0.01f;
        public const int ImpossibleTransitionTime = 10000;
        public const float DefaultMaxRemainTime = 10000;
        public const float FirstPersonStandCameraHeight = 1.55f;
        public const float FirstPersonCrouchCameraHeight = 1.0f;
        public const float FirstPersonProneCameraHeight = 0.25f;

        public static readonly string InjureyStateString = "UpperBody Add Layer.Injury";
        public static readonly int InjureyStateHash = Animator.StringToHash(InjureyStateString);
        public static readonly float InjureyStateDuration = 0.333f;

        /// <summary>
        /// 0.17f是站立相机位置和人眼位置的偏移
        /// </summary>
        public const float FirstPersonStandCameraForwardOffset = 0.17f;
        /// <summary>
        /// 0.18是蹲下相机位置和人眼位置的偏移
        /// </summary>
        public const float FirstPersonCrouchCameraForwardOffset = 0.17f;
        /// <summary>
        /// 美术制作的一个偏移量
        /// 0.36是相机位置和人眼位置的偏移
        /// </summary>
        public const float FirstPersonProneCameraForwardOffset = 0.27593f + 0.36f;

        public const float DefaultAnimationSpeed = 1;

        private AnimatorParametersHash()
        {
            HorizontalName = "XDirection";
            VerticalName = "ZDirection";
            UpDownName = "YDirection";

            PostureName = "State";
            ProneValue = 0;
            CrouchValue = 1;
            StandValue = 2;
            StableValue = 3;

            FrontPostureName = "FrontPosture";
            FrontStand = 0f;
            FrontCrouch = 1f;

            HandStateName = "UpDown";
            HandDownValue = -1;
            HandIdleValue = 0;
            HandUpValue = 1;

            ProneName = "Prone";
            ProneEnable = true;
            ProneDisable = false;

            ForceEndProneName = "ForceEndProne";
            ForceEndProneEnable = true;
            ForceEndProneDisable = false;

            ForceToProneName = "ForceToProne";
            ForceToProneEnable = true;
            ForceToProneDisable = false;

            MotionName = "Speed";
            MotionlessValue = 0;
            MotionNullValue = 0.1f;
            MotionValue = 1;


            JumpStateName = "JumpState";
            JumpStateNormal = 0.0f;
            JumpStateMove = 1.0f;
            
            MoveJumpStateName = "MoveJumpState";
            MoveJumpStateNormal = 0.0f;
            MoveJumpStateLF = -1.0f;
            MoveJumpStateRF = 1.0f;
            
            MovementName = "MoveState";
            WalkValue = 0;
            RunValue = 1;
            SprintValue = 2;

            IsWalkName = "IsWalk";
            IsWalkEnable = true;
            IsWalkDisable = false;

            FireName = "Fire";
            FireEnableValue = true;
            FireDisableValue = false;

            ClimbName = "Climb";
            ClimbEnableValue = true;
            ClimbDisableValue = false;

            ClimbEndName = "ClimbEnd";
            ClimbEndEnableValue = true;
            ClimbEndDisableValue = false;

            ClimbStateName = "ClimbState";
            VaultValue = 0;
            StepValue = 1;
            ClimbValue = 2;

            SightsFireName = "SightsFire";
            SightsFireEnableValue = true;
            SightsFireDisableValue = false;

            FireHoldName = "FireHold";
            FireHoldEnableValue = true;
            FireHoldDisableValue = false;

            FireEndName = "FireEnd";
            FireEndEnableValue = true;
            FireEndDisableValue = false;

            ReloadName = "Reload";
            ReloadEnableValue = true;
            ReloadDisableValue = false;

            ReloadEmptyName = "ReloadEmpty";
            ReloadEmptyEnableValue = true;
            ReloadEmptyDisableValue = false;

            InjuryName = "Injury";
            InjuryStartValue = true;
            InjuryEndValue = false;

            SlideName = "Slide";
            SlideEnable = true;
            SlideDisable = false;

            JumpStartName = "Jump";
            JumpStartEnable = true;
            JumpStartDisable = false;

            FreeFallName = "JumpLoop";
            FreeFallEnable = true;
            FreeFallDisable = false;

            HolsterName = "Holster";
            HolsterEnableValue = true;
            HolsterDisableValue = false;

            HolsterStateName = "HolsterState";
            HolsterFromLeftValue = 1.0f;
            HolsterFromRightValue = 0.0f;

            SelectName = "Select";
            SelectEnableValue = true;
            SelectDisableValue = false;

            DrawStateName = "SelectState";
            DrawLeftValue = 1.0f;
            DrawRightValue = 0.0f;

            SwitchWeaponName = "SwitchWeapon";
            SwitchWeaponEnableValue = true;
            SwitchWeaponDisableValue = false;

            UpperBodySpeedRatioName = "UBSpeedScale";
            FullBodySpeedScaleName = "FBSpeedScale";

            RescueName = "Rescue";
            RescueEnableValue = true;
            RescueDisableValue = false;

            SpecialReloadName = "SpecialReload";
            SpecialReloadEnableValue = true;
            SpecialReloadDisableValue = false;

            ForceSpecialReloadEndName = "ForceSpecialReloadEnd";
            ForceSpecialReloadEndEnableValue = true;
            ForceSpecialReloadEndDisableValue = false;

            SwimStateName = "SwimState";
            SwimStateSwimValue = 0;
            SwimStateDiveValue = 1;

            VehiclesAnimName = "VehiclesAnim";
            VehiclesAnimEnableValue = true;
            VehiclesAnimDisableValue = false;

            VehiclesAnimStateName = "VehiclesAnimState";

            VehiclesStateName = "VehiclesState";

            PickUpName = "PickUp";
            PickUpEnable = true;
            PickUpDisable = false;

            PropsName = "Props";
            PropsEnable = true;
            PropsDisable = false;

            PropsEndName = "PropsEnd";
            PropsEndEnable = true;
            PropsEndDisable = false;

            PropsStateName = "PropsState";

            OpenDoorName = "OpenDoor";
            OpenDoorEnable = true;
            OpenDoorDisable = false;

            MeleeAttackName = "Melee";
            MeleeAttackStart = true;
            MeleeAttackEnd = false;

            MeleeStateName = "MeleeState";
            NullMelee = 0;
            LightMeleeOne = 1;
            LightMeleeTwo = 2;
            ForceMelee = 3;

            StartThrowName = "Throw";
            StartThrowEnable = true;
            StartThrowDisable = false;

            ForceFinishThrowName = "ForceThrowEnd";
            ForceFinishThrowEnable = true;
            ForceFinishThrowDisable = false;

            NearThrowName = "ThrowState";
            NearThrowEnable = 0;
            NearThrowDisable = 1;

            ParachuteName = "Parachute";
            ParachuteEnableValue = true;
            ParachuteDisableValue = false;

            GripHandPoseStateName = "GripHandPoseState";

            GlidingName = "Gliding";
            GlidingEnableValue = true;
            GlidingDisableValue = false;

            GripHandPoseName = "GripHandPose";
            GripHandPoseEnableValue = true;
            GripHandPoseDisableValue = false;

            UseName = "Use";
            UseEnableValue = true;
            UseDisableValue = false;

            DismantleName = "Dismantle";
            DismantleEnableValue = true;
            DismantleDisableValue = false;

            ADSLayer = 6;
            ADSLayerP1 = 2;
            ADSEnableValue = 1;
            ADSDisableValue = 0;

            SwimLayer = 2;
            SwimEnableValue = 1;
            SwimDisableValue = 0;

            DyingLayer = 3;
            DyingEnableValue = 1;
            DyingDisableValue = 0;

            InterruptName = "Interrupt";
            InterruptEnable = true;
            InterruptDisable = false;

            UpperBodyLayer = NetworkAnimatorLayer.PlayerUpperBodyOverlayLayer;
            UpperBodyEnableValue = 1;
            UpperBodyDisableValue = 0;

            foreach (string s in _extraHashString)
            {
                StringToHash(s);
            }
        }

        #region XDirection
        private string _horizontalName = string.Empty;
        public string HorizontalName
        {
            get { return _horizontalName; }
            set { _horizontalName = value; HorizontalHash = StringToHash(value); }
        }
        public int HorizontalHash { get; private set; }
        #endregion

        #region YDirection
        private string _upDownName = string.Empty;
        public string UpDownName
        {
            get { return _upDownName; }
            set { _upDownName = value; UpDownHash = StringToHash(value); }
        }
        public int UpDownHash { get; private set; }
        #endregion

        #region ZDirection
        private string _verticalName = string.Empty;
        public string VerticalName
        {
            get { return _verticalName; }
            set { _verticalName = value; VerticalHash = StringToHash(value); }
        }
        public int VerticalHash { get; private set; }
        #endregion

        #region State
        private string _postureName = string.Empty;
        public string PostureName
        {
            get { return _postureName; }
            set { _postureName = value; PostureHash = StringToHash(value); }
        }
        public int PostureHash { get; private set; }
        public float ProneValue { get; private set; }
        public float CrouchValue { get; private set; }
        public float StandValue { get; private set; }
        public float StableValue { get; private set; }
        #endregion

        #region FrontPosture

        private string _frontPostureName = String.Empty;
        public string FrontPostureName
        {
            get { return _frontPostureName; }
            set { _frontPostureName = value;
                FrontPostureHash = StringToHash(value);
            }
        }
        public int FrontPostureHash { get; private set; }
        public float FrontStand { get; private set; }
        public float FrontCrouch { get; private set; }

        #endregion

        #region Prone
        private string _proneName = string.Empty;
        public string ProneName
        {
            get { return _proneName; }
            set { _proneName = value; ProneHash = StringToHash(value); }
        }
        public int ProneHash { get; private set; }
        public bool ProneEnable { get; private set; }
        public bool ProneDisable { get; private set; }
        #endregion

        #region HandState
        private string _handStateName = string.Empty;
        public string HandStateName
        {
            get { return _handStateName; }
            set { _handStateName = value; HandStateHash = StringToHash(value); }
        }
        public int HandStateHash { get; private set; }
        public float HandDownValue { get; private set; }
        public float HandIdleValue { get; private set; }
        public float HandUpValue { get; private set; }
        #endregion

        #region ForceEndProne
        private string _forceEndProneName = string.Empty;
        public string ForceEndProneName
        {
            get { return _forceEndProneName; }
            set { _forceEndProneName = value; ForceEndProneHash = StringToHash(value); }
        }
        public int ForceEndProneHash { get; private set; }
        public bool ForceEndProneEnable { get; private set; }
        public bool ForceEndProneDisable { get; private set; }
        #endregion
        
        #region ForceToProne
        private string _forceToProneName = string.Empty;
        public string ForceToProneName
        {
            get { return _forceToProneName; }
            set { _forceToProneName = value; ForceToProneHash = StringToHash(value); }
        }
        public int ForceToProneHash { get; private set; }
        public bool ForceToProneEnable { get; private set; }
        public bool ForceToProneDisable { get; private set; }
        #endregion

        #region JumpState
        private string _jumpStateName = string.Empty;
        public string JumpStateName
        {
            get { return _jumpStateName; }
            set { _jumpStateName = value; JumpStateHash = StringToHash(value); }
        }
        public int JumpStateHash { get; private set; }
        public float JumpStateNormal { get; private set; }
        public float JumpStateMove { get; private set; }
        #endregion

        #region MoveJumpState

        private string _moveJumpStateName = string.Empty;

        public string MoveJumpStateName
        {
            get { return _moveJumpStateName; }
            set
            {
                _moveJumpStateName = value;
                MoveJumpStateHash = StringToHash(value);
            }
        }
        public int MoveJumpStateHash { get; private set; }
        public float MoveJumpStateLF { get; private set; }
        public float MoveJumpStateRF { get; private set; }
        public float MoveJumpStateNormal { get; private set; }

        #endregion

        #region Slide

        private string _slideName = string.Empty;

        public string SlideName
        {
            get { return _slideName; }
            set
            {
                _slideName = value;
                SlideHash = StringToHash(_slideName);
            }
        }

        public int SlideHash
        {
            get;
            private set;
        }

        public bool SlideEnable { get; private set; }
        public bool SlideDisable { get; private set; }

        #endregion
        
        #region Jump
        private string _jumpStartName = string.Empty;
        public string JumpStartName
        {
            get { return _jumpStartName; }
            set { _jumpStartName = value; JumpStartHash = StringToHash(value); }
        }
        public int JumpStartHash { get; private set; }
        public bool JumpStartEnable { get; private set; }
        public bool JumpStartDisable { get; private set; }
        #endregion

        #region JumpLoop
        private string _freeFallName = string.Empty;
        public string FreeFallName
        {
            get { return _freeFallName; }
            set { _freeFallName = value; FreeFallHash = StringToHash(value); }
        }
        public int FreeFallHash { get; private set; }
        public bool FreeFallEnable { get; private set; }
        public bool FreeFallDisable { get; private set; }
        #endregion

        #region Speed
        private string _motionName = string.Empty;
        public string MotionName
        {
            get { return _motionName; }
            set { _motionName = value; MotionHash = StringToHash(value); }
        }
        public int MotionHash { get; private set; }
        public float MotionNullValue { get; private set; }
        public float MotionlessValue { get; private set; }
        public float MotionValue { get; private set; }
        #endregion

        #region MoveState
        private string _movementName = string.Empty;
        public string MovementName
        {
            get { return _movementName; }
            set { _movementName = value; MovementHash = StringToHash(value); }
        }
        public int MovementHash { get; private set; }
        public float WalkValue { get; private set; }
        public float RunValue { get; private set; }
        public float SprintValue { get; private set; }
        #endregion
        
        #region IsWalk
        private string _isWalkName = string.Empty;
        public string IsWalkName
        {
            get { return _isWalkName; }
            set { _isWalkName = value; IsWalkHash = StringToHash(value); }
        }
        public int IsWalkHash { get; private set; }
        public bool IsWalkEnable { get; private set; }
        public bool IsWalkDisable { get; private set; }
        #endregion

        #region Fire
        private string _fireName = string.Empty;
        public string FireName
        {
            get { return _fireName; }
            set { _fireName = value; FireHash = StringToHash(value); }
        }
        public int FireHash { get; private set; }
        public bool FireEnableValue { get; private set; }
        public bool FireDisableValue { get; private set; }
        #endregion

        #region climbing
        private string _climbName = string.Empty;
        public string ClimbName
        {
            get { return _climbName; }
            set { _climbName = value;  ClimbHash = StringToHash(value); }
        }
        public int ClimbHash { get; private set; }
        public bool ClimbEnableValue { get; private set; }
        public bool ClimbDisableValue { get; private set; }
        #endregion

        #region climbEnd
        private string _climbEndName = string.Empty;
        public string ClimbEndName
        {
            get { return _climbEndName; }
            set { _climbEndName = value;  ClimbEndHash = StringToHash(value); }
        }
        public int ClimbEndHash { get; private set; }
        public bool ClimbEndEnableValue { get; private set; }
        public bool ClimbEndDisableValue { get; private set; }
        #endregion

        #region climbState
        private string _climbStateName = string.Empty;
        public string ClimbStateName
        {
            get { return _climbStateName; }
            set { _climbStateName = value; ClimbStateHash = StringToHash(value); }
        }
        public int ClimbStateHash { get; private set; }
        public float VaultValue { get; private set; }
        public float StepValue { get; private set; }
        public float ClimbValue { get; private set; }
        #endregion

        #region SightsFire
        private string _sightsFireName = string.Empty;
        public string SightsFireName
        {
            get { return _sightsFireName; }
            set { _sightsFireName = value; SightsFireHash = StringToHash(value); }
        }
        public int SightsFireHash { get; private set; }
        public bool SightsFireEnableValue { get; private set; }
        public bool SightsFireDisableValue { get; private set; }
        #endregion

        #region FireHold
        private string _fireHoldName = string.Empty;
        public string FireHoldName
        {
            get { return _fireHoldName; }
            set { _fireHoldName = value; FireHoldHash = StringToHash(value); }
        }
        public int FireHoldHash { get; private set; }
        public bool FireHoldEnableValue { get; private set; }
        public bool FireHoldDisableValue { get; private set; }
        #endregion

        #region FireEnd
        private string _fireEndName = string.Empty;
        public string FireEndName
        {
            get { return _fireEndName; }
            set { _fireEndName = value; FireEndHash = StringToHash(value); }
        }
        public int FireEndHash { get; private set; }
        public bool FireEndEnableValue { get; private set; }
        public bool FireEndDisableValue { get; private set; }
        #endregion

        #region Injury
        private string _injuryName = string.Empty;
        public string InjuryName
        {
            get { return _injuryName; }
            set { _injuryName = value; InjuryHash = StringToHash(value); }
        }
        public int InjuryHash { get; private set; }
        public bool InjuryStartValue { get; private set; }
        public bool InjuryEndValue { get; private set; }
        #endregion

        #region Reload
        private string _reloadName = string.Empty;
        public string ReloadName
        {
            get { return _reloadName; }
            set { _reloadName = value; ReloadHash = StringToHash(value); }
        }
        public int ReloadHash { get; private set; }
        public bool ReloadEnableValue { get; private set; }
        public bool ReloadDisableValue { get; private set; }
        #endregion

        #region ReloadState
        private string _reloadEmptyName = string.Empty;
        public string ReloadEmptyName
        {
            get { return _reloadEmptyName; }
            set { _reloadEmptyName = value; ReloadEmptyHash = StringToHash(value); }
        }
        public int ReloadEmptyHash { get; private set; }
        public bool ReloadEmptyEnableValue { get; private set; }
        public bool ReloadEmptyDisableValue { get; private set; }
        #endregion

        #region Holster
        private string _holsterName = string.Empty;
        public string HolsterName
        {
            get { return _holsterName; }
            set { _holsterName = value; HolsterHash = StringToHash(value); }
        }
        public int HolsterHash { get; private set; }
        public bool HolsterEnableValue { get; private set; }
        public bool HolsterDisableValue { get; private set; }
        #endregion

        #region HolsterState

        private string _holsterStateName = string.Empty;

        public string HolsterStateName
        {
            get { return _holsterStateName; }
            set { _holsterName = value;
                HolsterStateHash = StringToHash(value);
            }
        }
        public int HolsterStateHash { get; private set; }
        public float HolsterFromLeftValue { get; private set; }
        public float HolsterFromRightValue { get; private set; }

        #endregion

        #region Select
        private string _selectName = string.Empty;
        public string SelectName
        {
            get { return _selectName; }
            set { _selectName = value; SelectHash = StringToHash(value); }
        }
        public int SelectHash { get; private set; }
        public bool SelectEnableValue { get; private set; }
        public bool SelectDisableValue { get; private set; }
        #endregion

        #region DrawState
        private string _drawStateName = string.Empty;
        public string DrawStateName
        {
            get { return _drawStateName; }
            set { _drawStateName = value;DrawStateHash = StringToHash(value); }
        }
        public int DrawStateHash { get; private set; }
        public float DrawRightValue { get; private set; }
        public float DrawLeftValue { get; private set; }
        #endregion

        #region SwitchWeapon
        private string _switchWeaponName = string.Empty;
        public string SwitchWeaponName
        {
            get { return _switchWeaponName; }
            set { _switchWeaponName = value; SwitchWeaponHash = StringToHash(value); }
        }
        public int SwitchWeaponHash { get; private set; }
        public bool SwitchWeaponEnableValue { get; private set; }
        public bool SwitchWeaponDisableValue { get; private set; }
        #endregion

        #region UBSpeedScale
        private string _upperBodySpeedRatioName = string.Empty;
        public string UpperBodySpeedRatioName
        {
            get { return _upperBodySpeedRatioName; }
            set { _upperBodySpeedRatioName = value; UpperBodySpeedRatioHash = StringToHash(value); }
        }
        public int UpperBodySpeedRatioHash { get; private set; }
        #endregion

        #region FBSpeedScale
        private string _fullBodySpeedScaleName = string.Empty;
        public string FullBodySpeedScaleName
        {
            get { return _fullBodySpeedScaleName; }
            set { _fullBodySpeedScaleName = value; FullBodySpeedRatioHash = StringToHash(value); }
        }
        public int FullBodySpeedRatioHash { get; private set; }
        #endregion

        #region Rescue
        private string _rescueName = string.Empty;
        public string RescueName
        {
            get { return _rescueName; }
            set { _rescueName = value; RescueHash = StringToHash(value); }
        }
        public int RescueHash { get; private set; }
        public bool RescueEnableValue { get; private set; }
        public bool RescueDisableValue { get; private set; }
        #endregion

        #region SpecialReload

        private string _specialReloadName = string.Empty;
        public string SpecialReloadName
        {
            get { return _specialReloadName; }
            set { _specialReloadName = value; SpecialReloadHash = StringToHash(value); }
        }
        public int SpecialReloadHash { get; private set; }
        public bool SpecialReloadEnableValue { get; private set; }
        public bool SpecialReloadDisableValue { get; private set; }

        private string _forceSpecialReloadEndName = string.Empty;
        public string ForceSpecialReloadEndName
        {
            get { return _forceSpecialReloadEndName; }
            set { _forceSpecialReloadEndName = value; ForceSpecialReloadEndHash = StringToHash(value); }
        }
        public int ForceSpecialReloadEndHash { get; private set; }
        public bool ForceSpecialReloadEndEnableValue { get; private set; }
        public bool ForceSpecialReloadEndDisableValue { get; private set; }

        #endregion

        #region SwimState

        private string _swimStateName = string.Empty;
        public string SwimStateName
        {
            get { return _swimStateName; }
            set { _swimStateName = value; SwimStateHash = StringToHash(value); }
        }
        public int SwimStateHash { get; private set; }
        public float SwimStateSwimValue { get; private set; }
        public float SwimStateDiveValue { get; private set; }

        #endregion

        #region PickUp(拾取)
        private string _pickUpName = string.Empty;
        public string PickUpName
        {
            get { return _pickUpName; }
            set { _pickUpName = value; PickUpHash = StringToHash(value); }
        }
        public int PickUpHash { get; private set; }
        public bool PickUpEnable { get; private set; }
        public bool PickUpDisable { get; private set; }
        #endregion

        #region Props
        private string _propsName = string.Empty;
        public string PropsName
        {
            get { return _propsName; }
            set { _propsName = value; PropsHash = StringToHash(value); }
        }
        public int PropsHash { get; private set; }
        public bool PropsEnable { get; private set; }
        public bool PropsDisable { get; private set; }
        #endregion

        #region PropsEnd
        private string _propsEndName = string.Empty;
        public string PropsEndName
        {
            get { return _propsEndName; }
            set { _propsEndName = value; PropsEndHash = StringToHash(value); }
        }
        public int PropsEndHash { get; private set; }
        public bool PropsEndEnable { get; private set; }
        public bool PropsEndDisable { get; private set; }
        #endregion

        #region PropsState
        private string _propsStateName = string.Empty;
        public string PropsStateName
        {
            get { return _propsStateName; }
            set { _propsStateName = value; PropsStateHash = StringToHash(value); }
        }
        public int PropsStateHash { get; private set; }
        #endregion

        #region OpenDoor
        private string _openDoorName = string.Empty;
        public string OpenDoorName
        {
            get { return _openDoorName; }
            set { _openDoorName = value; OpenDoorHash = StringToHash(value); }
        }
        public int OpenDoorHash { get; private set; }
        public bool OpenDoorEnable { get; private set; }
        public bool OpenDoorDisable { get; private set; }
        #endregion

        #region VehiclesAnim
        private string _vehiclesAnimName = string.Empty;
        public string VehiclesAnimName
        {
            get { return _vehiclesAnimName; }
            set { _vehiclesAnimName = value; VehiclesAnimHash = StringToHash(value); }
        }
        public int VehiclesAnimHash { get; private set; }
        public bool VehiclesAnimEnableValue { get; private set; }
        public bool VehiclesAnimDisableValue { get; private set; }
        #endregion

        #region VehiclesAnimState
        private string _vehiclesAnimStateName = string.Empty;
        public string VehiclesAnimStateName
        {
            get { return _vehiclesAnimStateName; }
            set { _vehiclesAnimStateName = value; VehiclesAnimStateHash = StringToHash(value); }
        }
        public int VehiclesAnimStateHash { get; private set; }
        #endregion

        #region VehiclesState
        private string _vehiclesStateName = string.Empty;
        public string VehiclesStateName
        {
            get { return _vehiclesStateName; }
            set { _vehiclesStateName = value; VehiclesStateHash = StringToHash(value); }
        }
        public int VehiclesStateHash { get; private set; }
        #endregion

        #region MeleeAttack
        private string _meleeAttackName = string.Empty;
        public string MeleeAttackName
        {
            get { return _meleeAttackName; }
            set { _meleeAttackName = value; MeleeAttackHash = StringToHash(value); }
        }
        public int MeleeAttackHash { get; private set; }
        public bool MeleeAttackStart { get; private set; }
        public bool MeleeAttackEnd { get; private set; }
        #endregion
        
        #region MeleeState

        private string _meleeStateName = string.Empty;
        public string MeleeStateName
        {
            get { return _meleeStateName; }
            set { _meleeStateName = value; MeleeStateHash = StringToHash(value); }
        }
        public int MeleeStateHash { get; private set; }
        public int NullMelee { get; private set; }
        public int LightMeleeOne { get; private set; }
        public int LightMeleeTwo { get; private set; }
        public int ForceMelee { get; private set; }

        #endregion

        #region Grenade
        private string _startThrowName = string.Empty;
        public string StartThrowName
        {
            get { return _startThrowName; }
            set { _startThrowName = value; StartThrowHash = StringToHash(value); }
        }
        public int StartThrowHash { get; private set; }
        public bool StartThrowEnable { get; private set; }
        public bool StartThrowDisable { get; private set; }

        private string _forceFinishThrowName = string.Empty;
        public string ForceFinishThrowName
        {
            get { return _forceFinishThrowName; }
            set { _forceFinishThrowName = value; ForceFinishThrowHash = StringToHash(value); }
        }
        public int ForceFinishThrowHash { get; private set; }
        public bool ForceFinishThrowEnable { get; private set; }
        public bool ForceFinishThrowDisable { get; private set; }

        private string _nearThrowName = string.Empty;
        public string NearThrowName
        {
            get { return _nearThrowName; }
            set { _nearThrowName = value; NearThrowHash = StringToHash(value); }
        }
        public int NearThrowHash { get; private set; }
        public float NearThrowEnable { get; private set; }
        public float NearThrowDisable { get; private set; }

        #endregion

        #region Parachute
        private string _parachuteName = string.Empty;
        public string ParachuteName
        {
            get { return _parachuteName; }
            set { _parachuteName = value; ParachuteHash = StringToHash(value); }
        }
        public int ParachuteHash { get; private set; }
        public bool ParachuteEnableValue { get; private set; }
        public bool ParachuteDisableValue { get; private set; }
        #endregion

        #region Gliding
        private string _glidingName = string.Empty;
        public string GlidingName
        {
            get { return _glidingName; }
            set { _glidingName = value; GlidingHash = StringToHash(value); }
        }
        public int GlidingHash { get; private set; }
        public bool GlidingEnableValue { get; private set; }
        public bool GlidingDisableValue { get; private set; }
        #endregion
        
        #region Use
        private string _useName = string.Empty;
        public string UseName
        {
            get { return _useName; }
            set { _useName = value; UseHash = StringToHash(value); }
        }
        public int UseHash { get; private set; }
        public bool UseEnableValue { get; private set; }
        public bool UseDisableValue { get; private set; }
        #endregion
        
        #region Dismantle
        private string _dismantleName = string.Empty;
        public string DismantleName
        {
            get { return _dismantleName; }
            set { _dismantleName = value; DismantleHash = StringToHash(value); }
        }
        public int DismantleHash { get; private set; }
        public bool DismantleEnableValue { get; private set; }
        public bool DismantleDisableValue { get; private set; }
        #endregion

        #region GripHandPose
        private string _gripHandPoseName = string.Empty;
        public string GripHandPoseName
        {
            get { return _gripHandPoseName; }
            set { _gripHandPoseName = value; GripHandPoseHash = StringToHash(value); }
        }
        public int GripHandPoseHash { get; private set; }
        public bool GripHandPoseEnableValue { get; private set; }
        public bool GripHandPoseDisableValue { get; private set; }
        #endregion

        #region GripHandPoseState
        private string _gripHandPoseStateName = string.Empty;
        public string GripHandPoseStateName
        {
            get { return _gripHandPoseStateName; }
            set { _gripHandPoseStateName = value; GripHandPoseStateHash = StringToHash(value); }
        }
        public int GripHandPoseStateHash { get; private set; }
        #endregion

        #region Interrupt
        private string _interruptName = string.Empty;
        public string InterruptName
        {
            get { return _interruptName; }
            set { _interruptName = value; InterruptHash = StringToHash(value); }
        }
        public int InterruptHash { get; private set; }
        public bool InterruptEnable;
        public bool InterruptDisable;
        #endregion

        public int ADSLayer { get; private set; }
        public int ADSLayerP1 { get; private set; }
        public float ADSEnableValue { get; private set; }
        public float ADSDisableValue { get; private set; }

        public int SwimLayer { get; private set; }
        public float SwimEnableValue { get; private set; }
        public float SwimDisableValue { get; private set; }

        public int DyingLayer { get; private set; }
        public float DyingEnableValue { get; private set; }
        public float DyingDisableValue { get; private set; }

        public int UpperBodyLayer { get; private set; }
        public float UpperBodyEnableValue { get; private set; }
        public float UpperBodyDisableValue { get; private set; }

        private int StringToHash(string str)
        {
            int ret = Animator.StringToHash(str);
            if (_logger.IsInfoEnabled)
            {
                _logger.InfoFormat("string:{0}==>hash:{1}", str, ret);
                _hashToString.Add(ret, str);
            }
            return ret;
        }

        private Dictionary<int, string> _hashToString = new Dictionary<int, string>();
        private List<string> _extraHashString = new List<string>()
        {
            "LowerBody Layer.Idle",
            "LowerBody Layer.Idle2Prone",
            "LowerBody Layer.ProneIdle",
            "LowerBody Layer.ProneMove",
            "LowerBody Layer.Prone2Idle",
            "LowerBody Layer.Move",
            "LowerBody Layer.Jump_Start",
            "LowerBody Layer.Jump_Loop",
            "LowerBody Layer.Jump_End",
            "LowerBody Layer.Idle2Move",
            "LowerBody Layer.Idle2Prone -> LowerBody Layer.ProneIdle",
            "LowerBody Layer.Idle -> LowerBody Layer.Idle2Prone",
            "InjuredMove Layer.Idle",
            "InjuredMove Layer.Move",
            "InjuredMove Layer.Idle -> InjuredMove Layer.Move",
            "InjuredMove Layer.Move -> InjuredMove Layer.Idle",
            "LowerBody Layer.Move -> LowerBody Layer.Jump_Loop",
            "LowerBody Layer.Move -> LowerBody Layer.Jump_Start",
            "UpperBody Add Layer.Injury"
        };

        public string GetHashString(int hash)
        {
            if (_hashToString.ContainsKey(hash))
            {
                return _hashToString[hash];
            }

            return hash.ToString();
        }
    }
}
