using UIComponent.UI.Manager;
using UIComponent.UI.Manager.Alert;

namespace Assets.App.Client.GameModules.Ui
{
    public class UiCommon
    {
        public static UIManager UIManager;
        public static TipManager TipManager;
        public static AlertManager AlertManager;
        public static void InitUI(string uiRootName)
        {
            UIManager = new UIManager(uiRootName);
            UIManager.CreateUICamera();
            UIManager.RootRenderMode(UnityEngine.RenderMode.ScreenSpaceCamera, UIManager.UICamera);
            TipManager = UIManager.GetTipManager();
            AlertManager = UIManager.GetAlertManager();
            //AlertManager.SetStyle<BattleAlertUIModel>("CommonNotice");
        }
    }
}