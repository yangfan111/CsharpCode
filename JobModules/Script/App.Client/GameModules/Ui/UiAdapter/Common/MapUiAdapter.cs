using Core.Enums;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{
    public class MapUIAdapter : MiniMapUiAdapter, IMapUiAdapter
    {
        private Contexts _contexts;

        public MapUIAdapter(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        public override bool Enable
        {
            get { return base.Enable && !IsDead(); } 

        }

        private PlayerEntity _player;

        private PlayerEntity Player
        {
            get { return _player ?? (_player = _contexts.player.flagSelfEntity); }
        }

        private bool IsDead()
        {
            return Player.gamePlay.IsDead();
        }

        public string ChannelName
        {
            get
            {
                if (_contexts.session.commonSession.RoomInfo.ChannelName == "null") return null;
                return _contexts.session.commonSession.RoomInfo.ChannelName;
            }
        }

        public string RoomName
        {
            get
            {
                if (_contexts.session.commonSession.RoomInfo.RoomDisplayId.Equals(0)) return null;
                return string.Format("{0}号房间", _contexts.session.commonSession.RoomInfo.RoomDisplayId);
            }
        }

        public int PlayerCount
        {
            get { return _contexts.ui.uI.PlayerCount; }
        }

        public int PlayerCapacity
        {
            get { return _contexts.session.commonSession.RoomInfo.RoomCapacity; }
        }
        public EUICampType MyCamp
        {
            get
            {
                PlayerEntity myEntity = _contexts.player.flagSelfEntity;
                if (null != myEntity)
                {
                    return (EUICampType)myEntity.playerInfo.Camp;
                }

                return EUICampType.None;
            }
        }

        public string GetWinConditionDescription()
        {
            var modeId = _contexts.session.commonSession.RoomInfo.ModeId;
            if (modeId.Equals(0)) return string.Empty;
            var conditionType = _contexts.session.commonSession.RoomInfo.ConditionType;
            var mode = SingletonManager.Get<GameModeConfigManager>().GetConfigById(modeId);
            if (mode.RuleId.Count <= 0) return string.Empty;
            var des = SingletonManager.Get<GameRuleConfigManager>()
                .GetCoditionDescriptionByIdAndType(mode.RuleId[0], conditionType);
            return des;
        }
        public string GetModeDescription()
        {
            var myCamp = MyCamp;
            var modeId = _contexts.session.commonSession.RoomInfo.ModeId;
            if (EUICampType.None.Equals(myCamp) || modeId.Equals(0)) return string.Empty;
            var config = SingletonManager.Get<GameModeConfigManager>().GetConfigById(modeId);
            if (EUICampType.T.Equals(myCamp))
            {
                return config.TeamBDescription;
            }
            else
            {
                return config.TeamADescription;
            }
        }
    }
}
