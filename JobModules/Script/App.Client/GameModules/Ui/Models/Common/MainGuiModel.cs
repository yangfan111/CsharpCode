using System.Collections;
using System.Collections.Generic;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared;
using App.Shared.WeaponLogic;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class MainGuiModel : AbstractModel, IUiSystem
    {
        private static  readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MainGuiModel));
        private MainGuiViewModel _viewModel = new MainGuiViewModel();
        private PlayerContext _playerContext;
        private Contexts _contexts;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public MainGuiModel(Contexts contexts)
        {
            _playerContext = contexts.player;
            _contexts = contexts;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            _viewModel.ShowGameObjectActiveSelf = true;
        }

       public void OnUiRender(float interval)
        {
            if (!_viewModel.ShowGameObjectActiveSelf)
            {
                return;
            }

            var player = _playerContext.flagSelfEntity;
            if (null == player)
            {
                Logger.Error("My player entity is null !");
                return;
            }

            var curHp = player.gamePlay.CurHp;
            var maxHp = player.gamePlay.MaxHp;
            _viewModel.HpNum = string.Format("{0}/{1}", curHp, maxHp);
            _viewModel.BulletCount = player.GetCurrentWeaponInfo(_contexts).Bullet.ToString();
        }
    }
}