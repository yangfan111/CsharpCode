namespace App.Client.GameModules.Ui
{
    using UserInputManager.Lib;

    internal static class UiConstant
    {
        public const string UiConfigBundleName = "uiconfig";

        //各种操作的层级
        public const EInputLayer userCmdKeyLayer = EInputLayer.Env;
        public const EInputLayer userCmdUIKeyLayer = EInputLayer.Ui;
        public const EInputLayer userCmdPointLayer = EInputLayer.Env + 2;

        //左手攻击和右手攻击的层级
        public const EInputLayer specicalCmdKeyLayer = EInputLayer.Env + 2;

        //菜单界面
        public const EInputLayer menuWindowLayer = EInputLayer.Env + 16;
        public const EInputLayer menuWindowKeyBlockLayer = EInputLayer.Top;
        public const EInputLayer menuWindowPointBlockLayer = EInputLayer.Top;

        //大地图界面
        public const EInputLayer maxMapWindowLayer = EInputLayer.Env;
        public const EInputLayer maxMapWindowKeyBlockLayer = EInputLayer.Env + 1;
        public const EInputLayer maxMapWindowPointBlockLayer = EInputLayer.Env + 1;

        //通用 确认框界面
        public const EInputLayer noticeWindowxLayer = EInputLayer.Env + 6;
        public const EInputLayer noticeWindowKeyBlockLayer = EInputLayer.Ui - 1;
        public const EInputLayer noticeWindowPointBlockLayer = EInputLayer.Ui - 1;

        //拆分道具界面
        public const EInputLayer splitWindowLayer = EInputLayer.Env + 6;
        public const EInputLayer splitWindowKeyBlockLayer = EInputLayer.Ui - 1;
        public const EInputLayer splitWindowPointBlockLayer = EInputLayer.Ui - 1;


        //聊天界面
        public const EInputLayer chatWindowLayer = EInputLayer.Env + 6;
        public const EInputLayer chatWindowKeyBlockLayer = EInputLayer.Ui - 1;
        public const EInputLayer chatWindowPointBlockLayer = EInputLayer.Ui - 1;


        //测距离界面
        public const EInputLayer rangingWindowLayer = EInputLayer.Env + 6;

        public const EInputLayer weaponBagWindowLayer = EInputLayer.Env;
        public const EInputLayer weaponBagWindowKeyBlockLayer = EInputLayer.Env + 1;

        //喷漆
        public const EInputLayer paintWindowLayer = EInputLayer.Env + 6;
        public const EInputLayer paintWindowKeyBlockLayer = EInputLayer.Env + 1;
        public const EInputLayer paintWindowPointBlockxLayer = EInputLayer.Env + 1;

        public const EInputLayer recordWindowKeyBlockLayer = EInputLayer.Env + 15;
    }

}
