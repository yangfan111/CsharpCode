using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Free;
using App.Client.GameModules.Ui.System;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Module;
using Core.SessionState;
using UIComponent.UI.Manager;
using UnityEngine;

namespace App.Client.GameModules.Ui
{
    public class UiLoadModule : GameModule
    {
        public UiLoadModule(ISessionState sessionState, Contexts contexts)
        {
//            var uiRoot = GameObject.Find("ClientUIRoot");
//            UiCommon.UIManager = new UIManager("ClientUIRoot");
//            UiCommon.TipManager = UiCommon.UIManager.GetTipManager();

            var loader = new UiResourceLoader(contexts.session.commonSession.AssetManager);
            AbstractModel.SetUiResourceLoader(loader);
            AddSystem(new UiLoadSystem(sessionState, contexts));
        }
    }
}
