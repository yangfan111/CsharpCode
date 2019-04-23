using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;
using Core.Utils;
using UnityEngine;
using Utils.Utils;
using XmlConfig;
using Object = UnityEngine.Object;

namespace Utils.Configuration
{
    public class CharacterSkyMoveConfig
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CharacterSkyMoveConfig));

        public float SkyGravity;
        public float SkyYawSpeed;//rotate around y
        public float SkyPitchSpeed;//rotate around x
        public float SkyRollSpeed;//rotate around z
        public float SkyRollBackSpeed; //rotate back around z
        public float SkyAcceleration;

        public float MaxGlidingRollAngle;
        public float MaxGlidingPitchUpAngle;
        public float GlidinRollVelocityDamper;
        public float MaxGlidingGravityVelocity;
        public float MaxGlidingPitchUpVerticalVelocity;
        public float MaxGlidingPitchDownVerticalVelocity;
        public float GlidingAirResistance;
        public float GlidingAirKeyInputResistance;

        public float ParachuteTime;
        public float MinParachuteHeight;
        public float ParachuteIdlePitchAngle;
        public float MaxParachutePitchDownAngle;
        public float MaxParachutePitchUpAngle;
        public float MaxParachuteRollAngle;
        public float MaxParachuteGravityVelocity;
        public float ParachuteGravityDamper;
        public float ParachuteHorizontalDamper;
        public float MaxParachutePitchUpVerticalVelocity;
        public float MaxParachutePitchDownVerticalVelocity;
        public float ParachuteSwingAcceleration;
        public float ParachuteSwingDeacceleration;
        public float ParachuteSwingAirResistance;
        public float MaxParachuteSwingVelocity;

        public float SkyLandDeacceleration;

        public void ValidateConfiguartion()
        {
            ValidateNonNegative(GetFieldName(() => SkyGravity));
            ValidateNonNegative(GetFieldName(() => SkyYawSpeed));
            ValidateNonNegative(GetFieldName(() => SkyPitchSpeed));
            ValidateNonNegative(GetFieldName(() => SkyRollSpeed));
            ValidateNonNegative(GetFieldName(() => SkyRollBackSpeed));
            ValidateNonNegative(GetFieldName(() => SkyAcceleration));

            ValidateNonNegative(GetFieldName(() => MaxGlidingRollAngle));
            ValidateNonNegative(GetFieldName(() => MaxGlidingPitchUpAngle));
            ValidateNonNegative(GetFieldName(() => GlidinRollVelocityDamper));
            ValidateNonNegative(GetFieldName(() => MaxGlidingGravityVelocity));
            ValidateNonNegative(GetFieldName(() => MaxGlidingPitchUpVerticalVelocity));
            ValidateNonNegative(GetFieldName(() => MaxGlidingPitchDownVerticalVelocity));
            ValidateNonNegative(GetFieldName(() => GlidingAirResistance));
            ValidateNonNegative(GetFieldName(() => GlidingAirKeyInputResistance));

            ValidateNonNegative(GetFieldName(() => ParachuteTime));
            ValidateNonNegative(GetFieldName(() => MinParachuteHeight));
            ValidateNonNegative(GetFieldName(() => ParachuteIdlePitchAngle));
            ValidateNonNegative(GetFieldName(() => MaxParachutePitchDownAngle));
            ValidateNonNegative(GetFieldName(() => MaxParachutePitchUpAngle));
            ValidateNonNegative(GetFieldName(() => MaxParachuteRollAngle));
            ValidateNonNegative(GetFieldName(() => MaxParachuteGravityVelocity));
            ValidateNonNegative(GetFieldName(() => ParachuteGravityDamper));
            ValidateNonNegative(GetFieldName(() => ParachuteHorizontalDamper));
            ValidateNonNegative(GetFieldName(() => MaxParachutePitchUpVerticalVelocity));
            ValidateNonNegative(GetFieldName(() => MaxParachutePitchDownVerticalVelocity));
            ValidateNonNegative(GetFieldName(() => ParachuteSwingAcceleration));
            ValidateNonNegative(GetFieldName(() => ParachuteSwingDeacceleration));
            ValidateNonNegative(GetFieldName(() => ParachuteSwingAirResistance));
            ValidateNonNegative(GetFieldName(() => MaxParachuteSwingVelocity));
            
            ValidateNonNegative(GetFieldName(() => SkyLandDeacceleration));
        }

        private static string GetFieldName<T>(System.Linq.Expressions.Expression<Func<T>> exp)
        {
            return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
        }
             

        private void ValidateNonNegative(string fieldName)
        {
            Validate<float>(fieldName, v => v < 0 ? -v : v, (name, v) =>
            {
                _logger.WarnFormat("field {0} value {1} is configurated as a negative value, must be non-negative!",
                    name, v);
            });
        }

        private void Validate<T>(string fieldName, Func<T, T> condition, Action<string, T> warnFunc)
        {
            try
            {
                T val = (T)GetType().GetField(fieldName).GetValue(this);
                var validatedVal = condition(val);
                if (!validatedVal.Equals(val))
                {
                    GetType().GetField(fieldName).SetValue(this, validatedVal); 
                    warnFunc(fieldName, val);
                }

            }
            catch
            {
                _logger.ErrorFormat("Can not validate field {0}", fieldName);
            }
        }
    }

    public class SteepConfig
    {
        public float[] UpSteepAngles;
        public float[] UpSteepBuffs;
        public float[] DownSteepAngles;
        public float[] DownSteepBuffs;
        const float SteepBuffMax = 0.99f;

        public float CalcSteepBuff(float steep)
        {
            float ret = 0.0f;
            if (steep > 0.0f)
            {
                for (int i = UpSteepAngles.Length - 1; i >= 0; --i)
                {
                    if (steep > UpSteepAngles[i])
                    {
                        ret = (steep - UpSteepAngles[i]) * UpSteepBuffs[i];
                        steep = steep - UpSteepAngles[i];
                    }
                }
            }

            ret = Mathf.Clamp(ret, -SteepBuffMax, SteepBuffMax);
            return ret;
        }
    }

    public class CharacterStateConfigManager : AbstractConfigManager<CharacterStateConfigManager>
    {
       
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CharacterStateConfigManager));

        struct SpeedCoefficient
        {
            public float[,] CoefficientByDirection;

            public SpeedCoefficient(float front, float rear, float left, float right,
                float leftFront, float leftRear, float rightFront, float rightRear)
            {
                CoefficientByDirection = new float[3, 3]
                {
                    { leftFront, front, rightFront },
                    { left, 0, right },
                    { leftRear, rear, rightRear }
                };
            }
        }

        enum Vertical
        {
            Front = 0,
            Neutral,
            Back
        }

        enum Horizontal
        {
            Left = 0,
            Neutral,
            Right
        }

        private Dictionary<int, SpeedCoefficient> _coefficients = new Dictionary<int, SpeedCoefficient>();
        private Dictionary<int, int> _influencedByWeapon = new Dictionary<int, int>();
        private Dictionary<int, int> _postureTransitionTime = new Dictionary<int, int>();
        private Dictionary<int, int> _movementTransitionTime = new Dictionary<int, int>();
        private Dictionary<int, AnimationCurve> _movementTransitionSpeedScale = new Dictionary<int, AnimationCurve>();
        private Dictionary<int, AnimationCurve> _postureTransitionSpeedScale = new Dictionary<int, AnimationCurve>();
        
        private AnimationCurve _airMoveCurve = new AnimationCurve(new Keyframe[3]
        {
            new Keyframe(0f, 1f),
            new Keyframe(1.0f, 0.95f),
            new Keyframe(1.5f, 0.5f)
        });
        
        private List<MovementCurveInfo> _movementCurve = new List<MovementCurveInfo>();
        private List<PostureCurveInfo> _postureCurve = new List<PostureCurveInfo>();
        
        private static readonly CharacterControllerCapsule DefaultCharacterControllerCapsule =
            new CharacterControllerCapsule
            {
                Posture = PostureInConfig.Stand,
                Height = 1.75f,
                Radius = 0.4f,
            };
        private float _jumpAcceleration;
        private float _diveSpeed;
        private float _swimSpeed;
        private float _standardAnimationSpeed;
        private float _horizontalSpeed;
        private float _horizontalLimit;
        private float _verticalSpeed;
        private float _verticalLimitMax;
        private float _verticalLimitMin;
        private float _verticalPeriodTimeMax;
        private float _verticalPeriodTimeMin;
        private CharacterSkyMoveConfig _skyMoveConfig;
        private SteepConfig _steepConfig;

        private float _meleeAttackWeightTransitionTime = 300.0f;
        private float _longLayerWeightTransitionTime = 200.0f;
        private float _shortLayerWeightTransitionTime = 100.0f;
        private float _zeroLayerWeightTransitionTime = 0.0f;
        private float _holsterTransitionTime = 200f;
        private float _attackTransitionTime = 100f;

        private float _beginSlowDownInWater;
        private float _stopSlowDownInWater;
        private float _steepLimitSprintBegin = 30;
        private float _steepLimitSprintStop = 27;
        private float _steepLimitRunBegin = 40;
        private float _steepLimitRunStop = 37;
        private float _steepAverRatio;

        private float _headFollowRotateMax;
        private float _headFollowRotateMin;
        private float _heckRotHorizontalIndex;
        private float _verticalHeadRotMax;
        private float _verticalHeadRotMin;
        private float _neckRotVerticalIndex;
        private float _headRotReversalTime;
        private float _handRotMax;
        private float _handRotMin;

        //private float _maxSlopeNum;
        
        public float MeleeLayerWeightTransitionTime
        {
            get { return _meleeAttackWeightTransitionTime; }
        }

        public float LongLayerWeightTransitionTime
        {
            get { return _longLayerWeightTransitionTime; }
        }


        public float ShortLayerWeightTransitionTime
        {
            get { return _shortLayerWeightTransitionTime; }
        }

        public float ZeroLayerWeightTransitionTime
        {
            get { return _zeroLayerWeightTransitionTime; }
        }

        public float PeekXTransition
        {
            get;
            private set;
        }

        public float PeekYTransition
        {
            get;
            private set;
        }

        public float PeekDegreeP1
        {
            get;
            private set;
        }

        public float SightPeekDegree
        {
            get;
            private set;
        }

        public float HorizontalHeadRotMax
        {
            get { return _headFollowRotateMax; }
        }

        public float HorizontalHeadRotMin
        {
            get { return _headFollowRotateMin; }
        }

        public float NeckRotHorizontalIndex
        {
            get { return _heckRotHorizontalIndex; }
        }

        public float VerticalHeadRotMax
        {
            get { return _verticalHeadRotMax; }
        }

        public float VerticalHeadRotMin
        {
            get { return _verticalHeadRotMin; }
        }

        public float NeckRotVerticalIndex
        {
            get { return _neckRotVerticalIndex; }
        }

        public float HeadRotReversalTime
        {
            get { return _headRotReversalTime; }
        }

        public float HandRotMax
        {
            get { return _handRotMax; }
        }

        public float HandRotMin
        {
            get { return _handRotMin; }
        }
        public float BeginSlowDownInWater
        {
            get { return _beginSlowDownInWater; }
        }

        public float StopSlowDownInWater             //持续冲刺指令，到该水面高度下恢复冲刺动作
        {
            get { return _stopSlowDownInWater; }
        }

        public float SteepLimitSprintBegin                 //开始禁止冲刺动作的坡度
        {
            get { return _steepLimitSprintBegin; }
        }

        public float SteepLimitSprintStop                   //持续冲刺指令，到该坡度下恢复冲刺动作
        {
            get { return _steepLimitSprintStop; }
        }

        public float SteepAverRatio                   //当前坡度在平均值计算时所占权重
        {
            get { return _steepAverRatio; }
        }

//        public float MaxSlopeNum
//        {
//            get { return _maxSlopeNum; }
//        }

        private const int DefaultTransitionTime = 100;

        public CharacterStateConfigManager() { }

        public float GetSpeed(PostureInConfig posture, MovementInConfig movement, bool isFront, bool isBack, bool isLeft, bool isRight, float standardSpeed)
        {
            float ret = 0;

            int id = CharacterStateConfigHelper.GenerateId(posture, movement);
            if (_coefficients.ContainsKey(id))
            {
                var v = isFront ? Vertical.Front : isBack ? Vertical.Back : Vertical.Neutral;
                var h = isLeft ? Horizontal.Left : isRight ? Horizontal.Right : Horizontal.Neutral;
                ret = _coefficients[id].CoefficientByDirection[(int)v, (int)h] * standardSpeed;
            }

            return ret;
        }

        /// <summary>
        /// 默认不受武器影响
        /// </summary>
        /// <param name="posture"></param>
        /// <param name="movement"></param>
        /// <returns></returns>
        public bool IsInfluencedByWeapon(PostureInConfig posture, MovementInConfig movement)
        {
            bool ret = false;
            int id = CharacterStateConfigHelper.GenerateId(posture, movement);
            if (_influencedByWeapon.ContainsKey(id))
            {
                ret = _influencedByWeapon[id] > 0;
            }

            return ret;
        }

        public int GetPostureTransitionTime(PostureInConfig stateOne, PostureInConfig stateTwo)
        {
            int id = CharacterStateConfigHelper.GenerateId(stateOne, stateTwo);
            if (_postureTransitionTime.ContainsKey(id))
            {
                return _postureTransitionTime[id];
            }
            else
            {
                Logger.WarnFormat("duration not defined for {0} to {1}", stateOne, stateTwo);
                return DefaultTransitionTime;
            }
        }

        public int GetMovementTransitionTime(MovementInConfig stateOne, MovementInConfig stateTwo)
        {
            int id = CharacterStateConfigHelper.GenerateId(stateOne, stateTwo);
            if (_movementTransitionTime.ContainsKey(id))
            {
                return _movementTransitionTime[id];
            }
            else
            {
                Logger.WarnFormat("duration not defined for {0} to {1}", stateOne, stateTwo);
                return DefaultTransitionTime;
            }
        }

        public float GetMovementTransitionSpeedScale(MovementInConfig stateOne, MovementInConfig stateTwo,
            float normalizeTime, float targetTime)
        {
            int id = CharacterStateConfigHelper.GenerateId(stateOne, stateTwo);
            if (_movementTransitionSpeedScale.ContainsKey(id))
            {
                return (_movementTransitionSpeedScale[id].Evaluate(targetTime) - _movementTransitionSpeedScale[id].Evaluate(normalizeTime)) / (targetTime - normalizeTime);
            }
            else
            {
                return 1.0f;
            }
        }

        public float GetPostureTransitionSpeedScale(PostureInConfig stateOne, PostureInConfig stateTwo,
            float normalizeTime)
        {
            int id = CharacterStateConfigHelper.GenerateId(stateOne, stateTwo);
            if (_postureTransitionSpeedScale.ContainsKey(id))
            {
                return _postureTransitionSpeedScale[id].Evaluate(normalizeTime);
            }

            return 0.0f;
        }

        public float GetJumpAcceleration()
        {
            return _jumpAcceleration;
        }

        public float GetDiveSpeed()
        {
            return _diveSpeed;
        }

        public float GetSwimSpeed()
        {
            return _swimSpeed;
        }

        public float GetStandardAnimationSpeed()
        {
            return _standardAnimationSpeed;
        }

        public float SightShiftHorizontalSpeed { get { return _horizontalSpeed; } }
        public float SightShiftHorizontalLimit { get { return _horizontalLimit; } }
        public float SightShiftVerticalSpeed { get { return _verticalSpeed; } }
        public float SightShiftVerticalLimitMax { get { return _verticalLimitMax; } }
        public float SightShiftVerticalLimitMin { get { return _verticalLimitMin; } }
        public float SightShiftVerticalPeriodTimeMax { get { return _verticalPeriodTimeMax; } }
        public float SightShiftVerticalPeriodTimeMin { get { return _verticalPeriodTimeMin; } }
        public CharacterSkyMoveConfig SkyMoveConfig { get { return _skyMoveConfig; } }
        public SteepConfig SteepConfig { get { return _steepConfig; } }
        private float _landSlowDownTime = 200.0f;

        public float LandSlowDownTime
        {
            get
            {
                return _landSlowDownTime;
                
            }
        }

        public AssetInfo AirMoveCurveAssetInfo;

        public AnimationCurve AirMoveCurve
        {
            get { return _airMoveCurve; }
            set { _airMoveCurve = value; }
        }
        

        public float HolsterTransitionTime
        {
            get { return _holsterTransitionTime; }
        }

        public List<MovementCurveInfo> MovementCurve
        {
            get { return _movementCurve; }
            set
            {
                _movementCurve = value;
                InitMovementSpeedScale();
            }
        }
        
        public List<PostureCurveInfo> PostureCurve
        {
            get { return _postureCurve; }
            set
            {
                _postureCurve = value;
                InitPostureSpeedScale();
            }
        }

        public float AttackTransitionTime
        {
            get { return _attackTransitionTime; }
        }

        public float SteepLimitRunBegin
        {
            get { return _steepLimitRunBegin; }
        }

        public float SteepLimitRunStop
        {
            get { return _steepLimitRunStop; }
        }


        private void ClearData()
        {
            _coefficients.Clear();
            _influencedByWeapon.Clear();
            _postureTransitionTime.Clear();
            _movementTransitionTime.Clear();
            _movementTransitionSpeedScale.Clear();
            _postureTransitionSpeedScale.Clear();
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.ErrorFormat("SpeedConfig is Empty");
                return;
            }
            ClearData();
            var cfg = XmlConfigParser<CharacterStateConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("character speed is illegal content:{0}", xml);
                return;
            }

            foreach (var v in cfg.SpeedCoefficients)
            {
                int id = CharacterStateConfigHelper.GenerateId(v.PostureState, v.MovementState);
                _coefficients.Add(id, new SpeedCoefficient
                (
                    v.Coefficient.Front, v.Coefficient.Rear, v.Coefficient.Left, v.Coefficient.Right,
                    v.Coefficient.LeftFront, v.Coefficient.LeftRear, v.Coefficient.RightFront, v.Coefficient.RightRear
                ));
                _influencedByWeapon.Add(id, v.InfluencedByWeapon);
            }
            foreach (var v in cfg.PostureTransitions)
            {
                int id = CharacterStateConfigHelper.GenerateId(v.StateOne, v.StateTwo);
                _postureTransitionTime.Add(id, v.Duration);
            }
            foreach (var v in cfg.MovementTransitions)
            {
                int id = CharacterStateConfigHelper.GenerateId(v.StateOne, v.StateTwo);
                _movementTransitionTime.Add(id, v.Duration);
            }

            _jumpAcceleration = cfg.JumpAcceleration;
            _diveSpeed = cfg.DiveSpeed;
            _swimSpeed = cfg.SwimSpeed;
            _standardAnimationSpeed = cfg.StandardAnimationSpeed;
            _horizontalSpeed = cfg.SightShiftHorizontalSpeed;
            _horizontalLimit = cfg.SightShiftHorizontalLimit;
            _verticalSpeed = cfg.SightShiftVerticalSpeed;
            _verticalLimitMax = cfg.SightShiftVerticalLimitMax;
            _verticalLimitMin = cfg.SightShiftVerticalLimitMin;
            _verticalPeriodTimeMax = cfg.SightShiftVerticalPeriodTimeMax;
            _verticalPeriodTimeMin = cfg.SightShiftVerticalPeriodTimeMin;

            AirMoveCurveAssetInfo = new AssetInfo(cfg.JumpCurveInfo.BundleName, cfg.JumpCurveInfo.AssetName);

            _skyMoveConfig = new CharacterSkyMoveConfig()
            {
                SkyGravity = cfg.SkyGravity,
                SkyYawSpeed = cfg.SkyYawSpeed,
                SkyPitchSpeed = cfg.SkyPitchSpeed,
                SkyRollSpeed = cfg.SkyRollSpeed,
                SkyRollBackSpeed = cfg.SkyRollBackSpeed,
                SkyAcceleration = cfg.SkyAcceleration,

                MaxGlidingRollAngle = cfg.MaxGlidingRollAngle,
                MaxGlidingPitchUpAngle = cfg.MaxGlidingPitchUpAngle,
                GlidinRollVelocityDamper = cfg.GlidingRollVelocityDamper,
                MaxGlidingGravityVelocity = cfg.MaxGlidingGravityVelocity,
                MaxGlidingPitchUpVerticalVelocity = cfg.MaxGlidingPitchUpVerticalVelocity,
                MaxGlidingPitchDownVerticalVelocity = cfg.MaxGlidingPitchDownVerticalVelocity,
                GlidingAirResistance = cfg.GlidingAirResistance,
                GlidingAirKeyInputResistance =  cfg.GlidingAirKeyInputResistance,

                ParachuteTime =  cfg.ParachuteTime,
                MinParachuteHeight =  cfg.MinParachuteHeight,
                ParachuteIdlePitchAngle = cfg.ParachuteIdlePitchAngle,
                MaxParachutePitchDownAngle = cfg.MaxParachutePitchDownAngle,
                MaxParachutePitchUpAngle = cfg.MaxParachutePitchUpAngle,
                MaxParachuteRollAngle = cfg.MaxParachuteRollAngle,
                MaxParachuteGravityVelocity = cfg.MaxParachuteGravityVelocity,
                ParachuteGravityDamper = cfg.ParachuteGravityDamper,
                ParachuteHorizontalDamper = cfg.ParachuteHorizontalDamper,
                MaxParachutePitchUpVerticalVelocity = cfg.MaxParachutePitchUpVerticalVelocity,
                MaxParachutePitchDownVerticalVelocity = cfg.MaxParachutePitchDownVerticalVelocity,
                ParachuteSwingAcceleration = cfg.ParachuteSwingAcceleration,
                ParachuteSwingDeacceleration = cfg.ParachuteSwingDeacceleration,
                ParachuteSwingAirResistance =  cfg.ParachuteSwingAirResistance,
                MaxParachuteSwingVelocity = cfg.MaxParachuteSwingVelocity,

                SkyLandDeacceleration = cfg.SkyLandDeacceleration
            };
            _skyMoveConfig.ValidateConfiguartion();

            _steepConfig = new SteepConfig()
            {
                DownSteepAngles = cfg.DownSteepAngle,
                DownSteepBuffs = cfg.DownSteepBuff,
                UpSteepAngles = cfg.UpSteepAngle,
                UpSteepBuffs = cfg.UpSteepBuff,
            };

            _longLayerWeightTransitionTime = cfg.LongLayerWeightTransitionTime;
            _holsterTransitionTime = cfg.HolsterTransitionTime;
            _attackTransitionTime = cfg.AttackTransitionTime;
            _shortLayerWeightTransitionTime = cfg.ShortLayerWeightTransitionTime;
            _zeroLayerWeightTransitionTime = cfg.ZeroLayerWeightTransitionTime;

            _beginSlowDownInWater = cfg.BeginSlowDownInWater;
            _stopSlowDownInWater = cfg.StopSlowDownInWater;

            _steepAverRatio = cfg.SteepAverRatio;
            _steepLimitSprintBegin = cfg.SteepLimitBegin;
            _steepLimitSprintStop = cfg.SteepLimitStop;
            _steepLimitRunBegin = cfg.SteepLimitRunBegin;
            _steepLimitRunStop = cfg.SteepLimitRunStop;
            //_maxSlopeNum = cfg.MaxSlopeNum;

            _headFollowRotateMax = cfg.HeadFollowRotateMaxH;
            _headFollowRotateMin = cfg.HeadFollowRotateMinH;
            _heckRotHorizontalIndex = cfg.NeckRotHorizontalIndex;
            _verticalHeadRotMax = cfg.HeadFollowRotateMaxV;
            _verticalHeadRotMin = cfg.HeadFollowRotateMinV;
            _neckRotVerticalIndex = cfg.NeckRotVerticalIndex;
            _headRotReversalTime = cfg.HeadRotReversalTime;

            _handRotMax = cfg.HandFollowRotateMax;
            _handRotMin = cfg.HandFollowRotateMin;

            PeekXTransition = cfg.PeekXTransition;
            PeekYTransition = cfg.PeekYTransition;
            PeekDegreeP1 = cfg.PeekDegreeP1;
            SightPeekDegree = cfg.SightPeekDegree;
            _landSlowDownTime = cfg.LandSlowDownTime;
        }

        private void InitMovementSpeedScale()
        {
            _movementTransitionSpeedScale.Clear();
            foreach (var info in _movementCurve)
            {
                _movementTransitionSpeedScale.Add(CharacterStateConfigHelper.GenerateId(info.StateOne, info.StateTwo), info.ScaleCurve);
            }
        }
        
        private void InitPostureSpeedScale()
        {
            _postureTransitionSpeedScale.Clear();
            foreach (var info in _postureCurve)
            {
                _postureTransitionSpeedScale.Add(CharacterStateConfigHelper.GenerateId(info.StateOne, info.StateTwo), info.ScaleCurve);
            }
        }
        
        
    }
}
