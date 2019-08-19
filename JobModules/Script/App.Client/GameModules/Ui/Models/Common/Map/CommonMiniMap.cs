using System;
using App.Client.GameModules.Ui.Models.Common.Map;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine;
using Core.Utils;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using UnityEngine.Profiling;
using App.Client.GameModules.Ui.Utils.MiniMaxMapCommon;
using App.Shared.Components.Ui;
using App.Shared.Configuration;
using Core.Components;
using Core.Ui.Map;
using DG.Tweening;
using UIComponent.UI;
using UnityEngine.UI;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common.Map
{
    public partial class CommonMiniMap : ClientAbstractModel, IUiHfrSystem
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonMiniMap));

        protected static float borderSize = 150; //地图资源会有额外边
        protected IMiniMapUiAdapter adapter;
        protected bool isGameObjectCreated = false;

        private float rateControlSpeed = 54;// km/h
        private float rateIncreaseInterval = 10;// km/h
        private int rateIncreaseMaxPresent = 5;

        protected float rate = 0;                                 // 像素/米
        public float miniMapWByPixel = 0;               //像素
        public float miniMapHByPixel = 0;
        protected float OriginalMiniMapRepresentWHByRice = 400;   //米
        protected float MiniMapRepresentWHByRice = 400;   //米
        protected float MapRealWHByRice = 0;              //地图实际宽高
        protected float playItemModelWidth = 0;

        protected Transform root = null;
        protected Transform bgRect = null;
        protected RectTransform rootRectTf = null;
        protected Transform miniMap = null;

        protected Transform mapBg = null;
        protected Transform kTouRoot = null;
        protected Transform routeRoot = null;
        protected Transform curDuquanRoot = null;
        protected Transform safeDuquanRoot = null;
        protected Transform curBombAreaRoot = null;
        protected Transform miniDisRoot = null;
        protected Transform lineRoot = null;
        protected Transform labelRoot = null;
        protected Transform playItemRoot = null;
        protected Transform markRoot = null;
        protected Transform girdRoot = null;
        protected Transform bioMarkRoot = null;
        protected Transform mapNameRoot = null;
        protected UIText mapNameText = null;

        protected RawImage mapBgRawIamge;

        protected BaneCircle _safeCircle;
        protected BaneCircle _duquanCircle;
        protected BombArea _bombArea;
        protected SafeMiniDis _safeMiniDis;
        protected AirPlane _airPlane;
        protected RouteLine _routeLine;
        protected MapGird _mapGird;
        protected MapGird1 _mapGird1;
        protected MapLabel _mapLabel;
        protected MapPlayer _mapPlayer;
        protected MapMark _mapMark;
        protected BioMark _bioMark;

        private float targetRate = 0;
        private float increaseRate;

        protected Vector2 lastPos = Vector2.zero; //记录上一次的人物在 以地图左下角的 相对位置 米
        protected Vector2 selfPlayPos = Vector2.zero;
        protected MapFixedVector3 selfPlayPos3D;  //真实世界的高度

        protected CommonMiniMapViewModel _viewModel = new CommonMiniMapViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonMiniMap(IMiniMapUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;

            MapRealWHByRice = adapter.CurMapSize;
            OriginalMiniMapRepresentWHByRice = MiniMapRepresentWHByRice = GetShowSize();
            Logger.InfoFormat("CurMapSize: \"{0}\" ", MapRealWHByRice);
            InitGui();

            girdRoot.gameObject.SetActive(false);
            mapNameText.text = SingletonManager.Get<MapConfigManager>().SceneParameters.Name;
        }
        public override void Update(float interval)
        {
            if (!isVisible) return;
            if (!isGameObjectCreated) return;
            RefreshGui(interval);
        }

        private void InitGui()
        {
            InitSetting();
            CalculateRate();
            //            InitPlayerItemList();
            //            InitMarkItemList();
            _safeCircle = new BaneCircle(safeDuquanRoot, false, true);
            _duquanCircle = new BaneCircle(curDuquanRoot, true, true);
            _bombArea = new BombArea(curBombAreaRoot);
            _safeMiniDis = new SafeMiniDis(miniDisRoot);
            if (!MapLevel.Min.Equals(adapter.MapLevel))
            {
//                _mapGird = new MapGird(lineRoot);
//                _mapGird1 = new MapGird1(girdRoot);
            }
//            _mapGird1 = new MapGird1(girdRoot);
            InitMapBg();
            _airPlane = new AirPlane(kTouRoot, Loader);
            _routeLine = new RouteLine(routeRoot);
            _mapLabel = new MapLabel(labelRoot);
            _mapPlayer = new MapPlayer(playItemRoot);
            _mapMark = new MapMark(markRoot);
            _bioMark = new BioMark(bioMarkRoot);
        }

        protected virtual void UpdateMapBg(Vector2 centerPos)
        {
            var textureW = mapBgRawIamge.texture.width;
            var textureH = mapBgRawIamge.texture.height;
            var uvRect = mapBgRawIamge.uvRect;
            var halfRect = MiniMapRepresentWHByRice * rate / 2f;
            var RealRect = MapRealWHByRice * rate;
            uvRect.x = (centerPos.x * rate - halfRect)/ RealRect;
            uvRect.y = (centerPos.y * rate - halfRect)/ RealRect;
//            uvRect.width = uvRect.height = halfRect * 2f / (RealRect);
            uvRect.width = uvRect.height = halfRect * 2f / (RealRect) * (textureW - borderSize * 2f) / textureW;
            uvRect.x = (uvRect.x * (textureW - borderSize * 2f) + borderSize) / textureW;
            uvRect.y = (uvRect.y * (textureH - borderSize * 2f) + borderSize) / textureH;
            mapBgRawIamge.uvRect = uvRect;
        }

        protected virtual void RefreshGui(float interval)
        {
            if (!isGameObjectCreated)
                return;
            GetSelfPosition();
            UpdateMapRate();
            UpdateMapBg(selfPlayPos);
//            UpdatePlayerItemList();

            _mapPlayer.Update(adapter.TeamInfos, selfPlayPos, rate, MiniMapRepresentWHByRice, adapter.MapLevel, selfPlayPos3D);
            _mapMark.Update(adapter.MapMarks, rate);
            _safeCircle.Update(adapter.NextDuquan, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _duquanCircle.Update(adapter.CurDuquan, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _bombArea.Update(adapter.BombArea, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _safeMiniDis.Update(adapter.NextDuquan, selfPlayPos, playItemModelWidth, rate, MiniMapRepresentWHByRice);
//            if (_mapGird != null) _mapGird.Update(rate, MapRealWHByRice, selfPlayPos, MiniMapRepresentWHByRice);
//            if (_mapGird1 != null) _mapGird1.Update(rate, MapRealWHByRice, selfPlayPos, MiniMapRepresentWHByRice, 0);

            UpdataMapBg();

            _airPlane.Update(interval, rate, adapter.KongTouList(), adapter.PlaneData);
            _routeLine.Updata(adapter.IsShowRouteLine, adapter.RouteLineStartPoint, adapter.RouteLineEndPoint, rate);
            _mapLabel.UpdateC4(adapter, rate);
            _bioMark.UpdateBioLabel(adapter.MotherPos, adapter.HeroPos,adapter.HumanPos, adapter.SupplyPos, rate);
        }

        protected void GetSelfPosition()
        {
            //var data = adapter.ObservePlayer;
            //selfPlayPos = data.Pos.ShiftedUIVector2();
            selfPlayPos = adapter.CurPlayerPos;
            selfPlayPos3D = adapter.CurPlayerPos3D;
        }

        private void InitSetting()
        {
            root = FindChildGo("root");
            bgRect = FindChildGo("bgRect");
            miniMap = FindChildGo("Bg");
            playItemRoot = FindChildGo("playRoot");
            markRoot = FindChildGo("markRoot");
//            markModel = FindChildGo("playItem");
            curDuquanRoot = FindChildGo("curDuquan");
            safeDuquanRoot = FindChildGo("safeDuquan");
            curBombAreaRoot = FindChildGo("bombArea");
            miniDisRoot = FindChildGo("miniMumDisatance");
//            lineModel = FindChildGo("lineModel");
            lineRoot = FindChildGo("lineRoot");
            girdRoot = FindChildGo("girdRoot");
            mapBg = FindChildGo("mapBg");
//            airPlane = FindChildGo("airPlane");
//            kTouModel = FindChildGo("kongtouModel");
            kTouRoot = FindChildGo("airPlaneRoot");
            var playItemModel = FindChildGo("playitem");
            playItemModelWidth = playItemModel.GetComponent<RectTransform>().sizeDelta.x;
//            routeLineGo = FindComponent<RectTransform>("routeLine");
            routeRoot = FindChildGo("routeRoot");
            labelRoot = FindChildGo("LabelRoot");
            bioMarkRoot = FindChildGo("BioRoot");
            mapNameRoot = FindChildGo("MapNameImage");
            mapNameText = FindChildGo("MapNameText").GetComponent<UIText>();
            rootRectTf = root.GetComponent<RectTransform>();
            mapBgRawIamge = mapBg.GetComponent<RawImage>();
            //保证canvas的 推荐分辨率等于 实际分辨率 解决犹豫网格线只有一个像素的 导致的精度丢失问题
            //            var canvasScaleCom = root.parent.GetComponent<CanvasScaler>();
            //            canvasScaleCom.referenceResolution = new Vector2(Screen.width, Screen.height);
            //            Logger.InfoFormat("minimap Mask Width:{0}  Height:{1}", miniMapWByPixel, miniMapHByPixel);
        }

        protected virtual void CalculateRate()
        {
            //因为小地图是 miniMapW * miniMapH 像素的框框  而实际地图是以自己为中心的MiniMapWH * MiniMapWH米的正方形 所以目前设计的一像素对应于一米 1:1 比例
            miniMapWByPixel = rootRectTf.sizeDelta.x;
            miniMapHByPixel = rootRectTf.sizeDelta.y;
            targetRate = rate = miniMapWByPixel / MiniMapRepresentWHByRice;
        }

        protected virtual float GetShowSize()
        {
            return SingletonManager.Get<MapConfigManager>().SceneParameters.MiniMapShowSize;
        }


        private float targetRice;
        private void UpdateMapRate()
        {
//          Debug.Log(adapter.MoveSpeed);
            float rice;
            if (adapter.MoveSpeed > rateControlSpeed)
            {
                var addSpeed = (int)((adapter.MoveSpeed - rateControlSpeed) / rateIncreaseInterval);
                if (addSpeed > rateIncreaseMaxPresent) addSpeed = rateIncreaseMaxPresent;
                rice = OriginalMiniMapRepresentWHByRice + OriginalMiniMapRepresentWHByRice * addSpeed * 0.1f;
            }
            else
            {
                rice = OriginalMiniMapRepresentWHByRice;
            }

            if (!targetRice.Equals(rice))
            {
                targetRice = rice;
                increaseRate = (targetRice - MiniMapRepresentWHByRice) / 10;
            }

//            var newRate = miniMapWByPixel / MiniMapRepresentWHByRice;
//            if (targetRate.Equals(newRate) == false)
//            {
//                targetRate = newRate;
//                increaseRate = (targetRate - rate) / 60;
//            }

            if (targetRice.Equals(MiniMapRepresentWHByRice) == false)
            {
                MiniMapRepresentWHByRice += increaseRate;
                if(Mathf.Abs(targetRice - MiniMapRepresentWHByRice) < Mathf.Abs(increaseRate)) MiniMapRepresentWHByRice = targetRice;
                rate = miniMapWByPixel / MiniMapRepresentWHByRice;
                SetMapRate();
            }
        }


        #region Map
        private void InitMapBg()
        {
            SetMapRate();

            //根据地图Id 设置mapBg的图片
            if (adapter != null)
            {
                var mapId = adapter.MapId;
                AbstractMapConfig mapConfig = SingletonManager.Get<MapConfigManager>().SceneParameters;
                if(mapConfig is AbstractMapConfig)
                {
                    Loader.RetriveTextureAsync(SingletonManager.Get<MapConfigManager>().SceneParameters.MapLargeBundlename, SingletonManager.Get<MapConfigManager>().SceneParameters.MapLarge, (data) =>
                    {
                        mapBgRawIamge.texture = data;
                    });
                }
                /*
                switch (mapId)
                {
                    case 1:
                        {
                            Loader.RetriveTextureAsync(SingletonManager.Get<MapConfigManager>().SceneParameters.MapLargeBundlename, SingletonManager.Get<MapConfigManager>().SceneParameters.MapLarge, (data) =>
                            {
                                mapBgRawIamge.texture = data;
                            });
                        }
                        break;
                }
                */
            }
        }

        private void SetMapRate()
        {
            var mapBgRectTran = _viewModel.BgRectTransform;
            if (mapBgRectTran == null)
                return;
            mapBgRectTran.sizeDelta = new Vector2(rate * MapRealWHByRice, rate * MapRealWHByRice);
            //默认假设人物在 实际地图的左下角 然后设置地图UI 为对应的位置
            mapBgRectTran.anchoredPosition = new Vector2(rate * MapRealWHByRice, rate * MapRealWHByRice) / 2;
            lastPos = Vector2.zero;
        }

        private void UpdataMapBg()
        {
            if (miniMap == null)
                return;

            //设置它的大小 因为小地图要显示400 * 400的范围的内容
            var mapBgRectTran = _viewModel.BgRectTransform;
            if (mapBgRectTran == null)
                return;
            //            mapBgRectTran.sizeDelta = new Vector2(rate * MapRealWHByRice, rate * MapRealWHByRice);

            //然后移动该图片
            if ((selfPlayPos.x >= 0 && selfPlayPos.x <= MapRealWHByRice) && (selfPlayPos.y >= 0 && selfPlayPos.y <= MapRealWHByRice))
            {
                Vector2 temperPos = selfPlayPos;
                if (temperPos != lastPos)
                {
                    mapBgRectTran.anchoredPosition = mapBgRectTran.anchoredPosition + (lastPos - temperPos) * rate;
                    lastPos = temperPos;
                }
            }
        }
        #endregion
    }
}    
