using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Player;
using Core.CharacterController;
using Core.CharacterState;
using Core.CharacterState.Movement;
using Core.CharacterState.Posture;
using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;
using XmlConfig;
using Core.Configuration;
using Core.Compare;
using Utils.Configuration;
using Utils.Singleton;


namespace App.Shared.GameModules.Player.CharacterState
{
    public class SpeedManager : ICharacterSpeed
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(SpeedManager));

        public const float Epsilon = 0.01f;
        public const float Gravity = 10.0f;
        public const float SlideSlopeGravity = 8.0f;

        private readonly PlayerEntity _player;
        private readonly ICharacterPosture _posture;
        private readonly ICharacterMovement _movement;
        private readonly ICharacterPostureInConfig _postureInConfig;
        private readonly ICharacterMovementInConfig _movementInConfig;

        private Dictionary<int, Action> _speedConditionDictionary;
        private PostureInConfig _currentPosture;
        private MovementInConfig _currentMovement;
        private Contexts _contexts;
        //至少需要把动画播放出来
        private static readonly float FullbodySpeedRatioMin = 0.01f;

        public SpeedManager(PlayerEntity player,
                            Contexts contexts,
                            ICharacterPosture posture,
                            ICharacterMovement movement,
                            ICharacterPostureInConfig postureInConfig,
                            ICharacterMovementInConfig movementInConfig)
        {
            _contexts = contexts;
            _player = player;
            _posture = posture;
            _movement = movement;
            _postureInConfig = postureInConfig;
            _movementInConfig = movementInConfig;
            _speedConditionDictionary = new Dictionary<int, Action>()
            {
                // 落地
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Land, MovementInConfig.Null),
                    () => { SetCurrentState(PostureInConfig.Null, MovementInConfig.Null); }
                },
                // 站立&静走
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Stand, MovementInConfig.Walk),
                    () => { SetCurrentState(PostureInConfig.Stand, MovementInConfig.Walk); }
                },
                // 站立&跑步
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Stand, MovementInConfig.Run),
                    () => { SetCurrentState(PostureInConfig.Stand, MovementInConfig.Run); }
                },
                // 站立&冲刺
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Stand, MovementInConfig.Sprint),
                    () => { SetCurrentState(PostureInConfig.Stand, MovementInConfig.Sprint); }
                },
                // 蹲下&静走
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Crouch, MovementInConfig.Walk),
                    () => { SetCurrentState(PostureInConfig.Crouch, MovementInConfig.Walk); }
                },
                // 蹲下&跑步
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Crouch, MovementInConfig.Run),
                    () => { SetCurrentState(PostureInConfig.Crouch, MovementInConfig.Run); }
                },
                // 蹲下&冲刺
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Crouch, MovementInConfig.Sprint),
                    () => { SetCurrentState(PostureInConfig.Crouch, MovementInConfig.Sprint); }
                },
                // 趴下&匍匐
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Prone, MovementInConfig.Null),
                    () => { SetCurrentState(PostureInConfig.Prone, MovementInConfig.Walk); }
                },
                // 游泳
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Swim, MovementInConfig.Null),
                    () => { SetCurrentState(PostureInConfig.Swim, MovementInConfig.Swim); }
                },
                // 潜水
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Dive, MovementInConfig.Null),
                    () => { SetCurrentState(PostureInConfig.Dive, MovementInConfig.Dive); }
                },
                // 受伤
                {
                    CharacterStateConfigHelper.GenerateId(PostureInConfig.Dying, MovementInConfig.Null),
                    () => { SetCurrentState(PostureInConfig.Dying, MovementInConfig.Walk); }
                }
            };
        }

        #region ICharacterSpeed

        public Vector3 GetSpeed(Vector3 lastVel, float deltaTime, float buff)
        {
            var vel = lastVel;
            vel.y = 0;
            var lastSpeed = vel.magnitude;

            if (_posture.GetNextPostureState() == PostureInConfig.Jump)
            {
                SetDefaultSpeeedRatio(buff);
                CalcMoveSpeedReduce(deltaTime, true);
            }
            else if (_posture.GetNextPostureState() == PostureInConfig.Slide)
            {
                SetDefaultSpeeedRatio(buff);
                CalcMoveSpeedReduce(deltaTime, false);
            }
            else
            {
                CalcMoveSpeedReduce(deltaTime, false);
                vel.Set(_movement.HorizontalValue, 0, _movement.VerticalValue);
                //_logger.InfoFormat("_movement data:{0}", vel.ToStringExt());
                if (CompareUtility.IsApproximatelyEqual(vel.sqrMagnitude, 0))
                {
                    vel.Set(lastVel.x, 0, lastVel.z);
                }
                vel.Normalize();
                
                var maxSpeed = GetMaxSpeed(buff);
                
                //_logger.InfoFormat("maxspeed:{0}, buff:{1}, next pos:{2}, next move:{3}", maxSpeed, buff,_postureInConfig.NextPosture(), _movementInConfig.NextMovement());
                if (CompareUtility.IsApproximatelyEqual(maxSpeed, lastSpeed))
                {
                    vel *= maxSpeed;
                }
                else
                {
                    if (_postureInConfig.InTransition() || _movementInConfig.InTransition())
                    {
                        var scale = Mathf.Abs(CalcTransitionSpeedScale(deltaTime));
                        var remainTime = Math.Max(_postureInConfig.TransitionRemainTime(), _movementInConfig.TransitionRemainTime()) * 0.001f;
                        var acceleratedSpeed = lastSpeed + (maxSpeed - lastSpeed) * deltaTime *  scale / remainTime;
                        //_logger.InfoFormat("prev speed:{0}, nextspeed:{1}, maxSpeed:{2}", lastSpeed, acceleratedSpeed, maxSpeed);
                        if (lastSpeed <= maxSpeed)
                        {
                            acceleratedSpeed = Math.Min(acceleratedSpeed, maxSpeed);
                        }
                        else
                        {
                            acceleratedSpeed = Math.Max(acceleratedSpeed, maxSpeed);
                        }
                        vel *= acceleratedSpeed;
                    }
                    else
                    {
                        vel *= maxSpeed;
                    }
                }

                CalcSpeedRatio(maxSpeed, vel.magnitude, buff);
            }

            if (_posture.IsNeedJumpSpeed())
            {
                vel.y = 3.4f;
            }
            else if (_posture.GetNextPostureState() == PostureInConfig.Swim)
            {
                vel.y = 0;
            }
            else if (_posture.GetNextPostureState() != PostureInConfig.Dive)
            {
                vel.y = lastVel.y - deltaTime * Gravity;
            }

            return vel;
        }

        private float CalcTransitionSpeedScale(float deltaTime)
        {
            var ret = 1.0f;
            if (_movementInConfig.InTransition())
            {
                var normalizeTime = 1 - _movementInConfig.TransitionRemainTime() / _movementInConfig.TransitionTime();
                var target = Mathf.Clamp01(normalizeTime + deltaTime * 1000f / _movementInConfig.TransitionTime());
                ret = SingletonManager.Get<CharacterStateConfigManager>()
                    .GetMovementTransitionSpeedScale(_movementInConfig.CurrentMovement(),
                        _movement.GetNextMovementState(),normalizeTime, target);
                //_logger.InfoFormat("current:{0}, next:{1}, normalize time:{2},target:{3} ret:{4}",_movementInConfig.CurrentMovement(),
                //    _movement.GetNextMovementState(), 1 - _movementInConfig.TransitionRemainTime()/_movementInConfig.TransitionTime(),target,ret);
            }
            return ret;
        }

        private void CalcMoveSpeedReduce(float deltaTime, bool isAir)
        {
            if (isAir)
            {
                _player.playerMove.AirTime += deltaTime;
                _player.playerMove.MoveSpeedRatio = GetAirSpeedRatio(_player.playerMove.AirTime);
            }
            else
            {
                _player.playerMove.AirTime = 0f;
                _player.playerMove.MoveSpeedRatio = 1.0f;
            }
        }

        private float GetAirSpeedRatio(float time)
        {
            //1-e^(-(1-x)*30)
            //return Mathf.Clamp01(1.0f - Mathf.Exp(-(1.5f - time) * 3.2f));
            //            return Mathf.Clamp01(Mathf.Cos(time));

            AnimationCurve com = SingletonManager.Get<CharacterStateConfigManager>().AirMoveCurve;

            if (com != null)
            {
                //_logger.InfoFormat("jump date:{0}", Mathf.Clamp(com.AireMoveCurve.Evaluate(time), 0f, float.MaxValue));
                return Mathf.Clamp(com.Evaluate(time), 0f, float.MaxValue);
            }
            else
            {
                return 1.0f;
            }

        }

        private void SetDefaultSpeeedRatio(float buff)
        {
            float weaponSpeed = _player.WeaponController().HeldWeaponAgent.BaseSpeed ;
            var newSpeedRatio =  weaponSpeed * (1.0f + buff) / SingletonManager.Get<CharacterStateConfigManager>().GetStandardAnimationSpeed();
            _player.playerMove.SpeedRatio = newSpeedRatio;
        }

        public Vector3 GetSpeedOffset(float buff)
        {
            Vector3 ret = Vector3.zero;
            if (_posture.GetNextPostureState() == PostureInConfig.Jump)
            {
                if (Mathf.Abs(_movement.HorizontalValue) > Epsilon)
                {
                    ret.x = (_movement.HorizontalValue > 0 ? 1 : -1) * SingletonManager.Get<CharacterStateConfigManager>().GetJumpAcceleration();
                }

                if (_movement.VerticalValue > Epsilon)
                {
                    ret.z = _movement.VerticalValue * SingletonManager.Get<CharacterStateConfigManager>().GetJumpAcceleration();
                }
                
            }
            else if (_posture.GetNextPostureState() == PostureInConfig.Dive)
            {
                if (_movement.IsUp)
                {
                    ret.y = SingletonManager.Get<CharacterStateConfigManager>().GetDiveSpeed() * _movement.UpDownValue;
                }
                else if (_movement.IsDown)
                {
                    ret.y = SingletonManager.Get<CharacterStateConfigManager>().GetDiveSpeed() * _movement.UpDownValue;
                }
            }
            else if (_posture.GetNextPostureState() == PostureInConfig.Swim)
            {
                if (_movement.IsDown)
                {
                    ret.y = -SingletonManager.Get<CharacterStateConfigManager>().GetSwimSpeed();
                }
            }

            return ret;
        }

        public void SetSpeedAffect(float affect)
        {
            _player.playerMove.SpeedAffect = affect;
        }

        public float SpeedRatio()
        {
            return _player.playerMove.SpeedRatio;
        }

        #endregion

        private float GetMaxSpeed(float buff)
        {
            float maxSpeed = 0;
            var totalBuff = 1 + buff;
            bool valid = false;
            var candidateId1 = CharacterStateConfigHelper.GenerateId(_postureInConfig.InTransition() ? _postureInConfig.NextPosture() : _postureInConfig.CurrentPosture(),
                 _movementInConfig.InTransition() ? _movementInConfig.NextMovement() : _movementInConfig.CurrentMovement());
            var candidateId2 = CharacterStateConfigHelper.GenerateId(_postureInConfig.NextPosture(), MovementInConfig.Null);
            var candidateId3 = CharacterStateConfigHelper.GenerateId(_postureInConfig.CurrentPosture(), MovementInConfig.Null);
            //_logger.InfoFormat("posture in transition:{0}, next posture:{1}, cur posture:{2}, move in transition:{3}, next move:{4}, CURRENT MOVE:{5}",_postureInConfig.InTransition() , _postureInConfig.NextPosture() , _postureInConfig.CurrentPosture(),
             //   _movementInConfig.InTransition() , _movementInConfig.NextMovement() , _movementInConfig.CurrentMovement());
            if (_speedConditionDictionary.ContainsKey(candidateId3))
            {
                valid = true;
                _speedConditionDictionary[candidateId3].Invoke();
            }
            else if (_speedConditionDictionary.ContainsKey(candidateId2))
            {
                valid = true;
                _speedConditionDictionary[candidateId2].Invoke();
            }
            else if (_speedConditionDictionary.ContainsKey(candidateId1))
            {
                valid = true;
                _speedConditionDictionary[candidateId1].Invoke();
            }


            if (valid)
            {
                float weaponSpeed = GetWeaponSpeed(_currentPosture, _currentMovement, _player.WeaponController().HeldWeaponAgent.BaseSpeed,
                    _player.WeaponController().HeldWeaponAgent.DefaultSpeed);
                maxSpeed = SingletonManager.Get<CharacterStateConfigManager>().GetSpeed(_currentPosture,
                    _currentMovement,
                    _movement.IsForth,
                    _movement.IsBack,
                    _movement.IsLeft,
                    _movement.IsRight,
                    weaponSpeed);
                //_logger.InfoFormat("_currentPosture:{0}, _currentMovement:{1},_movement hor:{2}, _movemnt vert:{3}", _currentPosture, _currentMovement, _movement.HorizontalValue, _movement.VerticalValue);
            }
            //_logger.InfoFormat("valid:{4}, _currentPosture:{0}, _currentMovement:{1},_movement hor:{2}, _movemnt vert:{3}", _currentPosture, _currentMovement, _movement.HorizontalValue, _movement.VerticalValue, valid);
            return maxSpeed * totalBuff;
        }

        private float GetWeaponSpeed(PostureInConfig posture, MovementInConfig movement, float baseSpeed,
            float defaultSpeed)
        {
            var ret = defaultSpeed;
            var isInfluencedByWeapon = SingletonManager.Get<CharacterStateConfigManager>().IsInfluencedByWeapon(posture, movement);
            ret = isInfluencedByWeapon ? baseSpeed : defaultSpeed;
            return ret;
        }

        private void SetCurrentState(PostureInConfig posture, MovementInConfig movement)
        {
            _currentPosture = posture;
            _currentMovement = movement;
        }

        private void CalcSpeedRatio(float maxSpeed, float curSpeed, float buff)
        {
            
            var scale = maxSpeed > curSpeed ? curSpeed / maxSpeed:1.0f;
            float weaponSpeed = GetWeaponSpeed(_currentPosture, _currentMovement, _player.WeaponController().HeldWeaponAgent.BaseSpeed,
                    _player.WeaponController().HeldWeaponAgent.DefaultSpeed);
            var newSpeedRatio = scale * weaponSpeed * (1.0f + buff) / SingletonManager.Get<CharacterStateConfigManager>().GetStandardAnimationSpeed();
            _player.playerMove.SpeedRatio = newSpeedRatio < FullbodySpeedRatioMin? FullbodySpeedRatioMin: newSpeedRatio;
            if (float.IsNaN(newSpeedRatio) || float.IsInfinity(newSpeedRatio))
            {
                _logger.ErrorFormat("curspeed:{0}, maxSpeed:{1}, weaponSpeed:{2}, standAnimationSpeed:{3}, buff:{4}, scale:{5}",
                    curSpeed,maxSpeed,weaponSpeed,SingletonManager.Get<CharacterStateConfigManager>().GetStandardAnimationSpeed(),buff,
                    scale
                    );
            }
        }
    }
}
