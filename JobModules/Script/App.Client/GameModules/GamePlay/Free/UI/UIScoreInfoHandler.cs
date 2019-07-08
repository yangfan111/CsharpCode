using System;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Assets.Utils.Configuration;
using Core.Enums;
using Core.Free;
using Core.Utils;
using Free.framework;
using Utils.Configuration;
using Utils.Singleton;
using AssetInfo = Utils.AssetManager.AssetInfo;

namespace App.Client.GameModules.GamePlay.Free.UI
{
    public class UIScoreInfoHandler : ISimpleMesssageHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UIScoreInfoHandler));

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ScoreInfo;
        }

        public void Handle(SimpleProto simpleProto)
        {
            var uidata = SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI;

            if (simpleProto.Ks[0] == 1)
            {
                uidata.JoinPlayerCount = simpleProto.Ins[0];
            }
            else if (simpleProto.Ks[0] == 2)
            {
                uidata.IsGameBegin = simpleProto.Bs[0];

                uidata.BeatCount = simpleProto.Ins[0];
                uidata.SurvivalCount = simpleProto.Ins[1];
            }
            else if (simpleProto.Ks[0] == 3)
            {
                if (simpleProto.Bs[0])
                {
                    //击杀信息
                    uidata.KillInfos.Add(CreateKillInfo(simpleProto.Ss[0], (long) simpleProto.Ds[0], simpleProto.Ins[0],
                        simpleProto.Ins[1], simpleProto.Ss[1], (long) simpleProto.Ds[1],
                        (EUIDeadType) simpleProto.Ins[3]));
                    uidata.KillMessageChanged = true;
                }

                //击杀反馈（只有自己显示）
                string killerName = simpleProto.Ss[0];
                int feedbackType = simpleProto.Ins[2];
                var player = uidata.Player;
                if (!player.hasPlayerInfo || !killerName.Equals(player.playerInfo.PlayerName)) return;
                Logger.Info("KillFeed:" + Convert.ToString(feedbackType, 2));

                for (var fbType = EUIKillFeedbackType.Normal; fbType <= EUIKillFeedbackType.Hit; fbType++)
                {
                    if (((feedbackType >> (int) fbType) & 1) == 1)
                    {
                        uidata.KillFeedBackList.Add((int) fbType);
                    }
                }
            }
        }

        public IKillInfoItem CreateKillInfo(string killerName, long killerTeamId, int killerWeaponId, int killType,
            string deadName, long deadTeamId, EUIDeadType deathType)
        {
            Logger.InfoFormat(
                "KillMessage:KillName={0},killerTeamId={1},killerWeaponId={2}, killType={3},deadName={4},deadTeamId={5},deathType={6}",
                killerName, killerTeamId, killerWeaponId, killType, deadName, deadTeamId, deathType);

            IKillInfoItem item = new KillInfoItem();
            item.createTime = SingletonManager.Get<FreeUiManager>().Contexts1.session.currentTimeObject.CurrentTime;
            item.killerName = killerName;
            item.killerTeamId = killerTeamId;
            item.killerWeaponId = killerWeaponId;
            item.killType = killType;
            item.deadName = deadName;
            item.deadTeamId = deadTeamId;
            item.deathType = deathType;

            var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(killerWeaponId);
            if (null != weaponConfig)
            {
                var config = SingletonManager.Get<WeaponAvatarConfigManager>().GetConfigById(weaponConfig.AvatorId);
                if (null != config)
                {
                    item.weaponAsset = new AssetInfo(config.IconBundle, config.KillIcon);
                }
            }

            return item;
        }
    }
}