using System;
using App.Client.GameMode;
using App.Client.GameModules.Room;
using App.Client.Scripts;
using App.Protobuf;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Components.Serializer;
using Core;
using Core.GameModule.Interface;
using Core.Network;
using Core.Room;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.ClientInit
{
    public class RequestRoomInfoSystem : IInitializeSystem, INetworkMessageHandler, IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RequestRoomInfoSystem));
        
        private ISessionState _sessionState;
      
        
        private Contexts _contexts;

        private float _lastSendTime;

        public RequestRoomInfoSystem(
            Contexts contexts,
            ISessionState sessionState)
        {
            _contexts = contexts;
          
            _sessionState = sessionState;
            _contexts.session.clientSessionObjects.MessageDispatcher.RegisterLater((int)EServer2ClientMessage.LoginSucc, this);
            _sessionState.CreateExitCondition(typeof(RequestRoomInfoSystem));
        }

    
        private bool _loginSuccReceived;
        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            switch (messageType) {
                case (int)EServer2ClientMessage.LoginSucc:
                    LoginSuccMessage msg = (LoginSuccMessage)messageBody;

                    _contexts.session.clientSessionObjects.GameRule = msg.GameRule;
                    _contexts.session.commonSession.RoomInfo.FromLoginSuccMsg(msg);
                    _contexts.session.commonSession.SessionMode = ModeUtil.CreateClientMode(_contexts, _contexts.session.commonSession.RoomInfo.ModeId);
                    //_contexts.session.commonSession.WeaponModeLogic =  ClientGameModeLogicFactoryManager.GetModeLogicFactory(_contexts, _contexts.session.commonSession.RoomInfo.ModeId).CreateWeaponModeLogic();

                    _loginSuccReceived = true;
                    break;
            }
            if ( _loginSuccReceived)
            {
                _sessionState.FullfillExitCondition(typeof(RequestRoomInfoSystem));
            }
        }

        public void Initialize()
        {

            if (SharedConfig.IsOffline)
            {
                ArtModule.ArtModule.Initialize(SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig);

                _contexts.session.clientSessionObjects.GameRule = GameRules.Offline;
                
                _contexts.session.commonSession.RoomInfo.MapId = SingletonManager.Get<ClientFileSystemConfigManager>().BootConfig.MapId;
               
                _contexts.session.commonSession.SessionMode = ModeUtil.CreateClientMode(_contexts, SharedConfig.ModeId);
               

                
                
                _sessionState.FullfillExitCondition(typeof(RequestRoomInfoSystem));
            }
        }

        public void Execute()
        {
            var channel =  _contexts.session.clientSessionObjects.NetworkChannel;
            var token =  _contexts.session.clientSessionObjects.LoginToken;
            if (channel != null && channel.IsConnected && !_loginSuccReceived)
            {
                //send to server every 1 second.
                var currentTime = Time.time;
                if (currentTime - _lastSendTime > 1.0f)
                {
                    var message = LoginMessage.Allocate();
                    message.Token = token;
                    message.LoginStage = ELoginStage.PreLogin;
                    message.ComponentSerializerVersion = ComponentSerializerManager.HashMd5;
                    channel.SendReliable((int)EClient2ServerMessage.Login, message);

                    _logger.InfoFormat("Sending Login Message {0}", message);
                    message.ReleaseReference();

                    _lastSendTime = currentTime;
                }
            }
        }

    }
}