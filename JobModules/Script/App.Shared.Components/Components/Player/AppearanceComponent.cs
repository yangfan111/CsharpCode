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
            return string.Format("FirstPersonHeight: {0}, FirstPersonForwardOffset: {1}, SightHorizontalShift: {2}, SightVerticalShift: {3}, SightVerticalShiftRange: {4}, SightHorizontalShiftDirection: {5}, SightVerticalShiftDirection: {6}, SightRemainVerticalPeriodTime: {7}, RandomSeed: {8}", FirstPersonHeight, FirstPersonForwardOffset, SightHorizontalShift, SightVerticalShift, SightVerticalShiftRange, SightHorizontalShiftDirection, SightVerticalShiftDirection, SightRemainVerticalPeriodTime, RandomSeed);
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
            return (int)EComponentIds.PlayerFirstPersonAppearance;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as FirstPersonAppearanceComponent;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(FirstPersonHeight, rightObj.FirstPersonHeight)
                       && CompareUtility.IsApproximatelyEqual(FirstPersonForwardOffset, rightObj.FirstPersonForwardOffset)
                       && CompareUtility.IsApproximatelyEqual(SightHorizontalShift, rightObj.SightHorizontalShift)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShift, rightObj.SightVerticalShift)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShiftRange, rightObj.SightVerticalShiftRange)
                       && CompareUtility.IsApproximatelyEqual(SightHorizontalShiftDirection, rightObj.SightHorizontalShiftDirection)
                       && CompareUtility.IsApproximatelyEqual(SightVerticalShiftDirection, rightObj.SightVerticalShiftDirection)
                       && CompareUtility.IsApproximatelyEqual(SightRemainVerticalPeriodTime, rightObj.SightRemainVerticalPeriodTime)
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
            return (int)EComponentIds.PlayerFirstPersonAppearanceUpdate;
        }
    }
        
        
        
    [Player]
    
    public class ThirdPersonAppearanceComponent : IPlaybackComponent
    {
        #region Properties

        // never use these directly
        [DontInitilize] [NetworkProperty] public int PostureValue;
        [DontInitilize] [NetworkProperty] public int ActionValue;
        [DontInitilize] [NetworkProperty] public float PeekDegree;
        [DontInitilize] [NetworkProperty] public bool NeedUpdateController;
        [DontInitilize] [NetworkProperty] public float CharacterHeight;
        [DontInitilize] [NetworkProperty] public Vector3 CharacterCenter;
        [DontInitilize] [NetworkProperty] public float CharacterRadius;

        [DontInitilize] public ThirdPersonPosture Posture
        {
            get { return (ThirdPersonPosture)PostureValue; }
            set { PostureValue = (int)value; }
        }
        [DontInitilize] public ThirdPersonAction Action
        {
            get { return (ThirdPersonAction)ActionValue; }
            set { ActionValue = (int)value; }
        }

        #endregion

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerThirdPersonAppearance;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            ThirdPersonAppearanceComponent leftComponent = left as ThirdPersonAppearanceComponent;
            ThirdPersonAppearanceComponent rightComponent = right as ThirdPersonAppearanceComponent;
            CopyFrom(rightComponent);
            PeekDegree = Mathf.Lerp(leftComponent.PeekDegree, rightComponent.PeekDegree, interpolationInfo.Ratio);
            CharacterHeight = Mathf.Lerp(leftComponent.CharacterHeight, rightComponent.CharacterHeight, interpolationInfo.Ratio);
            CharacterRadius = Mathf.Lerp(leftComponent.CharacterRadius, rightComponent.CharacterRadius, interpolationInfo.Ratio);
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as ThirdPersonAppearanceComponent;
            if (right != null)
            {
                Posture = right.Posture;
                Action = right.Action;
                PeekDegree = right.PeekDegree;
                NeedUpdateController = right.NeedUpdateController;
                CharacterHeight = right.CharacterHeight;
                CharacterCenter = right.CharacterCenter;
                CharacterRadius = right.CharacterRadius;
            };
        }

        public override string ToString()
        {
            return string.Format("PostureValue: {0}, ActionValue: {1}, PeekDegree: {2}, NeedUpdateController: {3}, CharacterHeight: {4}, CharacterCenter: {5}, CharacterRadius: {6}", PostureValue, ActionValue, PeekDegree, NeedUpdateController, CharacterHeight, CharacterCenter, CharacterRadius);
        }
    }

    [Player]
    
    public class LatestAppearanceComponent : ILatestAppearanceState, ISelfLatestComponent, IPlaybackComponent
    {
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOne { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneMuzzle { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneLowRail { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneMagazine { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneButtstock { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponOneScope { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwo { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoMuzzle { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoLowRail { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoMagazine { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoButtstock { get; set; }
        [DontInitilize] [NetworkProperty] public int PrimaryWeaponTwoScope { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArm { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArmMuzzle { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArmLowRail { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArmMagazine { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArmButtstock { get; set; }
        [DontInitilize] [NetworkProperty] public int SideArmScope { get; set; }
        [DontInitilize] [NetworkProperty] public int MeleeWeapon { get; set; }
        [DontInitilize] [NetworkProperty] public int ThrownWeapon { get; set; }
        [DontInitilize] [NetworkProperty] public int TacticWeapon { get; set; }
        [DontInitilize] [NetworkProperty] public int Cap { get; set; }
        [DontInitilize] [NetworkProperty] public int PendantFace { get; set; }
        [DontInitilize] [NetworkProperty] public int Inner { get; set; }
        [DontInitilize] [NetworkProperty] public int Armor { get; set; }
        [DontInitilize] [NetworkProperty] public int Outer { get; set; }
        [DontInitilize] [NetworkProperty] public int Glove { get; set; }
        [DontInitilize] [NetworkProperty] public int Waist { get; set; }
        [DontInitilize] [NetworkProperty] public int Trouser { get; set; }
        [DontInitilize] [NetworkProperty] public int Foot { get; set; }
        [DontInitilize] [NetworkProperty] public int Bag { get; set; }
        [DontInitilize] [NetworkProperty] public int Entirety { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterHair { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterHead { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterGlove { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterInner { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterTrouser { get; set; }
        [DontInitilize] [NetworkProperty] public int CharacterFoot { get; set; }
        [DontInitilize] [NetworkProperty] public int PropId { get; set; }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerLatestAppearance;
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
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
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
            }
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    
    public class PredictedAppearanceComponent : IPredictedPlaybackAppearanceState, IUserPredictionComponent, ICompensationComponent, IPlaybackComponent
    {
        [DontInitilize] [NetworkProperty] public int WeaponInHand { get; set; }
        [DontInitilize] [NetworkProperty] public int AlternativeWeaponLocator { get; set; }
        [DontInitilize] [NetworkProperty] public int AlternativeP3WeaponLocator { get; set; }
        [DontInitilize] [NetworkProperty] public int ReloadState { get; set; }
        [DontInitilize] [NetworkProperty] public bool EnableIK { get; set; }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerPredictedAppearance;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            PredictedAppearanceComponent rightComponent = right as PredictedAppearanceComponent;
            CopyFrom(rightComponent);
        }

        public bool IsApproximatelyEqual(object right)
        {
            var rightObj = right as PredictedAppearanceComponent;
            if (rightObj != null)
            {
                return CompareUtility.IsApproximatelyEqual(WeaponInHand, rightObj.WeaponInHand) &&
                    CompareUtility.IsApproximatelyEqual(AlternativeWeaponLocator, rightObj.AlternativeWeaponLocator) &&
                    CompareUtility.IsApproximatelyEqual(AlternativeP3WeaponLocator, rightObj.AlternativeP3WeaponLocator) &&
                    CompareUtility.IsApproximatelyEqual(ReloadState, rightObj.ReloadState) &&
                    CompareUtility.IsApproximatelyEqual(EnableIK, rightObj.EnableIK);
            }
            return false;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as PredictedAppearanceComponent;
            if (right != null)
            {
                WeaponInHand = right.WeaponInHand;
                AlternativeWeaponLocator = right.AlternativeWeaponLocator;
                AlternativeP3WeaponLocator = right.AlternativeP3WeaponLocator;
                ReloadState = right.ReloadState;
                EnableIK = right.EnableIK;
            };
        }

        public override string ToString()
        {
            return string.Format("WeaponInHand: {0}, AlternativeWeaponLocator: {1}, AlternativeP3WeaponLocator: {2}, ReloadState: {3}, EnableIK: {4}", WeaponInHand, AlternativeWeaponLocator, AlternativeP3WeaponLocator, ReloadState, EnableIK);
        }

      

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
