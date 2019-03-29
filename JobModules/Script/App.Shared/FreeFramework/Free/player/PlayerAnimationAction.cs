using com.wd.free.action;
using System;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using Free.framework;
using Core.Free;
using com.wd.free.util;
using Assets.App.Server.GameModules.GamePlay.Free;
using App.Shared.Player;
using App.Shared;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerAnimationAction : AbstractPlayerAction
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

        public static void DoAnimation(Contexts contexts, int ani, PlayerEntity player, bool server = true)
        {
            if (player != null)
            {
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
                            player.stateInterface.State.InterruptAction();
                            break;
                        case Rescue:
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
                            player.stateInterface.State.OpenDoor();
                            break;
                        case PlayerReborn:
                            {
                                player.stateInterface.State.PlayerReborn();
                                player.appearanceInterface.Appearance.PlayerReborn();
                                player.genericActionInterface.GenericAction.PlayerReborn(player);
                            }
                            break;
                        case Revive:
                            player.stateInterface.State.Revive();
                            break;
                        case Stand:
                            player.stateInterface.State.Stand();
                            break;
                        case PlantBomb:
                            player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                            player.stateInterface.State.BuriedBomb(null);
                            break;
                        case DefuseBomb:
                            if (!server)
                            {
                                player.DefuseBomb(contexts);
                            }
                            break;
                        case InterPlantBomb:
                            player.stateInterface.State.InterruptAction();
                            player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                            break;
                        case RescueEnd:
                            player.stateInterface.State.RescueEnd();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    player.stateInterface.State.UseProps(ani);
                    player.WeaponController().ForceUnArmHeldWeapon();
                    player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                }

                if (server)
                {
                    SimpleProto message = FreePool.Allocate();
                    message.Key = FreeMessageConstant.PlayerAnimation;
                    message.Ins.Add(ani);
                    FreeMessageSender.SendMessage(player, message);
                }
            }
        }
    }
}
