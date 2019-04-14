using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.Configuration;
using Assets.App.Client.GameModules.Ui;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UIComponent.UI.Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.SessionStates
{
    public class ClientSessionStateProgress : SessionStateProgress
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientSessionStateProgress));

        private static readonly string UiRootName = "ClientUIRoot";
        private static readonly string LoadingUiBundleName = "ui/client/prefab/common";
        private static readonly string LoadingUiAssetName = "CommonLoading";

        private static readonly string MapIconAssetName = "Map_0";

        private Contexts _contexts;
        private IUnityAssetManager _assetManager;
        private List<string> _tips;
        private GameObject _loadingUi;
//        private Slider _loadingSlider;
        private Image _bg;
        private Text _loadingProgressTxt;
        private Text _loadingTipTxt;
        private float _tipUpdateLastTime = 5;
        private float _tipUpdateMaxInterval = 5;
        private System.Random random = new System.Random();
      
        public ClientSessionStateProgress(IContexts contexts)
        {
            _contexts = contexts as Contexts;

            UiCommon.InitUI(UiRootName);

            _assetManager = _contexts.session.commonSession.AssetManager;

            _assetManager.LoadAssetAsync(GetType().ToString(), new AssetInfo(LoadingUiBundleName, LoadingUiAssetName), OnLoadSucc);
        }

        protected override void OnProgressUpdated(float progressRate, string tip)
        {
           // Debug.LogFormat("Progress {0},Text {1} SubProgress {2}", progressRate, tip, SingletonManager.Get<SubProgressBlackBoard>().ProgressRate);
            if (progressRate > 1 &&  _loadingUi != null)
            {
                _loadingUi.SetActive(false);
            }
            else
            {
//                if (_loadingSlider != null)
//                    _loadingSlider.value = progressRate;

                if (_loadingProgressTxt != null)
                    _loadingProgressTxt.text = String.Format("({0:P1}){1}",  progressRate, tip);
            }

            if (_tips == null)
            {
                GetTips();
            }
            else
            {
                ShowTips();
            }
        }

        private void ShowTips()
        {
            if (_tips.Count == 0) return;
            _tipUpdateLastTime += Time.deltaTime;
            if (_tipUpdateLastTime > _tipUpdateMaxInterval)
            {
                _tipUpdateLastTime = 0;
                if (_loadingTipTxt != null)
                    _loadingTipTxt.text = _tips[random.Next(0, _tips.Count)];
            }
        }

        private void GetTips()
        {
            var config = SingletonManager.Get<LoadingTipConfigManager>().GetConfig();
            if (config != null)
            {
                _tips = new List<string>();
                int tipType = (int)LoadingTipConfigManager.LoadingTipType.Client;
                foreach (var item in config.Items)
                {
                    if (item.Type.Equals(tipType) && (item.GameMode == null || item.GameMode.Count == 0 || item.GameMode.IndexOf(SharedConfig.GameRule) > -1))
                    {
                        _tips.Add(item.Tip);
                    }
                }
            }
        }

        private void OnLoadSucc(string source, UnityObject unityObj)
        {
            var obj = unityObj.AsGameObject;
            if (obj != null)
            {
                _loadingUi = obj;
                var rect = _loadingUi.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                FillField();
//                _loadingSlider = _loadingUi.GetComponentInChildren<Slider>();

                var bgBundle = SingletonManager.Get<MapConfigManager>().MapIconBundle;
                var bgAsset = SingletonManager.Get<MapConfigManager>().MapIconAsset;
                if (string.IsNullOrEmpty(bgBundle) || string.IsNullOrEmpty(bgAsset))
                {
                    bgBundle = AssetBundleConstant.Icon_Map;
                    bgAsset = MapIconAssetName;
                }
                _assetManager.LoadAssetAsync(GetType().ToString(), new AssetInfo(bgBundle, bgAsset),
                    (sour, data) =>
                    {
                        if (data.AsObject is Texture2D)
                        {
                            var tex = data.As<Texture2D>();
                            _bg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                        }
                        else
                        {
                            _bg.sprite = data.As<Sprite>();
                        }
                        
                        _bg.color = Color.white;
                    });


                var root = UiCommon.UIManager.UIRoot;
                if (root == null)
                {
                    _logger.ErrorFormat("The client ui root does not exist!");
                }
                else
                {
                    _loadingUi.transform.SetParent(root.transform, false);
                    _loadingUi.SetActive(true);
                }
            }
        }

        public void FillField()
        {
            _loadingUi.name = _loadingUi.name.Replace("(Clone)", "");
            Image[] gameobjects = _loadingUi.GetComponentsInChildren<Image>(true);
            foreach (var v in gameobjects)
            {
                var realName = v.gameObject.name.Replace("(Clone)", "");
                switch (realName)
                {
                    case "bg":
                        _bg = v;
                        break;
                }
            }
            Text[] texts = _loadingUi.GetComponentsInChildren<Text>(true);
            foreach (var v in texts)
            {
                var realName = v.gameObject.name.Replace("(Clone)", "");
                switch (realName)
                {
                    case "text":
                        _loadingProgressTxt = v;
                        break;
                    case "tipText":
                        _loadingTipTxt = v;
                        break;
                }
            }
        }
    }
}
