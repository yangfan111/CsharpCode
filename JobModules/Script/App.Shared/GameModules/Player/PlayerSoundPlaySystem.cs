using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Utils;
using System.Collections.Generic;
using App.Client.GameModules.Player;
using Entitas;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerSoundPlaySystem : AbstractGamePlaySystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerSoundPlaySystem));

        private Dictionary<EntityKey, bool> _inTransition = new Dictionary<EntityKey, bool>(EntityKeyComparer.Instance);
        private Dictionary<EntityKey, PostureInConfig> _lastPosture = new Dictionary<EntityKey, PostureInConfig>();


        public PlayerSoundPlaySystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.StateInterface));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnGamePlay(PlayerEntity player)
        {
            CheckPosture(player);
            CheckAction(player);
        }

        protected virtual void CheckAction(PlayerEntity player)
        {
        }

        protected virtual void CheckPosture(PlayerEntity player)
        {
            var curPosture = player.stateInterface.State.GetCurrentPostureState();
            var nextPosture = player.stateInterface.State.GetNextPostureState();
            if (_lastPosture.ContainsKey(player.entityKey.Value) && curPosture != _lastPosture[player.entityKey.Value])
            {
                if ((curPosture == PostureInConfig.Swim && _lastPosture[player.entityKey.Value] != PostureInConfig.Dive)
                    || (curPosture == PostureInConfig.Dive &&
                        _lastPosture[player.entityKey.Value] != PostureInConfig.Swim))
                {
                  //  player.soundManager.Value.PlayOnce(EPlayerSoundType.DropWater);
                }

                _lastPosture[player.entityKey.Value] = curPosture;
            }

            if (curPosture == nextPosture)
            {
                _inTransition[player.entityKey.Value] = false;
                return;
            }

            if (_inTransition.ContainsKey(player.entityKey.Value) && _inTransition[player.entityKey.Value])
            {
                return;
            }

            //switch (curPosture)
            //{
            //    case PostureInConfig.Prone:
            //        switch (nextPosture)
            //        {
            //            case PostureInConfig.ProneToStand:
            //            case PostureInConfig.ProneToCrouch:
            //                player.soundManager.Value.PlayOnce(EPlayerSoundType.GetUp);
            //                _inTransition[player.entityKey.Value] = true;
            //                break;
            //        }

            //        break;
            //    case PostureInConfig.Stand:
            //    case PostureInConfig.Crouch:
            //        if (nextPosture == PostureInConfig.ProneTransit)
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.GetDown);
            //            _inTransition[player.entityKey.Value] = true;
            //        }

            //        break;
            //    case PostureInConfig.Land:
            //        if (nextPosture == PostureInConfig.Stand)
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.Land);
            //            _inTransition[player.entityKey.Value] = true;
            //        }

            //        break;
            //}
        }
    }
}