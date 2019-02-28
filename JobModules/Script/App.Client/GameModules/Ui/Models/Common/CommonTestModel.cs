using System;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Protobuf;
using App.Shared;
using App.Shared.Components.ClientSession;
using App.Shared.Configuration;
using Assets.UiFramework.Libs;
using Core.Configuration.Equipment;
using Core.GameModule.Interface;
using UserInputManager.Lib;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonTestModel : AbstractModel, IUiSystem
    {
        Contexts context;
        private bool isGameObjectCreated = false;
        private const string uiIconsBundleName = "ui/icons";


        private CommonTestViewModel _viewModel = new CommonTestViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonTestModel(Contexts contexts)
        {
            this.context = contexts;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
        }
       public void OnUiRender(float interval)
        {
            RefreshGui();
        }

        private void InitGui()
        {

        }

        private void RefreshGui()
        {
            if (context != null && isGameObjectCreated)
            {
                var curPos = context.player.flagSelfEntity.position;
                if (curPos != null)
                {
                    var curPosValue = curPos.Value;
                    _viewModel.posText = "x: " + (int)curPosValue.x +"   y: " + (int)curPosValue.y + "   z: "+ (int)curPosValue.z;
                }
            }
        }
    }
}    
