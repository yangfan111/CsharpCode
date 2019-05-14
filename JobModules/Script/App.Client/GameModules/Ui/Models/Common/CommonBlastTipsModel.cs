using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Loxodon.Framework.ViewModels;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonBlastTipsModel : ClientAbstractModel, IUiHfrSystem
    {

        enum ABType
        {
            A,
            B,
            C4
        }


        private IBlastTipsUiAdapter adapter = null;

        private CommonBlastTipsViewModel _viewModel = new CommonBlastTipsViewModel();
        private bool isGameObjectCreated;
        private RectTransform rootRect;



        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonBlastTipsModel(IBlastTipsUiAdapter adapter) : base(adapter)
        {
            this.adapter = adapter;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGui();
            InitKeyBinding();
        }
        private void InitGui()
        {
            _viewModel.rootActiveSelf = true;
            rootRect = UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>();
        }

       
        public void RefreshGui()
        {
            if (!adapter.IsGameRulePass())
            {
                HideAll();
                return;
            }

            var comp = adapter.GetBlastData();
            UpdateAB(comp.BlastAPosition.ShiftedVector3(), ABType.A );
            UpdateAB(comp.BlastBPosition.ShiftedVector3(), ABType.B);
            if (adapter.IsCampPass() && comp.IsC4Droped)

                UpdateAB(comp.C4DropPosition.ShiftedVector3(), ABType.C4);
            else
                _viewModel.C4activeSelf = false;

            if (comp.C4SetStatus == 0)
            {
                _viewModel.iconredA = false;
                _viewModel.iconredB = false;
            }
            else if (comp.C4SetStatus==1)
            {
                _viewModel.iconredA = true;
                _viewModel.iconredB = false;
            }
            else if (comp.C4SetStatus == 2)
            {
                _viewModel.iconredA = false;
                _viewModel.iconredB = true;
            }
            else
            {
                
            }

            //Debug.Log("C4"+comp.IsC4Droped);
        }

        private void UpdateAB(Vector3 targetPos,ABType type)
        {


            if (InView(targetPos))
            {

                var selfPlayer = adapter.GetPlayerEntity();
                var playerTrans = selfPlayer.characterContoller.Value.transform;

                var distance = Mathf.FloorToInt(Vector3.Distance(targetPos, playerTrans.position));
                var mi = "m";
                

                var camera = selfPlayer.cameraObj.MainCamera;
                var result = UIUtils.WorldPosToRect(targetPos, camera, rootRect);

                switch (type)
                {
                    case ABType.A:
                        _viewModel.AactiveSelf = true;
                        _viewModel.AtitleText = distance + mi;
                        _viewModel.AUIPos = new Vector3(result.x, result.y, 0);
                        break;
                    case ABType.B:
                        _viewModel.BactiveSelf = true;
                        _viewModel.BtitleText = distance + mi;
                        _viewModel.BUIPos = new Vector3(result.x, result.y, 0);
                        break;
                    case ABType.C4:
                        _viewModel.C4activeSelf = true;
                        _viewModel.C4titleText = distance + mi;
                        _viewModel.C4UIPos = new Vector3(result.x, result.y, 0);
                        break;
                    default: break;
                }
                

            }
            else
            {
                switch (type)
                {
                    case ABType.A:
                        _viewModel.AactiveSelf = false;
                        break;
                    case ABType.B:
                        _viewModel.BactiveSelf = false;
                        break;
                    case ABType.C4:
                        _viewModel.C4activeSelf = false;
                        break;
                    default: break;
                }
                
            }

        }

       

        public override void Update(float interval)
        {
            base.Update(interval);
    
            RefreshGui();

        }


        private bool InView(Vector3 pos)
        {
            var cam = Camera.main;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(pos);
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InitKeyBinding()
        {
            //var receiver = new KeyReceiver(Layer.Ui, BlockType.None);
            
            //receiver.AddAction(UserInputKey.F1, (data) =>
            //{
            //  if (GetCanvasEnabled())
            //      SetUiState(false);
            //  else
            //      SetUiState(true);
            //});
            //adapter.RegisterKeyReceive(receiver);
        }

        void HideAll()
        {
            _viewModel.AactiveSelf = false;
            _viewModel.BactiveSelf = false;
            _viewModel.C4activeSelf = false;
        }
    }
}
