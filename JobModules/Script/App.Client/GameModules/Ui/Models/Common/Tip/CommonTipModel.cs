using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using UnityEngine.UI;
using UnityEngine;
using Assets.Utils.Configuration;
using Utils.Configuration;
using System.Collections.Generic;
using Assets.XmlConfig;
using UIComponent.Interface;
using System;
using App.Client.GameModules.Ui.Models.Common.Tip;
using UIComponent.UI.Manager;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class TipShowData
    {
        int categoryId;
        int temID;
        int contrastTemId;   //对比的武器的id
        int num;             //子弹个数

        public bool NeedContrast()
        {
            return contrastTemId > 0;
        }

        public TipShowData():this(-1,-1,-1,-1)
        {
        }

        public TipShowData(int _category, int _temID, int _contrastTemID, int _num)
        {
            categoryId = _category;
            temID = _temID;
            contrastTemId = _contrastTemID;
            num = _num;
            WeaponPartsList = new List<TipPartUiData>();
        }

        public bool IsMyWeapon { get; set; }
        public List<TipPartUiData> WeaponPartsList { get; set; }
        public int WeaponExp { get; set; }
        public int CategoryId
        {
            get
            {
                return categoryId;
            }

            set
            {
                categoryId = value;
            }
        }

        public int TemID
        {
            get
            {
                return temID;
            }

            set
            {
                temID = value;
            }
        }

        public int ContrastTemId
        {
            get
            {
                return contrastTemId;
            }

            set
            {
                contrastTemId = value;
            }
        }

        public int Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
            }
        }
    }

    public class CommonTipModel : AbstractModel, ITip
    {

        Contexts contexts;
        UiContext uiContext;
        private bool isGameObjectCreated = false;
        private TipShowData curShowTipData = new TipShowData();
        private List<Transform> attributeGoList = new List<Transform>();
        private CommonTipViewModel _viewModel = new CommonTipViewModel();
        private Transform targetTf = null;
        private Canvas canvas = null;
        private RectTransform rootTf;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
        }

        public CommonTipModel()
        {
            this.contexts = UiModule.contexts;
            uiContext = contexts.ui;
        }
        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            isGameObjectCreated = true;
            InitGUI();
        }

        private void InitGUI()
        {
            //this.SetVisible(false);
            HideTip();
            _viewModel.rootActiveSelf = true;          
            for (int i = 1; i <= 10; i++)
            {
                string name = "item" + i;
                var go = FindChildGo(name);
                if (go != null)
                    attributeGoList.Add(go);
            }           
        }

        #region 武器
        private void RefreshWeaponTips()
        {
            var weaponConfig = SingletonManager.Get<ItemBaseConfigManager>().GetConfigById((int)ECategory.Weapon, curShowTipData.TemID, true) as WeaponResConfigItem;

            if (weaponConfig == null)
                return;

            //刷新topGroup        
            {
                _viewModel.nameText = weaponConfig.Name;
                Loader.RetriveSpriteAsync(weaponConfig.IconBundle, weaponConfig.Icon,
                    (sprite) => { _viewModel.Icon = sprite; });

                _viewModel.infoOneActiveSelf = false;
                _viewModel.infoTwoActiveSelf = false;

                //刷新描述
                _viewModel.desText = weaponConfig.Description;
                switch ((EWeaponType_Config)weaponConfig.Type)
                {
                    case EWeaponType_Config.PrimeWeapon:
                        {
                            _viewModel.typeText = I2.Loc.ScriptLocalization.client_common.word21;
                            _viewModel.infoOneActiveSelf = true;
                            _viewModel.infoOneText = GetStrByCaliber(weaponConfig.Caliber);
                        }
                        break;
                    case EWeaponType_Config.SubWeapon:
                        {
                            _viewModel.typeText = I2.Loc.ScriptLocalization.client_common.word22;
                            _viewModel.infoOneActiveSelf = true;
                            _viewModel.infoOneText = GetStrByCaliber(weaponConfig.Caliber);
                        }
                        break;
                    case EWeaponType_Config.MeleeWeapon:
                        {
                            _viewModel.typeText = I2.Loc.ScriptLocalization.client_common.word23;
                        }
                        break;
                    case EWeaponType_Config.ThrowWeapon:
                        {
                            _viewModel.typeText = I2.Loc.ScriptLocalization.client_common.word24;
                            if (weaponConfig.SubType == 9)    //手雷
                            {
                                if (weaponConfig.Weight > 0)
                                {
                                    _viewModel.infoOneActiveSelf = true;
                                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word25 + (curShowTipData.Num * weaponConfig.Weight).ToString();
                                }
                            }
                            else
                            {
                                if (weaponConfig.Weight > 0)
                                {
                                    _viewModel.infoOneActiveSelf = true;
                                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word25 + weaponConfig.Weight.ToString();
                                }                                
                            }
                        }
                        break;
                    case EWeaponType_Config.Armor:
                    case EWeaponType_Config.Helmet:
                    {
                        RefreshDurableEquimentTips(weaponConfig);
                            return;
                    }
                        
                }
            }
            RefreshWeaponProperty();
            

        }

        private void RefreshWeaponProperty()
        {
            var weapongProperConfig = SingletonManager.Get<WeaponPropertyConfigManager>().FindByWeaponId(curShowTipData.TemID);
            WeaponPropertyConfigItem contrastWeaponProperConfig = null;
            if (curShowTipData.ContrastTemId > 0)
                contrastWeaponProperConfig = SingletonManager.Get<WeaponPropertyConfigManager>().FindByWeaponId(curShowTipData.ContrastTemId);
            if (weapongProperConfig == null) return;
            _viewModel.attrGroupActiveSelf = true;
            //刷新属性
            {
                foreach (var tran in attributeGoList)
                {
                    tran.gameObject.SetActive(false);
                }
                int usingIndex = 0;
                //威力
                float destValue = 0;
                if (weapongProperConfig.Power != 0 && usingIndex < attributeGoList.Count)
                {

                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Power;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Power, destValue, I2.Loc.ScriptLocalization.client_common.word26);
                }

                //射数
                if (weapongProperConfig.Limitcycle != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Limitcycle;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Limitcycle, destValue, I2.Loc.ScriptLocalization.client_common.word27);
                }

                //精准
                if (weapongProperConfig.Accurate != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Accurate;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Accurate, destValue, I2.Loc.ScriptLocalization.client_common.word28);
                }

                //稳定
                if (weapongProperConfig.Stability != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Stability;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Stability, destValue, I2.Loc.ScriptLocalization.client_common.word29);
                }
                //穿透
                if (weapongProperConfig.Penetrate != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Penetrate;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Penetrate, destValue, I2.Loc.ScriptLocalization.client_common.word30);
                }
                //范围
                if (weapongProperConfig.Scope != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Scope;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Scope, destValue, I2.Loc.ScriptLocalization.client_common.word31);
                }
                //装弹数
                if (weapongProperConfig.Bullet != 0 && usingIndex < attributeGoList.Count)
                {
                    if (contrastWeaponProperConfig != null)
                        destValue = contrastWeaponProperConfig.Bullet;
                    RefreshAttributeItem(attributeGoList[usingIndex++], weapongProperConfig.Bullet, destValue, I2.Loc.ScriptLocalization.client_common.word32);
                }
            }
        }

        private void RefreshDurableEquimentTips(WeaponResConfigItem data)
        {
            if (data == null) return;
            //刷新topGroup
            _viewModel.attrGroupActiveSelf = false;
            _viewModel.typeText = GetDurableTypeText((EWeaponType_Config) data.Type);
            _viewModel.nameText = data.Name;
            _viewModel.infoOneActiveSelf = true;
            _viewModel.infoOneText =
                data.Durable > 0 ? I2.Loc.ScriptLocalization.client_common.word36 + data.Durable : "";

            _viewModel.infoTwoText =
                data.Weight > 0 ? I2.Loc.ScriptLocalization.client_common.word25 + data.Weight : "";
        }

        private string GetDurableTypeText(EWeaponType_Config type)
        {
            switch (type)
            {
                case EWeaponType_Config.Armor:
                    return "防弹衣";
                case EWeaponType_Config.Helmet:
                    return "头盔";
                default:
                    return string.Empty;
            }
        }

        private string GetStrByCaliber(int Caliber)
        {
            string result = "";
            switch (Caliber)
            {
                case 1:
                    {
                        result = "556";
                    }
                    break;
                case 2:
                    {
                        result = "762";
                    }
                    break;
                case 3:
                    {
                        result = "9MM";
                    }
                    break;
                case 4:
                    {
                        result = "45acp";
                    }
                    break;
                case 5:
                    {
                        result = "12no";
                    }
                    break;
                case 6:
                    {
                        result = "300Mag";
                    }       
                    break;
                case 7:
                    {
                        result = "50AE";
                    }
                    break;
            }
            return result + I2.Loc.ScriptLocalization.client_common.word33;
        }
        private void RefreshAttributeItem(Transform go, float source, float dest, string titleName)
        {
            go.gameObject.SetActive(true);
            var title = go.Find("Text");
            var slider = go.Find("Slider");
            var cSlider = go.Find("CSlider");
            if (title && slider && cSlider)
            {
                var titleCom = title.GetComponent<Text>();
                var sliderCom = slider.GetComponent<Slider>();
                var fillArea = slider.Find("Fill Area");
                var sliderCollor = fillArea.Find("Fill");
                var cSliderCom = cSlider.GetComponent<Slider>();
                var cfillArea = cSlider.Find("Fill Area");
                var csliderCollor = cfillArea.Find("Fill");
                if (titleCom && sliderCom && cSliderCom && sliderCollor && csliderCollor)
                {
                    var sliderCollorCom = sliderCollor.GetComponent<Image>();
                    var csliderCollorCom = csliderCollor.GetComponent<Image>();
                    if (csliderCollorCom && sliderCollorCom)
                    {
                        titleCom.text = titleName;
                        if (dest == 0)
                        {
                            slider.SetAsLastSibling();
                            sliderCollorCom.color = Color.white;
                            csliderCollorCom.color = Color.red;
                        }
                        else
                        {
                            if (source > dest)
                            {
                                cSlider.SetAsLastSibling();
                                sliderCollorCom.color = Color.green;
                                csliderCollorCom.color = Color.white;
                            }
                            else
                            {
                                slider.SetAsLastSibling();
                                sliderCollorCom.color = Color.white;
                                csliderCollorCom.color = Color.red;
                            }
                        }
                        sliderCom.value = source / (float)100;
                        cSliderCom.value = dest / (float)100;
                    }
                }
            }
        }
        #endregion

        #region 非武器
        private void RefreshNoWeaponTips()
        {
            _viewModel.attrGroupActiveSelf = false;
            var category = curShowTipData.CategoryId;
            var temID = curShowTipData.TemID;
            switch ((ECategory)category)
            {
                case ECategory.GameItem:     //子弹 消耗道具
                    {
                        RefreshNoWeaponTipsBattleProp(temID);
                    }
                    break;
                case ECategory.WeaponPart:    //武器配件
                    {
                        RefreshNoWeaponTipsWeaponPart(temID);
                    }
                    break;
                case ECategory.Avatar:    //装扮  （背包防具）
                    {
                        RefreshNoWeaponTipsRoleAvatar(temID);
                    }
                    break;
                case ECategory.Prop:       //道具 
                    {
                        RefreshNoWeaponTipsProp(temID);
                    }
                    break;
                default:
                    {
                        Debug.Log(I2.Loc.ScriptLocalization.client_common.word34);
                    }
                    break;
            }
        }

        private void RefreshNoWeaponTipsBattleProp(int temID)
        {
            //var data = SingletonManager.Get<GameItemConfigManager>().GetConfigById(temID);
            var data = GetConfig(ECategory.GameItem, temID) as GameItemConfigItem;

            if (data != null)
            {
                _viewModel.nameText = data.Name;
                _viewModel.typeText = SingletonManager.Get<GameItemConfigManager>().GetTypeStrById(temID);
                Loader.RetriveSpriteAsync(data.IconBundle, data.Icon,
                      (sprite) => { _viewModel.Icon = sprite; });

                if (curShowTipData.Num > 0)
                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word35 + data.Weight * curShowTipData.Num;
                else
                    _viewModel.infoOneText = "";
                _viewModel.infoTwoText = "";
                _viewModel.desText = data.Description;
            }
        }

        ItemBaseConfig GetConfig(ECategory cat, int id,bool isSurvivalMode = false)
        {
            return SingletonManager.Get<ItemBaseConfigManager>().GetConfigById((int)cat,id, isSurvivalMode);
        }

        private void RefreshNoWeaponTipsWeaponPart(int temID)
        {
            var data = GetConfig(ECategory.WeaponPart, temID,true) as WeaponPartsConfigItem;
            //var data = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(temID);
            if (data != null)
            {
                //刷新topGroup
                _viewModel.nameText = data.Name;
                _viewModel.typeText = SingletonManager.Get<WeaponPartsConfigManager>().GetTypeStrById(temID);

                if (data.Weight > 0)
                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word25 + data.Weight.ToString();
                else
                    _viewModel.infoOneText = "";
                _viewModel.infoTwoText = "";
                Loader.RetriveSpriteAsync(data.IconBundle, data.Icon,
                        (sprite) => { _viewModel.Icon = sprite; });

                //刷新描述
                _viewModel.desText = data.Description;
            }
        }

        private void RefreshNoWeaponTipsRoleAvatar(int temID)
        {
            //var data = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(temID);
            var data = GetConfig(ECategory.Avatar, temID) as RoleAvatarConfigItem;
            if (data != null)
            {
                //刷新topGroup
                _viewModel.nameText = data.Name;
                _viewModel.typeText = SingletonManager.Get<RoleAvatarConfigManager>().GetTypeNameById(temID);
                {
                    var roleModeID = this.contexts.player.flagSelfEntity.playerInfo.RoleModelId;
                    var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModeID).Sex;
                    var sexEnumValue = (global::Utils.CharacterState.Sex)(sex);
                    var assetName = SingletonManager.Get<RoleAvatarConfigManager>().GetIcon(temID, sexEnumValue);
                    Loader.RetriveSpriteAsync(data.IconBundle, assetName,
                        (sprite) => { _viewModel.Icon = sprite; });
                }
                if (data.Capacity > 0)
                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word36 + data.Capacity;
                else
                    _viewModel.infoOneText = "";

                if (data.Weight > 0)
                    _viewModel.infoTwoText = I2.Loc.ScriptLocalization.client_common.word25 + data.Weight;
                else
                    _viewModel.infoTwoText = "";

                //刷新描述
                _viewModel.desText = data.Description;
            }
        }

        private void RefreshNoWeaponTipsProp(int temID)
        {
            //var data = SingletonManager.Get<PropConfigManager>().GetConfigById(temID);
            var data = GetConfig(ECategory.Prop, temID) as PropConfigItem;
            if (data != null)
            {
                //刷新topGroup
                _viewModel.nameText = data.Name;
                _viewModel.typeText = SingletonManager.Get<PropConfigManager>().GetTypeNameById(temID);
                Loader.RetriveSpriteAsync(data.IconBundle, data.Icon,
                     (sprite) => { _viewModel.Icon = sprite; });

                if (data.Weight > 0)
                    _viewModel.infoOneText = I2.Loc.ScriptLocalization.client_common.word25 + data.Weight;
                else
                    _viewModel.infoOneText = "";
                _viewModel.infoTwoText = "";

                //刷新描述
                _viewModel.desText = data.Description;
            }
        }
        #endregion

        #region Tips
        public void ShowTip(object data, Transform targetTf)
        {
            //this.SetVisible(false);  //暂时屏蔽
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
            if (showData != null && isGameObjectCreated)
            {
                curShowTipData.CategoryId = showData.CategoryId;
                curShowTipData.TemID = showData.TemID;
                curShowTipData.ContrastTemId = showData.ContrastTemId;
                curShowTipData.Num = showData.Num;

                if (showData.CategoryId == (int)ECategory.Weapon) //武器
                    RefreshWeaponTips();
                else
                    RefreshNoWeaponTips();
            }
            //HidePos();
            this.SetVisible(true);
        }

        private void HidePos()
        {
            var tf = GetTipRt();
            var pos = tf.anchoredPosition;
            pos.x = -9999;
            tf.anchoredPosition = pos;
        }

        public void HideTip()
        {
            HidePos();
            this.SetVisible(false);
        }



        public RectTransform GetTipRt()
        {
            return rootTf ?? (rootTf = FindChildGo("root") as RectTransform);
        }

        public Vector2 GetOffset()
        {
            return Vector2.zero;
        }
        #endregion
    }
}
