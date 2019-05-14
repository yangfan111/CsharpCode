using System.Collections.Generic;
using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PlayerInfoUIAdapter : UIAdapter, IPlayerInfoUIAdapter
    {
        private Contexts _contexts;
        private PlayerEntity _playerEntity;
        private Shared.Components.Ui.UIComponent _ui;

       
        public List<ITipData> CountdownTipDataList
        {
            get { return _contexts.ui.uI.CountdownTipDataList; }
        }

        public PlayerInfoUIAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _playerEntity = _contexts.player.flagSelfEntity;
        }

        public List<MiniMapTeamPlayInfo> TeamPlayerInfos
        {
            get
            {
                return _contexts.ui.map.TeamInfos;
            }
        }

        public int GetGameRule()
        {
            return _contexts.session.commonSession.RoomInfo.ModeId;
        }

        public int CurrentHp          //非受伤状态的当前血量
        {
            get
            {
                if (_playerEntity != null)
                {
                    return (int)_playerEntity.gamePlay.CurHp;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int MaxHp
        {
            get
            {
                if (_playerEntity != null)
                    return _playerEntity.gamePlay.MaxHp;
                else
                {
                    return 0;
                }
            }
        }

        public int HpPercent
        {
            get
            {
                if (MaxHp == 0)
                    return 0;
                return Mathf.CeilToInt((CurrentHp * 1f / MaxHp)*100);
            }
        }
        
    }
}