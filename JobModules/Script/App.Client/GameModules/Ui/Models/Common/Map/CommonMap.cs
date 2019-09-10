using System;
using System.Collections.Generic;
using UnityEngine;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using App.Shared.GameModules.Player;
using Assets.App.Client.GameModules.Ui;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public class CommonMap : CommonMiniMap
    {
        public static int MaxZoom = 16;

        private Vector2 _centerPos = Vector2.zero;

        private KeyHandler keyReceive = null;
        private PointerKeyHandler _pointerKeyHandler = null;
        private Camera uiCamera = null;

        private bool _dragging;
        private Vector2 _dragStart = Vector2.zero; //拖拽变量 保存鼠标在mapbg的位置和 锚点的位置
        private Vector2 _dragingPos = Vector2.zero; //拖拽变量 保存鼠标在mapbg的位置和 锚点的位置

        public CommonMap(IMiniMapUiAdapter adapter) : base(adapter)
        {
//            adapter.Enable = false;
            if (UiCommon.UIManager.GetRootRenderMode().Equals(RenderMode.ScreenSpaceCamera))
            {
                uiCamera = UiCommon.UIManager.UICamera;
            }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            SetMapRoot();
            InitKeyBinding();
            CalculateRate();
            BindUIEventTrigger();

//            _mapGird1.InitGird();
            _safeMiniDis.Hide();

            
            //临时处理  多层canvas嵌套  会导致显示错误
            var graphicRaycaster = _viewModel.CommonMiniMap.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null) GameObject.Destroy(graphicRaycaster);
            var canvas = _viewModel.CommonMiniMap.GetComponent<Canvas>();
            if (canvas != null) GameObject.Destroy(canvas);
            mapNameRoot.gameObject.SetActive(false);
            bgRect.gameObject.SetActive(false);
        }

        protected override void CalculateRate()
        {
            miniMapWByPixel = rootRectTf.sizeDelta.x;
            miniMapHByPixel = rootRectTf.sizeDelta.y;
            rate = miniMapWByPixel / MiniMapRepresentWHByRice;
        }

        public void SetMapRoot()
        {
            float canavsW = root.parent.GetComponent<RectTransform>().rect.width;
            float canvasH = root.parent.GetComponent<RectTransform>().rect.height;
            var maskWh = canavsW > canvasH ? canvasH : canavsW;

            rootRectTf.anchorMin = new Vector2(0.5f, 0.5f);
            rootRectTf.anchorMax = new Vector2(0.5f, 0.5f);
            rootRectTf.anchoredPosition = new Vector2(0, 0);
            rootRectTf.pivot = new Vector2(0.5f, 0.5f);
            rootRectTf.sizeDelta = new Vector2(maskWh, maskWh);
            rootRectTf.anchoredPosition = Vector2.zero;

            var rect = _viewModel.BgRectTransform;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = rootRectTf.sizeDelta;
        }


        private void BindUIEventTrigger()
        {
            //添加拖拽功能
            List<EventTrigger.Entry> entries = new List<EventTrigger.Entry>();
            EventTrigger.Entry pointClick = new EventTrigger.Entry();
            pointClick.eventID = EventTriggerType.PointerDown;
            pointClick.callback.AddListener(OnPointerDown);
            entries.Add(pointClick);

            EventTrigger.Entry pointUpDrag = new EventTrigger.Entry();
            pointUpDrag.eventID = EventTriggerType.PointerUp;
            pointUpDrag.callback.AddListener(OnPointerUp);
            entries.Add(pointUpDrag);

            EventTrigger.Entry pointDrag = new EventTrigger.Entry();
            pointDrag.eventID = EventTriggerType.Drag;
            pointDrag.callback.AddListener(OnDrag);
            entries.Add(pointDrag);

            EventTrigger.Entry pointEndDrag = new EventTrigger.Entry();
            pointEndDrag.eventID = EventTriggerType.EndDrag;
            pointEndDrag.callback.AddListener(OnEndDrag);
            entries.Add(pointEndDrag);

            _viewModel.BgUIEventTriggerListener.triggers.AddRange(entries);
        }

        protected override float GetShowSize()
        {
            return adapter.CurMapSize;
        }

        public void InitKeyBinding()
        {
//            var handler = new Keyhandler(UiConstant.maxMapWindowLayer, BlockType.None);
//            handler.AddAction(UserInputKey.ShowMaxMap, (data) =>
//            {
//                if (root != null)
//                {
//                    ShowMap(!adapter.Enable);
//                }
//            });
//            //adapter.RegisterKeyReceive(handler);
//            adapter.RegisterOpenKey(handler);

            //            DynamicKeyReceive();
            _pointerKeyHandler = new PointerKeyHandler(UiConstant.maxMapWindowPointBlockLayer, BlockType.All);
            keyReceive = new KeyHandler(UiConstant.maxMapWindowKeyBlockLayer, BlockType.All);
            keyReceive.BindKeyAction(UserInputKey.ChangeMapRate, OnChangeMapRate);
            keyReceive.BindKeyAction(UserInputKey.AddMark, (data) =>
            {
                if (MapLevel.Min.Equals(adapter.MapLevel) == false)
                {
                    Vector2 markPos = adapter.CurPlayerPos;
                    adapter.SendMarkMessage(markPos);
                }
            });
            keyReceive.BindKeyAction(UserInputKey.MouseAddMark, (data) =>
            {
                Vector2 hitPoint = Vector2.zero;
                Vector2 hitPoint1 = Vector2.zero;
                if (MapLevel.Min.Equals(adapter.MapLevel) == false)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTf, Input.mousePosition, uiCamera, out hitPoint1))
                    {
//                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBgRectTran, Input.mousePosition, uiCamera, out hitPoint))
//                        {
                            var markPos = _centerPos + hitPoint1 / rate;
                            adapter.SendMarkMessage(markPos);
//                        }
                    }
                }
            });
            keyReceive.BindKeyAction(UserInputKey.LocationCurPlay, OnLocationCurPlay);

//            keyReceive.AddAction(UserInputKey.HideWindow, (data) =>
//            {
//                if (adapter.Enable)
//                {
//                    ShowMap(!adapter.Enable);
//                }
//            });
        }

        private void OnLocationCurPlay(KeyData data)
        {
            if (MiniMapRepresentWHByRice < OriginalMiniMapRepresentWHByRice) //缩放模式的时候
            {
                //将当前玩家尽量移动到地图中心 也就是移动背景图 
                var curPlayPosByPice = adapter.CurPlayerPos;
                Vector2 curPlayPosByPixel = (curPlayPosByPice - _centerPos) * rate;
                var offset = curPlayPosByPixel - Vector2.zero;
                SetMapBgLocationByDrag(-offset);
            }
        }

        private void OnChangeMapRate(KeyData data)
        {
            if (data.Axis.Equals(0)) return;
            if (MapLevel.Min.Equals(adapter.MapLevel)) return;
            if (isGameObjectCreated == true)
            {
                Vector2 hitPoint = Vector2.zero;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTf, Input.mousePosition, uiCamera, out hitPoint))
                {
                    //                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBgRectTran, Input.mousePosition, uiCamera, out hitPoint))
                    //                        {
                    var newPoint = Vector2.zero;
                    if (data.Axis > 0)    //正向滚动滑轮  大地图控件可以显示的真实地图大小缩小一半
                    {
                        var temperNum = MiniMapRepresentWHByRice / 2;
                        if (temperNum >= OriginalMiniMapRepresentWHByRice / MaxZoom)
                        {
                            MiniMapRepresentWHByRice = temperNum;
                           
                        }
                    }
                    else if (data.Axis < 0)               //反向滚动滑轮  大地图控件可以显示的真实地图大小扩大一半
                    {
                        var temperNum = MiniMapRepresentWHByRice * 2;
                        if (temperNum <= OriginalMiniMapRepresentWHByRice)
                        {
                            MiniMapRepresentWHByRice = temperNum;
                            
                        }
                    }
                    newPoint = newPoint - hitPoint;

                    var lastRate = rate;
                    //更具滚轮的移动 改变MaxMapWHByRice
                    CalculateRate();
                    var rect = _viewModel.BgRectTransform;
                    rect.sizeDelta = new Vector2(OriginalMiniMapRepresentWHByRice * rate, OriginalMiniMapRepresentWHByRice * rate);
                    rect.anchoredPosition = AdjustMapBgLocation(rect.anchoredPosition + (rect.anchoredPosition - hitPoint) * (rate / lastRate - 1));
                    //                        }
                }
            }
        }
//        public void ShowMap(bool visible)
//        {
//            if (root != null)
//            {
//                if (visible && !adapter.Enable)
//                {
//                    adapter.Enable = true;
//                    PlayerStateUtil.AddUIState(EPlayerUIState.MapOpen, adapter.gamePlay);
//                }
//                else if (!visible && adapter.Enable)
//                {
//                    adapter.Enable = false;
//                    PlayerStateUtil.RemoveUIState(EPlayerUIState.MapOpen, adapter.gamePlay);
//                }
//            }
//        }
        protected override void OnCanvasEnabledUpdate(bool visible)
        {
            if (root != null)
            {
                if (visible)
                {
                    SetMapRoot();
                    CalculateRate();
                    //                    adapter.SetCrossActive(false);
                    //                    adapter.HideUiGroup(Core.Ui.UiGroup.MapHide);

                    adapter.RegisterKeyReceive(keyReceive);
                    adapter.RegisterPointerReceive(_pointerKeyHandler);
                }
                else
                {
//                    adapter.SetCrossActive(true);
//                    adapter.ShowUiGroup(Core.Ui.UiGroup.MapHide);
                    adapter.UnRegisterKeyReceive(keyReceive);
                    adapter.UnRegisterPointerReceive(_pointerKeyHandler);
                }
            }
        }

        private void GetCenterPosition()
        {
            var rect = _viewModel.BgRectTransform;
            _centerPos = (-rect.anchoredPosition +  rect.sizeDelta / 2 ) / rate;
        }

        protected override void RefreshGui(float interval)
        {
            if (!isGameObjectCreated)
                return;
            GetSelfPosition();
            GetCenterPosition();
            UpdateMapBg(_centerPos);

            _mapPlayer.Update(adapter.TeamInfos, _centerPos, rate, MiniMapRepresentWHByRice, adapter.MapLevel, selfPlayPos3D);
            _mapMark.Update(adapter.MapMarks, rate);
            _safeCircle.Update(adapter.NextDuquan, _centerPos, rate, MiniMapRepresentWHByRice);
            _duquanCircle.Update(adapter.CurDuquan, _centerPos, rate, MiniMapRepresentWHByRice);
            _bombArea.Update(adapter.BombArea, _centerPos, rate, MiniMapRepresentWHByRice);
//            _safeMiniDis.Update(adapter.NextDuquan, selfPlayPos, playItemModelWidth, rate, MiniMapRepresentWHByRice);
//            if (_mapGird1 != null) _mapGird1.Update(rate, MapRealWHByRice, _centerPos, MiniMapRepresentWHByRice, OriginalMiniMapRepresentWHByRice);

            _airPlane.Update(interval, rate, adapter.KongTouList(), adapter.PlaneData);
            _routeLine.Updata(adapter.IsShowRouteLine, adapter.RouteLineStartPoint, adapter.RouteLineEndPoint, rate);
            _mapLabel.UpdateC4(adapter, rate);
        }


        #region MapBgEventTrigger
        private void OnPointerDown(BaseEventData eventData)
        {
            //在缩放模式下 才能拖拽
            if (MiniMapRepresentWHByRice >= OriginalMiniMapRepresentWHByRice) return;
            var data = eventData as PointerEventData;
            Vector2 mouseDown = data.position;
            Vector2 mouseUguiPos = Vector2.zero;
            bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle((_viewModel.BgRectTransform), mouseDown, uiCamera, out mouseUguiPos);
            if (isRect)
            {
                _dragging = true;
                _dragStart =  mouseUguiPos;                   //计算图片位置和鼠标点在mask上的的差值
            }
        }
        private void OnPointerUp(BaseEventData eventData)
        {
            _dragStart = Vector2.zero;
            _dragging = false;
        }

        private void OnDrag(BaseEventData eventData)
        {
           
            if (_dragging)
            {
                var data = eventData as PointerEventData;
                Vector2 mouseDrag = data.position;   //当鼠标拖动时的屏幕坐标
                Vector2 uguiPos = Vector2.zero;
                bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(_viewModel.BgRectTransform, mouseDrag, uiCamera, out uguiPos);
                if (isRect)
                {
                    _dragingPos = uguiPos;
                    SetMapBgLocationByDrag(_dragingPos - _dragStart);
//                    _dragStart = _dragingPos;
                }
                //Debug.Log("OnDrag");
                Cursor.visible = false;
            }
        }
        private void OnEndDrag(BaseEventData eventData)
        {
            //Debug.Log("OnEndDrag");
            _dragStart = Vector2.zero;
            Cursor.visible = true;
        }

        /**
         *  传入一个像素单位的offset   移动mapBg这个ui的位置 并且更新maskcontrol的中心点的的位置（米）
         *  当offset为正时  mapbg向右移动 中心店坐标变小 反之
         */
        private void SetMapBgLocationByDrag(Vector2 offset /*像素单位*/)
        {
//            Debug.Log("ddd " + offset.ToString());
            var border = Math.Abs((_viewModel.BgRectTransform.rect.width - _viewModel.mapRectTransform.rect.width) / 2);
            var lastPos = _viewModel.BgRectTransform.anchoredPosition;
            var newPos = lastPos + offset; //*2 是提高 拖动速度效果而已
            _viewModel.BgRectTransform.anchoredPosition = AdjustMapBgLocation(newPos);
        }
        private Vector2 AdjustMapBgLocation(Vector2 newPos)
        {
            var border = Math.Abs((_viewModel.BgRectTransform.rect.width - _viewModel.mapRectTransform.rect.width) / 2);
            if (newPos.x > border)
            {
                newPos.x = border;
            }
            else if (newPos.x < -border)
            {
                newPos.x = -border;
            }
            if (newPos.y > border)
            {
                newPos.y = border;
            }
            else if (newPos.y < -border)
            {
                newPos.y = -border;
            }
            return newPos;
        }

        #endregion
    }
}    
