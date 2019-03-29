using System;
using System.Collections.Generic;
using Sharpen;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class ChatUiAdapter : UIAdapter, IChatUiAdapter
    {
        private static float _checkMaxDistance = 200;
        private Contexts _contexts;
        private List<long> _nearPlayerList = new AList<long>();
        private List<long> _campPlayerList = new AList<long>();

        public ChatUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }


        public bool IsSinglePlayer
        {
            get
            {
                return _contexts.session.commonSession.RoomInfo.TeamCapacity <= 1;
            }
        }

        public List<long> NearPlayerInfo
        {
            get
            {
                _nearPlayerList.Clear();
                PlayerEntity myEntity = _contexts.player.flagSelfEntity;
                if (null != myEntity)
                {
                    foreach (var playerEntity in _contexts.player.GetEntities())
                    {
                        if (playerEntity != myEntity && playerEntity.hasPlayerInfo &&
                            Vector3.Distance(myEntity.position.Value, playerEntity.position.Value) < _checkMaxDistance)
                        {
                            _nearPlayerList.Add(playerEntity.playerInfo.PlayerId);
                        }
                    }
                }
                return _nearPlayerList;
            }
        }



        public List<int> ChannelList
        {
            get
            {
                var modeId = _contexts.session.commonSession.RoomInfo.ModeId;
                var config = SingletonManager.Get<GameModeConfigManager>().GetConfigById(modeId);
                if (config != null)
                {
                    return config.Channel;
                }
                var list = new List<int>
                {
                    8,7,6,5
                    //10,9,6,5
                };
                return list;
            }
        }

        public int DefaultChannel
        {
            get
            {
                var modeId = _contexts.session.commonSession.RoomInfo.ModeId;
                var config = SingletonManager.Get<GameModeConfigManager>().GetConfigById(modeId);
                if (config != null)
                {
                    return config.DefaultChannel;
        }
                return 7;
            }
        }

        public bool HaveCorps
        {
            get { return false; }
        }

        public List<long> CampPlayerInfo
        {
            get
            {
                _campPlayerList.Clear();
                PlayerEntity myEntity = _contexts.player.flagSelfEntity;
                if (null != myEntity)
                {
                    foreach (var playerEntity in _contexts.player.GetEntities())
                    {
                        if (playerEntity != myEntity && myEntity.playerInfo.Camp == playerEntity.playerInfo.Camp)
                        {
                            _campPlayerList.Add(playerEntity.playerInfo.PlayerId);
                        }
                    }
                }
                return _campPlayerList;
            }
        }

        public long MyselfId
        {
            get
            {
                PlayerEntity myEntity = _contexts.player.flagSelfEntity;

                if(null != myEntity)
                {
                    return myEntity.playerInfo.PlayerId;
                }

                return 0;
            }
        }

        public EUIChatListState ChatListState
        {
            get { return _contexts.ui.chat.ChatListState; }
            set { _contexts.ui.chat.ChatListState = value; }
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }

        public Action<object> AddMessageAction
        {
            set { _contexts.ui.chat.AddChatMessageDataAction = value; }
            get { return _contexts.ui.chat.AddChatMessageDataAction; }
    }

        public Action<object> GetPersonalOnlineStatusCallback
        {
            get { return _contexts.ui.chat.GetPersonalOnlineStatusCallback; }
            set { _contexts.ui.chat.GetPersonalOnlineStatusCallback = value; }
}
    }
}
