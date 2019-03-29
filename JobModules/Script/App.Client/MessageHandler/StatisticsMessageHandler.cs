using App.Protobuf;
using Assets.App.Client.GameModules.Ui;
using Core.Enums;
using Core.Network;
using Core.Statistics;

namespace App.Client.MessageHandler
{
    public class StatisticsMessageHandler : INetworkMessageHandler
    {
        private Contexts _contexts;
        public StatisticsMessageHandler(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void Handle(INetworkChannel networkChannel, int messageType, object messageBody)
        {
            var message = messageBody as BattleStatisticsMessage;
            if (null == message)
                return;

            PlayerEntity player = _contexts.player.flagSelfEntity;
            BattleData data = player.statisticsData.Battle;

            data.Killer.PlayerLv = message.Killer.PlayerLv;
            data.Killer.PlayerName = message.Killer.PlayerName;
            data.Killer.BackId = message.Killer.BackId;
            data.Killer.TitleId = message.Killer.TitleId;
            data.Killer.BadgeId = message.Killer.BadgeId;
            data.Killer.WeaponId = message.Killer.WeaponId;
            data.Killer.DeadType = (EUIDeadType)message.Killer.DeadType;

            foreach (var opponent in message.Opponents)
            {
                OpponentBattleInfo info = new OpponentBattleInfo();
                info.PlayerLv = opponent.PlayerLv;
                info.PlayerName = opponent.PlayerName;
                info.BackId = opponent.BackId;
                info.TitleId = opponent.TitleId;
                info.BadgeId = opponent.BadgeId;
                info.WeaponId = opponent.WeaponId;
                info.IsKill = opponent.IsKill;
                info.Damage = opponent.Damage;
                data.OpponentList.Add(info);
            }

            //player.statisticsData.IsShow = true;
            _contexts.ui.uISession.UiState[UiNameConstant.CommonTechStatModel] = true;
        }
    }
}
