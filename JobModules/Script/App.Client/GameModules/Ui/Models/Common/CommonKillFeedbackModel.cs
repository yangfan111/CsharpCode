using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using App.Client.GameModules.Ui.UiAdapter;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UIComponent.UI;
using Utils.Configuration;
using Utils.Singleton;
using Assets.App.Client.GameModules.Ui;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonKillFeedbackModel : ClientAbstractModel, IUiSystem
    {
        private IKillFeedBackUiAdapter adapter = null;

        private Dictionary<int, GameObject> killEffectDict = new Dictionary<int, GameObject>();

        private CommonKillFeedbackViewModel _viewModel = new CommonKillFeedbackViewModel();
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }
        public CommonKillFeedbackModel(IKillFeedBackUiAdapter adapter):base(adapter)
        {
            this.adapter = adapter;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitGui();
        }
        private void InitGui()
        {
            groupRoot = FindChildGo("IconParent");
            animeRoot = FindChildGo("AnimeRoot");
        }

        private bool isNeedShow = false;
        public override void Update(float interval)
        {
            UpdateAnime();
            UpdateKillFeedList();
        }

        private void UpdateAnime()
        {
            if (isNeedUpdate && killFeedList != null && killFeedList.Count > 0)
            {
                var list = killFeedList;
                GameObject ret;
                for(int i = 0; i < list.Count;i++)
                {
                    var it = list[i];
                    if (killEffectDict.TryGetValue(it, out ret))
                    {
                        if (ret != new GameObject())
                        ret.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        killEffectDict.Add(it, new GameObject());
                        Loader.LoadAsync(bundle, GetEffectNameById(it), (obj) =>
                        {
                            var root = GameObject.Instantiate(animeRoot, groupRoot);
                            root.gameObject.SetActive(true);
                            GameObject go = obj as GameObject;
                            go.transform.SetParent(root, false);
                            killEffectDict[it] = go;
                        });
                    }
                }

                isNeedUpdate = false;
            }
        }

        private string bundle = AssetBundleConstant.Effect;
        private Transform groupRoot,animeRoot;
        private List<int> killFeedList = new List<int>();
        private bool isNeedUpdate;
        private void UpdateKillFeedList()
        {
            var list = adapter.Types;
            if (list == null || list.Count == 0) return;
            list.Sort();
            killFeedList.Clear();
            killFeedList.AddRange(list);

            isNeedUpdate = true;
            for (int i = 0; i < groupRoot.childCount; i++)
            {
                groupRoot.GetChild(i).gameObject.SetActive(false);
            }
            list.Clear();
        }

        private string GetEffectNameById(int id)
        {
            return  SingletonManager.Get<KillFeedBackConfigManager>().GetEffectNameById(id);
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);
            {
                if (enable == false)
                {
                    UITool.HideChilds(groupRoot);
                }
            }
        }

    }
}    
