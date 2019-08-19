using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using App.Shared.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core;
using Core.EntityComponent;
using Core.Free;
using Core.Room;
using Core.Utils;
using Free.framework;
using System;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerAnimationAction : AbstractPlayerAction, IRule
    {
        public const int PickUp = 101;
        public const int Rescue = 102;
        public const int Dying = 103;
        public const int Stop = 104;
        public const int Dive = 105;
        public const int Crouch = 106;
        public const int Ashore = 107;
        public const int BeenHit = 108;
        public const int Fire = 109;
        public const int Freefall = 110;
        public const int Gliding = 111;
        public const int OpenDoor = 112;
        public const int PlayerReborn = 113;
        public const int Revive = 114;
        public const int Stand = 115;
        public const int PlantBomb = 116;
        public const int DefuseBomb = 117;
        public const int InterPlantBomb = 118;
        public const int RescueEnd = 119;
        public const int Interrupt = 120;
        public const int SuccessPose = 121;
        public const int InterruptSwitchWeapon = 122;

        private string state;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);

            int realState = FreeUtil.ReplaceInt(state, args);

            if (fd != null)
            {
                DoAnimation(args.GameContext, realState, fd.Player);
            }
        }
private  static LoggerAdapter _loggerAdapter = new LoggerAdapter("PlayerAnimationAction");
        public static void DoAnimation(Contexts contexts, int ani, PlayerEntity player, bool server = true)
        {
            if (player != null)
            {
                var pose = 0;
                if (ani > 100)
                {
                    switch (ani)
                    {
                        case Stop:
                            if(!server)
                            {
                                player.StopAnimation();
                            }
                            break;
                        case PickUp:
                            player.stateInterface.State.PickUp();
                            break;
                        case Interrupt:
                            _loggerAdapter.Info("[Tmp]Interrupt");
                            player.stateInterface.State.InterruptAction();
                            break;
                        case Rescue:
                            player.stateInterface.State.SetPostureCrouch();
                            player.stateInterface.State.Rescue();
                            break;
                        case Dying:
                            player.stateInterface.State.Dying();
                            break;
                        case Dive:
                            player.stateInterface.State.Dive();
                            break;
                        case Crouch:
                            player.stateInterface.State.Crouch();
                            break;
                        case Ashore:
                            player.stateInterface.State.Ashore();
                            break;
                        case BeenHit:
                            player.stateInterface.State.BeenHit();
                            break;
                        case Fire:
                            player.stateInterface.State.Fire();
                            break;
                        case Freefall:
                            player.stateInterface.State.Freefall();
                            break;
                        case Gliding:
                            player.stateInterface.State.Gliding();
                            break;
                        case OpenDoor:
                            player.StateInteractController().InterruptCharactor();

                            player.stateInterface.State.OpenDoor();
                            player.AudioController().PlaySimpleAudio(EAudioUniqueId.OpenDoor);
                            break;
                        case PlayerReborn:
                            if (player.WeaponController().RelatedThrowAction.IsReady)
                            {
                                player.WeaponController().RelatedThrowAction.InternalCleanUp();
                            }
                            player.stateInterface.State.PlayerReborn();
                            player.appearanceInterface.Appearance.PlayerReborn();
                            player.genericActionInterface.GenericAction.PlayerReborn(player);
                            break;
                        case Revive:
                            player.stateInterface.State.Revive();
                            player.stateInterface.State.SetPostureCrouch();
                            break;
                        case Stand:
                            player.stateInterface.State.Stand();
                            break;
                        case PlantBomb:
                            player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                            player.WeaponController().RelatedThrowAction.ThrowingEntityKey = new EntityKey(0, (short) EEntityType.End);
                            player.WeaponController().RelatedThrowAction.LastFireWeaponKey = -1;
                            player.stateInterface.State.BuriedBomb(null);
                            break;
                        case DefuseBomb:
                            if (!server)
                            {
                                player.DefuseBomb(contexts);
                            }
                            break;
                        case InterPlantBomb:
                            _loggerAdapter.Info("[Tmp]InterPlantBomb Interrupt");
                            player.stateInterface.State.InterruptAction();
                            player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                            break;
                        case RescueEnd:
                            player.stateInterface.State.RescueEnd();
                            break;
                        case SuccessPose:
                            if (player.playerInfo.CampInfo != null && player.playerInfo.CampInfo.Preset.Count > 0)
                            {
                                foreach (Preset p in player.playerInfo.CampInfo.Preset)
                                {
                                    if (p.camp == player.playerInfo.Camp)
                                    {
                                        var config = SingletonManager.Get<IndividuationConfigManager>().GetConfigById(p.pose);
                                        if (config != null)
                                        {
                                            pose = config.PoseId;
                                        }
                                        else
                                        {
                                            config = SingletonManager.Get<IndividuationConfigManager>().GetConfigById(3);
                                            if (null != config) pose = config.PoseId;
                                        }

                                        break;
                                    }
                                }
                            }
                            break;
                        case InterruptSwitchWeapon:
                            player.stateInterface.State.InterruptSwitchWeapon();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    player.stateInterface.State.UseProps(ani);
                    player.WeaponController().UnArmWeapon(false);
                    player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                }

                if (server)
                {
                    SimpleProto message = FreePool.Allocate();
                    message.Key = FreeMessageConstant.PlayerAnimation;
                    message.Ins.Add(ani);
                    message.Ins.Add(pose);
                    FreeMessageSender.SendMessage(player, message);
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerAnimationAction;
        }
    }
}
