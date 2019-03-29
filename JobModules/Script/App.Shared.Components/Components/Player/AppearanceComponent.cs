using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Appearance;
using Core.Compare;
using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Core.Compensation;
using UnityEngine;
using Core.SyncLatest;
using Core.UpdateLatest;
using Utils.Appearance;
using Utils.Utils;

namespace App.Shared.Components.Player
{
    [Player]
    public class AppearanceInterfaceComponent : IGameComponent
    {
        public ICharacterAppearance Appearance;
        [DontInitilize] public ICharacterFirstPersonAppearance FirstPersonAppearance;

        public int GetComponentId()
        {
            return (int) EComponentIds.AppearanceGameObject;
        }
    }

    [Player]
    public class CharacterControllerInterfaceComponent : IComponent
    {
        public ICharacterControllerAppearance CharacterController;
    }

    [Player]
    public class FirstPersonAppearanceComponent : IUserPredictionComponent, IPredictedAppearanceState
    {
        #region Properties

        public override string ToString()
        {
            return string.Format(
                "FirstPersonHeight: {0}, FirstPersonForwardOffset: {1}, SightHorizontalShift: {2}, SightVerticalShift: {3}, SightVerticalShiftRange: {4}, SightHorizontalShiftDirection: {5}, SightVerticalShiftDirection: {6}, SightRemainVerticalPeriodTime: {7}, RandomSeed: {8}",
                FirstPersonHeight, FirstPersonForwardOffset, SightHorizontalShift, SightVerticalShift,
                SightVerticalShiftRange, SightHorizontalShiftDirection, SightVerticalShiftDirection,
                SightRemainVerticalPeriodTime, RandomSeed);
        }

        [DontInitilize] [NetworkProperty] public float FirstPersonHeight { get; set; }
        [DontInitilize] [NetworkProperty] public float FirstPersonForwardOffset { get; set; }
        [DontInitilize] [NetworkProperty] public float SightHorizontalShift { get; set; }
        [DontInitilize] [NetworkProperty] public float SightVerticalShift { get; set; }
        [DontInitilize] [NetworkProperty] public float SightVerticalShiftRange { get; set; }
        [DontInitilize] [NetworkProperty] public int SightHorizontalShiftDirection { get; set; }
        [DontInitilize] [NetworkProperty] public int SightVerticalShiftDirection { get; set; }
        [DontInitilize] [NetworkProperty] public int SightRemainVerticalPeriodTime { get; set; }
        [NetworkProperty] public int RandomSeed { get; set; }

        #endregion

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFirstPersonAppearance;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as FirstPersonAppearanceComponent;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(FirstPersonHeight, rightObj.FirstPersonHeight)
                       && CompareUtility.IsApproximatelyEqual(FirstPersonForwardOffset,
                           rightObj.FirstPersonForwardOffset)
                       && CompareUtility.IsApproximatelyEqual(SightHorizontalShift, rightObj.SightHorizontalShift)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShift, rightObj.SightVerticalShift)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShiftRange, rightObj.SightVerticalShiftRange)
                       && CompareUtility.IsApproximatelyEqual(SightHorizontalShiftDirection,
                           rightObj.SightHorizontalShiftDirection)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShiftDirection,
                           rightObj.SightVerticalShiftDirection)
                       && CompareUtility.IsApproximatelyEqual(SightRemainVerticalPeriodTime,
                           rightObj.SightRemainVerticalPeriodTime)
                       && CompareUtility.IsApproximatelyEqual(RandomSeed, rightObj.RandomSeed);
            }

            return false;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as FirstPersonAppearanceComponent;
            if (right != null)
            {
                FirstPersonHeight = right.FirstPersonHeight;
                FirstPersonForwardOffset = right.FirstPersonForwardOffset;
                SightHorizontalShift = right.SightHorizontalShift;
                SightVerticalShift = right.SightVerticalShift;
                SightVerticalShiftRange = right.SightVerticalShiftRange;
                SightHorizontalShiftDirection = right.SightHorizontalShiftDirection;
                SightVerticalShiftDirection = right.SightVerticalShiftDirection;
                SightRemainVerticalPeriodTime = right.SightRemainVerticalPeriodTime;
                RandomSeed = right.RandomSeed;
            }
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }


    [Player]
    public class FirstPersonAppearanceUpdateComponent : IUpdateComponent
    {
        [DontInitilize] [NetworkProperty] public float SightHorizontalShift { get; set; }
        [DontInitilize] [NetworkProperty] public float SightVerticalShift { get; set; }
        [DontInitilize] [NetworkProperty] public float SightVerticalShiftRange { get; set; }
        [DontInitilize] [NetworkProperty] public int SightHorizontalShiftDirection { get; set; }
        [DontInitilize] [NetworkProperty] public int SightVerticalShiftDirection { get; set; }
        [DontInitilize] [NetworkProperty] public int SightRemainVerticalPeriodTime { get; set; }
        [DontInitilize] [NetworkProperty] public int RandomSeed { get; set; }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as FirstPersonAppearanceUpdateComponent;
            SightHorizontalShift = right.SightHorizontalShift;
            SightVerticalShift = right.SightVerticalShift;
            SightVerticalShiftRange = right.SightVerticalShiftRange;
            SightHorizontalShiftDirection = right.SightHorizontalShiftDirection;
            SightVerticalShiftDirection = right.SightVerticalShiftDirection;
            SightRemainVerticalPeriodTime = right.SightRemainVerticalPeriodTime;
            RandomSeed = right.RandomSeed;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFirstPersonAppearanceUpdate;
        }
    }


    [Player]
    public class ThirdPersonAppearanceComponent : IPlaybackComponent
    {
        #region Properties

        // never use these directly
        [DontInitilize] [NetworkProperty] public int PostureValue;
        [DontInitilize] [NetworkProperty] public int NextPostureValue;
        [DontInitilize] [NetworkProperty] public int ActionValue;
        [DontInitilize] [NetworkProperty] public int MovementValue;
        [DontInitilize] [NetworkProperty] public float PeekDegree;
        [DontInitilize] [NetworkProperty] public bool NeedUpdateController;
        [DontInitilize] [NetworkProperty] public float CharacterHeight;
        [DontInitilize] [NetworkProperty] public Vector3 CharacterCenter;
        [DontInitilize] [NetworkProperty] public float CharacterRadius;

        [DontInitilize]
        public ThirdPersonPosture Posture
        {
            get { return (ThirdPersonPosture) PostureValue; }
            set { PostureValue = (int) value; }
        }
        
        [DontInitilize]
        public ThirdPersonPosture NextPosture
        {
            get { return (ThirdPersonPosture) NextPostureValue; }
            set { NextPostureValue = (int) value; }
        }

        [DontInitilize]
        public ThirdPersonAction Action
        {
            get { return (ThirdPersonAction) ActionValue; }
            set { ActionValue = (int) value; }
        }

        [DontInitilize]
        public ThirdPersonMovement Movement
        {
            get { return (ThirdPersonMovement) MovementValue; }
            set { MovementValue = (int) value; }
        }

        #endregion

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerThirdPersonAppearance;
        }

        public bool IsInterpolateEveryFrame()
        {
            return true;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            ThirdPersonAppearanceComponent leftComponent = left as ThirdPersonAppearanceComponent;
            ThirdPersonAppearanceComponent rightComponent = right as ThirdPersonAppearanceComponent;
            CopyFrom(rightComponent);
            PeekDegree = Mathf.Lerp(leftComponent.PeekDegree, rightComponent.PeekDegree, interpolationInfo.Ratio);
            CharacterHeight = Mathf.Lerp(leftComponent.CharacterHeight, rightComponent.CharacterHeight,
                interpolationInfo.Ratio);
            CharacterRadius = Mathf.Lerp(leftComponent.CharacterRadius, rightComponent.CharacterRadius,
                interpolationInfo.Ratio);
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as ThirdPersonAppearanceComponent;
            if (right != null)
            {
                Posture = right.Posture;
                NextPosture = right.NextPosture;
                Action = right.Action;
                Movement = right.Movement;
                PeekDegree = right.PeekDegree;
                NeedUpdateController = right.NeedUpdateController;
                CharacterHeight = right.CharacterHeight;
                CharacterCenter = right.CharacterCenter;
                CharacterRadius = right.CharacterRadius;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "PostureValue: {0}, ActionValue: {1}, PeekDegree: {2}, NeedUpdateController: {3}, CharacterHeight: {4}, CharacterCenter: {5}, CharacterRadius: {6}",
                PostureValue, ActionValue, PeekDegree, NeedUpdateController, CharacterHeight, CharacterCenter,
                CharacterRadius);
        }
    }

    [Player]
    public class LatestAppearanceComponent : ISelfLatestComponent, IPlaybackComponent
    {
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOne;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneMuzzle;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneLowRail;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneMagazine;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneButtstock;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneScope;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwo;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoMuzzle;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoLowRail;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoMagazine;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoButtstock;
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoScope;
        [DontInitilize] [NetworkProperty] public int SideArm;
        [DontInitilize] [NetworkProperty] public int SideArmMuzzle;
        [DontInitilize] [NetworkProperty] public int SideArmLowRail;
        [DontInitilize] [NetworkProperty] public int SideArmMagazine;
        [DontInitilize] [NetworkProperty] public int SideArmButtstock;
        [DontInitilize] [NetworkProperty] public int SideArmScope;
        [DontInitilize] [NetworkProperty] public int MeleeWeapon;
        [DontInitilize] [NetworkProperty] public int ThrownWeapon;
        [DontInitilize] [NetworkProperty] public int TacticWeapon;

        [DontInitilize] [NetworkProperty] public int Cap;
        [DontInitilize] [NetworkProperty] public int PendantFace;
        [DontInitilize] [NetworkProperty] public int Inner;
        [DontInitilize] [NetworkProperty] public int Armor;
        [DontInitilize] [NetworkProperty] public int Outer;
        [DontInitilize] [NetworkProperty] public int Glove;
        [DontInitilize] [NetworkProperty] public int Waist;
        [DontInitilize] [NetworkProperty] public int Trouser;
        [DontInitilize] [NetworkProperty] public int Foot;
        [DontInitilize] [NetworkProperty] public int Bag;
        [DontInitilize] [NetworkProperty] public int Entirety;
        [DontInitilize] [NetworkProperty] public int CharacterHair;
        [DontInitilize] [NetworkProperty] public int CharacterHead;
        [DontInitilize] [NetworkProperty] public int CharacterGlove;
        [DontInitilize] [NetworkProperty] public int CharacterInner;
        [DontInitilize] [NetworkProperty] public int CharacterTrouser;
        [DontInitilize] [NetworkProperty] public int CharacterFoot;
        [DontInitilize] [NetworkProperty] public int PropId;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerLatestAppearance;
        }

        public void Init()
        {
            PrimaryWeaponOne = UniversalConsts.InvalidIntId;
            PrimaryWeaponOneMuzzle = UniversalConsts.InvalidIntId;
            PrimaryWeaponOneLowRail = UniversalConsts.InvalidIntId;
            PrimaryWeaponOneMagazine = UniversalConsts.InvalidIntId;
            PrimaryWeaponOneButtstock = UniversalConsts.InvalidIntId;
            PrimaryWeaponOneScope = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwo = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwoMuzzle = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwoLowRail = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwoMagazine = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwoButtstock = UniversalConsts.InvalidIntId;
            PrimaryWeaponTwoScope = UniversalConsts.InvalidIntId;
            SideArm = UniversalConsts.InvalidIntId;
            SideArmMuzzle = UniversalConsts.InvalidIntId;
            SideArmLowRail = UniversalConsts.InvalidIntId;
            SideArmMagazine = UniversalConsts.InvalidIntId;
            SideArmButtstock = UniversalConsts.InvalidIntId;
            SideArmScope = UniversalConsts.InvalidIntId;
            MeleeWeapon = UniversalConsts.InvalidIntId;
            ThrownWeapon = UniversalConsts.InvalidIntId;
            TacticWeapon = UniversalConsts.InvalidIntId;
            Cap = UniversalConsts.InvalidIntId;
            PendantFace = UniversalConsts.InvalidIntId;
            Inner = UniversalConsts.InvalidIntId;
            Armor = UniversalConsts.InvalidIntId;
            Outer = UniversalConsts.InvalidIntId;
            Glove = UniversalConsts.InvalidIntId;
            Waist = UniversalConsts.InvalidIntId;
            Trouser = UniversalConsts.InvalidIntId;
            Foot = UniversalConsts.InvalidIntId;
            Bag = UniversalConsts.InvalidIntId;
            Entirety = UniversalConsts.InvalidIntId;
            CharacterHair = UniversalConsts.InvalidIntId;
            CharacterHead = UniversalConsts.InvalidIntId;
            CharacterGlove = UniversalConsts.InvalidIntId;
            CharacterInner = UniversalConsts.InvalidIntId;
            CharacterTrouser = UniversalConsts.InvalidIntId;
            CharacterFoot = UniversalConsts.InvalidIntId;
            PropId = UniversalConsts.InvalidIntId;
        }

        public bool IsInterpolateEveryFrame()
        {
            return false;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as LatestAppearanceComponent;
            if (right != null)
            {
                PrimaryWeaponOne = right.PrimaryWeaponOne;
                PrimaryWeaponOneMuzzle = right.PrimaryWeaponOneMuzzle;
                PrimaryWeaponOneLowRail = right.PrimaryWeaponOneLowRail;
                PrimaryWeaponOneMagazine = right.PrimaryWeaponOneMagazine;
                PrimaryWeaponOneButtstock = right.PrimaryWeaponOneButtstock;
                PrimaryWeaponOneScope = right.PrimaryWeaponOneScope;
                PrimaryWeaponTwo = right.PrimaryWeaponTwo;
                PrimaryWeaponTwoMuzzle = right.PrimaryWeaponTwoMuzzle;
                PrimaryWeaponTwoLowRail = right.PrimaryWeaponTwoLowRail;
                PrimaryWeaponTwoMagazine = right.PrimaryWeaponTwoMagazine;
                PrimaryWeaponTwoButtstock = right.PrimaryWeaponTwoButtstock;
                PrimaryWeaponTwoScope = right.PrimaryWeaponTwoScope;
                SideArm = right.SideArm;
                SideArmMuzzle = right.SideArmMuzzle;
                SideArmLowRail = right.SideArmLowRail;
                SideArmMagazine = right.SideArmMagazine;
                SideArmButtstock = right.SideArmButtstock;
                SideArmScope = right.SideArmScope;
                MeleeWeapon = right.MeleeWeapon;
                ThrownWeapon = right.ThrownWeapon;
                TacticWeapon = right.TacticWeapon;
                Cap = right.Cap;
                PendantFace = right.PendantFace;
                Inner = right.Inner;
                Armor = right.Armor;
                Outer = right.Outer;
                Glove = right.Glove;
                Waist = right.Waist;
                Trouser = right.Trouser;
                Foot = right.Foot;
                Bag = right.Bag;
                Entirety = right.Entirety;
                CharacterHair = right.CharacterHair;
                CharacterHead = right.CharacterHead;
                CharacterGlove = right.CharacterGlove;
                CharacterInner = right.CharacterInner;
                CharacterTrouser = right.CharacterTrouser;
                CharacterFoot = right.CharacterFoot;
                PropId = right.PropId;
            }
        }
    }

    [Player]
    public class ClientAppearanceComponent : IUpdateComponent, IPlaybackComponent
    {
        [DontInitilize] [NetworkProperty] public int AlternativeWeaponLocator;
        [DontInitilize] [NetworkProperty] public int AlternativeP3WeaponLocator;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerClientAppearance;
        }
        
        public bool IsInterpolateEveryFrame()
        {
            return false;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var leftComponent = left as ClientAppearanceComponent;
            CopyFrom(leftComponent);
        }
        
        public void CopyFrom(object rightComponent)
        {
            var right = (ClientAppearanceComponent)rightComponent;
            if (right != null)
            {
                AlternativeWeaponLocator = right.AlternativeWeaponLocator;
                AlternativeP3WeaponLocator = right.AlternativeP3WeaponLocator;
            }
        }
    }

    [Player]
    public class PredictedAppearanceComponent : IUserPredictionComponent, ICompensationComponent, IPlaybackComponent
    {
        [DontInitilize] [NetworkProperty] public int WeaponInHand;
        [DontInitilize] [NetworkProperty] public int ReloadState;

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerPredictedAppearance;
        }

        public bool IsInterpolateEveryFrame()
        {
            return false;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var rightComponent = right as PredictedAppearanceComponent;
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightObj = (PredictedAppearanceComponent)right;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(WeaponInHand, rightObj.WeaponInHand);
            }

            return false;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = (PredictedAppearanceComponent)rightComponent;
            if (right != null)
            {
                WeaponInHand = right.WeaponInHand;
                ReloadState = right.ReloadState;
            }
        }

        public override string ToString()
        {
            return string.Format("WeaponInHand: {0}, ReloadState: {1}", WeaponInHand, ReloadState);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}