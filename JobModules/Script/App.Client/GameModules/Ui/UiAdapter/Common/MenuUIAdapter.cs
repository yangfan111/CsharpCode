using App.Shared;
using App.Shared.Components.Player;
using App.Shared.Components.Ui;
using System;
using Assets.App.Client.GameModules.Ui;
using AlertWindowStyle = UIComponent.UI.Manager.Alert.AlertWindowStyle;
namespace App.Client.GameModules.Ui.UiAdapter
{
    public class MenuUiAdapter : UIAdapter, IMenuUiAdapter
    {
        private Contexts _contexts;
        public MenuUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        private int playStaue = 1;
        public int PlayStaue     //玩家状态  0 死亡  1 存活
        {
            get
            {
                //return playStaue;
                return _contexts.player.flagSelfEntity.gamePlay.LifeState;
            }

            set
            {
                playStaue = value;
            }
        }

        private bool warVictory = false;
        public bool WarVictory             //玩家胜利
        {
            get
            {
                return warVictory;
            }

            set
            {
                warVictory = value;
            }
        }

        //        public INoticeUiAdapter NoticeUIAdatper
        //        {
        //            get { return _contexts.ui.uiDataAdapter.NoticeUiInfo; }
        //        }

        

        public void SetInputManagerEnable(bool isEnabled)
        {
            _contexts.userInput.userInputManager.Mgr.SetEnable(isEnabled);
        }

        public void SetCrossVisible(bool isActive)
        {
            _contexts.ui.uI.IsShowCrossHair = isActive;
        }


        public void ShowNoticeWindow(string title, Action yesCallback, Action noCallback, string yesText, string noText)
        {
            UiCommon.AlertManager.AddDataToQueueAndShow(AlertWindowStyle.YESNO, title,
                yesCallback, noCallback, yesText, noText, 0, null);
            //_contexts.ui.uI.ShowNoticeWindow(NoticeWindowStyle.YESNO, title, yesCallback, noCallback, yesText, noText, 0, null);
        }

        public GamePlayComponent gamePlay
        {
            get
            {
                return _contexts.player.flagSelfEntity.gamePlay;
            }
        }

        public bool IsHXMod
        {
            get
            {
                bool res = SharedConfig.IsHXMod && _contexts.player.flagSelfEntity.gamePlay.ChickenHxWin;
                return res;
            }

        }
    }
}