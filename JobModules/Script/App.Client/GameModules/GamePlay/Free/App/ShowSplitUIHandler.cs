using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui;
using App.Shared.Components.Ui;
using Utils.Singleton;

namespace Assets.App.Client.GameModules.GamePlay.Free.App
{
    class ShowSplitUIHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return FreeMessageConstant.ShowSplitUI == key;
        }

        public void Handle(SimpleProto data)
        {
            int cat = data.Ins[0];
            int id = data.Ins[1];
            int count = data.Ins[2];
            string name = data.Ss[1];
            string key = data.Ss[0];

            if (count > 1)
            {
                SingletonManager.Get<FreeUiManager>().Contexts1.ui.uI.SplitPropInfo = new SplitPropInfo(cat, id, count, name, key);
            }
        }
    }
}
