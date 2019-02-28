using App.Shared.Components.Ui;
using System;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IMenuUiAdapter : IAbstractUiAdapter
    {
        int PlayStaue { get; set;}
        bool WarVictory { get; set; }
//        INoticeUiAdapter NoticeUIAdatper { get; }
        
        void SetInputManagerEnable(bool isEnabled);
        void SetCrossVisible(bool isActive);
        void ShowNoticeWindow(string title, Action yesCallback, Action noCallback, string yesText, string noText);
    }
}
