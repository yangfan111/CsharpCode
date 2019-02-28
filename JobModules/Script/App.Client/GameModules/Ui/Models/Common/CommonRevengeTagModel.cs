using App.Client.GameModules.Ui.UiAdapter.Common;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonRevengeTagModel : ClientAbstractModel, IUiHfrSystem
    {
        private CommonRevengeTagViewModel _viewModel = new CommonRevengeTagViewModel();
        RevengeTagUiAdapter adapter;


        public CommonRevengeTagModel(RevengeTagUiAdapter adapter) : base(adapter)
        {
            this.adapter = adapter;
            _viewModel.Show = false;
        }
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();   
        }

        private void InitVariable()
        {
            camera = Camera.main;
            canvasSize = UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        }

        private string curKillerName = string.Empty;
        private Vector3 curKillerPos = Vector3.zero;
        public override void Update(float interval)
        {
            //UpdateName();
            UpdateState();
            UpdatePos();
        }
        private void UpdateState()
        {
            if (adapter.KillerChanged)
            {
                _beginTime = Time.time;
                needShow = true;
                adapter.KillerChanged = false;
            }
            if (Time.time - _beginTime > 8f)
            {
                needShow = false;
            }
        }

        private bool needShow;
        private float _beginTime;
        private void UpdatePos()
        {
            if (!needShow) { _viewModel.Show = false; return; }
            var newKillerPos = adapter.KillerTopPos;
            if (!curKillerPos.Equals(newKillerPos) || newKillerPos.Equals(Vector3.zero))
            {
                curKillerPos = newKillerPos;
                Vector2 position2D;
                if (ConvertTo2DPos(curKillerPos, out position2D))
                {
                    _viewModel.TagPos = position2D;
                    _viewModel.Show = true;
                }
                else
                {
                    _viewModel.Show = false;
                }
            }
        }

        private bool ConvertTo2DPos(Vector3 pos,out Vector2 pos2D)
        {
            var viewPortPos = camera.WorldToViewportPoint(pos);
            if (!InView(pos, viewPortPos))
            {
                pos2D = Vector2.zero;
                return false;
            }

            canvasSize = UiCommon.UIManager.GetCanvasSize();
            Vector2 viewPortRelative = new Vector2(viewPortPos.x - 0.5f, viewPortPos.y - 0.5f);
            Vector2 screenPos = new Vector2(viewPortRelative.x * canvasSize.x, viewPortRelative.y * canvasSize.y);
            pos2D = screenPos;
            return true;
        }

        //private void UpdateName()
        //{
        //    var newKillerName = adapter.KillerName;
        //    if (string.IsNullOrEmpty(newKillerName))
        //    {
        //        _viewModel.NameText = string.Empty;
        //        curKillerName = string.Empty;
        //        return;
        //    }
        //    if (!curKillerName.Equals(newKillerName))
        //    {
        //        curKillerName = newKillerName;
        //        _viewModel.NameText = curKillerName;
        //    }
        //}
        private Camera camera;
        private Vector2 canvasSize;

        private bool InView(Vector3 pos, Vector2 viewPos)
        {
            var cam = Camera.main;
            Vector3 dir = (pos - cam.transform.position).normalized;
            float dot = Vector3.Dot(cam.transform.forward, dir);//判断物体是否在相机前面
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                return true;
            else
                return false;
        }
    }

    
}
