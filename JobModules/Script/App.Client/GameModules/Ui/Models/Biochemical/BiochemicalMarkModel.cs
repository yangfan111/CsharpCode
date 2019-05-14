using System;
using System.Collections.Generic;
using System.Linq;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Biochemical;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Biochemical
{

    public class BiochemicalMarkModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BiochemicalMarkModel));

        enum EMarkType
        {
            Mother = 1,
            Hero,
            Hum
        }
        private Transform _origMotherTf, _origHeroTf, _origHumTf;
        private Transform _root;

        private Dictionary<EMarkType, Dictionary<long,GameObject>> _markPool;

        private Dictionary<EMarkType, GameObject> _origGoDict;

        private Dictionary<EMarkType, HashSet<long>> _idListCache;

        private BiochemicalMarkViewModel _viewModel = new BiochemicalMarkViewModel();
        IBiochemicalMarkUiAdapter adapter;


        public BiochemicalMarkModel(BiochemicalMarkUiAdapter adapter) : base(adapter)
        {
            this.adapter = adapter;
        }

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        private RectTransform _canvasRect;
        public RectTransform CanvasRect
        {
            get
            {
                return _canvasRect ?? (_canvasRect =
                           UiCommon.UIManager.UIRoot.GetComponent<Canvas>().GetComponent<RectTransform>());
            }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitDict();
        }

        private void InitDict()
        {
            _idListCache = new Dictionary<EMarkType, HashSet<long>>();
            _markPool = new Dictionary<EMarkType, Dictionary<long, GameObject>>();
            foreach (EMarkType it in Enum.GetValues(typeof(EMarkType)))
            {
                _markPool.Add(it, new Dictionary<long, GameObject>());
                _idListCache.Add(it, new HashSet<long>());
            }

            _origGoDict = new Dictionary<EMarkType, GameObject>();
            try
            {
                _origGoDict.Add(EMarkType.Mother, _origMotherTf.gameObject);
                _origGoDict.Add(EMarkType.Hero, _origHeroTf.gameObject);
                _origGoDict.Add(EMarkType.Hum, _origHumTf.gameObject);
                _origMotherTf.gameObject.SetActive(false);
                _origHeroTf.gameObject.SetActive(false);
                _origHumTf.gameObject.SetActive(false);
            }
            catch(Exception e)
            {
                Logger.Error(e);
            }
        }

        private void InitVariable()
        {
            _origMotherTf = FindChildGo("MotherTag");
            _origHeroTf = FindChildGo("HeroTag");
            _origHumTf = FindChildGo("HumTag");
            _root = FindChildGo("Show");
        }

        private string curKillerName = string.Empty;
        private Vector3 curKillerPos = Vector3.zero;
        public override void Update(float interval)
        {
            UpdateVisible();
            UpdatePos();
        }

        private void UpdateVisible()
        {
            UpdateVisible(EMarkType.Mother, adapter.MotherIdList);
            UpdateVisible(EMarkType.Hum, adapter.HumanIdList);
            UpdateVisible(EMarkType.Hero, adapter.HeroIdList);
        }

        private void UpdateVisible(EMarkType type, List<long> idList)
        {
            var origList = _idListCache[type];
            if (origList.SetEquals(idList)) return;

            var newList = new HashSet<long>(idList);
            origList.ExceptWith(idList);
            _idListCache[type] = newList;

            foreach (var id in origList)
            {
                UIUtils.SetActive(GetMark(type, id), false);
            }

            //foreach (var id in newList)
            //{
            //    UIUtils.SetActive(GetMark(type, id), true);
            //}
        }

        private void UpdatePos()
        {
            UpdatePos(EMarkType.Mother, adapter.MotherIdList);
            UpdatePos(EMarkType.Hum, adapter.HumanIdList);
            UpdatePos(EMarkType.Hero, adapter.HeroIdList);
        }

        private void UpdatePos(EMarkType type, List<long> idList)
        {
            foreach (var id in idList)
            {
                var pos = adapter.GetTopPos(id);
                var go = GetMark(type, id);
                if (pos != null && UIUtils.InView(pos.Value))
                {
                    var pos2D = UIUtils.WorldPosToRect(pos.Value, CanvasRect);
                    (go.transform as RectTransform).anchoredPosition = pos2D;
                    UIUtils.SetActive(go, true);
                }
                else
                {
                    UIUtils.SetActive(go, false);
                }
            }
        }


        #region CreateMark

        private GameObject GetMark(EMarkType type,long id)
        {
            GameObject item = null;
            var dict = _markPool[type];

            return dict.TryGetValue(id,out item) ? item : CreateMark(type,id);
        }

        private GameObject CreateMark(EMarkType type,long id)
        {
            var origTf = _origGoDict[type];
            var dict = _markPool[type];
            var newGo = UnityEngine.Object.Instantiate(origTf, _root, false);
            dict[id] = newGo;
            return newGo;
        }

        #endregion
    }
}
