using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Room;
using Entitas;

namespace App.Server.Robot
{
    interface IRobotsManager
    {
        void Update();
    }
    class RobotsManager:IRobotsManager
    {
        private PlayerContext _player;
        private IGroup<PlayerEntity> _robotGroup;
        private IGroup<PlayerEntity> _humanGroup;
        private IRoomManager _roomManager;
        public RobotsManager(Contexts contexts, IRoomManager roomManager)
        {
            _roomManager = roomManager;
            _player = contexts.player;
            _humanGroup = _player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Network, PlayerMatcher.PlayerInfo)
                .NoneOf(PlayerMatcher.Robot));
            _robotGroup =
                _player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Network, PlayerMatcher.PlayerInfo,
                    PlayerMatcher.Robot));
        }

        public void Update()
        {
            if (_humanGroup.count == 0 && _robotGroup.count == 0)
            {
                foreach (var playerEntity in _robotGroup.GetEntities())
                {
                    playerEntity.network.NetworkChannel.Disconnect();
                }
            }

            if (_humanGroup.count > 0 && _robotGroup.count == 0)
            {
                foreach (var robotPlayerInfo in _roomManager.GetRobotPlayerInfos())
                {
                    _roomManager.RequestPlayerInfo(robotPlayerInfo.Token, new RobotFakeNetworkChannel());
                }
            }
        }

    }
}
