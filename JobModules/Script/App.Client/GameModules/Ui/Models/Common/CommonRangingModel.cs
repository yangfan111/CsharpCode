using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using UnityEngine;
using UserInputManager.Lib;
using App.Client.GameModules.Ui.Utils;
using DG.Tweening;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Core.GameModule.Interface;
using Utils.Appearance;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonRangingModel : ClientAbstractModel , IUiHfrSystem
    {
        private IRangingUiAdapter adapter = null;
        private bool isGameObjectCreated = false;
        private Tween disappearTween = null;
        private RectTransform uiPos = null;
        private float disappearTime = 5;
        private RectTransform rootRect;
        private Vector3 contactPos;

        private CommonRangingViewModel _viewModel = new CommonRangingViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonRangingModel(IRangingUiAdapter adapter):base(adapter)
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
            _viewModel.rootActiveSelf = false;
            uiPos = FindComponent<RectTransform>("bg");
            rootRect = UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>();
        }

        //OnUiRender
        public void RefreshGui()
        {
            if (isGameObjectCreated)
            {
                var info = adapter.RangeInfo; 
                if (info != null)
                {
                    if (InView(info.contactPos))
                    {
                        _viewModel.rootActiveSelf = true;

                        //刷新距离
                        var distance = info.distance > 999 ? 999 : info.distance;
                        _viewModel.titleText = distance + I2.Loc.ScriptLocalization.client_common.word17;

                        contactPos = info.contactPos;
                        if (disappearTween != null)
                        {
                            disappearTween.Kill();
                        }
                        //开始倒计时
                        disappearTween = UIUtils.CallTween(1, 0,
                            null,
                            (value) =>
                            {
                                disappearTween = null;
                                _viewModel.rootActiveSelf = false;
                            },
                            disappearTime);
                    }
                    else
                    {
                        Debug.Log("out in view");
                    }
                    adapter.RangeInfo = null;
                }
            }
        }

        public override void Update(float interval)
        {
            base.Update(interval);
            UdapteMarkPos();
        }

        private void UdapteMarkPos()
        {
            if (disappearTween == null) return;

            if (InView(contactPos))
            {
                _viewModel.rootActiveSelf = true;
                var selfPlayer = adapter.GetPlayerEntity();
                var camera = selfPlayer.cameraObj.MainCamera;
               
                Vector2 result = UIUtils.WorldPosToRect(contactPos, camera, rootRect);
                uiPos.anchoredPosition3D = new Vector3(result.x, result.y, 0);
            }
            else
            {
                _viewModel.rootActiveSelf = false;
            }
        }

        private bool InView(Vector3 pos)
        {
            var cam = Camera.main;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(pos);
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);//判断物体是否在相机前面
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
               // Debug.Log("in view");
                return true;
            }
            else
            {
                //Debug.Log("out in view");
                return false;
            }
        }

        private void InitKeyBinding()
        {
            var receiver = new KeyReceiver(UiConstant.rangingWindowLayer, BlockType.None);
            receiver.AddAction(UserInputKey.CheckRanging, (data) =>
            {
               
                Debug.Log("CheckRanging..........................");
                var selfPlayer = adapter.GetPlayerEntity();
                if (selfPlayer.gamePlay.IsObserving())
                    return;
                var playerTrans = selfPlayer.characterContoller.Value.transform;

                var cameraPos = selfPlayer.cameraObj.MainCamera.transform.position;
                var cameraForword = selfPlayer.cameraObj.MainCamera.transform.forward;

                RaycastHit hit;
                int bricksLayer = LayerMask.NameToLayer("Player");  //不检测player层级的东西
                int layerMask = 1 << bricksLayer;
                layerMask = ~layerMask;
                if (Physics.Raycast(cameraPos, cameraForword, out hit, int.MaxValue, layerMask))
                {
                    var hitPos = hit.point;
                    var distance = Vector3.Distance(hitPos, playerTrans.position);
                    distance = Mathf.Max(distance, 1);
                    //Debug.Log("name:" + hit.collider.gameObject.name);

                    if (adapter.RangeInfo == null)
                    {
                        adapter.RangeInfo = new RangingInfo((long)distance, hitPos);
                    }
                    else
                    {
                        adapter.RangeInfo.distance = (long)distance;
                        adapter.RangeInfo.contactPos = hitPos;                         
                    }
                }
                RefreshGui();
            });
            adapter.RegisterKeyReceive(receiver);
        }
    }
}    
