using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;
using Utils.Compare;
using XmlConfig;

namespace Core.CharacterState.Movement
{
    class MovementManager : FsmManager, ICharacterMovement, IFsmUpdate, ICharacterMovementInConfig
    {
        private readonly MovementFsm _commonFsm;
        private float _forthValue;
        private float _backValue;
        private float _leftValue;
        private float _rightValue;
        private float _upValue;
        private float _downValue;

        private FsmUpdateType _directionUpdateType = FsmUpdateType.ResponseToInput;
        private FsmUpdateType _movementUpdateType = FsmUpdateType.ResponseToInput | FsmUpdateType.ResponseToAnimation;

        public MovementManager(IFsmTransitionHelper infoProvider)
        {
            _commonFsm = new MovementFsm("Movement");
            _commonFsm.Init(infoProvider);
            AddFsm(_commonFsm);
        }

        public void Reset(Action<FsmOutput> addOutput)
        {
            _commonFsm.Reset(addOutput);
        }

        public void SetName(string name)
        {
            _commonFsm.Name = name;
        }

        #region IFsmUpdate

        public void Update(IAdaptiveContainer<IFsmInputCommand> commands,
                           int frameInterval,
                           Action<FsmOutput> addOutput,
                           FsmUpdateType updateType)
        {
            if ((updateType & _directionUpdateType) != 0)
            {
                UpdateDirection(commands, addOutput);
            }
            
            if ((updateType & _movementUpdateType) != 0)
            {
                _commonFsm.Update(commands, frameInterval, addOutput);
            }
        }

        private void UpdateDirection(IAdaptiveContainer<IFsmInputCommand> commands, Action<FsmOutput> addOutput)
        {
            _forthValue = 0f;
            _backValue = 0f;
            _leftValue = 0f;
            _rightValue = 0f;
            _upValue = 0f;
            _downValue = 0f;
            int lenght = commands.Length;
            IFsmInputCommand extraCommand = null;
            for (int i = 0; i < lenght; ++i)
            {
                var command = commands[i];
                if (command.IsMatch(FsmInput.Forth))
                {
                    _forthValue = command.AdditioanlValue;

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VerticalHash,
                                             AnimatorParametersHash.Instance.VerticalName,
                                             command.AdditioanlValue,
                                             CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                }
                else if (command.IsMatch(FsmInput.Back))
                {
                    _backValue = command.AdditioanlValue;

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VerticalHash,
                                             AnimatorParametersHash.Instance.VerticalName,
                                             command.AdditioanlValue,
                                             CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                }
                else if (command.IsMatch(FsmInput.Left))
                {
                    _leftValue = command.AdditioanlValue;

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HorizontalHash,
                                             AnimatorParametersHash.Instance.HorizontalName,
                                             command.AdditioanlValue,
                                             CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                }
                else if (command.IsMatch(FsmInput.Right))
                {
                    _rightValue = command.AdditioanlValue;

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HorizontalHash,
                                             AnimatorParametersHash.Instance.HorizontalName,
                                             command.AdditioanlValue,
                                             CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                }
                else if (command.IsMatch(FsmInput.Up))
                {
                    _upValue = command.AdditioanlValue;
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpDownHash,
                        AnimatorParametersHash.Instance.UpDownName,
                        command.AdditioanlValue,
                        CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    command.Handled = true;
                }
                else if (command.IsMatch(FsmInput.Down))
                {
                    _downValue = command.AdditioanlValue;
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpDownHash,
                        AnimatorParametersHash.Instance.UpDownName,
                        command.AdditioanlValue,
                        CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    command.Handled = true;
                }

                if (command.IsMatch(FsmInput.DiveUpDown))
                {
                    extraCommand = command;
                }
            }

            if (extraCommand != null)
            {
                _downValue = _upValue = 0;
                if (!CompareUtility.IsApproximatelyEqual(extraCommand.AdditioanlValue, 0.0f))
                {
                    if (extraCommand.AdditioanlValue > 0)
                    {
                        _upValue = extraCommand.AdditioanlValue;
                    }
                    else
                    {
                        _downValue = extraCommand.AdditioanlValue;
                    }
                    
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpDownHash,
                        AnimatorParametersHash.Instance.UpDownName,
                        extraCommand.AdditioanlValue,
                        CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    extraCommand.Handled = true;
                }
            }

            if (!IsForth && !IsBack)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VerticalHash,
                                         AnimatorParametersHash.Instance.VerticalName,
                                         0f,
                                         CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }
            if (!IsLeft && !IsRight)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HorizontalHash,
                                         AnimatorParametersHash.Instance.HorizontalName,
                                         0f,
                                         CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }

            if (!IsUp && !IsDown)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpDownHash,
                    AnimatorParametersHash.Instance.UpDownName,
                    0f,
                    CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }
        }

        #endregion

        #region ICharacterMovement

        public MovementInConfig GetCurrentMovementState()
        {
            return _commonFsm.GetCurrentMovementState();
        }

        public MovementInConfig GetNextMovementState()
        {
            return _commonFsm.GetNextMovementState();
        }

        public bool IsForth { get { return _forthValue != 0; } }
        public bool IsBack { get { return _backValue != 0; } }
        public bool IsLeft { get { return _leftValue != 0; } }
        public bool IsRight { get { return _rightValue != 0; } }
        public bool IsUp
        {
            get { return _upValue != 0; }
        }

        public bool IsDown
        {
            get { return _downValue != 0; }
        }

        public float HorizontalValue { get { return IsLeft ? _leftValue : IsRight ? _rightValue : 0; } }
        public float VerticalValue { get { return IsForth ? _forthValue : IsBack ? _backValue : 0; } }
        public float UpDownValue
        {
            get { return IsUp ? _upValue : IsDown ? _downValue : 0; }
        }

        public void UpdateAxis(float horizontalValue, float verticalValue, float upDownValue)
        {
            _forthValue = 0f;
            _backValue = 0f;
            _leftValue = 0f;
            _rightValue = 0f;
            _upValue = 0f;
            _downValue = 0f;
            if (horizontalValue != 0)
            {
                if (horizontalValue > 0)
                {
                    _rightValue = horizontalValue;
                }
                else
                {
                    _leftValue = horizontalValue;
                }
            }

            if (verticalValue != 0)
            {
                if (verticalValue > 0)
                {
                    _forthValue = verticalValue;
                }
                else
                {
                    _backValue = verticalValue;
                }
            }

            if (upDownValue != 0)
            {
                if (upDownValue > 0)
                {
                    _upValue = upDownValue;
                }
                else
                {
                    _downValue = upDownValue;
                }
            }
            
        }

        #endregion

        #region ICharacterMovementInConfig

        public bool InTransition()
        {
            return _commonFsm.InTransition();
        }

        public float TransitionRemainTime()
        {
            return _commonFsm.TransitionRemainTime();
        }

        public float TransitionTime()
        {
            return _commonFsm.TransitionTime();
        }

        public MovementInConfig CurrentMovement()
        {
            return _commonFsm.GetCurrentMovementState();
        }

        public MovementInConfig NextMovement()
        {
            return _commonFsm.GetNextMovementState();
        }

        #endregion
    }
}
