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
using DG.Tweening;
using UnityEngine.UI;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{
    public partial class CommonMiniMap : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonMiniMap));
        private IMiniMapUiAdapter adapter;
        private bool isGameObjectCreated = false;

        private float rateControlSpeed = 54;// km/h
        private float rateIncreaseInterval = 10;// km/h
        private int rateIncreaseMaxPresent = 5;

        float rate = 0;                                 // 像素/米
        public float miniMapWByPixel = 0;               //像素
        public float miniMapHByPixel = 0;
        private float OriginalMiniMapRepresentWHByRice = 400;   //米
        private float MiniMapRepresentWHByRice = 400;   //米
        private float MapRealWHByRice = 0;              //地图实际宽高
        private float playItemModelWidth = 0;

        private Transform root = null;
        private Transform miniMap = null;

        private Transform mapBg = null;
        private Transform kTouRoot = null;
        private Transform routeRoot = null;
        private Transform curDuquanRoot = null;
        private Transform safeDuquanRoot = null;
        private Transform curBombAreaRoot = null;
        private Transform miniDisRoot = null;
        private Transform lineRoot = null;
        private Transform labelRoot = null;
        private Transform playItemRoot = null;
        private Transform markRoot = null;

        private BaneCircle _safeCircle;
        private BaneCircle _duquanCircle;
        private BombArea _bombArea;
        private SafeMiniDis _safeMiniDis;
        private AirPlane _airPlane;
        private RouteLine _routeLine;
        private MapGird _mapGird;
        private MapLabel _mapLabel;
        private MapPlayer _mapPlayer;
        private MapMark _mapMark;

        private float targetRate = 0;
        private float increaseRate;

        private Vector2 lastPos = Vector2.zero; //记录上一次的人物在 以地图左下角的 相对位置 米
        private Vector2 selfPlayPos = Vector2.zero;

        private CommonMiniMapViewModel _viewModel = new CommonMiniMapViewModel();
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
            Logger.InfoFormat("CurMapSize: \"{0}\" ", MapRealWHByRice);
            InitGui();
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
//            InitPlayerItemList();
//            InitMarkItemList();
            _safeCircle = new BaneCircle(safeDuquanRoot, false, true);
            _duquanCircle = new BaneCircle(curDuquanRoot, true, true);
            _bombArea = new BombArea(curBombAreaRoot);
            _safeMiniDis = new SafeMiniDis(miniDisRoot);
            if (!MapLevel.Min.Equals(adapter.MapLevel))
            {
                _mapGird = new MapGird(lineRoot);
            }
            InitMapBg();
            _airPlane = new AirPlane(kTouRoot, Loader);
            _routeLine = new RouteLine(routeRoot);
            _mapLabel = new MapLabel(labelRoot);
            _mapPlayer = new MapPlayer(playItemRoot);
            _mapMark = new MapMark(markRoot);
            PreparedSprite();
        }


        private void RefreshGui(float interval)
        {
            if (!isGameObjectCreated)
                return;
            GetSelfPosition();
            UpdateMapRate();
//            UpdatePlayerItemList();

            _mapPlayer.Update(adapter.TeamInfos, selfPlayPos, rate, MiniMapRepresentWHByRice, adapter.MapLevel);
            _mapMark.Update(adapter.TeamInfos, rate);
            _safeCircle.Update(adapter.NextDuquan, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _duquanCircle.Update(adapter.CurDuquan, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _bombArea.Update(adapter.BombArea, selfPlayPos, rate, MiniMapRepresentWHByRice);
            _safeMiniDis.Update(adapter.NextDuquan, selfPlayPos, playItemModelWidth, rate, MiniMapRepresentWHByRice);
            if (_mapGird != null) _mapGird.Update(rate, MapRealWHByRice, selfPlayPos, MiniMapRepresentWHByRice);

            UpdataMapBg();

            _airPlane.Update(interval, rate, adapter.KongTouList(), adapter.PlaneData);
            _routeLine.Updata(adapter.IsShowRouteLine, adapter.RouteLineStartPoint, adapter.RouteLineEndPoint, rate);
            _mapLabel.UpdateC4(adapter.IsC4Drop, adapter.C4DropPosition, rate);
        }

        private void GetSelfPosition()
        {
            var data = adapter.TeamInfos.Find((info=>info.IsPlayer == true));
            selfPlayPos = data.Pos;
        }

        private void InitSetting()
        {
            root = FindChildGo("root");
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
            mapBg = FindChildGo("mapBg");
//            airPlane = FindChildGo("airPlane");
//            kTouModel = FindChildGo("kongtouModel");
            kTouRoot = FindChildGo("airPlaneRoot");
            var playItemModel = FindChildGo("playitem");
            playItemModelWidth = playItemModel.GetComponent<RectTransform>().sizeDelta.x;
//            routeLineGo = FindComponent<RectTransform>("routeLine");
            routeRoot = FindChildGo("routeRoot");
            labelRoot = FindChildGo("LabelRoot");

          
            OriginalMiniMapRepresentWHByRice = MiniMapRepresentWHByRice = SingletonManager.Get<MapConfigManager>().SceneParameters.MiniMapShowSize;

            //因为小地图是 miniMapW * miniMapH 像素的框框  而实际地图是以自己为中心的MiniMapWH * MiniMapWH米的正方形 所以目前设计的一像素对应于一米 1:1 比例
            miniMapWByPixel = root.GetComponent<RectTransform>().sizeDelta.x;
            miniMapHByPixel = root.GetComponent<RectTransform>().sizeDelta.y;
            targetRate = rate = miniMapWByPixel / MiniMapRepresentWHByRice;

            //保证canvas的 推荐分辨率等于 实际分辨率 解决犹豫网格线只有一个像素的 导致的精度丢失问题
//            var canvasScaleCom = root.parent.GetComponent<CanvasScaler>();
//            canvasScaleCom.referenceResolution = new Vector2(Screen.width, Screen.height);
//            Logger.InfoFormat("minimap Mask Width:{0}  Height:{1}", miniMapWByPixel, miniMapHByPixel);
        }

        private void UpdateMapRate()
        {
//          Debug.Log(adapter.MoveSpeed);
            if (adapter.MoveSpeed > rateControlSpeed)
            {
                var addSpeed = (int)((adapter.MoveSpeed - rateControlSpeed) / rateIncreaseInterval);
                if (addSpeed > rateIncreaseMaxPresent) addSpeed = rateIncreaseMaxPresent;
                 MiniMapRepresentWHByRice = OriginalMiniMapRepresentWHByRice + OriginalMiniMapRepresentWHByRice * addSpeed * 0.1f;
            }
            else
            {
                MiniMapRepresentWHByRice = OriginalMiniMapRepresentWHByRice;
            }

            var newRate = miniMapWByPixel / MiniMapRepresentWHByRice;
            if (targetRate.Equals(newRate) == false)
            {
                targetRate = newRate;
                increaseRate = (targetRate - rate) / 60;
            }

            if (targetRate.Equals(rate) == false)
            {
                rate += increaseRate;
                if(Mathf.Abs(targetRate - rate) < Mathf.Abs(increaseRate)) rate = targetRate;
                SetMapRate();
            }
        }

        private void PreparedSprite()
        {
            SpriteComon.GetInstance().PreparedSprites();
        }

        #region Map
        private void InitMapBg()
        {
            SetMapRate();

            //根据地图Id 设置mapBg的图片
            var mapBgImg = mapBg.GetComponent<Image>();
            if (adapter != null)
            {
                var mapId = adapter.MapId;
                switch (mapId)
                {
                    case 1:
                        {
                            Loader.RetriveSpriteAsync(SingletonManager.Get<MapConfigManager>().SceneParameters.MapLargeBundlename, SingletonManager.Get<MapConfigManager>().SceneParameters.MapLarge, (sprite) =>
                            {
                                mapBgImg.sprite = sprite;
                            });
                        }
                        break;
                }
            }
        }

        private void SetMapRate()
        {
            var mapBgRectTran = miniMap.GetComponent<RectTransform>();
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
            var mapBgRectTran = miniMap.GetComponent<RectTransform>();
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
