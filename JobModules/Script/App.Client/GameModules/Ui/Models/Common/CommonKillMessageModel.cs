using System;
using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Shared.Components.Ui;
using App.Shared.Components.Ui.UiAdapter;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonKillMessageModel : ClientAbstractModel, IUiSystem
    {
        private bool test = false;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonKillMessageModel));
        private CommonKillMessageViewModel _viewModel = new CommonKillMessageViewModel();

        private IKillMessageUiAdapter _adapter;

        private int _cellLength = 5;//消息框最多显示的消息数量

        private float _cellViewHeight;//消息的高度，用于图标比例自适应;

        private List<Transform> _infoItemList = new List<Transform>(); //正在使用的

        private Transform infoItemModel = null;
        private Transform infoItemRoot = null;

        private Color _teamMateColor = new Color32(0x93, 0xec, 0x82, 0xff);
        private Color _nonTeamMateColor = new Color32(0xff, 0x5e, 0x5e, 0xff);
	    private Dictionary<AssetInfo, Sprite> _assetPool = new Dictionary<AssetInfo, Sprite>(AssetInfo.AssetInfoComparer.Instance);


		private class CellInfo//击杀消息所包含的内容
        {
            public UIText DeadName;
            public Image HeadShotIcon;
            public UIText KillerName;
            public Image DeathTypeIcon;
            public Image HurtShotIcon;
            public Transform Root;
            public Canvas RootCanvas;
        }

        private List<CellInfo> _cellInfoRefList = new List<CellInfo>();//消息索引对应的内容引用
        private int _curCellIndex;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitKillInfos();
            _viewModel.Show = true;
        }

        private void InitKillInfos()
        {
            infoItemRoot = FindChildGo("PlayerInfoGroup");
            infoItemModel = FindChildGo("KillMessageInfo");
            if (infoItemRoot == null || infoItemModel == null)
                return;

            infoItemModel.GetComponent<Canvas>().enabled = false;
            _cellViewHeight = infoItemModel.GetComponent<RectTransform>().sizeDelta.y;

            for (int index = 0; index < _cellLength; index++)
            {
                Transform transform = UnityEngine.Object.Instantiate(infoItemModel, infoItemRoot);
                _infoItemList.Add(transform);
                InitCellInfoRef(transform);
                (transform as RectTransform).anchoredPosition -= new Vector2(0, (5 + _cellViewHeight) * index);
            }

        }

        private void InitCellInfoRef(Transform transform)
        {
            CellInfo cellInfo = new CellInfo();
            cellInfo.DeadName = transform.Find("DeadName").GetComponent<UIText>();
            cellInfo.DeathTypeIcon = transform.Find("DeathTypeIcon").GetComponent<Image>();
            cellInfo.HeadShotIcon = transform.Find("HeadShotIcon").GetComponent<Image>();
            cellInfo.HurtShotIcon = transform.Find("HurtShotIcon").GetComponent<Image>();
            cellInfo.KillerName = transform.Find("KillerName").GetComponent<UIText>();
            cellInfo.Root = transform;
            cellInfo.RootCanvas = transform.GetComponent<Canvas>();
            _cellInfoRefList.Add(cellInfo);
        }

        public CommonKillMessageModel(IKillMessageUiAdapter adapter):base(adapter)
        {
            _adapter = adapter;
        }

       public override void Update(float interval)
        {      
            _adapter.UpdateKillInfo();

            if (!_adapter.KillMessageChanged)
            {
                return;
            }
            UpdateKillMessageInfo();
            _adapter.KillMessageChanged = false;
            
        }
            
        /// <summary>
        /// 更新击杀信息
        /// </summary>
        private void UpdateKillMessageInfo()
        {
            for (int i = 0; i < _cellInfoRefList.Count; i++)
            {
                _curCellIndex = i;
                var tf = _infoItemList[_curCellIndex];
                var cell = _cellInfoRefList[i];
                if (_adapter.KillInfos.Count > i)
                {
                    IKillInfoItem item = _adapter.KillInfos[i];
                    UpdateMessageToCell(item, cell);
                    UIUtils.SetEnable(cell.RootCanvas, true);
                    UpdateLayout(item,cell);
                }
                else
                {
                    UIUtils.SetEnable(cell.RootCanvas, false);
                }
            }
        }

        private void UpdateLayout(IKillInfoItem item, CellInfo cell)
        {
            Vector2 totalWidth = new Vector2();
            if (cell.DeadName.enabled)
            {
                var size = cell.DeadName.rectTransform.sizeDelta;
                cell.DeadName.rectTransform.anchoredPosition = totalWidth;
                totalWidth -= new Vector2(size.x, 0);
            }

            var haveHurtIcon = cell.HurtShotIcon.enabled;
            var haveDeathIcon = cell.DeathTypeIcon.enabled;
            var haveKillerName = cell.KillerName.rectTransform.sizeDelta.x > 0;
            var haveHeadShotIcon = cell.HeadShotIcon.enabled;
            if (haveHurtIcon || haveDeathIcon)//icon和text留20pixel的空隙
            {
                totalWidth -= new Vector2(20, 0);
            }

            if (haveHurtIcon)
            {
                var size = (cell.HurtShotIcon.rectTransform).sizeDelta;
                (cell.HurtShotIcon.rectTransform).anchoredPosition = totalWidth;
                totalWidth -= new Vector2(size.x, 0);
            }
            if (haveDeathIcon)
            {
                if (haveHurtIcon)
                {
                    totalWidth -= new Vector2(5, 0);//icon和icon之间留5pixel

                }
                if (item.deathType == EUIDeadType.Weapon || item.deathType == EUIDeadType.Unarmed)//需要翻转
                {
                    cell.DeathTypeIcon.rectTransform.localScale = new Vector3(-1, 1, 1);
                    var size = (cell.DeathTypeIcon).rectTransform.sizeDelta;
                    totalWidth -= new Vector2(size.x, 0);
                    cell.DeathTypeIcon.rectTransform.anchoredPosition = totalWidth;
                }
            else
                {
                    cell.DeathTypeIcon.rectTransform.localScale = new Vector3(1, 1, 1);
                    var size = (cell.DeathTypeIcon).rectTransform.sizeDelta;
                    cell.DeathTypeIcon.rectTransform.anchoredPosition = totalWidth;
                    totalWidth -= new Vector2(size.x, 0);
            }
        }

            if (haveKillerName)
            {
                totalWidth -= new Vector2(20, 0);

                var size = (cell.KillerName).rectTransform.sizeDelta;
                cell.KillerName.rectTransform.anchoredPosition = totalWidth;
                totalWidth -= new Vector2(size.x, 0);

            }

            if (haveHeadShotIcon)
            {
                totalWidth -= new Vector2(20, 0);

                var size = (cell.HeadShotIcon.rectTransform).sizeDelta;
                (cell.HeadShotIcon.rectTransform).anchoredPosition = totalWidth;
                totalWidth -= new Vector2(size.x, 0);

            }

            (cell.Root as RectTransform).sizeDelta = new Vector2(-totalWidth.x,_cellViewHeight);

        }

        //private TextGenerationSettings settings;
        /// <summary>
        /// 更新击杀信息
        /// </summary>
        private void UpdateMessageToCell(IKillInfoItem item,CellInfo cell)
        {
            SetDeadAndKillerInfo(item,cell);
            SetDeathTypeIcon(item,cell);
            SetAtkInfoIcon(item,cell);
        }

        
        /// <summary>
        /// 设置击杀类型图标,爆头和击倒
        /// </summary>
        private void SetAtkInfoIcon(IKillInfoItem item,CellInfo cell)
        {
            var type = item.killType;
            UIUtils.SetEnable(cell.HeadShotIcon, (type & (int)EUIKillType.Crit) == (int)EUIKillType.Crit);
            UIUtils.SetEnable(cell.HurtShotIcon, (type & (int)EUIKillType.Hit) == (int)EUIKillType.Hit);

        }

        /// <summary>
        /// 设置击杀信息中死亡条件对应的图标
        /// </summary>
        private void SetDeathTypeIcon(IKillInfoItem item, CellInfo cell)
        {
			string bundleName = AssetBundleConstant.Icon_Killinfo;
			String deathIconName = null;

            UIUtils.SetActive(cell.KillerName.transform, true);

            var deathType = item.deathType;

            cell.DeathTypeIcon.rectTransform.sizeDelta = new Vector2(0, _cellViewHeight);
            if (deathType <= 0)
            {
                return;
            }
            switch (deathType)
            {
                case EUIDeadType.Weapon:
	                bundleName = item.weaponAsset.BundleName;
	                deathIconName = item.weaponAsset.AssetName;
                    break;
                default:
                    var cfg = SingletonManager.Get<TypeForDeathConfigManager>();
                    var deathTypeItem = cfg.GetConfigById((int)deathType);
                    bundleName = deathTypeItem.BundleName;
                    deathIconName = deathTypeItem.KillIcon;
                    break;
            }


            if (null != bundleName && null != deathIconName)
            {
                AssetInfo assetInfo = new AssetInfo(bundleName, deathIconName);
	            if (_assetPool.ContainsKey(assetInfo))
	            {
		            var image = _assetPool[assetInfo];
		            cell.DeathTypeIcon.sprite = image;
	                if (image == null || image.rect.height == 0)
                    {
                        return;
	                }
                    cell.DeathTypeIcon.rectTransform.sizeDelta =
			            new Vector2(_cellViewHeight * image.rect.width / image.rect.height, _cellViewHeight);
				}
	            else
	            {
		            _assetPool.Add(assetInfo, new Sprite());
		            Loader.RetriveSpriteAsync(bundleName, deathIconName, (sprite) =>
		            {
		                if (sprite == null || sprite.rect.height == 0)
		                {
		                    return;
		                }
                        cell.DeathTypeIcon.sprite = sprite;
			            cell.DeathTypeIcon.rectTransform.sizeDelta =
				            new Vector2(_cellViewHeight * sprite.rect.width / sprite.rect.height, _cellViewHeight);
			            _assetPool[assetInfo] = sprite;
		                UpdateLayout(item, cell);
		            });
				}
				
            }
        }
        
        /// <summary>
        /// 设置击杀信息中死者和击杀者信息
        /// </summary>
        private void SetDeadAndKillerInfo(IKillInfoItem item, CellInfo cell)
        {
 
            CellInfo cellInfo = cell;

            cellInfo.DeadName.text = item.deadName;
            float tempWidth1 = cellInfo.DeadName.preferredWidth;
            cellInfo.DeadName.rectTransform.sizeDelta = new Vector2(tempWidth1, _cellViewHeight);

            cellInfo.KillerName.text = item.killerName;
            float tempWidth2 = cellInfo.KillerName.preferredWidth;
            cellInfo.KillerName.rectTransform.sizeDelta = new Vector2(tempWidth2, _cellViewHeight);
	        bool isKillerInTeam = item.killerTeamId == _adapter.SelfTeamId;
	        bool isDeadInTeam = item.deadTeamId == _adapter.SelfTeamId;

            cellInfo.KillerName.color = isKillerInTeam ? _teamMateColor : _nonTeamMateColor;
            cellInfo.DeadName.color = isDeadInTeam ? _teamMateColor : _nonTeamMateColor;


        }

    }

}
