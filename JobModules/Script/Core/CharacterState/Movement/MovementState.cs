using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using UnityEngine;
using Core.Configuration;
using XmlConfig;
using Utils.CharacterState;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Movement
{
    class MovementState : FsmState
    {
        public static MovementState CreateIdleState()
        {
            MovementState state = new MovementState(MovementStateId.Idle);

            #region Idle to Walk

            state.AddTransition(
                (command, addOutput) =>
                {
                    var walkRet = command.IsMatch(FsmInput.Walk);
                    var runRet = command.IsMatch(FsmInput.Run) || command.IsMatch(FsmInput.Sprint);
                    
                    if (walkRet || runRet)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.IsWalkHash,
                            AnimatorParametersHash.Instance.IsWalkName,
                            walkRet,
                            CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                                                 AnimatorParametersHash.Instance.MotionName,
                                                 AnimatorParametersHash.Instance.MotionValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MovementHash,
                                                 AnimatorParametersHash.Instance.MovementName,
                                                 AnimatorParametersHash.Instance.WalkValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }
                    return walkRet || runRet;
                },
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Idle))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Walk,
                null,
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Idle, MovementInConfig.Walk),
                new[] { FsmInput.Walk, FsmInput.Run, FsmInput.Sprint });
            #endregion

            #region idle to divemove
            state.AddTransition(new DiveTransition(state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    bool ret = command.IsMatch(FsmInput.DiveMove);
                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                            AnimatorParametersHash.Instance.MotionName,
                            AnimatorParametersHash.Instance.MotionValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Idle))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.DiveMove,
                null,
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Idle, MovementInConfig.DiveMove)
                ), new[] { FsmInput.DiveMove});

            #endregion

            return state;
        }

        public static MovementState CreateWalkState()
        {
            MovementState state = new MovementState(MovementStateId.Walk);

            #region Walk to Idle

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Idle),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Run) || command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int) MovementStateId.Idle,
                (normalizedTime, addOutput) =>
                {
                    if (normalizedTime >= 1)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                                                 AnimatorParametersHash.Instance.MotionName,
                                                 AnimatorParametersHash.Instance.MotionlessValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                },
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Walk, MovementInConfig.Idle),
                new[] { FsmInput.Idle });

            #endregion

            #region Walk to Run

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Run),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int) MovementStateId.Run,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.WalkValue,
                                          AnimatorParametersHash.Instance.RunValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Walk, MovementInConfig.Run),
                new[] { FsmInput.Run });

            #endregion

            #region Walk to Sprint

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Sprint),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int) MovementStateId.Sprint,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.WalkValue,
                                          AnimatorParametersHash.Instance.SprintValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Walk, MovementInConfig.Sprint),
                new[] { FsmInput.Sprint });

            #endregion

            #region to divemovement
            AddTransitionToMovement(state);
            #endregion
            return state;
        }

        public static MovementState CreateRunState()
        {
            MovementState state = new MovementState(MovementStateId.Run);

            #region Run to Idle

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Idle),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Walk) || command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int) MovementStateId.Idle,
                (normalizedTime, addOutput) =>
                {
                    if (normalizedTime >= 1)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                                                 AnimatorParametersHash.Instance.MotionName,
                                                 AnimatorParametersHash.Instance.MotionlessValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                },
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Run, MovementInConfig.Idle),
                new[] { FsmInput.Idle });

            #endregion

            #region Run to Walk

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Walk),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Walk,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.RunValue,
                                          AnimatorParametersHash.Instance.WalkValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Run, MovementInConfig.Walk),
                new[] { FsmInput.Walk });

            #endregion

            #region Run to Sprint

            state.AddTransition(
                (command, addOutput) =>FsmTransition.SimpleCommandHandler(command, FsmInput.Sprint),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Sprint,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.RunValue,
                                          AnimatorParametersHash.Instance.SprintValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Run, MovementInConfig.Sprint),
                new[] { FsmInput.Sprint });

            #endregion

            #region to divemovement
            AddTransitionToMovement(state);
            #endregion
            return state;
        }

        public static MovementState CreateSprintState()
        {
            MovementState state = new MovementState(MovementStateId.Sprint);

            #region Sprint to Idle

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Idle),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Walk) || command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Idle,
                (normalizedTime, addOutput) =>
                {
                    if (normalizedTime >= 1)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                                                 AnimatorParametersHash.Instance.MotionName,
                                                 AnimatorParametersHash.Instance.MotionlessValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                },
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Sprint, MovementInConfig.Idle),
                new[] { FsmInput.Idle });

            #endregion

            #region Sprint to Walk

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Walk),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Run))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Walk,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.SprintValue,
                                          AnimatorParametersHash.Instance.WalkValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Sprint, MovementInConfig.Walk),
                new[] { FsmInput.Walk });

            #endregion

            #region Sprint to Run 

            state.AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Run),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Sprint))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    if (command.IsMatch(FsmInput.Idle) || command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ChangeRoad;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Run,
                FsmTransition.GetLerpFunc(AnimatorParametersHash.Instance.MovementHash,
                                          AnimatorParametersHash.Instance.MovementName,
                                          AnimatorParametersHash.Instance.SprintValue,
                                          AnimatorParametersHash.Instance.RunValue,
                                          CharacterView.FirstPerson | CharacterView.ThirdPerson),
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.Sprint, MovementInConfig.Run),
                new[] { FsmInput.Run });

            #endregion

            #region to divemovement
            AddTransitionToMovement(state);
            #endregion

            return state;
        }

        public static FsmState CreateDiveMoveState()
        {
            MovementState state = new MovementState(MovementStateId.DiveMove);

            #region DiveMove to Idle
            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(),
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Idle),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Sprint) || command.IsMatch(FsmInput.Run) || command.IsMatch(FsmInput.Walk))
                    {
                        return FsmTransitionResponseType.ForceEnd;
                    }
                    return FsmTransitionResponseType.NoResponse;
                },
                (int)MovementStateId.Idle,
                (normalizedTime, addOutput) =>
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                        AnimatorParametersHash.Instance.MotionName,
                        AnimatorParametersHash.Instance.MotionlessValue,
                        CharacterView.FirstPerson | CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                },
                SingletonManager.Get<CharacterStateConfigManager>().GetMovementTransitionTime(MovementInConfig.DiveMove, MovementInConfig.Idle),null, true), new[] { FsmInput.Idle });

            #endregion

            #region DiveMove to move
            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Walk))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null,
                (int)MovementStateId.Walk,
                null,
                0), new[] {FsmInput.Walk });
            
            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Run))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null,
                (int)MovementStateId.Run,
                null,
                0), new[] {FsmInput.Run});
            
            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Sprint))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null,
                (int)MovementStateId.Sprint,
                null,
                0), new[] { FsmInput.Sprint });

            #endregion
            return state;
        }

        protected static void AddTransitionToMovement(MovementState state)
        {
            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.DiveMove);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                            AnimatorParametersHash.Instance.MotionName,
                            AnimatorParametersHash.Instance.MotionValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                    }
                    return ret;
                },
                null, (int)MovementStateId.DiveMove, null, 0, new[] { FsmInput.DiveMove });
        }

        public MovementState(MovementStateId id) : base((short) id)
        {
        }
    }
}
