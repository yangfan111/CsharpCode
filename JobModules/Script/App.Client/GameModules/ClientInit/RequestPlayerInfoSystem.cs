using System;
using App.Client.GameModules.Room;
using App.Client.Scripts;
using App.Protobuf;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Configuration;
using App.Shared.EntityFactory;

using App.Shared.Player;
using Core;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Network;
using Core.Room;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Singleton;
using Vector3 = UnityEngine.Vector3;

namespace App.Client.GameModules.ClientInit
{
    public class RequestPlayerInfoSystem : IInitializeSystem, INetworkMessageHandler, IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RequestPlayerInfoSystem));

        private ISessionState _sessionState;

        private Contexts _contexts;
        public RequestPlayerInfoSystem(
            Contexts contexts,
            ISessionState sessionState)
        {
            _contexts = contexts;

            _sessionState = sessionState;
            _contexts.session.clientSessionObjects.MessageDispatcher.RegisterLater((int)EServer2ClientMessage.PlayerInfo, this);
            _sessionState.CreateExitCondition(typeof(RequestPlayerInfoSystem));

            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }


        private bool _loginSuccReceived;
        private bool _loginSended;
        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {

            if (messageType == (int)EServer2ClientMessage.PlayerInfo)
            {
                PlayerInfoMessage msg = (PlayerInfoMessage)messageBody;
                _logger.InfoFormat("recv PlayerInfo {0}", msg);
                PlayerInfo info = new PlayerInfo();
                info.ConvertFrom(msg);
                _loginSuccReceived = true;
                Vector3 position = new Vector3();
                var playerEntity = PlayerEntityFactory.CreateNewPlayerEntity(_contexts.player, position, info, true, false);
                playerEntity.ReplaceNetwork(networkChannel);

            }

        }

        public void Initialize()
        {

        }

        public void Execute()
        {
            if (SharedConfig.IsOffline)
            {
                if (!_loginSended)
                {
                    _loginSended = true;
                    var sessionObjects = _contexts.session.commonSession;
                    IEntityIdGenerator idGenerator = sessionObjects.EntityIdGenerator;
                    var id = idGenerator.GetNextEntityId();
                    var _offlineSelector = new OfflineSnapshotSelector(new EntityKey(id, (int)EEntityType.Player),
                        _contexts.session.commonSession.GameContexts);
                    _contexts.session.clientSessionObjects.SnapshotSelectorContainer.SnapshotSelector =
                        _offlineSelector;

                    _contexts.session.clientSessionObjects.SimulationTimer.CurrentTime = 0;
                    _contexts.session.entityFactoryObject.SceneObjectEntityFactory = new ServerSceneObjectEntityFactory(
                        _contexts.sceneObject, _contexts.player, sessionObjects.EntityIdGenerator, sessionObjects.EntityIdGenerator,
                        _contexts.session.currentTimeObject);
                    _contexts.session.entityFactoryObject.MapObjectEntityFactory =
                        new ServerMapObjectEntityFactory(_contexts.mapObject, sessionObjects.EntityIdGenerator);

                    IPlayerInfo playerInfo = TestUtility.CreateTestPlayer();
                    playerInfo.EntityId = id;
                    var player = PlayerEntityFactory.CreateNewPlayerEntity(_contexts.player,
                        SingletonManager.Get<MapsDescription>().SceneParameters.PlayerBirthPosition, playerInfo, true, false);
                    _offlineSelector.Init();
                    _sessionState.FullfillExitCondition(typeof(RequestPlayerInfoSystem));
                    SingletonManager.Get<SubProgressBlackBoard>().Step();
                }
            }
            else
            {
                var channel = _contexts.session.clientSessionObjects.NetworkChannel;
                var token = _contexts.session.clientSessionObjects.LoginToken;
                if (channel != null && channel.IsConnected)
                {
                    if (!_loginSended)
                    {
                        _loginSended = true;
                        var message = LoginMessage.Allocate();
                        message.Token = token;
                        message.LoginStage = ELoginStage.GetPlayerInfo;
                        channel.SendReliable((int)EClient2ServerMessage.Login, message);

                        _logger.InfoFormat("Sending Login Message {0}", message);
                        message.ReleaseReference();
                    }

                    if (_loginSuccReceived)
                    {
                        _sessionState.FullfillExitCondition(typeof(RequestPlayerInfoSystem));
                        SingletonManager.Get<SubProgressBlackBoard>().Step();
                    }
                }
            }
        }

    }
}