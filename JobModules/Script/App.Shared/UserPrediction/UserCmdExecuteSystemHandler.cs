using System.Collections.Generic;
using System.Linq;
using Core.GameModule.System;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Entitas;

namespace App.Server
{
    public class UserCmdExecuteSystemHandler : IUserCmdExecuteSystemHandler
    {
        private Contexts _serverContexts;
        private List<IUserCmdOwner> _userCmdOwners = new List<IUserCmdOwner>();
        private PlayerEntity[] _lastPlayerEntities;
        private IGroup<PlayerEntity> _players;
        public UserCmdExecuteSystemHandler(Contexts serverContexts)
        {
            _serverContexts = serverContexts;
            _players = serverContexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.FirstPersonModel, PlayerMatcher.ThirdPersonModel,
                PlayerMatcher.State, PlayerMatcher.HitBox));
        }


        public List<IUserCmdOwner> UserCmdOwnerList
        {
            get
            {
                var vehicleEntities = _players.GetEntities();
                if (_lastPlayerEntities != vehicleEntities)
                {
                    _lastPlayerEntities = vehicleEntities;
                    _userCmdOwners.Clear();
                    int count = vehicleEntities.Length;
                    for (int i = 0; i < count; i++)
                    {
                        _userCmdOwners.Add(vehicleEntities[i].userCmdOwner.OwnerAdater);
                    }
                }
                return _userCmdOwners;
            }
        }
    }
}