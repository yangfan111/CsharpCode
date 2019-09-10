using System.Collections.Generic;
using System.Linq;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Entitas;

namespace App.Shared
{
    public class ServerServerUserCmdList : IServerUserCmdList
    {
        private List<IPlayerUserCmdGetter> _userCmdOwners = new List<IPlayerUserCmdGetter>();
        private PlayerEntity[] _lastPlayerEntities;
        private IGroup<PlayerEntity> _players;
        public ServerServerUserCmdList(Contexts serverContexts)
        {
            _players = serverContexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.FirstPersonModel, PlayerMatcher.ThirdPersonModel,
                PlayerMatcher.State, PlayerMatcher.HitBox));
        }


        public List<IPlayerUserCmdGetter> UserCmdOwnerList
        {
            get
            {
                var entities = _players.GetEntities();
                if (_lastPlayerEntities != entities)
                {
                    _lastPlayerEntities = entities;
                    _userCmdOwners.Clear();
                    int count = entities.Length;
                    for (int i = 0; i < count; i++)
                    {
                        _userCmdOwners.Add(entities[i].UserCmdController());
                    }
                }
                return _userCmdOwners;
            }
        }
    }
}