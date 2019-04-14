using App.Client.GameModules.GamePlay.Free.UI;
using Assets.Sources.Free.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.util;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ChickenUIUtil
    {
        public static void ShowBottomTip(string msg)
        {
            SimpleFreeUI ui = SingletonManager.Get<FreeUiManager>().GetUi("downTipUI");
            if(ui != null && !StringUtil.IsNullOrEmpty(msg))
            {
                FreePrefabComponent txt = (FreePrefabComponent)ui.GetComponent(0);
                txt.SetFieldValue("Content", msg);

                ui.Show(5000);
            }
        }

        public static void ShowTopTip(string msg)
        {
            SimpleFreeUI ui = SingletonManager.Get<FreeUiManager>().GetUi("upTipUI");
            if (ui != null && !StringUtil.IsNullOrEmpty(msg))
            {
                FreePrefabComponent txt = (FreePrefabComponent)ui.GetComponent(0);
                txt.SetFieldValue("Content", msg);

                ui.Show(5000);
            }
        }
    }
}
