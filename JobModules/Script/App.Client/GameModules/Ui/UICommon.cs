using Assets.UiFramework.Libs;
using UIComponent.UI.Manager;
using UIComponent.UI.Manager.Alert;
using UnityEngine;

namespace Assets.App.Client.GameModules.Ui
{
    public class UiCommon
    {
        public static UIManager UIManager;
        public static TipManager TipManager;
        public static AlertManager AlertManager;
        public static void InitUI(string uiRootName, MonoBehaviour CoRoutine)
        {
            UIManager = new UIManager(uiRootName);
            UIManager.CreateUICamera();
            UIManager.RootRenderMode(UnityEngine.RenderMode.ScreenSpaceCamera, UIManager.UICamera);
            TipManager = UIManager.GetTipManager();
            AlertManager = UIManager.GetAlertManager(false);
            if (CoroutineManager.GetInstance() == null) new CoroutineManager(CoRoutine);

            if(StatsMonitor.StatsMonitor.Instance==null)
            {
                var Monitor = Resources.Load("Stats Monitor");
                Object.Instantiate(Monitor);
            }
           

        }
    }
}