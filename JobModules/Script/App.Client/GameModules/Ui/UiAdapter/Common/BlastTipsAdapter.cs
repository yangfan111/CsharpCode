using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Shared.Components;
using App.Shared.Components.Ui;
using Core.Enums;

namespace Assets.App.Client.GameModules.Ui.UiAdapter.Common
{
    public class BlastTipsAdapter : UIAdapter, IBlastTipsUiAdapter
    {
        private Contexts _contexts;

        public BlastTipsAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public BlastComponent GetBlastData()
        {
           return _contexts.ui.blast;
        }

        public int GetGameRule()
        {
            return _contexts.session.commonSession.RoomInfo.ModeId;
        }

        public PlayerEntity GetPlayerEntity()
        {
            return _contexts.ui.uI.Player;
        }

        public bool IsCampPass()
        {
            var player = GetPlayerEntity();
            var camp = player.playerInfo.Camp;
            return camp == (int) EUICampType.T;
        }

        public bool IsGameRulePass()
        {
            return GameRules.IsBomb(GetGameRule());
        }

        public bool NeedShow()
        {
            return IsGameRulePass() && IsCampPass();
        }
    }
}
