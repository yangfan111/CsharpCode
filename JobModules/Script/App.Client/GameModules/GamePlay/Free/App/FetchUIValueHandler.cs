using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Core.Enums;
using Core.Room;
using Utils.Singleton;
using Assets.Sources.Free.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace App.Client.GameModules.GamePlay.Free.App
{
    class FetchUIValueHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.FetchUIValue;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            string uiKey = data.Ss[0];

            GameObject obj = UnityUiUtility.FindUIObject(uiKey);
            if(obj != null)
            {
                Image[] imgs = obj.GetComponentsInChildren<Image>();
                foreach(Image img in imgs)
                {
                    
                }
            }
        }
    }
}
