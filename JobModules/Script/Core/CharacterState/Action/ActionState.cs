using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Action.States;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Core.Configuration;
using XmlConfig;
using Utils.CharacterState;
using Core.CharacterState.Action.Transitions;
using Utils.Configuration;
using Random = UnityEngine.Random;
using Core.CharacterState.Action.States.SpecialFire;
using Utils.Singleton;

namespace Core.CharacterState.Action
{
    class ActionState : FsmState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ActionState));

        public static ActionState CreateCommonNullState()
        {
            ActionState state = new ActionState(ActionStateId.CommonNull);

            #region UpperBody Additive Animation Layer

            #region CommonNull => Fire
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Fire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHash,
                                                 AnimatorParametersHash.Instance.FireName,
                                                 AnimatorParametersHash.Instance.FireEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);
                        
                        command.Handled = true;

                        return true;
                    }
                    return false;
                },
                null, (int) ActionStateId.Fire, null, 0, new[] { FsmInput.Fire });

            #endregion

            #region CommonNull => SightsFire

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SightsFire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SightsFireHash,
                                                 AnimatorParametersHash.Instance.SightsFireName,
                                                 AnimatorParametersHash.Instance.SightsFireEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }
                    return false;
                },
                null, (int)ActionStateId.Fire, null, 0, new[] { FsmInput.SightsFire });

            #endregion

            #region CommonNull => SpecialFire(栓动)

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SpecialFire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHash,
                                                 AnimatorParametersHash.Instance.FireName,
                                                 AnimatorParametersHash.Instance.FireEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.SpecialFire, null, 0, new[] { FsmInput.SpecialFire });

            #endregion

            #region CommonNull => SpecialSightsFire(栓动)

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SpecialSightsFire))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SightsFireHash,
                                                 AnimatorParametersHash.Instance.SightsFireName,
                                                 AnimatorParametersHash.Instance.SightsFireEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHash,
                                                 AnimatorParametersHash.Instance.FireName,
                                                 AnimatorParametersHash.Instance.FireEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.SpecialFire, null, 0, new[] { FsmInput.SpecialSightsFire });

            #endregion

            #region CommonNull => Injury

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Injury))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuryHash,
                                                 AnimatorParametersHash.Instance.InjuryName,
                                                 AnimatorParametersHash.Instance.InjuryStartValue,
                                                 CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);
                        
                        command.Handled = true;

                        return true;
                    }

                    return false;
                }, null, (int) ActionStateId.Injury, null, 0, new[] { FsmInput.Injury });
            
            #endregion
            
            #region CommonNull => Reload
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Reload))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadHash,
                                                 AnimatorParametersHash.Instance.ReloadName,
                                                 AnimatorParametersHash.Instance.ReloadEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                        return true;
                    }
                    else if (command.IsMatch(FsmInput.ReloadEmpty))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadEmptyHash,
                                                 AnimatorParametersHash.Instance.ReloadEmptyName,
                                                 AnimatorParametersHash.Instance.ReloadEmptyEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                        return true;
                    }
                    
                    command.Handled = true;
                    
                    return false;
                },
                null, (int) ActionStateId.Reload, null, 0, new[] { FsmInput.Reload, FsmInput.ReloadEmpty });
            
            #endregion

            #endregion
            
            #region UpperBody Overlay Animation Layer
            
            #region CommonNull to Unarm
            
            state.AddTransition(
                (command, addOutput) =>
                {    
                    if (command.IsMatch(FsmInput.Unarm))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterStateHash,
                                                 AnimatorParametersHash.Instance.HolsterStateName,
                                                 command.AdditioanlValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterHash,
                                                 AnimatorParametersHash.Instance.HolsterName,
                                                 AnimatorParametersHash.Instance.HolsterEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        //TurnOnUpperBodyOverlay(addOutput);

                        command.Handled = true;
                        
                        return true;
                    }

                    return false;
                }, 
                null,
                (int) ActionStateId.Unarm,
                (normalizedTime, addOutput) => { LerpOpenUpperBodyLayer(addOutput, normalizedTime); }, 
                (int)SingletonManager.Get<CharacterStateConfigManager>().HolsterTransitionTime, new[] { FsmInput.Unarm });
            
            #endregion

            #region CommonNull to Draw

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Draw))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DrawStateHash,
                                                 AnimatorParametersHash.Instance.DrawStateName,
                                                 command.AdditioanlValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SelectHash,
                                                 AnimatorParametersHash.Instance.SelectName,
                                                 AnimatorParametersHash.Instance.SelectEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        TurnOnUpperBodyOverlay(addOutput);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.Draw, null, 0, new[] { FsmInput.Draw });

            #endregion
            
            #region CommonNull to SwitchWeapon
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SwitchWeapon))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterStateHash,
                                                 AnimatorParametersHash.Instance.HolsterStateName,
                                                 command.AdditioanlValue / 10,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DrawStateHash,
                                                 AnimatorParametersHash.Instance.DrawStateName,
                                                 command.AdditioanlValue % 10,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterHash,
                                                 AnimatorParametersHash.Instance.HolsterName,
                                                 AnimatorParametersHash.Instance.HolsterEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwitchWeaponHash,
                                                 AnimatorParametersHash.Instance.SwitchWeaponName,
                                                 AnimatorParametersHash.Instance.SwitchWeaponEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        
                        //TurnOnUpperBodyOverlay(addOutput);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.SwitchWeapon, (normalizedTime, addOutput) => { LerpOpenUpperBodyLayer(addOutput, normalizedTime); }, (int)SingletonManager.Get<CharacterStateConfigManager>().HolsterTransitionTime, new[] { FsmInput.SwitchWeapon });
            
            #endregion
            
            #region CommonNull to PickUp
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.PickUp))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PickUpHash,
                                                 AnimatorParametersHash.Instance.PickUpName,
                                                 AnimatorParametersHash.Instance.PickUpEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        
                        TurnOnUpperBodyOverlay(addOutput);

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.PickUp, null, 0, new[] { FsmInput.PickUp });

            #endregion

            #region CommonNull to OpenDoor

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.OpenDoor))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.OpenDoorHash,
                                                 AnimatorParametersHash.Instance.OpenDoorName,
                                                 AnimatorParametersHash.Instance.OpenDoorEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        TurnOnUpperBodyOverlay(addOutput);

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.OpenDoor, null, 0, new[] { FsmInput.OpenDoor });

            #endregion

            #region CommonNull to Props

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Props))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsHash,
                                                 AnimatorParametersHash.Instance.PropsName,
                                                 AnimatorParametersHash.Instance.PropsEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PropsStateHash,
                                                 AnimatorParametersHash.Instance.PropsStateName,
                                                 command.AdditioanlValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        TurnOnUpperBodyOverlay(addOutput);

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.Props, null, 0, new[] { FsmInput.Props });

            #endregion

            #region CommonNull to Melee

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.LightMeleeAttackOne))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeAttackName,
                            AnimatorParametersHash.Instance.LightMeleeOne,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                    else if (command.IsMatch(FsmInput.LightMeleeAttackTwo))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeAttackName,
                            AnimatorParametersHash.Instance.LightMeleeTwo,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                    else if (command.IsMatch(FsmInput.MeleeSpecialAttack))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeAttackName,
                            AnimatorParametersHash.Instance.ForceMelee,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                    else
                        return false;
                    
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeAttackHash,
                                             AnimatorParametersHash.Instance.MeleeAttackName,
                                             AnimatorParametersHash.Instance.MeleeAttackStart,
                                             CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                    addOutput(FsmOutput.Cache);
                    
                    command.Handled = true;
                    
                    TurnOnUpperBodyOverlay(addOutput);
                    
                    return true;
                },
                null, (int) ActionStateId.MeleeAttack, null, 0, new[] { FsmInput.LightMeleeAttackOne, FsmInput.MeleeSpecialAttack, FsmInput.LightMeleeAttackTwo });
            
            #endregion
            
            #region CommonNull to Grenade
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.StartNearGrenade))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.NearThrowHash,
                                                 AnimatorParametersHash.Instance.NearThrowName,
                                                 AnimatorParametersHash.Instance.NearThrowEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                    else if (command.IsMatch(FsmInput.StartFarGrenade))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.NearThrowHash,
                                                 AnimatorParametersHash.Instance.NearThrowName,
                                                 AnimatorParametersHash.Instance.NearThrowDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }
                    else
                        return false;

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.StartThrowHash,
                                             AnimatorParametersHash.Instance.StartThrowName,
                                             AnimatorParametersHash.Instance.StartThrowEnable,
                                             CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                    addOutput(FsmOutput.Cache);
                    
                    command.Handled = true;
                    
                    TurnOnUpperBodyOverlay(addOutput);
                    
                    return true;
                },
                null, (int) ActionStateId.Grenade, null, 0, new[] { FsmInput.StartNearGrenade, FsmInput.StartFarGrenade});
            
            #endregion
            
            #region CommonNull to SpecialReload
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.SpecialReload))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SpecialReloadHash,
                                                 AnimatorParametersHash.Instance.SpecialReloadName,
                                                 AnimatorParametersHash.Instance.SpecialReloadEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        
                        TurnOnUpperBodyOverlay(addOutput);
                        
                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.SpecialReload, null, 0, new[] { FsmInput.SpecialReload });
            
            #endregion
            
            #endregion

            #region FullBody Animation Layer
            
            #region CommonNull to Gliding
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Gliding))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.GlidingHash,
                                                 AnimatorParametersHash.Instance.GlidingName,
                                                 AnimatorParametersHash.Instance.GlidingEnableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.Gliding, null, 0, new[] { FsmInput.Gliding });
            
            #endregion
            
            #region CommonNull to BuriedBomb
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.BuriedBomb))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UseHash,
                            AnimatorParametersHash.Instance.UseName,
                            AnimatorParametersHash.Instance.UseEnableValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.BuriedBomb, null, 0, new[] { FsmInput.BuriedBomb });
            
            #endregion
            
            #region CommonNull to DismantleBomb
            
            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.DismantleBomb))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DismantleHash,
                            AnimatorParametersHash.Instance.DismantleName,
                            AnimatorParametersHash.Instance.DismantleEnableValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.DismantleBomb, null, 0, new[] { FsmInput.DismantleBomb });
            
            #endregion
            
            #endregion
            
            return state;
        }

        #region KeepState
        public static ActionState CreateKeepNullState()
        {
            ActionState state = new ActionState(ActionStateId.KeepNull);

            #region KeepNull to Sight

            state.AddTransition(new OverlayNullToSightTransition(state.AvailableTransitionId(),
                (int)ActionStateId.Sight,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Null, PostureInConfig.Sight)),
                new[] { FsmInput.Sight });

            #endregion

            #region KeepNull to VehiclesAnim

            state.AddTransition(
                (command, addOutput) =>
                {
                    float seatId = command.AdditioanlValue % 10;
                    int postureId = (int)(command.AdditioanlValue / 10);

                    if (command.IsMatch(FsmInput.DriveStart))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesAnimHash,
                                                 AnimatorParametersHash.Instance.VehiclesAnimName,
                                                 AnimatorParametersHash.Instance.VehiclesAnimEnableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesAnimStateHash,
                                                 AnimatorParametersHash.Instance.VehiclesAnimStateName,
                                                 seatId,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesStateHash,
                                                 AnimatorParametersHash.Instance.VehiclesStateName,
                                                 (float)postureId,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.GripHandPoseHash,
                                              AnimatorParametersHash.Instance.GripHandPoseName,
                                              AnimatorParametersHash.Instance.GripHandPoseDisableValue,
                                              CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                              AnimatorParametersHash.Instance.ProneName,
                                              AnimatorParametersHash.Instance.ProneDisable,
                                              CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceEndProneHash,
                                              AnimatorParametersHash.Instance.ForceEndProneName,
                                              AnimatorParametersHash.Instance.ForceEndProneEnable,
                                              CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                              AnimatorParametersHash.Instance.PostureName,
                                              AnimatorParametersHash.Instance.StandValue,
                                              CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.Drive, null, 0, new[] { FsmInput.DriveStart });

            #endregion

            #region KeepNull to Rescue

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Rescue))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RescueHash,
                                                 AnimatorParametersHash.Instance.RescueName,
                                                 AnimatorParametersHash.Instance.RescueEnableValue,
                                                 CharacterView.ThirdPerson | CharacterView.FirstPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.Rescue, null, 0, new[] { FsmInput.Rescue });

            #endregion

            return state;
        }
        
        public static ActionState CreateVehiclesAnimState()
        {
            return new DriveState(ActionStateId.Drive);
        }

        public static ActionState CreateSightP3State()
        {
            ActionState state = new ActionState(ActionStateId.Sight);

            #region SightP3 To KeepNull

            state.AddTransition(new SightToOverlayNullTransition(state.AvailableTransitionId(),
                (int)ActionStateId.KeepNull,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Sight, PostureInConfig.Null)),
                new[] { FsmInput.CancelSight });

            #endregion

            return state;
        }
        #endregion

        #region UpperBody Additive Animation Layer

        public static ActionState CreateFireState()
        {
            return new FireState(ActionStateId.Fire);
        }

        public static ActionState CreateSpecialFireState()
        {
            return new SpecialFireState(ActionStateId.SpecialFire);
        }

        public static ActionState CreateSpecialFireHold()
        {
            return new SpecialFireHoldState(ActionStateId.SpecialFireHold);
        }

        public static ActionState CreateSpecialFireEnd()
        {
            return new SpecialFireEndState(ActionStateId.SpecialFireEnd);
        }

        public static ActionState CreateInjuryState()
        {
            return new InjuryState(ActionStateId.Injury);
        }

        public static ActionState CreateReloadState()
        {
            return new ReloadState(ActionStateId.Reload);
        }

        #endregion

        #region UpperBody Overlay Animation Layer

        public static ActionState CreateUnarmState()
        {
            return new UnarmState(ActionStateId.Unarm);
        }

        public static ActionState CreateDrawState()
        {
            return new DrawState(ActionStateId.Draw);
        }

        public static ActionState CreateSwitchWeaponState()
        {
            return new SwitchWeaponState(ActionStateId.SwitchWeapon);
        }
        
        public static ActionState CreatePickUpState()
        {
            return new PickUpState(ActionStateId.PickUp);
        }

        public static ActionState CreateOpenDoorState()
        {
            return new OpenDoorState(ActionStateId.OpenDoor);
        }

        public static ActionState CreatePropsState()
        {
            return new PropsState(ActionStateId.Props);
        }
        
        public static ActionState CreateMeleeAttackState()
        {
            return new MeleeAttackState(ActionStateId.MeleeAttack);
        }
        
        public static ActionState CreateGrenadeState()
        {
            return new GrenadeState(ActionStateId.Grenade);
        }
        
        public static ActionState CreateSpecialReloadState()
        {
            return new SpecialReloadState(ActionStateId.SpecialReload);
        }
       
        #endregion

        #region FullBody Animation Layer
        
        public static ActionState CreateRescueState()
        {
            ActionState state = new ActionState(ActionStateId.Rescue);

            #region Rescue to KeepNull

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.RescueEnd))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RescueHash,
                                                 AnimatorParametersHash.Instance.RescueName,
                                                 AnimatorParametersHash.Instance.RescueDisableValue,
                                                 CharacterView.ThirdPerson | CharacterView.FirstPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.KeepNull, null, 0, new[] { FsmInput.RescueEnd }); 
            
            #endregion

            return state;
        }

        public static ActionState CreateDismantleBombState()
        {
            return new DismantleBombState(ActionStateId.DismantleBomb);
        }

        public static ActionState CreateBuriedBombState()
        {
            return new BuriedBombState(ActionStateId.BuriedBomb);
        }
        
        public static ActionState CreateGlidingState()
        {
            ActionState state = new ActionState(ActionStateId.Gliding);

            #region Gliding to Parachute

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Parachuting))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.GlidingHash,
                                                 AnimatorParametersHash.Instance.GlidingName,
                                                 AnimatorParametersHash.Instance.GlidingDisableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ParachuteHash,
                                                 AnimatorParametersHash.Instance.ParachuteName,
                                                 AnimatorParametersHash.Instance.ParachuteEnableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.Parachuting, null, 0, new[] { FsmInput.Parachuting });

            #endregion

            return state;
        }

        public static ActionState CreateParachutingState()
        {
            ActionState state = new ActionState(ActionStateId.Parachuting);

            #region Parachuting to CommonNull

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ParachutingEnd))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ParachuteHash,
                                                 AnimatorParametersHash.Instance.ParachuteName,
                                                 AnimatorParametersHash.Instance.ParachuteDisableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.ParachutingEnd });

            #endregion

            return state;
        }
        
        #endregion
        
        public ActionState(ActionStateId id) : base((short) id)
        {
        }

        public static void LerpUpperBodyLayerWeight(IFsmInputCommand command, Action<FsmOutput> addOutput ,float threshold)
        {
            float timeRemain = command.AlternativeAdditionalValue * (1 - command.AdditioanlValue);

            if (timeRemain <= threshold)
            {
                FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.UpperBodyLayer,
                    Mathf.Lerp(AnimatorParametersHash.Instance.UpperBodyEnableValue,
                        AnimatorParametersHash.Instance.UpperBodyDisableValue,
                        (threshold - timeRemain) / threshold),
                    CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }
        }

        public static void LerpOpenUpperBodyLayer(Action<FsmOutput> addOutput, float ratio)
        {
            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.UpperBodyLayer,
                Mathf.Lerp(AnimatorParametersHash.Instance.UpperBodyDisableValue,
                    AnimatorParametersHash.Instance.UpperBodyEnableValue,
                    Mathf.Clamp01(ratio)),
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }

        protected static void TurnOnUpperBodyOverlay(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.UpperBodyLayer,
                AnimatorParametersHash.Instance.UpperBodyEnableValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }

        protected static void TurnOffUpperBodyOverlay(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.UpperBodyLayer,
                AnimatorParametersHash.Instance.UpperBodyDisableValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }
    }
}
