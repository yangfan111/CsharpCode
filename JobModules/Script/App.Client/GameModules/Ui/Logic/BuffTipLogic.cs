using System;
using App.Shared.Components;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using App.Client.CastObjectUtil;
using Assets.XmlConfig;
using Assets.Utils.Configuration;
using App.Shared.Util;
using UnityEngine;
using App.Shared;

namespace App.Client.GameModules.Ui.Logic
{
    public class BuffTipLogic : IBuffTipLogic
    {
        private IUserCmdGenerator _userCmdGenerator;
        private PlayerContext _playerContext;
        private UiContext _uiContext;

        public string StateTip
        {
            get { return _uiContext.uI.BuffTip; }
        }

        public BuffTipLogic(PlayerContext playerContext, UiContext uiContext, IUserCmdGenerator cmdGenerator)
        {
            _playerContext = playerContext;
            _userCmdGenerator = cmdGenerator;
            _uiContext = uiContext;
        }

        public void Action()
        {
        }

        public bool HasTipState()
        {
            return _uiContext.uI.ShowBuffTip && !string.IsNullOrEmpty(_uiContext.uI.BuffTip);
        }
    }
}