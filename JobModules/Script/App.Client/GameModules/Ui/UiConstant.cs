namespace App.Client.GameModules.Ui
{
    using UserInputManager.Lib;

    internal static class UiConstant
    {
        public const string UiConfigBundleName = "uiconfig";

        //各种操作的层级
        public const Layer userCmdKeyLayer = Layer.Env;
        public const Layer userCmdUIKeyLayer = Layer.Ui;
        public const Layer userCmdPointLayer = Layer.Env + 2;

        //左手攻击和右手攻击的层级
        public const Layer specicalCmdKeyLayer = Layer.Env + 2;

        //菜单界面
        public const Layer menuWindowLayer = Layer.Env + 16;
        public const Layer menuWindowKeyBlockLayer = Layer.Top;
        public const Layer menuWindowPointBlockLayer = Layer.Top;

        //大地图界面
        public const Layer maxMapWindowLayer = Layer.Env;
        public const Layer maxMapWindowKeyBlockLayer = Layer.Env + 1;
        public const Layer maxMapWindowPointBlockLayer = Layer.Env + 1;

        //通用 确认框界面
        public const Layer noticeWindowLayer = Layer.Env + 6;
        public const Layer noticeWindowKeyBlockLayer = Layer.Ui - 1;
        public const Layer noticeWindowPointBlockLayer = Layer.Ui - 1;

        //拆分道具界面
        public const Layer splitWindowLayer = Layer.Env + 6;
        public const Layer splitWindowKeyBlockLayer = Layer.Ui - 1;
        public const Layer splitWindowPointBlockLayer = Layer.Ui - 1;


        //聊天界面
        public const Layer chatWindowLayer = Layer.Env + 6;
        public const Layer chatWindowKeyBlockLayer = Layer.Ui - 1;
        public const Layer chatWindowPointBlockLayer = Layer.Ui - 1;


        //测距离界面
        public const Layer rangingWindowLayer = Layer.Env + 6;

        public const Layer weaponBagWindowLayer = Layer.Env;
        public const Layer weaponBagWindowKeyBlockLayer = Layer.Env + 1;

        //喷漆
        public const Layer paintWindowLayer = Layer.Env + 6;
        public const Layer paintWindowKeyBlockLayer = Layer.Env + 1;
        public const Layer paintWindowPointBlockLayer = Layer.Env + 1;
    }

}
