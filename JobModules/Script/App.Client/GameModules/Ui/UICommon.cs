using UIComponent.UI.Manager;

namespace Assets.App.Client.GameModules.Ui
{
    public class UiCommon
    {
        public static UIManager UIManager;
        public static TipManager TipManager;

        public static void InitUI(string uiRootName)
        {
            UiCommon.UIManager = new UIManager(uiRootName);
            UiCommon.UIManager.CreateUICamera();
            UiCommon.UIManager.RootRenderMode(UnityEngine.RenderMode.ScreenSpaceCamera, UiCommon.UIManager.UICamera);
            UiCommon.TipManager = UiCommon.UIManager.GetTipManager();
        }
    }
}