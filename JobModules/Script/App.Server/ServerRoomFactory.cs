﻿using App.Client.GameModules.GamePlay.Free;
using App.Server.Scripts.Config;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core;
using Core.Room;
using Core.Utils;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Server
{
    public class ServerRoomFactory
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerRoomFactory));

        private RoomEventDispatcher _dispatcher;
        private ICoRoutineManager _coRouitneManager;
        private IUnityAssetManager _assetMananger;
        private IPlayerTokenGenerator _tokenGenerator;
        private Contexts _contexts;

        public ServerRoomFactory(RoomEventDispatcher dispatcher, ICoRoutineManager coRoutineManager, IUnityAssetManager assetMananger)
        {
            _dispatcher = dispatcher;
            _coRouitneManager = coRoutineManager;
            _assetMananger = assetMananger;
            _contexts = new Contexts();
        }

 

        public IPlayerTokenGenerator TokenGenerator
        {
            set { _tokenGenerator = value; }
        }

        public ServerRoom Create(RequestCreateRoomMessage message)
        {
            _logger.InfoFormat("Handle CreateRoom Message");
           var contextsServerWrapper = new ContextsServerWrapper(_contexts,_assetMananger,_coRouitneManager);
            var serverRoom = new ServerRoom(new RoomId(1), contextsServerWrapper, _dispatcher, _tokenGenerator);
            ResetEventDispacther();

            if (message != null)
            {
                HallRoom hallRoom = new HallRoom(_dispatcher, _contexts);
                hallRoom.HallRoomId = message.HallRoomId;
                hallRoom.ModeId = message.ModeId;
                hallRoom.TeamCapacity = message.TeamCapacity;
                hallRoom.MapId = message.MapId;
                hallRoom.RevivalTime = message.RevivalTime;
                hallRoom.MultiAngleStatus = message.MultiAngleStatus;
                hallRoom.WatchStatus = message.WatchStatus;
                hallRoom.HelpStatus = message.HelpStatus;
                hallRoom.HasFriendHarm = message.HasFriendHarm;
                hallRoom.WaitTimeNum = message.WaitTimeNum;
                hallRoom.OverTime = message.OverTime;
                hallRoom.ConditionValue = message.ConditionValue;
                hallRoom.ConditionType = message.ConditionType;
                hallRoom.ChannelName = message.ChannelName;
                hallRoom.RoomName = message.CustomRoomName;
                hallRoom.RoomCapacity = message.CustomRoomCapacity;
                hallRoom.RoomDisplayId = (int)message.CustomRoomDisplayId;
                hallRoom.AllowReConnect = message.AllowReConnect;
                hallRoom.Init();

                serverRoom.SetHallRoom(hallRoom);
                serverRoom.SetRoomInfo(hallRoom);
                serverRoom.SetGameMode(message.ModeId, message.MapId);
                
                _logger.InfoFormat("Create Room {0}", message);
            }
            else
            {
                HallRoom hallRoom = new DummyHallRoom(_dispatcher, _contexts);
                serverRoom.SetGameMode(RuleMap.GetRuleId(SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.Rule), SingletonManager.Get<ServerFileSystemConfigManager>().BootConfig.MapId);
                //hallRoom.Init(); //本地测试无需发战报
                serverRoom.SetHallRoom(hallRoom);
            }
            return serverRoom;
        }

        private void ResetEventDispacther()
        {
            _contexts.session.serverSessionObjects.RoomEventDispatchter = _dispatcher;
        }

        public Contexts contexts()
        {
            return _contexts;
        }
    }
}
