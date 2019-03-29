using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.Utility;
using App.Shared.Components.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.AssetManager;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonWeaponBagTipModel : ClientAbstractModel, IUiSystem
    {

        private CommonWeaponBagTipViewModel _viewModel = new CommonWeaponBagTipViewModel();
        private IWeaponBagTipUiAdapter _weaponBagState;

        //TODO 空槽位可能会有特殊图标代替
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonWeaponBagTipModel(IWeaponBagTipUiAdapter weaponBagState):base(weaponBagState)
        {
            _weaponBagState = weaponBagState;
        }
    }
}


