using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Shared.Components;
using App.Shared.Components.Ui;
using App.Shared.Components.UserInput;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;
using Core.Ui;
using KinematicCharacterController;
using UserInputManager.Lib;

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
            return _contexts.player.flagSelfEntity;
        }

        public bool IsCampPass()
        {
            var player = GetPlayerEntity();
            var cam = player.playerInfo.Camp;
            return (cam == (int)EUICampType.T);
        }

        public bool IsGameRulePass()
        {
            return (GetGameRule() == GameRules.Bomb);
        }

        public bool NeedShow()
        {
            var player = GetPlayerEntity();
            var cam = player.playerInfo.Camp;
            var camNeedShow = (cam == (int)EUICampType.T);
            return (GetGameRule() == GameRules.Bomb) && camNeedShow;
        }
    }
}
