using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using System;
using App.Client.GameModules.Player;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using UnityEngine;
using Core.Configuration;
using Core;
using Utils.Appearance;
using WeaponConfigNs;

namespace App.Client.GameModules.ClientPlayer
{
    public class PlayerPlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerPlaybackSystem));

        private Contexts _contexts;
        private IGroup<PlayerEntity> _players;
        public PlayerPlaybackSystem(Contexts contexts):base(contexts)
        {
            _contexts = contexts;
           
        }


        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
           return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.ThirdPersonModel).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.IsOnVehicle();
        }


        protected override void OnPlayBack(PlayerEntity player)
        {
       

                bool playbackPlayer = true;
                if (player.hasPlayerSkyMove)
                {
                    var playerSkyMove = player.playerSkyMove;
                    if (playerSkyMove.IsMoveEnabled)
                    {
                        if (!playerSkyMove.IsParachuteLoading && playerSkyMove.Parachute == null)
                        {
                            PlayerSkyMoveUtility.DelayLoadParachute(player, _contexts);
                            playerSkyMove.IsParachuteLoading = true;
                        }

                        if (playerSkyMove.Parachute != null)
                        {
                            if ((playerSkyMove.MoveStage == (int) SkyMoveStage.Parachuting))
                            {
                                if (!playerSkyMove.IsParachuteAttached)
                                {
                                    PlayerSkyMoveUtility.AttachParachute(_contexts, player, true);
                                }
                                else
                                {
                                    playerSkyMove.Parachute.transform.position = playerSkyMove.Position;
                                    playerSkyMove.Parachute.transform.rotation = playerSkyMove.Rotation;
                                }

                                playbackPlayer = false;
                            }else if (playerSkyMove.Parachute.gameObject.activeSelf)
                            {
                                playerSkyMove.Parachute.gameObject.SetActive(false);
                            }
                        }
                    }

                    if (playerSkyMove.IsParachuteAttached &&
                        playerSkyMove.MoveStage != (int)SkyMoveStage.Parachuting)
                    {
                        PlayerSkyMoveUtility.DetachParachute(_contexts, player);
                    }
                }

                var playerRoot = player.RootGo();

                if (playbackPlayer)
                {
                    playerRoot.transform.SetPositionAndRotation(player.position.Value, player.orientation.ModelView);
                    
                }
               

            
        }

    }
}