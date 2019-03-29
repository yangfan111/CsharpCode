using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UserInputManager.Lib;
using UnityEngine;
using System.Collections.Generic;
using App.Client.GameModules.Ui.System;
using UnityEngine.EventSystems;
using Core.Utils;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public partial class CommonMaxMap : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonMiniMap));
        private IMiniMapUiAdapter adapter = null;
        private bool isGameObjectCreated = false;

        float _rate = 0;      // 像素/米
        float lastRate = 0;
        Vector2 maskCenterPInMapByRice = Vector2.zero;      //遮罩control的中心店在 实际地图的坐标 米
        float maxMapMaskWByPixel = 0;  //像素
        float maxMapMaskHByPixel = 0;    
        private float MaxMapRepresentWHByRice = 0;          //米  大地图此时 代表的实际地图的米数  根据鼠标滚轮 变化这个值
        private float MaxMapRealWHByRice = 0;               //地图实际宽高

        private Transform root = null;
        private RectTransform rootRtf = null;
        private Camera uiCamera = null;

        public float rate
        {
            get { return _rate; }
            set {
                _rate = value;
                PlayerAddMarkUtility.rate = value;
            }
        }

        KeyReceiver keyReceive = null;
        PointerReceiver pointerReceiver = null;

        private CommonMaxMapViewModel _viewModel = new CommonMaxMapViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonMaxMap(IMiniMapUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
            adapter.Enable = false;
            if (UiCommon.UIManager.GetRootRenderMode().Equals(RenderMode.ScreenSpaceCamera))
            {
                uiCamera = UiCommon.UIManager.UICamera;
            }
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;

            MaxMapRealWHByRice = adapter.CurMapSize;

//           MaxMapRepresentWHByRice = adapter.MapShowSizeByRice.x;
            MaxMapRepresentWHByRice = MaxMapRealWHByRice;

            InitGui();
            InitKeyBinding();
            BindUIEventTrigger();
        }
        public override void Update(float interval)
        {
            if (!isVisible) return;

            RefreshGui(interval);
        }

        public void InitKeyBinding()
        {
            var receiver = new KeyReceiver(UiConstant.maxMapWindowLayer, BlockType.None);
            receiver.AddAction(UserInputKey.ShowMaxMap, (data) =>
            {
                if (root != null)
                {
                    ShowMap(!adapter.Enable);
                }
            });
            adapter.RegisterKeyReceive(receiver);

            DynamicKeyReceive();
        }

        public void DynamicKeyReceive()
        {
            pointerReceiver = new PointerReceiver(UiConstant.maxMapWindowPointBlockLayer, BlockType.All);
            keyReceive = new KeyReceiver(UiConstant.maxMapWindowKeyBlockLayer, BlockType.All);  
            keyReceive.AddAction(UserInputKey.ChangeMapRate, (data) =>
            {
                if (isGameObjectCreated == true && MapLevel.Min.Equals(adapter.MapLevel) == false)
                {
                    Vector2 hitPoint = Vector2.zero;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(maxMapMaskControl.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out hitPoint))
                    {
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBgRectTran, Input.mousePosition, uiCamera, out hitPoint))
                        {
                            if (data.Axis > 0)    //正向滚动滑轮  大地图控件可以显示的真实地图大小缩小一半
                            {
                                var temperNum = MaxMapRepresentWHByRice / 2;
                                if (temperNum >= MaxMapRealWHByRice / 16)
                                {
                                    MaxMapRepresentWHByRice = temperNum;
                                }
                            }
                            else if (data.Axis < 0)               //反向滚动滑轮  大地图控件可以显示的真实地图大小扩大一半
                            {
                                var temperNum = MaxMapRepresentWHByRice * 2;
                                if (temperNum <= MaxMapRealWHByRice)
                                {
                                    MaxMapRepresentWHByRice = temperNum;
                                }
                            }

                            //更具滚轮的移动 改变MaxMapWHByRice
                            rate = maxMapMaskWByPixel / MaxMapRepresentWHByRice;
                        }
                    }
                }
            });
            keyReceive.AddAction(UserInputKey.AddMark, (data) =>
            {
                if (MapLevel.Min.Equals(adapter.MapLevel) == false)
                {
                    Vector2 markPos = adapter.CurPlayerPos;
                    adapter.SendMarkMessage(markPos);
                }
            });
            keyReceive.AddAction(UserInputKey.MouseAddMark, (data) =>
            {
                Vector2 hitPoint = Vector2.zero;
                Vector2 hitPoint1 = Vector2.zero;
                if (MapLevel.Min.Equals(adapter.MapLevel) == false)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(maxMapMaskControlRect, Input.mousePosition, uiCamera, out hitPoint1))
                    {
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBgRectTran, Input.mousePosition, uiCamera, out hitPoint))
                        {
                            var markPos = maskCenterPInMapByRice + hitPoint1 / rate;
                            adapter.SendMarkMessage(markPos);
                        }
                    }
                }
            });
            keyReceive.AddAction(UserInputKey.LocationCurPlay, (data) =>
            {
                if (MaxMapRepresentWHByRice != 8000) //缩放模式的时候
                {
                    //将当前玩家尽量移动到地图中心 也就是移动背景图 
                    var curPlayPosByPice = adapter.CurPlayerPos;
                    Vector2 curPlayPosByPixel = (curPlayPosByPice - maskCenterPInMapByRice) * rate;
                    var offset = curPlayPosByPixel - Vector2.zero;
                    SetMapBgLocationByDrag(-offset);
                }
            });

            keyReceive.AddAction(UserInputKey.HideWindow, (data) => 
            {
                if (adapter.Enable)
                {
                    ShowMap(!adapter.Enable);
                }
            });
        }

        protected override void OnCanvasEnabledUpdate(bool visible)
        {
            if (root != null)
            {
                if (visible)
                {
                    adapter.SetCrossActive(false);
                    adapter.HideUiGroup(Core.Ui.UiGroup.MapHide);

                    adapter.RegisterKeyReceive(keyReceive);
                    adapter.RegisterPointerReceive(pointerReceiver);
                }
                else
                {
                    adapter.SetCrossActive(true);
                    adapter.ShowUiGroup(Core.Ui.UiGroup.MapHide);
                    adapter.UnRegisterKeyReceive(keyReceive);
                    adapter.UnRegisterPointerReceive(pointerReceiver);
                }
            }
        }

        public void ShowMap(bool visible)
        {
            if (root != null)
            {
                if (visible && !adapter.Enable)
                {
                    adapter.Enable = true;
                }
                else if (!visible && adapter.Enable)
                {
                    adapter.Enable = false;
                }
            }
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

            _viewModel.maskBgET = entries;
        }

        private void InitGui()
        {
            InitSetting();
            InitMapBg();
            RefePosSetting();  //必须在mapbg update之后才可以 获取最新的mask中心点的位置
            InitGrids();
            InitPlayerItemList();
            InitMarkItemList();
            InitDuQuanAndSafe();
            InitAirPlane();
            InitRouteLine();
        }

        private void RefreshGui(float interval)
        {
            if (!isGameObjectCreated || !isVisible)
                return;

            if (lastRate != rate)
            {
                UpdataMapBg();
                lastRate = rate;
            }

            RefePosSetting(); //必须在mapbg update之后才可以       
            UpdateGrids();
            UpdatePlayerItemList(); //必须在更新毒圈之前更新玩家位置 因为最短距离要根据当前玩家的位置
            UpdateMarkItemList();
            UpdateDuQuanAndSafe();
            UpateAirPlane(interval);
            UpdateRouteLine();
        }

        private void InitSetting()
        {
            maxMapMaskControl = FindChildGo("Bg");
            mapBg = FindChildGo("mapBg");
            mapBgRectTran = mapBg.GetComponent<RectTransform>();
            maxMapMaskControlRect = maxMapMaskControl.GetComponent<RectTransform>();

            playItemModel = FindChildGo("playItem");
            playItemRoot = FindChildGo("playRoot");
            markRoot = FindChildGo("markRoot");
            markModel = FindChildGo("markItem");

            curDuquanRoot = FindChildGo("curDuquan");
            safeDuquanRoot = FindChildGo("safeDuquan");
            curBombAreaRoot = FindChildGo("bombArea");

            lineModel = FindChildGo("lineModel");
            lineRoot = FindChildGo("lineRoot");

            root = FindChildGo("root");
            rootRtf = root.GetComponent<RectTransform>();
            adapter.Enable = false;

            miniDisTran = FindChildGo("miniDis");

            airPlane = FindChildGo("airPlane");
            kTouModel = FindChildGo("kongtouModel");
            kTouRoot = FindChildGo("airPlaneRoot");

            routeLineGo = FindComponent<RectTransform>("routeLine");

            //保证canvas的 推荐分辨率等于 实际分辨率 解决犹豫网格线只有一个像素的 导致的精度丢失问题
//            var canvasScaleCom = root.parent.GetComponent<CanvasScaler>();
//            canvasScaleCom.referenceResolution = new Vector2(Screen.width, Screen.height);

            //保证 可是窗口是正方形  因为大地图代表的是 宽=高的 实际地图区域， 并且要求大地图 顶着屏幕的上下方, 应该用画布的大小 而不是screen实际屏幕的大小
            float canavsW = root.parent.GetComponent<RectTransform>().rect.width;
            float canvasH = root.parent.GetComponent<RectTransform>().rect.height;
            var maskWh = canavsW > canvasH ? canvasH : canavsW;
            rootRtf.pivot = new Vector2(0.5f, 0.5f);
            rootRtf.sizeDelta = new Vector2(maskWh, maskWh);
            rootRtf.anchoredPosition = Vector2.zero;
            maxMapMaskControlRect.sizeDelta = new Vector2(maskWh, maskWh);

            maxMapMaskWByPixel = rootRtf.sizeDelta.x;
            maxMapMaskHByPixel = rootRtf.sizeDelta.y;
            rate = maxMapMaskWByPixel / MaxMapRepresentWHByRice;
            lastRate = rate;
            maskCenterPInMapByRice = new Vector2(MaxMapRealWHByRice / 2, MaxMapRealWHByRice / 2);

            playItemModelWidth = playItemModel.GetComponent<RectTransform>().sizeDelta.x;
        }

        private void RefePosSetting()
        {
            //根据当前的缩放比例 修改玩家和标记的参考点， 修改毒圈、安全区、轰炸区的参考点 为mask control的中心点            
            PlayRefePosByRice = maskCenterPInMapByRice;
            playRefePosByPixel = Vector2.zero;
            markRefePosByRice = maskCenterPInMapByRice;
            markRefePosByPixel = Vector2.zero;
            duquanRefePosByRice = maskCenterPInMapByRice;
            duquanRefePosByPixel = Vector2.zero;
        }
    }
}    
