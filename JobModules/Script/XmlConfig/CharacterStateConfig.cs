﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XmlConfig
{
    /// <summary>
    /// 对于向前是基准值，对于左右，后是系数
    /// </summary>
    public class Speed
    {
        // 前
        public float Front;
        // 后
        public float Rear;
        // 左
        public float Left;
        // 右
        public float Right;
        // 左前
        public float LeftFront;
        // 左后
        public float LeftRear;
        // 右前
        public float RightFront;
        // 右后
        public float RightRear;
    }

    public enum PostureInConfig
    {
        Null,
        Prone,
        ProneTransit,
        ProneToStand,
        ProneToCrouch,
        Crouch,
        Stand,
        Swim,
        Dive,
        DyingTransition,
        Dying,
        Sight,
        Land,
        Jump,
        Climb,
        Slide,
        Ladder,
        End
    }

    public enum LeanInConfig
    {
        Null,
        NoPeek,       
        PeekLeft,
        PeekRight,
        End
    }

    public enum MovementInConfig
    {
        Null,
        Idle,
        Walk,
        Run,
        Sprint,
        Swim,
        Dive,
        Injured,
        DiveIdle,
        DiveMove,
        EnterLadder,
        Ladder,
        End
    }

    public enum ActionInConfig
    {
        Null,
        // upperbody additive layer
        Reload,
        SpecialReload,
        Fire,
        SpecialFireHold,
        SpecialFireEnd,
        Injury,
        // over layer
        PickUp,
        SwitchWeapon,
        MeleeAttack,
        Grenade,
        // whole Over layer
        Parachuting,
        Gliding,
		OpenDoor,		 
        Props,
        BuriedBomb,
        DismantleBomb,
        End
    }

    public enum ActionKeepInConfig
    {
        Null,
        Drive,
        Sight,
        Rescue
    }

    public class SpeedCoefficient
    {
        public PostureInConfig PostureState;
        public MovementInConfig MovementState;
        public Speed Coefficient;
        public int InfluencedByWeapon;
    }

    public class PostureTransitionInfo
    {
        public PostureInConfig StateOne;
        public PostureInConfig StateTwo;
        public int Duration;
    }

    public class MovementTransitionInfo
    {
        public MovementInConfig StateOne;
        public MovementInConfig StateTwo;
        public int Duration;
    }

    public class CharacterControllerCapsule
    {
        public PostureInConfig Posture;
        public float Height;
        public float Radius;
    }

    public class InputValueLimit
    {
        public static float MaxAxisValue = 1.0f;
    }

    public class CharacterStateConfig
    {
        public SpeedCoefficient[] SpeedCoefficients;
        public PostureTransitionInfo[] PostureTransitions;
        public MovementTransitionInfo[] MovementTransitions;
        public float JumpAcceleration;
        public float DiveSpeed = 4.0f;
        public float SwimSpeed = 4.0f;
        public float StandardAnimationSpeed;

        public float SightShiftHorizontalSpeed;
        public float SightShiftHorizontalLimit;
        public float SightShiftVerticalSpeed;
        public float SightShiftVerticalLimitMax;
        public float SightShiftVerticalLimitMin;
        public float SightShiftVerticalPeriodTimeMax;
        public float SightShiftVerticalPeriodTimeMin;

        //sky move parameters
        public float SkyGravity = 7.14f;
        public float SkyYawSpeed = 4.0f;//rotate around y
        public float SkyPitchSpeed = 10.0f;//rotate around x
        public float SkyRollSpeed = 5.0f;//rotate around z
        public float SkyRollBackSpeed = 10.0f;//rotate back around z
        public float SkyAcceleration = 10.0f;

        public float MaxGlidingRollAngle = 45.0f;
        public float MaxGlidingPitchUpAngle = 5.0f;
        public float GlidingRollVelocityDamper = 0.1f;
        public float MaxGlidingGravityVelocity = 180.0f;
        public float MaxGlidingPitchUpVerticalVelocity = 56.0f;
        public float MaxGlidingPitchDownVerticalVelocity = 56.0f;
        public float GlidingAirResistance = 0.5f;
        public float GlidingAirKeyInputResistance = 0.5f;

        public float ParachuteTime = 4.0f;
        public float MinParachuteHeight = 600.0f;
        public float ParachuteIdlePitchAngle = 10.0f;
        public float MaxParachutePitchDownAngle = 45.0f;
        public float MaxParachutePitchUpAngle = 5.0f;
        public float MaxParachuteRollAngle = 45.0f;
        public float MaxParachuteGravityVelocity = 30.0f;
        public float ParachuteGravityDamper = 180.0f;
        public float ParachuteHorizontalDamper = 1.0f;
        public float MaxParachutePitchUpVerticalVelocity = 10.0f;
        public float MaxParachutePitchDownVerticalVelocity = 34.0f;
        public float ParachuteSwingAcceleration = 3.0f;
        public float ParachuteSwingDeacceleration = 1.5f;
        public float ParachuteSwingAirResistance = 0.6f;
        public float MaxParachuteSwingVelocity = 10.0f;

        public float SkyLandDeacceleration = 100.0f;

        public float[] UpSteepAngle = new float[] {
            0.0f, 15.0f, 30.0f, 45.0f
        };

        public float[] UpSteepBuff = new float[]
        {
           -0.5f, -1.0f, -1.5f, -2.0f
        };

        public float[] DownSteepAngle = new float[] {
            0.0f, -15.0f, -30.0f, -45.0f
        };

        public float[] DownSteepBuff = new float[]
        {
            0.5f, 1.0f, 1.5f, 2.0f
        };


        public float LongLayerWeightTransitionTime = 250.0f;
        public float ShortLayerWeightTransitionTime = 60.0f;
        public float ZeroLayerWeightTransitionTime = 0.0f;
        public float HolsterTransitionTime = 200f;
        public float AttackTransitionTime = 100f;

        //public float BeginSlowDownInWater = 0.3f;
        //public float SteepLimit = 45.0f;
        public float BeginSlowDownInWater = 0.6f;
        public float StopSlowDownInWater = 0.5f;
        public float SteepLimitBegin = 45.0f;
        public float SteepLimitStop = 40.0f;

        public float SteepLimitRunBegin = 40.0f;
        public float SteepLimitRunStop = 37.0f;
        public float SteepAverRatio = 0.3f;

        public float PeekXTransition = 0.3f;
        public float PeekYTransition = -0.1f;
        public float PeekDegreeP1 = 5.0f;
        public float SightPeekDegree = 30.0f;
        public float LandSlowDownTime = 200.0f;

        public float HeadFollowRotateMaxH = 80;
        public float HeadFollowRotateMinH = -80;
        public float NeckRotHorizontalIndex = 0.5f;
        public float HeadFollowRotateMaxV = 30;
        public float HeadFollowRotateMinV = -30;
        public float NeckRotVerticalIndex = 0.5f;
        public float HeadRotSpeed = 0.3f;
        public float NoHeadRotStartAngle = 135;

        public float HandFollowRotateMax = 30;
        public float HandFollowRotateMin = -30;
        
        public ItemAssetInfo JumpCurveInfo = new ItemAssetInfo
        {
            BundleName = "tables",
            AssetName = "AirMoveCurve"
        };

    }

    public static class CharacterStateConfigHelper
    {
        public static int GenerateId(PostureInConfig enumOne, MovementInConfig enumTwo)
        {
            return ((int)enumOne << 16) + (int)enumTwo;
        }

        public static int GenerateId(PostureInConfig enumOne, PostureInConfig enumTwo)
        {
            return ((int)enumOne << 16) + (int)enumTwo;
        }

        public static int GenerateId(MovementInConfig enumOne, MovementInConfig enumTwo)
        {
            return ((int)enumOne << 16) + (int)enumTwo;
        }
    }
}
