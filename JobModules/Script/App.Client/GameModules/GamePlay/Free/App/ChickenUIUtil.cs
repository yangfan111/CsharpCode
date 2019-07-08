using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Assets.Sources.Free.UI;
using com.cpkf.yyjd.tools.util;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.App
{
    public class ChickenUIUtil
    {
        public static void ShowBottomTip(string msg)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            if(!StringUtil.IsNullOrEmpty(msg))
            {
                contexts.ui.uISession.UiState[UiNameConstant.CommonOperationTipModel] = true;
                contexts.ui.uI.OperationTipData = new BaseTipData
                {
                    Title = msg,
                    DurationTime = 5000
                };
            }
        }

        public static void ShowTopTip(string msg)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            if(!StringUtil.IsNullOrEmpty(msg))
            {
                contexts.ui.uISession.UiState[UiNameConstant.CommonSystemTipModel] = true;
                contexts.ui.uI.SystemTipDataQueue.Enqueue(new BaseTipData
                {
                    Title = msg,
                    DurationTime = 5000
                });
            }
        }
    }
}
