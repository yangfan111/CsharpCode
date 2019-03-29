using Core.Fsm;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Core.Animation;
using Core.Appearance;
using Core.EntityComponent;
using Sharpen;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;
using Utils.Compare;

namespace App.Shared.GameModules.Player.CharacterState
{

    class FsmOutputAction
    {
        public Action<PlayerEntity, FsmOutput> ClientAction;
        public Action<PlayerEntity, FsmOutput> ServerAction;
    }
    
    public class FsmOutputBaseSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmOutputBaseSystem));
        private List<FsmOutput> _fsmOutput = new List<FsmOutput>();

        private static FsmOutputAction[] _FsmOutputAction = new FsmOutputAction[(int)FsmOutputType.End];

        static FsmOutputBaseSystem()
        {
            Action<PlayerEntity, FsmOutput> empty = (entity, output) => { }; 
            //ChangeDiveSensitivity

            for (int i = 0; i < _FsmOutputAction.Length; ++i)
            {
                _FsmOutputAction[i] = new FsmOutputAction
                {
                    ClientAction = empty,
                    ServerAction = empty
                };
            }
            _FsmOutputAction[(short) FsmOutputType.ChangeDiveSensitivity].ServerAction = empty;
            _FsmOutputAction[(short) FsmOutputType.ChangeDiveSensitivity].ClientAction = (player, output) =>
            {
                player.appearanceInterface.Appearance.SetInputSchemeActionField((int) output.FloatValue,
                    InputSchemeConst.ActionHorizontal);
                player.appearanceInterface.Appearance.SetInputSchemeActionField((int) output.FloatValue,
                    InputSchemeConst.ActionVertical);
                player.appearanceInterface.Appearance.SetInputSchemeActionField((int) output.FloatValue,
                    InputSchemeConst.ActionUpDown);
            };
            
            //CharacterControllerHeight
            _FsmOutputAction[(short) FsmOutputType.CharacterControllerHeight].ServerAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerHeight(output.FloatValue);
            };
            _FsmOutputAction[(short) FsmOutputType.CharacterControllerHeight].ClientAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerHeight(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(
                    new KeyValuePair<short, float>((short) FsmOutputType.CharacterControllerHeight, output.FloatValue));
            };

            //CharacterControllerJumpHeight
            _FsmOutputAction[(short) FsmOutputType.CharacterControllerJumpHeight].ServerAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerHeight(output.FloatValue, false);
            };

            _FsmOutputAction[(short) FsmOutputType.CharacterControllerJumpHeight].ClientAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerHeight(output.FloatValue, false);
                entity.stateInterVar.StateInterCommands.Commands.Add(
                    new KeyValuePair<short, float>((short) FsmOutputType.CharacterControllerJumpHeight,
                        output.FloatValue));
            };

            //CharacterControllerRadius
            _FsmOutputAction[(short) FsmOutputType.CharacterControllerRadius].ServerAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerRadius(output.FloatValue);
            };
                
            _FsmOutputAction[(short) FsmOutputType.CharacterControllerRadius].ClientAction = (entity, output) =>
            {
                entity.characterControllerInterface.CharacterController.SetCharacterControllerRadius(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(new KeyValuePair<short, float>((short)FsmOutputType.CharacterControllerRadius, output.FloatValue));
            };
            
            //FirstPersonHeight
            _FsmOutputAction[(short) FsmOutputType.FirstPersonHeight].ServerAction = (entity, output) =>
            {
                entity.appearanceInterface.FirstPersonAppearance.SetFirstPersonHeight(output.FloatValue);
            };
                
            _FsmOutputAction[(short) FsmOutputType.FirstPersonHeight].ClientAction = (entity, output) =>
            {
                entity.appearanceInterface.FirstPersonAppearance.SetFirstPersonHeight(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(new KeyValuePair<short, float>((short)FsmOutputType.FirstPersonHeight, output.FloatValue));
            };
            
            //FirstPersonForwardOffset
            _FsmOutputAction[(short) FsmOutputType.FirstPersonForwardOffset].ServerAction = (entity, output) =>
            {
                entity.appearanceInterface.FirstPersonAppearance.SetFirstPersonForwardOffset(output.FloatValue);
            };
                
            _FsmOutputAction[(short) FsmOutputType.FirstPersonForwardOffset].ClientAction = (entity, output) =>
            {
                entity.appearanceInterface.FirstPersonAppearance.SetFirstPersonForwardOffset(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(new KeyValuePair<short, float>((short)FsmOutputType.FirstPersonForwardOffset, output.FloatValue));
            };
            
            // Peek
            _FsmOutputAction[(short) FsmOutputType.Peek].ServerAction = (entity, output) =>
            {
                IBoneRigging boneRigging = entity.characterBoneInterface.CharacterBone;
                output.Valid = false;
                boneRigging.Peek(output.FloatValue);
            };
                
            _FsmOutputAction[(short) FsmOutputType.Peek].ClientAction = (entity, output) =>
            {
                IBoneRigging boneRigging = entity.characterBoneInterface.CharacterBone;
                output.Valid = false;
                boneRigging.Peek(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(new KeyValuePair<short, float>((short)FsmOutputType.Peek, output.FloatValue));

            };
            
            //FirstPersonSight
            _FsmOutputAction[(short) FsmOutputType.FirstPersonSight].ServerAction = (entity, output) =>
            {
                IBoneRigging boneRigging = entity.characterBoneInterface.CharacterBone;
                output.Valid = false;
                boneRigging.SightProgress(output.FloatValue);
            };
                
            _FsmOutputAction[(short) FsmOutputType.FirstPersonSight].ClientAction = (entity, output) =>
            {
                IBoneRigging boneRigging = entity.characterBoneInterface.CharacterBone;
                output.Valid = false;
                boneRigging.SightProgress(output.FloatValue);
                entity.stateInterVar.StateInterCommands.Commands.Add(new KeyValuePair<short, float>((short)FsmOutputType.FirstPersonSight, output.FloatValue));
            };
            
            //InterruptAction
            _FsmOutputAction[(short) FsmOutputType.InterruptAction].ServerAction = empty;
            _FsmOutputAction[(short) FsmOutputType.InterruptAction].ClientAction = (entity, output) =>
            {
                entity.stateInterface.State.AddInterruptInput((FsmInput) output.FloatValue);
            };

            _FsmOutputAction[(short) FsmOutputType.RebindAnimator].ServerAction = (entity, output) =>
            {
                if (entity.hasThirdPersonAnimator)
                {
                    entity.thirdPersonAnimator.UnityAnimator.Rebind();
                    Logger.InfoFormat("ThirdPerson   AnimatorRebind");
                }

                if (entity.hasFirstPersonAnimator)
                {
                    entity.firstPersonAnimator.UnityAnimator.Rebind();
                    Logger.InfoFormat("FirstPerson   AnimatorRebind");
                }
            };
            _FsmOutputAction[(short) FsmOutputType.RebindAnimator].ClientAction = (entity, output) =>
            {
                if (entity.hasThirdPersonAnimator)
                {
                    entity.thirdPersonAnimator.UnityAnimator.Rebind();
                    Logger.InfoFormat("ThirdPerson   AnimatorRebind");
                }

                if (entity.hasFirstPersonAnimator)
                {
                    entity.firstPersonAnimator.UnityAnimator.Rebind();
                    Logger.InfoFormat("FirstPerson   AnimatorRebind");
                }
            };

            _FsmOutputAction[(short) FsmOutputType.LayerWeight].ServerAction =
                _FsmOutputAction[(short) FsmOutputType.LayerWeight].ClientAction =
                    SetLayerWeightOut;

            _FsmOutputAction[(short) FsmOutputType.Bool].ServerAction =
                _FsmOutputAction[(short) FsmOutputType.Bool].ClientAction =
                    SetCommonOut;
            
            _FsmOutputAction[(short) FsmOutputType.Float].ServerAction =
                _FsmOutputAction[(short) FsmOutputType.Float].ClientAction =
                    SetCommonOut;
            
            _FsmOutputAction[(short) FsmOutputType.Int].ServerAction =
                _FsmOutputAction[(short) FsmOutputType.Int].ClientAction =
                    SetCommonOut;
        }
        
        private int _fsmOutputIndex;

        private static bool _animatorP1Changed;
        private static bool _animatorP3Changed;
        
        private static bool _animatorP1NeedUpdate = false;
        private static bool _animatorP3NeedUpdate = false;
        
        public FsmOutputBaseSystem()
        {
            for (int i = 0; i < 10; i++)
            {
                _fsmOutput.Add(new FsmOutput());
            }
        }

        public void ResetOutput()
        {
            _fsmOutputIndex = 0;
            foreach (var v in _fsmOutput)
            {
                v.Valid = false;
            }

            _animatorP1NeedUpdate = false;
            _animatorP3NeedUpdate = false;
        }

        public void AddOutput(FsmOutput output)
        {
            _fsmOutput[_fsmOutputIndex].CopyFrom(output);
            _fsmOutput[_fsmOutputIndex].Valid = true;

            _fsmOutputIndex++;
            if (_fsmOutputIndex >= _fsmOutput.Count)
            {
                _fsmOutput.Add(new FsmOutput());
            }
        }

        public void SetOutput(PlayerEntity player)
        {
            IBoneRigging boneRigging = player.characterBoneInterface.CharacterBone;

            foreach (var output in _fsmOutput)
            {
                if (output.Valid)
                {
                    if (SharedConfig.IsServer)
                    {
                        _FsmOutputAction[(int) output.Type].ServerAction(player, output);
                    }
                    else
                    {
                        _FsmOutputAction[(int) output.Type].ClientAction(player, output);
                    }
                }
            }
        }

        private static void SetLayerWeightOut(PlayerEntity player, FsmOutput output)
        {
            if ((output.View & CharacterView.FirstPerson) != 0)
            {
                SetLayerWeight(player.firstPersonAnimator.UnityAnimator, output);
                _animatorP1Changed = true;
                // next frame update
                //_animatorP1NeedUpdate = true;
            }

            if ((output.View & CharacterView.ThirdPerson) != 0)
            {
                SetLayerWeight(player.thirdPersonAnimator.UnityAnimator, output);
                _animatorP3Changed = true;
                //// next frame update
                //_animatorP3NeedUpdate = true;
               //Logger.InfoFormat("change due to SetLayerWeight");
            }
        }

        private static void SetCommonOut(PlayerEntity player, FsmOutput output)
        {
            if ((output.View & CharacterView.FirstPerson) != 0)
                SetAnimatorParameterP1(player.firstPersonAnimator.UnityAnimator, output, player.fpAnimStatus);
            if ((output.View & CharacterView.ThirdPerson) != 0)
                SetAnimatorParameterP3(player.thirdPersonAnimator.UnityAnimator, output, player.networkAnimator);
        }

        public bool AnimatorP1ChangedTrigger()
        {
            var ret = _animatorP1Changed;
            _animatorP1Changed = false;
            return ret;
        }

        public bool AnimatorP3ChangedTrigger()
        {
            var ret = _animatorP3Changed;
            _animatorP3Changed = false;
            return ret;
        }
        
        public bool NeedUpdateP1()
        {
            return _animatorP1NeedUpdate;
        }

        public bool NeedUpdateP3()
        {
            return _animatorP3NeedUpdate;
        }

        private static void SetAnimatorParameterP1(Animator animator, FsmOutput output, FpAnimStatusComponent latestValue)
        {
            var index = latestValue.AnimatorParameterIndex[output.TargetHash];
            var latestParam = latestValue.AnimatorParameters[index];

            var isChange = SetAnimatorParameter(animator, output, latestParam);
            _animatorP1Changed = isChange || _animatorP1Changed;
            _animatorP1NeedUpdate |= (isChange && output.UpdateImmediate);
        }

        private static void SetAnimatorParameterP3(Animator animator, FsmOutput output, NetworkAnimatorComponent latestValue)
        {
            if(!latestValue.AnimatorParameterIndex.ContainsKey(output.TargetHash))
            {
                Logger.ErrorFormat("animator param is not found:{0}!!!", output.Target);
            }
            var index = latestValue.AnimatorParameterIndex[output.TargetHash];
            var latestParam = latestValue.AnimatorParameters[index];

            var isChange = SetAnimatorParameter(animator, output, latestParam);
            _animatorP3Changed = isChange || _animatorP3Changed;
            _animatorP3NeedUpdate |= (isChange && output.UpdateImmediate);
        }

        private static bool SetAnimatorParameter(Animator animator, FsmOutput output, NetworkAnimatorParameter latestValue)
        {
            bool ret = false;

            switch (output.Type)
            {
                case FsmOutputType.Bool:
                    ret = SetBool(animator, output, latestValue);
                    break;
                case FsmOutputType.Float:
                    ret = SetFloat(animator, output, latestValue);
                    break;
                case FsmOutputType.Int:
                    ret = SetInt(animator, output, latestValue);
                    break;
            }

            return ret;
        }

        private static bool CompareParamHistory(FsmOutput newValue, NetworkAnimatorParameter latestValue)
        {
            bool ret = false;
            
            switch (newValue.Type)
            {
                case FsmOutputType.Bool:
                    ret = !CompareUtility.IsApproximatelyEqual(newValue.BoolValue, latestValue.BoolValue);
                    if (ret)
                    {
                        //Logger.ErrorFormat("change due to bool:{0}, from:{1},to:{2}", newValue.TargetHash, latestValue.BoolValue,newValue.BoolValue);
                        latestValue.SetParam(AnimatorControllerParameterType.Bool,
                            newValue.BoolValue,
                            newValue.TargetHash);
                    }
                        
                    
                    break;
                case FsmOutputType.Float:
                    ret = !CompareUtility.IsApproximatelyEqual(newValue.FloatValue, latestValue.FloatValue, 0.001f);
                    if (ret)
                    {
                        //Logger.ErrorFormat("change due to FloatValue:{0}, from:{1},to:{2}", newValue.TargetHash, latestValue.FloatValue,newValue.FloatValue);
                        latestValue.SetParam(AnimatorControllerParameterType.Float,
                            newValue.FloatValue,
                            newValue.TargetHash);
                    }
                        
                    

                    break;
                case FsmOutputType.Int:
                    ret = !CompareUtility.IsApproximatelyEqual(newValue.IntValue, latestValue.IntValue);
                    if (ret)
                    {
                        //Logger.ErrorFormat("change due to IntValue:{0}, from:{1},to:{2}", newValue.TargetHash, latestValue.IntValue,newValue.IntValue);
                        latestValue.SetParam(AnimatorControllerParameterType.Int,
                            newValue.IntValue,
                            newValue.TargetHash);
                    }
                        
                    

                    break;
            }

            return ret;
        }

        private static bool SetBool(Animator animator, FsmOutput output, NetworkAnimatorParameter latetstParam)
        {
            if (CompareParamHistory(output, latetstParam))
            {
                animator.SetBool(output.TargetHash, output.BoolValue);
                if (output.UpdateImmediate)
                {
                    animator.Update(0, false);
                }
                return true;
            }

            return false;
        }

        private static bool SetFloat(Animator animator, FsmOutput output, NetworkAnimatorParameter latetstParam)
        {
            if (CompareParamHistory(output, latetstParam))
            {
                animator.SetFloat(output.TargetHash, output.FloatValue);
                
                return true;
            }

            return false;
        }

        private static bool SetInt(Animator animator, FsmOutput output, NetworkAnimatorParameter latetstParam)
        {
            if (CompareParamHistory(output, latetstParam))
            {
                animator.SetInteger(output.TargetHash, output.IntValue);

                return true;
            }

            return false;
        }

        private static bool SetLayerWeight(Animator animator, FsmOutput output)
        {
            animator.SetLayerWeight(output.IntValue, output.FloatValue);
            return true;
        }
    }
}
