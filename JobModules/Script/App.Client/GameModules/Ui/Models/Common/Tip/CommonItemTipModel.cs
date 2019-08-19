using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.Models.Common.Tip;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Enums;
using UIComponent.Interface;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common
{

    public class CommonItemTipModel : AbstractModel, ITip
    {
        private CommonItemTipViewModel viewModel = new CommonItemTipViewModel();

        protected override IUiViewModel ViewModel
        {
            get { return viewModel; }
        }

        private Vector2 offset;

        public CommonItemTipModel()
        {
            offset = new Vector2(20, -20);
        }

        UIList partsUIList;
        UIList attrUIList;

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            Init();
            HidePos();
        }

        private void Init()
        {
            partsUIList = FindComponent<UIList>("PartsGroup");
            partsUIList.OnceInit = () =>
            {
                Transform line = partsUIList.transform.GetChild(0);
                if (line == null) return;
                line.SetAsLastSibling();
            };

            attrUIList = FindComponent<UIList>("AttrGroup");
            attrUIList.OnceInit = () =>
            {
                Transform line = attrUIList.transform.GetChild(0);
                if (line == null) return;
                line.SetAsLastSibling();
            };
            mRt = ViewInstance.transform as RectTransform;
        }

        private RectTransform mRt;

        protected void Refresh()
        {
            RefreshCommonData();
            RefreshWeapon();
            RefreshWeaponAttr();
            RefreshSuper();

            LayoutRebuilder.ForceRebuildLayoutImmediate(mRt);
            SetVisible(true);
        }

        private ItemBaseConfig GetConfig(int cat, int id,bool isSurvivalMode = false)
        {
            var config = SingletonManager.Get<ItemBaseConfigManager>()
                .GetConfigById((int)cat, id, isSurvivalMode);
            return config;
        }

        private ItemBaseConfig GetConfig(bool isSurvivalMode = false)
        {
            var config = GetConfig((int) curShowTipData.CategoryId, curShowTipData.TemID, isSurvivalMode);
            return config;
        }

        void RefreshCommonData()
        {
            var config = GetConfig();
            var assetIconInfo = AssetBundleConstant.GetTipQualityAssetInfo(config.Xlv);
            Loader.RetriveSpriteAsync(assetIconInfo.BundleName, assetIconInfo.AssetName,
                (sprite) => viewModel.ItemTipQuality = sprite);

            viewModel.NameText = config.Name;
            viewModel.Desc = config.Description;
            viewModel.SymbolBorderActive = true;

            if (config.Category == (int)ECategory.Role)
            {
                var camp = (EUICampType)(config as RoleItem).Camp;
                assetIconInfo = AssetBundleConstant.GetCampSmallIcon(camp);
            }
            else if (config.Category == (int)ECategory.Avatar)
            {
                var camp = (EUICampType)SingletonManager.Get<RoleAvatarConfigManager>().GetCamp(config.Id);
                if (camp == EUICampType.None)
                {
                    viewModel.SymbolBorderActive = false;
                    return;
                }
                else
                {
                    assetIconInfo = AssetBundleConstant.GetCampSmallIcon(camp);
                }
            }
            else if (config.Category == (int)ECategory.Weapon)
            {
                int workShop = (config as WeaponResConfigItem).Workshop;
                assetIconInfo = SingletonManager.Get<WeaponWorkShopConfigManager>().GetWorkShopIcon(workShop);
            }
            else if (config.Category == (int)ECategory.WeaponPart)
            {
                int workShop = (config as WeaponPartsConfigItem).Workshop;
                assetIconInfo = SingletonManager.Get<WeaponWorkShopConfigManager>().GetWorkShopIcon(workShop);
            }
            else
            {
                viewModel.SymbolBorderActive = false;
                return;
            }

            Loader.RetriveSpriteAsync(assetIconInfo.BundleName, assetIconInfo.AssetName,
                (sprite) => viewModel.SymbolSprite = sprite);

            assetIconInfo = AssetBundleConstant.GetSmallItemQualityLayer(config.Xlv);
            Loader.RetriveSpriteAsync(assetIconInfo.BundleName, assetIconInfo.AssetName,
                (sprite) => viewModel.SymbolBorderSprite = sprite);
        }

        void RefreshWeapon()
        {
            viewModel.LevelGroupActive = false;
            viewModel.MaxLevelGroupActive = false;
            viewModel.ExpGroupActive = false;
            viewModel.PartsDescActive = false;
            viewModel.PartsGroupActive = false;
            var config = GetConfig();

            if (config.Category == (int)ECategory.Weapon)
            {
                if (IsWeaponAvatar(config))
                {
                    return;
                }
                if (IsMyWeapon())
                {
                    RefreshWeaponExp();
                    RefreshWeaponParts();
                }
                else
                {
                    int maxLv = SingletonManager.Get<WeaponUpdateConfigManager>()
                        .GetMaxLv(config.Id);
                    if (maxLv > 0)
                    {
                        viewModel.ExpGroupActive = true;
                        viewModel.LevelGroupActive = true;
                        viewModel.LevelText = "1";
                        viewModel.MaxLevelText = "/" + maxLv;
                        viewModel.ExpBar = 0;
                    }
                }
            }
        }

        private bool IsWeaponAvatar(ItemBaseConfig config)
        {
            return config.Category == (int)ECategory.Weapon && config.Type > (int)EWeaponType_Config.Armor;
        }

        private bool IsMyWeapon()
        {
            //return true;
            return curShowTipData.IsMyWeapon;
        }

        private void RefreshWeaponExp()
        {
            var exp = curShowTipData.WeaponExp;
            var config = GetConfig();
            int level;
            float ratio;
            var maxLevel = SingletonManager.Get<WeaponUpdateConfigManager>().GetMaxLvAndLevel(config.Id,exp,out level,out ratio);
            if(maxLevel > 0)  //说明可升级
            {
                if (maxLevel == level)
                {
                    viewModel.MaxLevelGroupActive = true;
                }
                else
                {
                    viewModel.ExpGroupActive = true;
                    viewModel.LevelGroupActive = true;
                    viewModel.LevelText = level.ToString();
                    viewModel.MaxLevelText = "/" + maxLevel;
                    viewModel.ExpBar = ratio;
                }
            }
            
        }

        void RefreshWeaponParts()
        {
            int equipNum = curShowTipData.WeaponPartsList.Count;
            int id = curShowTipData.TemID;
            var slotList = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(id).ApplyPartsSlot;

            int allNum = slotList.Count;
            if (allNum > 0)
            {
                viewModel.PartsDescActive = true;
                viewModel.PartsGroupActive = true;
                viewModel.EquipNum = equipNum + "/";
                viewModel.AllNum = allNum.ToString();

                RefreshWeaponPartsItem(curShowTipData.WeaponPartsList);
            }
        }

        List<WeaponPartsConfigItem> partsDataList = new List<WeaponPartsConfigItem>();

        void RefreshWeaponPartsItem(List<TipPartUiData> slotList)
        {
            partsUIList.SetDataList<TipPartItem, TipPartUiData>(slotList);
        }

        List<ContrastPropertyItemData> arrtDataList = new List<ContrastPropertyItemData>();

        WeaponProperty GetWeaponProperty(int id)
        {
            WeaponProperty wp = new WeaponProperty();

            var propertyConfig = SingletonManager.Get<WeaponPropertyConfigManager>().FindByWeaponId(id);
            if (propertyConfig != null)
            {
                wp = propertyConfig.GetWeaponProperty();
            }

            return wp;
        }

        void RefreshWeaponAttr()
        {
            var config = GetConfig();
            viewModel.AttrGroupActive = false;
            WeaponProperty contrastProperty = new WeaponProperty();
            if (config.Category == (int)ECategory.Weapon ||
                config.Category == (int)ECategory.WeaponPart) //需要加上武器配件的//
            {
                if (IsWeaponAvatar(config))
                {
                    return;
                }
                if (curShowTipData.NeedContrast())
                {
                    contrastProperty = GetWeaponProperty(curShowTipData.ContrastTemId);
                }
                viewModel.AttrGroupActive = true;
                arrtDataList.Clear();//todo
                WeaponProperty wp = new WeaponProperty();
                if (config.Category == (int)ECategory.Weapon)
                {
                    wp = GetWeaponProperty(config.Id);

                    if (IsMyWeapon())//todo,还需添加装备配件的属性
                    {
                        foreach (var it in curShowTipData.WeaponPartsList)
                        {
                            var newWp = GetWeaponProperty(it.Id);
                            wp.Add(newWp);
                        }
                    }
                }

                foreach (var item in TipConst.WeaponPropertyName)
                {
                    string propertyName = item.Key;
                    if (propertyName.Equals(TipConst.BulletPropertyName) ||
                        propertyName.Equals(TipConst.MaxBulletPropertyName))
                    {
                        continue;
                    }

                    var cpropertyInfo = wp.GetType().GetField(propertyName);
                    float propertyValue = (float)cpropertyInfo.GetValue(wp);
                    if (propertyValue > 0)
                    {
                        var itemData = new ContrastPropertyItemData()
                        {
                            IsBullet = false,
                            Value = propertyValue,
                            Name = TipConst.GetWeaponPropertyName(propertyName),
                        };
                        if (curShowTipData.NeedContrast())
                        {
                            var contrastProperValue = (float)cpropertyInfo.GetValue(contrastProperty);
                            itemData.DiffValue = itemData.Value - contrastProperValue;
                        }
                        arrtDataList.Add(itemData);
                    }
                }

                int BulletValue = wp.Bullet;
                int MaxBulletValue = wp.Bulletmax;
                if (BulletValue != 0 && MaxBulletValue != 0)
                {
                    var itemData = new ContrastPropertyItemData()
                    {
                        IsBullet = true,
                        Bullet = BulletValue,
                        MaxBullet = MaxBulletValue,
                        Name = TipConst.GetWeaponPropertyName(TipConst.BulletPropertyName),
                    };
                    arrtDataList.Add(itemData);
                }

                attrUIList.SetDataList<WeaponPropertyBarItem, ContrastPropertyItemData>(arrtDataList);
            }
        }

        void RefreshSuper()
        {
            viewModel.SuperGroupActive = false;
        }

        public void ShowTip(object data, Transform targetTf)
        {
            ShowItemTip(data, targetTf);
        }

        public void HideTip()
        {
            HidePos();
            SetVisible(false);
        }

        private void HidePos()
        {
            var tf = GetTipRt();
            var pos = tf.anchoredPosition;
            pos.x = -9999;
            tf.anchoredPosition = pos;
        }

        public RectTransform GetTipRt()
        {
            return mRt;
        }

        public Vector2 GetOffset()
        {
            return offset;
        }

        public bool IsVisible
        {
            get { return isVisible; }
        }

        private Canvas canvas = null;
        private TipShowData curShowTipData = new TipShowData();
        private void ShowItemTip(object data, Transform targetTf)
        {
            if (canvas == null)
            {
                var canvasArray = this.FindAllComponents<Canvas>();
                if (canvasArray.Length > 0)
                {
                    canvas = canvasArray[0];
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 1000;
                }
            }

            var showData = data as TipShowData;
            if (showData != null && _viewInitialized)
            {
                curShowTipData = showData;
                Refresh();
            }
            //HidePos();
            this.SetVisible(true);
        }
    }
}
