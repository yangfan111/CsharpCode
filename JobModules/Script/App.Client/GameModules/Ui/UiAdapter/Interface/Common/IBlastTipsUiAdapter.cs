using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Common
{
    public interface IBlastTipsUiAdapter : IAbstractUiAdapter
    {
        PlayerEntity GetPlayerEntity();

        int GetGameRule();

        bool IsCampPass();

        bool IsGameRulePass();

        bool NeedShow();

        BlastComponent GetBlastData();
    }
}
