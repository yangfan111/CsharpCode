using System.Collections.Generic;
using App.Shared.Components.Ui;
using App.Shared.Components.Ui.UiAdapter;
using Core.Enums;
using Core.Utils;
using AssetInfo = Utils.AssetManager.AssetInfo;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public class KillInfoItem : IKillInfoItem
    {
        public int createTime { get; set; }
        public string killerName { get; set; }
        public long killerTeamId { get; set; }
        public int killerWeaponId { get; set; }
        public int killType { get; set; }
        public string deadName { get; set; }
        public long deadTeamId { get; set; }
        public EUIDeadType deathType { get; set; }
        public AssetInfo weaponAsset { get; set; }
    }

    public class KillMessageUiAdapter : UIAdapter, IKillMessageUiAdapter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(KillMessageUiAdapter));

        private static int MaxKillInfoCount = 5;
        private static int KillInfoShowTime = 15000;

        private Contexts _contexts;

        public override bool IsReady()
        {
            return null != _contexts.player.flagSelfEntity;
        }

        public bool KillMessageChanged
        {
            get
            {
                return _contexts.ui.uI.KillMessageChanged;
            }
            set
            {
                _contexts.ui.uI.KillMessageChanged = value;
            }
        }


        public KillMessageUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void UpdateKillInfo()
        {

            for (int i = 0; i < KillInfos.Count; i++)
            {
                if (_contexts.session.currentTimeObject.CurrentTime - KillInfos[i].createTime > KillInfoShowTime)
                {
                    KillInfos.RemoveAt(i);
                    KillMessageChanged = true;
                }
            }

            while (KillInfos.Count >= MaxKillInfoCount)
            {
                KillInfos.RemoveAt(0);
            }
        }



        public List<IKillInfoItem> KillInfos
        {
            get
            {
                return _contexts.ui.uI.KillInfos;
            }
        }

        public long SelfTeamId
        {
            get { return _contexts.player.flagSelfEntity.playerInfo.TeamId; }
        }
    }
}
