using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Assets.UiFramework.Libs;
using UnityEngine.UI;
using UIComponent.UI;

namespace App.Client.GameModules.Ui.ViewModels.Chicken
{
    public class ChickenBagViewModel : ViewModelBase, IUiViewModel
    {
        private class ChickenBagView : UIView
        {
            public Text WeightText;
            [HideInInspector]
            public string oriWeightText;
            public GameObject GearLastingLayerShow1;
            public Image GearLastingLayerValue1;
            public Image GearIconSprite1;
            public UIImageLoader GearIconAsset1;
            public UIImageLoader GearIconBundle1;
            public GameObject GearIconGroupShow1;
            public GameObject GearLastingBgShow1;
            public Text GearLastingNumText1;
            public Text GearNameText1;
            public GameObject GearLastingLayerShow2;
            public Image GearLastingLayerValue2;
            public Image GearIconSprite2;
            public UIImageLoader GearIconAsset2;
            public UIImageLoader GearIconBundle2;
            public GameObject GearIconGroupShow2;
            public GameObject GearLastingBgShow2;
            public Text GearLastingNumText2;
            public Text GearNameText2;
            public GameObject GearLastingLayerShow3;
            public Image GearLastingLayerValue3;
            public Image GearIconSprite3;
            public UIImageLoader GearIconAsset3;
            public UIImageLoader GearIconBundle3;
            public GameObject GearIconGroupShow3;
            public GameObject GearLastingBgShow3;
            public Text GearLastingNumText3;
            public Text GearNameText3;
            public GameObject GearLastingLayerShow4;
            public Image GearLastingLayerValue4;
            public Image GearIconSprite4;
            public UIImageLoader GearIconAsset4;
            public UIImageLoader GearIconBundle4;
            public GameObject GearIconGroupShow4;
            public GameObject GearLastingBgShow4;
            public Text GearLastingNumText4;
            public Text GearNameText4;
            public GameObject UpperRailSlotShow1;
            public GameObject MuzzleSlotShow1;
            public GameObject LowerRailSlotShow1;
            public GameObject MagazineSlotShow1;
            public GameObject StockSlotShow1;
            public UIImageLoader UpperRailIconAsset1;
            public UIImageLoader UpperRailIconBundle1;
            public GameObject UpperRailIconShow1;
            public UIImageLoader MuzzleIconAsset1;
            public UIImageLoader MuzzleIconBundle1;
            public GameObject MuzzleIconShow1;
            public UIImageLoader LowerRailIconAsset1;
            public UIImageLoader LowerRailIconBundle1;
            public GameObject LowerRailIconShow1;
            public UIImageLoader MagazineIconAsset1;
            public UIImageLoader MagazineIconBundle1;
            public GameObject MagazineIconShow1;
            public UIImageLoader StockIconAsset1;
            public UIImageLoader StockIconBundle1;
            public GameObject StockIconShow1;
            public GameObject UpperRailSlotShow2;
            public GameObject MuzzleSlotShow2;
            public GameObject LowerRailSlotShow2;
            public GameObject MagazineSlotShow2;
            public GameObject StockSlotShow2;
            public UIImageLoader UpperRailIconAsset2;
            public UIImageLoader UpperRailIconBundle2;
            public GameObject UpperRailIconShow2;
            public UIImageLoader MuzzleIconAsset2;
            public UIImageLoader MuzzleIconBundle2;
            public GameObject MuzzleIconShow2;
            public UIImageLoader LowerRailIconAsset2;
            public UIImageLoader LowerRailIconBundle2;
            public GameObject LowerRailIconShow2;
            public UIImageLoader MagazineIconAsset2;
            public UIImageLoader MagazineIconBundle2;
            public GameObject MagazineIconShow2;
            public UIImageLoader StockIconAsset2;
            public UIImageLoader StockIconBundle2;
            public GameObject StockIconShow2;
            public GameObject UpperRailSlotShow3;
            public GameObject MuzzleSlotShow3;
            public GameObject LowerRailSlotShow3;
            public GameObject MagazineSlotShow3;
            public GameObject StockSlotShow3;
            public UIImageLoader UpperRailIconAsset3;
            public UIImageLoader UpperRailIconBundle3;
            public GameObject UpperRailIconShow3;
            public UIImageLoader MuzzleIconAsset3;
            public UIImageLoader MuzzleIconBundle3;
            public GameObject MuzzleIconShow3;
            public UIImageLoader LowerRailIconAsset3;
            public UIImageLoader LowerRailIconBundle3;
            public GameObject LowerRailIconShow3;
            public UIImageLoader MagazineIconAsset3;
            public UIImageLoader MagazineIconBundle3;
            public GameObject MagazineIconShow3;
            public UIImageLoader StockIconAsset3;
            public UIImageLoader StockIconBundle3;
            public GameObject StockIconShow3;
            public Image WeaponIconSprite1;
            public UIImageLoader WeaponIconAsset1;
            public UIImageLoader WeaponIconBundle1;
            public GameObject WeaponGroupShow1;
            public Text WeaponNameText1;
            public Image WeaponIconSprite2;
            public UIImageLoader WeaponIconAsset2;
            public UIImageLoader WeaponIconBundle2;
            public GameObject WeaponGroupShow2;
            public Text WeaponNameText2;
            public Image WeaponIconSprite3;
            public UIImageLoader WeaponIconAsset3;
            public UIImageLoader WeaponIconBundle3;
            public GameObject WeaponGroupShow3;
            public Text WeaponNameText3;
            public Image WeaponIconSprite4;
            public UIImageLoader WeaponIconAsset4;
            public UIImageLoader WeaponIconBundle4;
            public GameObject WeaponGroupShow4;
            public Text WeaponNameText4;
            public Image WeaponIconSprite5;
            public UIImageLoader WeaponIconAsset5;
            public UIImageLoader WeaponIconBundle5;
            public GameObject WeaponGroupShow5;
            public Text WeaponNameText5;
            public UIImageLoader WearIconAsset1;
            public UIImageLoader WearIconBundle1;
            public Text WearNameText1;
            public GameObject WearGroupShow1;
            public UIImageLoader WearIconAsset2;
            public UIImageLoader WearIconBundle2;
            public Text WearNameText2;
            public GameObject WearGroupShow2;
            public UIImageLoader WearIconAsset3;
            public UIImageLoader WearIconBundle3;
            public Text WearNameText3;
            public GameObject WearGroupShow3;
            public UIImageLoader WearIconAsset4;
            public UIImageLoader WearIconBundle4;
            public Text WearNameText4;
            public GameObject WearGroupShow4;
            public UIImageLoader WearIconAsset5;
            public UIImageLoader WearIconBundle5;
            public Text WearNameText5;
            public GameObject WearGroupShow5;
            public UIImageLoader WearIconAsset6;
            public UIImageLoader WearIconBundle6;
            public Text WearNameText6;
            public GameObject WearGroupShow6;
            
            public void FillField()
            {
                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "WeightText":
                            WeightText = v;
                            break;
                        case "GearLastingNum1":
                            GearLastingNumText1 = v;
                            break;
                        case "GearName1":
                            GearNameText1 = v;
                            break;
                        case "GearLastingNum2":
                            GearLastingNumText2 = v;
                            break;
                        case "GearName2":
                            GearNameText2 = v;
                            break;
                        case "GearLastingNum3":
                            GearLastingNumText3 = v;
                            break;
                        case "GearName3":
                            GearNameText3 = v;
                            break;
                        case "GearLastingNum4":
                            GearLastingNumText4 = v;
                            break;
                        case "GearName4":
                            GearNameText4 = v;
                            break;
                        case "WeaponName1":
                            WeaponNameText1 = v;
                            break;
                        case "WeaponName2":
                            WeaponNameText2 = v;
                            break;
                        case "WeaponName3":
                            WeaponNameText3 = v;
                            break;
                        case "WeaponName4":
                            WeaponNameText4 = v;
                            break;
                        case "WeaponName5":
                            WeaponNameText5 = v;
                            break;
                        case "WearName1":
                            WearNameText1 = v;
                            break;
                        case "WearName2":
                            WearNameText2 = v;
                            break;
                        case "WearName3":
                            WearNameText3 = v;
                            break;
                        case "WearName4":
                            WearNameText4 = v;
                            break;
                        case "WearName5":
                            WearNameText5 = v;
                            break;
                        case "WearName6":
                            WearNameText6 = v;
                            break;
                    }
                }

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "GearLastingLayer1":
                            GearLastingLayerShow1 = v.gameObject;
                            break;
                        case "GearIconGroup1":
                            GearIconGroupShow1 = v.gameObject;
                            break;
                        case "GearLastingBg1":
                            GearLastingBgShow1 = v.gameObject;
                            break;
                        case "GearLastingLayer2":
                            GearLastingLayerShow2 = v.gameObject;
                            break;
                        case "GearIconGroup2":
                            GearIconGroupShow2 = v.gameObject;
                            break;
                        case "GearLastingBg2":
                            GearLastingBgShow2 = v.gameObject;
                            break;
                        case "GearLastingLayer3":
                            GearLastingLayerShow3 = v.gameObject;
                            break;
                        case "GearIconGroup3":
                            GearIconGroupShow3 = v.gameObject;
                            break;
                        case "GearLastingBg3":
                            GearLastingBgShow3 = v.gameObject;
                            break;
                        case "GearLastingLayer4":
                            GearLastingLayerShow4 = v.gameObject;
                            break;
                        case "GearIconGroup4":
                            GearIconGroupShow4 = v.gameObject;
                            break;
                        case "GearLastingBg4":
                            GearLastingBgShow4 = v.gameObject;
                            break;
                        case "UpperRailSlot1":
                            UpperRailSlotShow1 = v.gameObject;
                            break;
                        case "MuzzleSlot1":
                            MuzzleSlotShow1 = v.gameObject;
                            break;
                        case "LowerRailSlot1":
                            LowerRailSlotShow1 = v.gameObject;
                            break;
                        case "MagazineSlot1":
                            MagazineSlotShow1 = v.gameObject;
                            break;
                        case "StockSlot1":
                            StockSlotShow1 = v.gameObject;
                            break;
                        case "UpperRailIcon1":
                            UpperRailIconShow1 = v.gameObject;
                            break;
                        case "MuzzleIcon1":
                            MuzzleIconShow1 = v.gameObject;
                            break;
                        case "LowerRailIcon1":
                            LowerRailIconShow1 = v.gameObject;
                            break;
                        case "MagazineIcon1":
                            MagazineIconShow1 = v.gameObject;
                            break;
                        case "StockIcon1":
                            StockIconShow1 = v.gameObject;
                            break;
                        case "UpperRailSlot2":
                            UpperRailSlotShow2 = v.gameObject;
                            break;
                        case "MuzzleSlot2":
                            MuzzleSlotShow2 = v.gameObject;
                            break;
                        case "LowerRailSlot2":
                            LowerRailSlotShow2 = v.gameObject;
                            break;
                        case "MagazineSlot2":
                            MagazineSlotShow2 = v.gameObject;
                            break;
                        case "StockSlot2":
                            StockSlotShow2 = v.gameObject;
                            break;
                        case "UpperRailIcon2":
                            UpperRailIconShow2 = v.gameObject;
                            break;
                        case "MuzzleIcon2":
                            MuzzleIconShow2 = v.gameObject;
                            break;
                        case "LowerRailIcon2":
                            LowerRailIconShow2 = v.gameObject;
                            break;
                        case "MagazineIcon2":
                            MagazineIconShow2 = v.gameObject;
                            break;
                        case "StockIcon2":
                            StockIconShow2 = v.gameObject;
                            break;
                        case "UpperRailSlot3":
                            UpperRailSlotShow3 = v.gameObject;
                            break;
                        case "MuzzleSlot3":
                            MuzzleSlotShow3 = v.gameObject;
                            break;
                        case "LowerRailSlot3":
                            LowerRailSlotShow3 = v.gameObject;
                            break;
                        case "MagazineSlot3":
                            MagazineSlotShow3 = v.gameObject;
                            break;
                        case "StockSlot3":
                            StockSlotShow3 = v.gameObject;
                            break;
                        case "UpperRailIcon3":
                            UpperRailIconShow3 = v.gameObject;
                            break;
                        case "MuzzleIcon3":
                            MuzzleIconShow3 = v.gameObject;
                            break;
                        case "LowerRailIcon3":
                            LowerRailIconShow3 = v.gameObject;
                            break;
                        case "MagazineIcon3":
                            MagazineIconShow3 = v.gameObject;
                            break;
                        case "StockIcon3":
                            StockIconShow3 = v.gameObject;
                            break;
                        case "WeaponGroup1":
                            WeaponGroupShow1 = v.gameObject;
                            break;
                        case "WeaponGroup2":
                            WeaponGroupShow2 = v.gameObject;
                            break;
                        case "WeaponGroup3":
                            WeaponGroupShow3 = v.gameObject;
                            break;
                        case "WeaponGroup4":
                            WeaponGroupShow4 = v.gameObject;
                            break;
                        case "WeaponGroup5":
                            WeaponGroupShow5 = v.gameObject;
                            break;
                        case "WearGroup1":
                            WearGroupShow1 = v.gameObject;
                            break;
                        case "WearGroup2":
                            WearGroupShow2 = v.gameObject;
                            break;
                        case "WearGroup3":
                            WearGroupShow3 = v.gameObject;
                            break;
                        case "WearGroup4":
                            WearGroupShow4 = v.gameObject;
                            break;
                        case "WearGroup5":
                            WearGroupShow5 = v.gameObject;
                            break;
                        case "WearGroup6":
                            WearGroupShow6 = v.gameObject;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "GearLastingLayer1":
                            GearLastingLayerValue1 = v;
                            break;
                        case "GearIcon1":
                            GearIconSprite1 = v;
                            break;
                        case "GearLastingLayer2":
                            GearLastingLayerValue2 = v;
                            break;
                        case "GearIcon2":
                            GearIconSprite2 = v;
                            break;
                        case "GearLastingLayer3":
                            GearLastingLayerValue3 = v;
                            break;
                        case "GearIcon3":
                            GearIconSprite3 = v;
                            break;
                        case "GearLastingLayer4":
                            GearLastingLayerValue4 = v;
                            break;
                        case "GearIcon4":
                            GearIconSprite4 = v;
                            break;
                        case "WeaponIcon1":
                            WeaponIconSprite1 = v;
                            break;
                        case "WeaponIcon2":
                            WeaponIconSprite2 = v;
                            break;
                        case "WeaponIcon3":
                            WeaponIconSprite3 = v;
                            break;
                        case "WeaponIcon4":
                            WeaponIconSprite4 = v;
                            break;
                        case "WeaponIcon5":
                            WeaponIconSprite5 = v;
                            break;
                    }
                }

                UIImageLoader[] uiimageloaders = gameObject.GetComponentsInChildren<UIImageLoader>(true);
                foreach (var v in uiimageloaders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "GearIcon1":
                            GearIconAsset1 = v;
                            GearIconBundle1 = v;
                            break;
                        case "GearIcon2":
                            GearIconAsset2 = v;
                            GearIconBundle2 = v;
                            break;
                        case "GearIcon3":
                            GearIconAsset3 = v;
                            GearIconBundle3 = v;
                            break;
                        case "GearIcon4":
                            GearIconAsset4 = v;
                            GearIconBundle4 = v;
                            break;
                        case "UpperRailIcon1":
                            UpperRailIconAsset1 = v;
                            UpperRailIconBundle1 = v;
                            break;
                        case "MuzzleIcon1":
                            MuzzleIconAsset1 = v;
                            MuzzleIconBundle1 = v;
                            break;
                        case "LowerRailIcon1":
                            LowerRailIconAsset1 = v;
                            LowerRailIconBundle1 = v;
                            break;
                        case "MagazineIcon1":
                            MagazineIconAsset1 = v;
                            MagazineIconBundle1 = v;
                            break;
                        case "StockIcon1":
                            StockIconAsset1 = v;
                            StockIconBundle1 = v;
                            break;
                        case "UpperRailIcon2":
                            UpperRailIconAsset2 = v;
                            UpperRailIconBundle2 = v;
                            break;
                        case "MuzzleIcon2":
                            MuzzleIconAsset2 = v;
                            MuzzleIconBundle2 = v;
                            break;
                        case "LowerRailIcon2":
                            LowerRailIconAsset2 = v;
                            LowerRailIconBundle2 = v;
                            break;
                        case "MagazineIcon2":
                            MagazineIconAsset2 = v;
                            MagazineIconBundle2 = v;
                            break;
                        case "StockIcon2":
                            StockIconAsset2 = v;
                            StockIconBundle2 = v;
                            break;
                        case "UpperRailIcon3":
                            UpperRailIconAsset3 = v;
                            UpperRailIconBundle3 = v;
                            break;
                        case "MuzzleIcon3":
                            MuzzleIconAsset3 = v;
                            MuzzleIconBundle3 = v;
                            break;
                        case "LowerRailIcon3":
                            LowerRailIconAsset3 = v;
                            LowerRailIconBundle3 = v;
                            break;
                        case "MagazineIcon3":
                            MagazineIconAsset3 = v;
                            MagazineIconBundle3 = v;
                            break;
                        case "StockIcon3":
                            StockIconAsset3 = v;
                            StockIconBundle3 = v;
                            break;
                        case "WeaponIcon1":
                            WeaponIconAsset1 = v;
                            WeaponIconBundle1 = v;
                            break;
                        case "WeaponIcon2":
                            WeaponIconAsset2 = v;
                            WeaponIconBundle2 = v;
                            break;
                        case "WeaponIcon3":
                            WeaponIconAsset3 = v;
                            WeaponIconBundle3 = v;
                            break;
                        case "WeaponIcon4":
                            WeaponIconAsset4 = v;
                            WeaponIconBundle4 = v;
                            break;
                        case "WeaponIcon5":
                            WeaponIconAsset5 = v;
                            WeaponIconBundle5 = v;
                            break;
                        case "WearIcon1":
                            WearIconAsset1 = v;
                            WearIconBundle1 = v;
                            break;
                        case "WearIcon2":
                            WearIconAsset2 = v;
                            WearIconBundle2 = v;
                            break;
                        case "WearIcon3":
                            WearIconAsset3 = v;
                            WearIconBundle3 = v;
                            break;
                        case "WearIcon4":
                            WearIconAsset4 = v;
                            WearIconBundle4 = v;
                            break;
                        case "WearIcon5":
                            WearIconAsset5 = v;
                            WearIconBundle5 = v;
                            break;
                        case "WearIcon6":
                            WearIconAsset6 = v;
                            WearIconBundle6 = v;
                            break;
                    }
                }

            }
        }


        private string _weightText;
        private bool _gearLastingLayerShow1;
        private float _gearLastingLayerValue1;
        private Sprite _gearIconSprite1;
        private string _gearIconAsset1;
        private string _gearIconBundle1;
        private bool _gearIconGroupShow1;
        private bool _gearLastingBgShow1;
        private string _gearLastingNumText1;
        private string _gearNameText1;
        private bool _gearLastingLayerShow2;
        private float _gearLastingLayerValue2;
        private Sprite _gearIconSprite2;
        private string _gearIconAsset2;
        private string _gearIconBundle2;
        private bool _gearIconGroupShow2;
        private bool _gearLastingBgShow2;
        private string _gearLastingNumText2;
        private string _gearNameText2;
        private bool _gearLastingLayerShow3;
        private float _gearLastingLayerValue3;
        private Sprite _gearIconSprite3;
        private string _gearIconAsset3;
        private string _gearIconBundle3;
        private bool _gearIconGroupShow3;
        private bool _gearLastingBgShow3;
        private string _gearLastingNumText3;
        private string _gearNameText3;
        private bool _gearLastingLayerShow4;
        private float _gearLastingLayerValue4;
        private Sprite _gearIconSprite4;
        private string _gearIconAsset4;
        private string _gearIconBundle4;
        private bool _gearIconGroupShow4;
        private bool _gearLastingBgShow4;
        private string _gearLastingNumText4;
        private string _gearNameText4;
        private bool _upperRailSlotShow1;
        private bool _muzzleSlotShow1;
        private bool _lowerRailSlotShow1;
        private bool _magazineSlotShow1;
        private bool _stockSlotShow1;
        private string _upperRailIconAsset1;
        private string _upperRailIconBundle1;
        private bool _upperRailIconShow1;
        private string _muzzleIconAsset1;
        private string _muzzleIconBundle1;
        private bool _muzzleIconShow1;
        private string _lowerRailIconAsset1;
        private string _lowerRailIconBundle1;
        private bool _lowerRailIconShow1;
        private string _magazineIconAsset1;
        private string _magazineIconBundle1;
        private bool _magazineIconShow1;
        private string _stockIconAsset1;
        private string _stockIconBundle1;
        private bool _stockIconShow1;
        private bool _upperRailSlotShow2;
        private bool _muzzleSlotShow2;
        private bool _lowerRailSlotShow2;
        private bool _magazineSlotShow2;
        private bool _stockSlotShow2;
        private string _upperRailIconAsset2;
        private string _upperRailIconBundle2;
        private bool _upperRailIconShow2;
        private string _muzzleIconAsset2;
        private string _muzzleIconBundle2;
        private bool _muzzleIconShow2;
        private string _lowerRailIconAsset2;
        private string _lowerRailIconBundle2;
        private bool _lowerRailIconShow2;
        private string _magazineIconAsset2;
        private string _magazineIconBundle2;
        private bool _magazineIconShow2;
        private string _stockIconAsset2;
        private string _stockIconBundle2;
        private bool _stockIconShow2;
        private bool _upperRailSlotShow3;
        private bool _muzzleSlotShow3;
        private bool _lowerRailSlotShow3;
        private bool _magazineSlotShow3;
        private bool _stockSlotShow3;
        private string _upperRailIconAsset3;
        private string _upperRailIconBundle3;
        private bool _upperRailIconShow3;
        private string _muzzleIconAsset3;
        private string _muzzleIconBundle3;
        private bool _muzzleIconShow3;
        private string _lowerRailIconAsset3;
        private string _lowerRailIconBundle3;
        private bool _lowerRailIconShow3;
        private string _magazineIconAsset3;
        private string _magazineIconBundle3;
        private bool _magazineIconShow3;
        private string _stockIconAsset3;
        private string _stockIconBundle3;
        private bool _stockIconShow3;
        private Sprite _weaponIconSprite1;
        private string _weaponIconAsset1;
        private string _weaponIconBundle1;
        private bool _weaponGroupShow1;
        private string _weaponNameText1;
        private Sprite _weaponIconSprite2;
        private string _weaponIconAsset2;
        private string _weaponIconBundle2;
        private bool _weaponGroupShow2;
        private string _weaponNameText2;
        private Sprite _weaponIconSprite3;
        private string _weaponIconAsset3;
        private string _weaponIconBundle3;
        private bool _weaponGroupShow3;
        private string _weaponNameText3;
        private Sprite _weaponIconSprite4;
        private string _weaponIconAsset4;
        private string _weaponIconBundle4;
        private bool _weaponGroupShow4;
        private string _weaponNameText4;
        private Sprite _weaponIconSprite5;
        private string _weaponIconAsset5;
        private string _weaponIconBundle5;
        private bool _weaponGroupShow5;
        private string _weaponNameText5;
        private string _wearIconAsset1;
        private string _wearIconBundle1;
        private string _wearNameText1;
        private bool _wearGroupShow1;
        private string _wearIconAsset2;
        private string _wearIconBundle2;
        private string _wearNameText2;
        private bool _wearGroupShow2;
        private string _wearIconAsset3;
        private string _wearIconBundle3;
        private string _wearNameText3;
        private bool _wearGroupShow3;
        private string _wearIconAsset4;
        private string _wearIconBundle4;
        private string _wearNameText4;
        private bool _wearGroupShow4;
        private string _wearIconAsset5;
        private string _wearIconBundle5;
        private string _wearNameText5;
        private bool _wearGroupShow5;
        private string _wearIconAsset6;
        private string _wearIconBundle6;
        private string _wearNameText6;
        private bool _wearGroupShow6;
        public string WeightText { get { return _weightText; } set {if(_weightText != value) Set(ref _weightText, value, "WeightText"); } }
        public bool GearLastingLayerShow1 { get { return _gearLastingLayerShow1; } set {if(_gearLastingLayerShow1 != value) Set(ref _gearLastingLayerShow1, value, "GearLastingLayerShow1"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GearLastingLayerValue1 { get { return _gearLastingLayerValue1; } set {if(_gearLastingLayerValue1 != value) Set(ref _gearLastingLayerValue1, value, "GearLastingLayerValue1"); } }
        public Sprite GearIconSprite1 { get { return _gearIconSprite1; } set {if(_gearIconSprite1 != value) Set(ref _gearIconSprite1, value, "GearIconSprite1"); if(null != _view && null != _view.GearIconSprite1 && null == value) _view.GearIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public string GearIconAsset1 { get { return _gearIconAsset1; } set {if(_gearIconAsset1 != value) Set(ref _gearIconAsset1, value, "GearIconAsset1"); } }
        public string GearIconBundle1 { get { return _gearIconBundle1; } set {if(_gearIconBundle1 != value) Set(ref _gearIconBundle1, value, "GearIconBundle1"); } }
        public bool GearIconGroupShow1 { get { return _gearIconGroupShow1; } set {if(_gearIconGroupShow1 != value) Set(ref _gearIconGroupShow1, value, "GearIconGroupShow1"); } }
        public bool GearLastingBgShow1 { get { return _gearLastingBgShow1; } set {if(_gearLastingBgShow1 != value) Set(ref _gearLastingBgShow1, value, "GearLastingBgShow1"); } }
        public string GearLastingNumText1 { get { return _gearLastingNumText1; } set {if(_gearLastingNumText1 != value) Set(ref _gearLastingNumText1, value, "GearLastingNumText1"); } }
        public string GearNameText1 { get { return _gearNameText1; } set {if(_gearNameText1 != value) Set(ref _gearNameText1, value, "GearNameText1"); } }
        public bool GearLastingLayerShow2 { get { return _gearLastingLayerShow2; } set {if(_gearLastingLayerShow2 != value) Set(ref _gearLastingLayerShow2, value, "GearLastingLayerShow2"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GearLastingLayerValue2 { get { return _gearLastingLayerValue2; } set {if(_gearLastingLayerValue2 != value) Set(ref _gearLastingLayerValue2, value, "GearLastingLayerValue2"); } }
        public Sprite GearIconSprite2 { get { return _gearIconSprite2; } set {if(_gearIconSprite2 != value) Set(ref _gearIconSprite2, value, "GearIconSprite2"); if(null != _view && null != _view.GearIconSprite2 && null == value) _view.GearIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public string GearIconAsset2 { get { return _gearIconAsset2; } set {if(_gearIconAsset2 != value) Set(ref _gearIconAsset2, value, "GearIconAsset2"); } }
        public string GearIconBundle2 { get { return _gearIconBundle2; } set {if(_gearIconBundle2 != value) Set(ref _gearIconBundle2, value, "GearIconBundle2"); } }
        public bool GearIconGroupShow2 { get { return _gearIconGroupShow2; } set {if(_gearIconGroupShow2 != value) Set(ref _gearIconGroupShow2, value, "GearIconGroupShow2"); } }
        public bool GearLastingBgShow2 { get { return _gearLastingBgShow2; } set {if(_gearLastingBgShow2 != value) Set(ref _gearLastingBgShow2, value, "GearLastingBgShow2"); } }
        public string GearLastingNumText2 { get { return _gearLastingNumText2; } set {if(_gearLastingNumText2 != value) Set(ref _gearLastingNumText2, value, "GearLastingNumText2"); } }
        public string GearNameText2 { get { return _gearNameText2; } set {if(_gearNameText2 != value) Set(ref _gearNameText2, value, "GearNameText2"); } }
        public bool GearLastingLayerShow3 { get { return _gearLastingLayerShow3; } set {if(_gearLastingLayerShow3 != value) Set(ref _gearLastingLayerShow3, value, "GearLastingLayerShow3"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GearLastingLayerValue3 { get { return _gearLastingLayerValue3; } set {if(_gearLastingLayerValue3 != value) Set(ref _gearLastingLayerValue3, value, "GearLastingLayerValue3"); } }
        public Sprite GearIconSprite3 { get { return _gearIconSprite3; } set {if(_gearIconSprite3 != value) Set(ref _gearIconSprite3, value, "GearIconSprite3"); if(null != _view && null != _view.GearIconSprite3 && null == value) _view.GearIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public string GearIconAsset3 { get { return _gearIconAsset3; } set {if(_gearIconAsset3 != value) Set(ref _gearIconAsset3, value, "GearIconAsset3"); } }
        public string GearIconBundle3 { get { return _gearIconBundle3; } set {if(_gearIconBundle3 != value) Set(ref _gearIconBundle3, value, "GearIconBundle3"); } }
        public bool GearIconGroupShow3 { get { return _gearIconGroupShow3; } set {if(_gearIconGroupShow3 != value) Set(ref _gearIconGroupShow3, value, "GearIconGroupShow3"); } }
        public bool GearLastingBgShow3 { get { return _gearLastingBgShow3; } set {if(_gearLastingBgShow3 != value) Set(ref _gearLastingBgShow3, value, "GearLastingBgShow3"); } }
        public string GearLastingNumText3 { get { return _gearLastingNumText3; } set {if(_gearLastingNumText3 != value) Set(ref _gearLastingNumText3, value, "GearLastingNumText3"); } }
        public string GearNameText3 { get { return _gearNameText3; } set {if(_gearNameText3 != value) Set(ref _gearNameText3, value, "GearNameText3"); } }
        public bool GearLastingLayerShow4 { get { return _gearLastingLayerShow4; } set {if(_gearLastingLayerShow4 != value) Set(ref _gearLastingLayerShow4, value, "GearLastingLayerShow4"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GearLastingLayerValue4 { get { return _gearLastingLayerValue4; } set {if(_gearLastingLayerValue4 != value) Set(ref _gearLastingLayerValue4, value, "GearLastingLayerValue4"); } }
        public Sprite GearIconSprite4 { get { return _gearIconSprite4; } set {if(_gearIconSprite4 != value) Set(ref _gearIconSprite4, value, "GearIconSprite4"); if(null != _view && null != _view.GearIconSprite4 && null == value) _view.GearIconSprite4.sprite = ViewModelUtil.EmptySprite; } }
        public string GearIconAsset4 { get { return _gearIconAsset4; } set {if(_gearIconAsset4 != value) Set(ref _gearIconAsset4, value, "GearIconAsset4"); } }
        public string GearIconBundle4 { get { return _gearIconBundle4; } set {if(_gearIconBundle4 != value) Set(ref _gearIconBundle4, value, "GearIconBundle4"); } }
        public bool GearIconGroupShow4 { get { return _gearIconGroupShow4; } set {if(_gearIconGroupShow4 != value) Set(ref _gearIconGroupShow4, value, "GearIconGroupShow4"); } }
        public bool GearLastingBgShow4 { get { return _gearLastingBgShow4; } set {if(_gearLastingBgShow4 != value) Set(ref _gearLastingBgShow4, value, "GearLastingBgShow4"); } }
        public string GearLastingNumText4 { get { return _gearLastingNumText4; } set {if(_gearLastingNumText4 != value) Set(ref _gearLastingNumText4, value, "GearLastingNumText4"); } }
        public string GearNameText4 { get { return _gearNameText4; } set {if(_gearNameText4 != value) Set(ref _gearNameText4, value, "GearNameText4"); } }
        public bool UpperRailSlotShow1 { get { return _upperRailSlotShow1; } set {if(_upperRailSlotShow1 != value) Set(ref _upperRailSlotShow1, value, "UpperRailSlotShow1"); } }
        public bool MuzzleSlotShow1 { get { return _muzzleSlotShow1; } set {if(_muzzleSlotShow1 != value) Set(ref _muzzleSlotShow1, value, "MuzzleSlotShow1"); } }
        public bool LowerRailSlotShow1 { get { return _lowerRailSlotShow1; } set {if(_lowerRailSlotShow1 != value) Set(ref _lowerRailSlotShow1, value, "LowerRailSlotShow1"); } }
        public bool MagazineSlotShow1 { get { return _magazineSlotShow1; } set {if(_magazineSlotShow1 != value) Set(ref _magazineSlotShow1, value, "MagazineSlotShow1"); } }
        public bool StockSlotShow1 { get { return _stockSlotShow1; } set {if(_stockSlotShow1 != value) Set(ref _stockSlotShow1, value, "StockSlotShow1"); } }
        public string UpperRailIconAsset1 { get { return _upperRailIconAsset1; } set {if(_upperRailIconAsset1 != value) Set(ref _upperRailIconAsset1, value, "UpperRailIconAsset1"); } }
        public string UpperRailIconBundle1 { get { return _upperRailIconBundle1; } set {if(_upperRailIconBundle1 != value) Set(ref _upperRailIconBundle1, value, "UpperRailIconBundle1"); } }
        public bool UpperRailIconShow1 { get { return _upperRailIconShow1; } set {if(_upperRailIconShow1 != value) Set(ref _upperRailIconShow1, value, "UpperRailIconShow1"); } }
        public string MuzzleIconAsset1 { get { return _muzzleIconAsset1; } set {if(_muzzleIconAsset1 != value) Set(ref _muzzleIconAsset1, value, "MuzzleIconAsset1"); } }
        public string MuzzleIconBundle1 { get { return _muzzleIconBundle1; } set {if(_muzzleIconBundle1 != value) Set(ref _muzzleIconBundle1, value, "MuzzleIconBundle1"); } }
        public bool MuzzleIconShow1 { get { return _muzzleIconShow1; } set {if(_muzzleIconShow1 != value) Set(ref _muzzleIconShow1, value, "MuzzleIconShow1"); } }
        public string LowerRailIconAsset1 { get { return _lowerRailIconAsset1; } set {if(_lowerRailIconAsset1 != value) Set(ref _lowerRailIconAsset1, value, "LowerRailIconAsset1"); } }
        public string LowerRailIconBundle1 { get { return _lowerRailIconBundle1; } set {if(_lowerRailIconBundle1 != value) Set(ref _lowerRailIconBundle1, value, "LowerRailIconBundle1"); } }
        public bool LowerRailIconShow1 { get { return _lowerRailIconShow1; } set {if(_lowerRailIconShow1 != value) Set(ref _lowerRailIconShow1, value, "LowerRailIconShow1"); } }
        public string MagazineIconAsset1 { get { return _magazineIconAsset1; } set {if(_magazineIconAsset1 != value) Set(ref _magazineIconAsset1, value, "MagazineIconAsset1"); } }
        public string MagazineIconBundle1 { get { return _magazineIconBundle1; } set {if(_magazineIconBundle1 != value) Set(ref _magazineIconBundle1, value, "MagazineIconBundle1"); } }
        public bool MagazineIconShow1 { get { return _magazineIconShow1; } set {if(_magazineIconShow1 != value) Set(ref _magazineIconShow1, value, "MagazineIconShow1"); } }
        public string StockIconAsset1 { get { return _stockIconAsset1; } set {if(_stockIconAsset1 != value) Set(ref _stockIconAsset1, value, "StockIconAsset1"); } }
        public string StockIconBundle1 { get { return _stockIconBundle1; } set {if(_stockIconBundle1 != value) Set(ref _stockIconBundle1, value, "StockIconBundle1"); } }
        public bool StockIconShow1 { get { return _stockIconShow1; } set {if(_stockIconShow1 != value) Set(ref _stockIconShow1, value, "StockIconShow1"); } }
        public bool UpperRailSlotShow2 { get { return _upperRailSlotShow2; } set {if(_upperRailSlotShow2 != value) Set(ref _upperRailSlotShow2, value, "UpperRailSlotShow2"); } }
        public bool MuzzleSlotShow2 { get { return _muzzleSlotShow2; } set {if(_muzzleSlotShow2 != value) Set(ref _muzzleSlotShow2, value, "MuzzleSlotShow2"); } }
        public bool LowerRailSlotShow2 { get { return _lowerRailSlotShow2; } set {if(_lowerRailSlotShow2 != value) Set(ref _lowerRailSlotShow2, value, "LowerRailSlotShow2"); } }
        public bool MagazineSlotShow2 { get { return _magazineSlotShow2; } set {if(_magazineSlotShow2 != value) Set(ref _magazineSlotShow2, value, "MagazineSlotShow2"); } }
        public bool StockSlotShow2 { get { return _stockSlotShow2; } set {if(_stockSlotShow2 != value) Set(ref _stockSlotShow2, value, "StockSlotShow2"); } }
        public string UpperRailIconAsset2 { get { return _upperRailIconAsset2; } set {if(_upperRailIconAsset2 != value) Set(ref _upperRailIconAsset2, value, "UpperRailIconAsset2"); } }
        public string UpperRailIconBundle2 { get { return _upperRailIconBundle2; } set {if(_upperRailIconBundle2 != value) Set(ref _upperRailIconBundle2, value, "UpperRailIconBundle2"); } }
        public bool UpperRailIconShow2 { get { return _upperRailIconShow2; } set {if(_upperRailIconShow2 != value) Set(ref _upperRailIconShow2, value, "UpperRailIconShow2"); } }
        public string MuzzleIconAsset2 { get { return _muzzleIconAsset2; } set {if(_muzzleIconAsset2 != value) Set(ref _muzzleIconAsset2, value, "MuzzleIconAsset2"); } }
        public string MuzzleIconBundle2 { get { return _muzzleIconBundle2; } set {if(_muzzleIconBundle2 != value) Set(ref _muzzleIconBundle2, value, "MuzzleIconBundle2"); } }
        public bool MuzzleIconShow2 { get { return _muzzleIconShow2; } set {if(_muzzleIconShow2 != value) Set(ref _muzzleIconShow2, value, "MuzzleIconShow2"); } }
        public string LowerRailIconAsset2 { get { return _lowerRailIconAsset2; } set {if(_lowerRailIconAsset2 != value) Set(ref _lowerRailIconAsset2, value, "LowerRailIconAsset2"); } }
        public string LowerRailIconBundle2 { get { return _lowerRailIconBundle2; } set {if(_lowerRailIconBundle2 != value) Set(ref _lowerRailIconBundle2, value, "LowerRailIconBundle2"); } }
        public bool LowerRailIconShow2 { get { return _lowerRailIconShow2; } set {if(_lowerRailIconShow2 != value) Set(ref _lowerRailIconShow2, value, "LowerRailIconShow2"); } }
        public string MagazineIconAsset2 { get { return _magazineIconAsset2; } set {if(_magazineIconAsset2 != value) Set(ref _magazineIconAsset2, value, "MagazineIconAsset2"); } }
        public string MagazineIconBundle2 { get { return _magazineIconBundle2; } set {if(_magazineIconBundle2 != value) Set(ref _magazineIconBundle2, value, "MagazineIconBundle2"); } }
        public bool MagazineIconShow2 { get { return _magazineIconShow2; } set {if(_magazineIconShow2 != value) Set(ref _magazineIconShow2, value, "MagazineIconShow2"); } }
        public string StockIconAsset2 { get { return _stockIconAsset2; } set {if(_stockIconAsset2 != value) Set(ref _stockIconAsset2, value, "StockIconAsset2"); } }
        public string StockIconBundle2 { get { return _stockIconBundle2; } set {if(_stockIconBundle2 != value) Set(ref _stockIconBundle2, value, "StockIconBundle2"); } }
        public bool StockIconShow2 { get { return _stockIconShow2; } set {if(_stockIconShow2 != value) Set(ref _stockIconShow2, value, "StockIconShow2"); } }
        public bool UpperRailSlotShow3 { get { return _upperRailSlotShow3; } set {if(_upperRailSlotShow3 != value) Set(ref _upperRailSlotShow3, value, "UpperRailSlotShow3"); } }
        public bool MuzzleSlotShow3 { get { return _muzzleSlotShow3; } set {if(_muzzleSlotShow3 != value) Set(ref _muzzleSlotShow3, value, "MuzzleSlotShow3"); } }
        public bool LowerRailSlotShow3 { get { return _lowerRailSlotShow3; } set {if(_lowerRailSlotShow3 != value) Set(ref _lowerRailSlotShow3, value, "LowerRailSlotShow3"); } }
        public bool MagazineSlotShow3 { get { return _magazineSlotShow3; } set {if(_magazineSlotShow3 != value) Set(ref _magazineSlotShow3, value, "MagazineSlotShow3"); } }
        public bool StockSlotShow3 { get { return _stockSlotShow3; } set {if(_stockSlotShow3 != value) Set(ref _stockSlotShow3, value, "StockSlotShow3"); } }
        public string UpperRailIconAsset3 { get { return _upperRailIconAsset3; } set {if(_upperRailIconAsset3 != value) Set(ref _upperRailIconAsset3, value, "UpperRailIconAsset3"); } }
        public string UpperRailIconBundle3 { get { return _upperRailIconBundle3; } set {if(_upperRailIconBundle3 != value) Set(ref _upperRailIconBundle3, value, "UpperRailIconBundle3"); } }
        public bool UpperRailIconShow3 { get { return _upperRailIconShow3; } set {if(_upperRailIconShow3 != value) Set(ref _upperRailIconShow3, value, "UpperRailIconShow3"); } }
        public string MuzzleIconAsset3 { get { return _muzzleIconAsset3; } set {if(_muzzleIconAsset3 != value) Set(ref _muzzleIconAsset3, value, "MuzzleIconAsset3"); } }
        public string MuzzleIconBundle3 { get { return _muzzleIconBundle3; } set {if(_muzzleIconBundle3 != value) Set(ref _muzzleIconBundle3, value, "MuzzleIconBundle3"); } }
        public bool MuzzleIconShow3 { get { return _muzzleIconShow3; } set {if(_muzzleIconShow3 != value) Set(ref _muzzleIconShow3, value, "MuzzleIconShow3"); } }
        public string LowerRailIconAsset3 { get { return _lowerRailIconAsset3; } set {if(_lowerRailIconAsset3 != value) Set(ref _lowerRailIconAsset3, value, "LowerRailIconAsset3"); } }
        public string LowerRailIconBundle3 { get { return _lowerRailIconBundle3; } set {if(_lowerRailIconBundle3 != value) Set(ref _lowerRailIconBundle3, value, "LowerRailIconBundle3"); } }
        public bool LowerRailIconShow3 { get { return _lowerRailIconShow3; } set {if(_lowerRailIconShow3 != value) Set(ref _lowerRailIconShow3, value, "LowerRailIconShow3"); } }
        public string MagazineIconAsset3 { get { return _magazineIconAsset3; } set {if(_magazineIconAsset3 != value) Set(ref _magazineIconAsset3, value, "MagazineIconAsset3"); } }
        public string MagazineIconBundle3 { get { return _magazineIconBundle3; } set {if(_magazineIconBundle3 != value) Set(ref _magazineIconBundle3, value, "MagazineIconBundle3"); } }
        public bool MagazineIconShow3 { get { return _magazineIconShow3; } set {if(_magazineIconShow3 != value) Set(ref _magazineIconShow3, value, "MagazineIconShow3"); } }
        public string StockIconAsset3 { get { return _stockIconAsset3; } set {if(_stockIconAsset3 != value) Set(ref _stockIconAsset3, value, "StockIconAsset3"); } }
        public string StockIconBundle3 { get { return _stockIconBundle3; } set {if(_stockIconBundle3 != value) Set(ref _stockIconBundle3, value, "StockIconBundle3"); } }
        public bool StockIconShow3 { get { return _stockIconShow3; } set {if(_stockIconShow3 != value) Set(ref _stockIconShow3, value, "StockIconShow3"); } }
        public Sprite WeaponIconSprite1 { get { return _weaponIconSprite1; } set {if(_weaponIconSprite1 != value) Set(ref _weaponIconSprite1, value, "WeaponIconSprite1"); if(null != _view && null != _view.WeaponIconSprite1 && null == value) _view.WeaponIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public string WeaponIconAsset1 { get { return _weaponIconAsset1; } set {if(_weaponIconAsset1 != value) Set(ref _weaponIconAsset1, value, "WeaponIconAsset1"); } }
        public string WeaponIconBundle1 { get { return _weaponIconBundle1; } set {if(_weaponIconBundle1 != value) Set(ref _weaponIconBundle1, value, "WeaponIconBundle1"); } }
        public bool WeaponGroupShow1 { get { return _weaponGroupShow1; } set {if(_weaponGroupShow1 != value) Set(ref _weaponGroupShow1, value, "WeaponGroupShow1"); } }
        public string WeaponNameText1 { get { return _weaponNameText1; } set {if(_weaponNameText1 != value) Set(ref _weaponNameText1, value, "WeaponNameText1"); } }
        public Sprite WeaponIconSprite2 { get { return _weaponIconSprite2; } set {if(_weaponIconSprite2 != value) Set(ref _weaponIconSprite2, value, "WeaponIconSprite2"); if(null != _view && null != _view.WeaponIconSprite2 && null == value) _view.WeaponIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public string WeaponIconAsset2 { get { return _weaponIconAsset2; } set {if(_weaponIconAsset2 != value) Set(ref _weaponIconAsset2, value, "WeaponIconAsset2"); } }
        public string WeaponIconBundle2 { get { return _weaponIconBundle2; } set {if(_weaponIconBundle2 != value) Set(ref _weaponIconBundle2, value, "WeaponIconBundle2"); } }
        public bool WeaponGroupShow2 { get { return _weaponGroupShow2; } set {if(_weaponGroupShow2 != value) Set(ref _weaponGroupShow2, value, "WeaponGroupShow2"); } }
        public string WeaponNameText2 { get { return _weaponNameText2; } set {if(_weaponNameText2 != value) Set(ref _weaponNameText2, value, "WeaponNameText2"); } }
        public Sprite WeaponIconSprite3 { get { return _weaponIconSprite3; } set {if(_weaponIconSprite3 != value) Set(ref _weaponIconSprite3, value, "WeaponIconSprite3"); if(null != _view && null != _view.WeaponIconSprite3 && null == value) _view.WeaponIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public string WeaponIconAsset3 { get { return _weaponIconAsset3; } set {if(_weaponIconAsset3 != value) Set(ref _weaponIconAsset3, value, "WeaponIconAsset3"); } }
        public string WeaponIconBundle3 { get { return _weaponIconBundle3; } set {if(_weaponIconBundle3 != value) Set(ref _weaponIconBundle3, value, "WeaponIconBundle3"); } }
        public bool WeaponGroupShow3 { get { return _weaponGroupShow3; } set {if(_weaponGroupShow3 != value) Set(ref _weaponGroupShow3, value, "WeaponGroupShow3"); } }
        public string WeaponNameText3 { get { return _weaponNameText3; } set {if(_weaponNameText3 != value) Set(ref _weaponNameText3, value, "WeaponNameText3"); } }
        public Sprite WeaponIconSprite4 { get { return _weaponIconSprite4; } set {if(_weaponIconSprite4 != value) Set(ref _weaponIconSprite4, value, "WeaponIconSprite4"); if(null != _view && null != _view.WeaponIconSprite4 && null == value) _view.WeaponIconSprite4.sprite = ViewModelUtil.EmptySprite; } }
        public string WeaponIconAsset4 { get { return _weaponIconAsset4; } set {if(_weaponIconAsset4 != value) Set(ref _weaponIconAsset4, value, "WeaponIconAsset4"); } }
        public string WeaponIconBundle4 { get { return _weaponIconBundle4; } set {if(_weaponIconBundle4 != value) Set(ref _weaponIconBundle4, value, "WeaponIconBundle4"); } }
        public bool WeaponGroupShow4 { get { return _weaponGroupShow4; } set {if(_weaponGroupShow4 != value) Set(ref _weaponGroupShow4, value, "WeaponGroupShow4"); } }
        public string WeaponNameText4 { get { return _weaponNameText4; } set {if(_weaponNameText4 != value) Set(ref _weaponNameText4, value, "WeaponNameText4"); } }
        public Sprite WeaponIconSprite5 { get { return _weaponIconSprite5; } set {if(_weaponIconSprite5 != value) Set(ref _weaponIconSprite5, value, "WeaponIconSprite5"); if(null != _view && null != _view.WeaponIconSprite5 && null == value) _view.WeaponIconSprite5.sprite = ViewModelUtil.EmptySprite; } }
        public string WeaponIconAsset5 { get { return _weaponIconAsset5; } set {if(_weaponIconAsset5 != value) Set(ref _weaponIconAsset5, value, "WeaponIconAsset5"); } }
        public string WeaponIconBundle5 { get { return _weaponIconBundle5; } set {if(_weaponIconBundle5 != value) Set(ref _weaponIconBundle5, value, "WeaponIconBundle5"); } }
        public bool WeaponGroupShow5 { get { return _weaponGroupShow5; } set {if(_weaponGroupShow5 != value) Set(ref _weaponGroupShow5, value, "WeaponGroupShow5"); } }
        public string WeaponNameText5 { get { return _weaponNameText5; } set {if(_weaponNameText5 != value) Set(ref _weaponNameText5, value, "WeaponNameText5"); } }
        public string WearIconAsset1 { get { return _wearIconAsset1; } set {if(_wearIconAsset1 != value) Set(ref _wearIconAsset1, value, "WearIconAsset1"); } }
        public string WearIconBundle1 { get { return _wearIconBundle1; } set {if(_wearIconBundle1 != value) Set(ref _wearIconBundle1, value, "WearIconBundle1"); } }
        public string WearNameText1 { get { return _wearNameText1; } set {if(_wearNameText1 != value) Set(ref _wearNameText1, value, "WearNameText1"); } }
        public bool WearGroupShow1 { get { return _wearGroupShow1; } set {if(_wearGroupShow1 != value) Set(ref _wearGroupShow1, value, "WearGroupShow1"); } }
        public string WearIconAsset2 { get { return _wearIconAsset2; } set {if(_wearIconAsset2 != value) Set(ref _wearIconAsset2, value, "WearIconAsset2"); } }
        public string WearIconBundle2 { get { return _wearIconBundle2; } set {if(_wearIconBundle2 != value) Set(ref _wearIconBundle2, value, "WearIconBundle2"); } }
        public string WearNameText2 { get { return _wearNameText2; } set {if(_wearNameText2 != value) Set(ref _wearNameText2, value, "WearNameText2"); } }
        public bool WearGroupShow2 { get { return _wearGroupShow2; } set {if(_wearGroupShow2 != value) Set(ref _wearGroupShow2, value, "WearGroupShow2"); } }
        public string WearIconAsset3 { get { return _wearIconAsset3; } set {if(_wearIconAsset3 != value) Set(ref _wearIconAsset3, value, "WearIconAsset3"); } }
        public string WearIconBundle3 { get { return _wearIconBundle3; } set {if(_wearIconBundle3 != value) Set(ref _wearIconBundle3, value, "WearIconBundle3"); } }
        public string WearNameText3 { get { return _wearNameText3; } set {if(_wearNameText3 != value) Set(ref _wearNameText3, value, "WearNameText3"); } }
        public bool WearGroupShow3 { get { return _wearGroupShow3; } set {if(_wearGroupShow3 != value) Set(ref _wearGroupShow3, value, "WearGroupShow3"); } }
        public string WearIconAsset4 { get { return _wearIconAsset4; } set {if(_wearIconAsset4 != value) Set(ref _wearIconAsset4, value, "WearIconAsset4"); } }
        public string WearIconBundle4 { get { return _wearIconBundle4; } set {if(_wearIconBundle4 != value) Set(ref _wearIconBundle4, value, "WearIconBundle4"); } }
        public string WearNameText4 { get { return _wearNameText4; } set {if(_wearNameText4 != value) Set(ref _wearNameText4, value, "WearNameText4"); } }
        public bool WearGroupShow4 { get { return _wearGroupShow4; } set {if(_wearGroupShow4 != value) Set(ref _wearGroupShow4, value, "WearGroupShow4"); } }
        public string WearIconAsset5 { get { return _wearIconAsset5; } set {if(_wearIconAsset5 != value) Set(ref _wearIconAsset5, value, "WearIconAsset5"); } }
        public string WearIconBundle5 { get { return _wearIconBundle5; } set {if(_wearIconBundle5 != value) Set(ref _wearIconBundle5, value, "WearIconBundle5"); } }
        public string WearNameText5 { get { return _wearNameText5; } set {if(_wearNameText5 != value) Set(ref _wearNameText5, value, "WearNameText5"); } }
        public bool WearGroupShow5 { get { return _wearGroupShow5; } set {if(_wearGroupShow5 != value) Set(ref _wearGroupShow5, value, "WearGroupShow5"); } }
        public string WearIconAsset6 { get { return _wearIconAsset6; } set {if(_wearIconAsset6 != value) Set(ref _wearIconAsset6, value, "WearIconAsset6"); } }
        public string WearIconBundle6 { get { return _wearIconBundle6; } set {if(_wearIconBundle6 != value) Set(ref _wearIconBundle6, value, "WearIconBundle6"); } }
        public string WearNameText6 { get { return _wearNameText6; } set {if(_wearNameText6 != value) Set(ref _wearNameText6, value, "WearNameText6"); } }
        public bool WearGroupShow6 { get { return _wearGroupShow6; } set {if(_wearGroupShow6 != value) Set(ref _wearGroupShow6, value, "WearGroupShow6"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private ChickenBagView _view;
		
		public void Destory()
        {
            if (_viewGameObject != null)
            {
				UnityEngine.Object.Destroy(_viewGameObject);
            }
        }
		public void Visible(bool isViaible)
		{
		    if (_viewGameObject != null)
            {
				_viewGameObject.SetActive(isViaible);
            }
		
		}
		public void SetCanvasEnabled(bool value)
        {
            if (_viewCanvas != null)
            {
                _viewCanvas.enabled = value;
            }
        }
        public void CreateBinding(GameObject obj)
        {
			_viewGameObject = obj;
			_viewCanvas = _viewGameObject.GetComponent<Canvas>();

			bool bFirst = false;
			var view = obj.GetComponent<ChickenBagView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<ChickenBagView>();
				view.FillField();
			}
			DataInit(view);
			SpriteReset();
			view.BindingContext().DataContext = this;
			if(bFirst)
			{
				SaveOriData(view);
				ViewBind(view);
			}
			_view = view;

        }
		private void EventTriggerBind(ChickenBagView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static ChickenBagViewModel()
        {
            Type type = typeof(ChickenBagViewModel);
            foreach (var property in type.GetProperties())
            {
                if (property.CanWrite)
                {
                    PropertySetter.Add(property.Name, property);
                }
            }
			foreach (var methodInfo in type.GetMethods())
            {
                if (methodInfo.IsPublic)
                {
                    MethodSetter.Add(methodInfo.Name, methodInfo);
                }
            }
        }

		void ViewBind(ChickenBagView view)
		{
		     BindingSet<ChickenBagView, ChickenBagViewModel> bindingSet =
                view.CreateBindingSet<ChickenBagView, ChickenBagViewModel>();
            bindingSet.Bind(view.WeightText).For(v => v.text).To(vm => vm.WeightText).OneWay();
            bindingSet.Bind(view.GearLastingLayerShow1).For(v => v.activeSelf).To(vm => vm.GearLastingLayerShow1).OneWay();
            bindingSet.Bind(view.GearLastingLayerValue1).For(v => v.fillAmount).To(vm => vm.GearLastingLayerValue1).OneWay();
            bindingSet.Bind(view.GearIconSprite1).For(v => v.sprite).To(vm => vm.GearIconSprite1).OneWay();
            bindingSet.Bind(view.GearIconAsset1).For(v => v.AssetName).To(vm => vm.GearIconAsset1).OneWay();
            bindingSet.Bind(view.GearIconBundle1).For(v => v.BundleName).To(vm => vm.GearIconBundle1).OneWay();
            bindingSet.Bind(view.GearIconGroupShow1).For(v => v.activeSelf).To(vm => vm.GearIconGroupShow1).OneWay();
            bindingSet.Bind(view.GearLastingBgShow1).For(v => v.activeSelf).To(vm => vm.GearLastingBgShow1).OneWay();
            bindingSet.Bind(view.GearLastingNumText1).For(v => v.text).To(vm => vm.GearLastingNumText1).OneWay();
            bindingSet.Bind(view.GearNameText1).For(v => v.text).To(vm => vm.GearNameText1).OneWay();
            bindingSet.Bind(view.GearLastingLayerShow2).For(v => v.activeSelf).To(vm => vm.GearLastingLayerShow2).OneWay();
            bindingSet.Bind(view.GearLastingLayerValue2).For(v => v.fillAmount).To(vm => vm.GearLastingLayerValue2).OneWay();
            bindingSet.Bind(view.GearIconSprite2).For(v => v.sprite).To(vm => vm.GearIconSprite2).OneWay();
            bindingSet.Bind(view.GearIconAsset2).For(v => v.AssetName).To(vm => vm.GearIconAsset2).OneWay();
            bindingSet.Bind(view.GearIconBundle2).For(v => v.BundleName).To(vm => vm.GearIconBundle2).OneWay();
            bindingSet.Bind(view.GearIconGroupShow2).For(v => v.activeSelf).To(vm => vm.GearIconGroupShow2).OneWay();
            bindingSet.Bind(view.GearLastingBgShow2).For(v => v.activeSelf).To(vm => vm.GearLastingBgShow2).OneWay();
            bindingSet.Bind(view.GearLastingNumText2).For(v => v.text).To(vm => vm.GearLastingNumText2).OneWay();
            bindingSet.Bind(view.GearNameText2).For(v => v.text).To(vm => vm.GearNameText2).OneWay();
            bindingSet.Bind(view.GearLastingLayerShow3).For(v => v.activeSelf).To(vm => vm.GearLastingLayerShow3).OneWay();
            bindingSet.Bind(view.GearLastingLayerValue3).For(v => v.fillAmount).To(vm => vm.GearLastingLayerValue3).OneWay();
            bindingSet.Bind(view.GearIconSprite3).For(v => v.sprite).To(vm => vm.GearIconSprite3).OneWay();
            bindingSet.Bind(view.GearIconAsset3).For(v => v.AssetName).To(vm => vm.GearIconAsset3).OneWay();
            bindingSet.Bind(view.GearIconBundle3).For(v => v.BundleName).To(vm => vm.GearIconBundle3).OneWay();
            bindingSet.Bind(view.GearIconGroupShow3).For(v => v.activeSelf).To(vm => vm.GearIconGroupShow3).OneWay();
            bindingSet.Bind(view.GearLastingBgShow3).For(v => v.activeSelf).To(vm => vm.GearLastingBgShow3).OneWay();
            bindingSet.Bind(view.GearLastingNumText3).For(v => v.text).To(vm => vm.GearLastingNumText3).OneWay();
            bindingSet.Bind(view.GearNameText3).For(v => v.text).To(vm => vm.GearNameText3).OneWay();
            bindingSet.Bind(view.GearLastingLayerShow4).For(v => v.activeSelf).To(vm => vm.GearLastingLayerShow4).OneWay();
            bindingSet.Bind(view.GearLastingLayerValue4).For(v => v.fillAmount).To(vm => vm.GearLastingLayerValue4).OneWay();
            bindingSet.Bind(view.GearIconSprite4).For(v => v.sprite).To(vm => vm.GearIconSprite4).OneWay();
            bindingSet.Bind(view.GearIconAsset4).For(v => v.AssetName).To(vm => vm.GearIconAsset4).OneWay();
            bindingSet.Bind(view.GearIconBundle4).For(v => v.BundleName).To(vm => vm.GearIconBundle4).OneWay();
            bindingSet.Bind(view.GearIconGroupShow4).For(v => v.activeSelf).To(vm => vm.GearIconGroupShow4).OneWay();
            bindingSet.Bind(view.GearLastingBgShow4).For(v => v.activeSelf).To(vm => vm.GearLastingBgShow4).OneWay();
            bindingSet.Bind(view.GearLastingNumText4).For(v => v.text).To(vm => vm.GearLastingNumText4).OneWay();
            bindingSet.Bind(view.GearNameText4).For(v => v.text).To(vm => vm.GearNameText4).OneWay();
            bindingSet.Bind(view.UpperRailSlotShow1).For(v => v.activeSelf).To(vm => vm.UpperRailSlotShow1).OneWay();
            bindingSet.Bind(view.MuzzleSlotShow1).For(v => v.activeSelf).To(vm => vm.MuzzleSlotShow1).OneWay();
            bindingSet.Bind(view.LowerRailSlotShow1).For(v => v.activeSelf).To(vm => vm.LowerRailSlotShow1).OneWay();
            bindingSet.Bind(view.MagazineSlotShow1).For(v => v.activeSelf).To(vm => vm.MagazineSlotShow1).OneWay();
            bindingSet.Bind(view.StockSlotShow1).For(v => v.activeSelf).To(vm => vm.StockSlotShow1).OneWay();
            bindingSet.Bind(view.UpperRailIconAsset1).For(v => v.AssetName).To(vm => vm.UpperRailIconAsset1).OneWay();
            bindingSet.Bind(view.UpperRailIconBundle1).For(v => v.BundleName).To(vm => vm.UpperRailIconBundle1).OneWay();
            bindingSet.Bind(view.UpperRailIconShow1).For(v => v.activeSelf).To(vm => vm.UpperRailIconShow1).OneWay();
            bindingSet.Bind(view.MuzzleIconAsset1).For(v => v.AssetName).To(vm => vm.MuzzleIconAsset1).OneWay();
            bindingSet.Bind(view.MuzzleIconBundle1).For(v => v.BundleName).To(vm => vm.MuzzleIconBundle1).OneWay();
            bindingSet.Bind(view.MuzzleIconShow1).For(v => v.activeSelf).To(vm => vm.MuzzleIconShow1).OneWay();
            bindingSet.Bind(view.LowerRailIconAsset1).For(v => v.AssetName).To(vm => vm.LowerRailIconAsset1).OneWay();
            bindingSet.Bind(view.LowerRailIconBundle1).For(v => v.BundleName).To(vm => vm.LowerRailIconBundle1).OneWay();
            bindingSet.Bind(view.LowerRailIconShow1).For(v => v.activeSelf).To(vm => vm.LowerRailIconShow1).OneWay();
            bindingSet.Bind(view.MagazineIconAsset1).For(v => v.AssetName).To(vm => vm.MagazineIconAsset1).OneWay();
            bindingSet.Bind(view.MagazineIconBundle1).For(v => v.BundleName).To(vm => vm.MagazineIconBundle1).OneWay();
            bindingSet.Bind(view.MagazineIconShow1).For(v => v.activeSelf).To(vm => vm.MagazineIconShow1).OneWay();
            bindingSet.Bind(view.StockIconAsset1).For(v => v.AssetName).To(vm => vm.StockIconAsset1).OneWay();
            bindingSet.Bind(view.StockIconBundle1).For(v => v.BundleName).To(vm => vm.StockIconBundle1).OneWay();
            bindingSet.Bind(view.StockIconShow1).For(v => v.activeSelf).To(vm => vm.StockIconShow1).OneWay();
            bindingSet.Bind(view.UpperRailSlotShow2).For(v => v.activeSelf).To(vm => vm.UpperRailSlotShow2).OneWay();
            bindingSet.Bind(view.MuzzleSlotShow2).For(v => v.activeSelf).To(vm => vm.MuzzleSlotShow2).OneWay();
            bindingSet.Bind(view.LowerRailSlotShow2).For(v => v.activeSelf).To(vm => vm.LowerRailSlotShow2).OneWay();
            bindingSet.Bind(view.MagazineSlotShow2).For(v => v.activeSelf).To(vm => vm.MagazineSlotShow2).OneWay();
            bindingSet.Bind(view.StockSlotShow2).For(v => v.activeSelf).To(vm => vm.StockSlotShow2).OneWay();
            bindingSet.Bind(view.UpperRailIconAsset2).For(v => v.AssetName).To(vm => vm.UpperRailIconAsset2).OneWay();
            bindingSet.Bind(view.UpperRailIconBundle2).For(v => v.BundleName).To(vm => vm.UpperRailIconBundle2).OneWay();
            bindingSet.Bind(view.UpperRailIconShow2).For(v => v.activeSelf).To(vm => vm.UpperRailIconShow2).OneWay();
            bindingSet.Bind(view.MuzzleIconAsset2).For(v => v.AssetName).To(vm => vm.MuzzleIconAsset2).OneWay();
            bindingSet.Bind(view.MuzzleIconBundle2).For(v => v.BundleName).To(vm => vm.MuzzleIconBundle2).OneWay();
            bindingSet.Bind(view.MuzzleIconShow2).For(v => v.activeSelf).To(vm => vm.MuzzleIconShow2).OneWay();
            bindingSet.Bind(view.LowerRailIconAsset2).For(v => v.AssetName).To(vm => vm.LowerRailIconAsset2).OneWay();
            bindingSet.Bind(view.LowerRailIconBundle2).For(v => v.BundleName).To(vm => vm.LowerRailIconBundle2).OneWay();
            bindingSet.Bind(view.LowerRailIconShow2).For(v => v.activeSelf).To(vm => vm.LowerRailIconShow2).OneWay();
            bindingSet.Bind(view.MagazineIconAsset2).For(v => v.AssetName).To(vm => vm.MagazineIconAsset2).OneWay();
            bindingSet.Bind(view.MagazineIconBundle2).For(v => v.BundleName).To(vm => vm.MagazineIconBundle2).OneWay();
            bindingSet.Bind(view.MagazineIconShow2).For(v => v.activeSelf).To(vm => vm.MagazineIconShow2).OneWay();
            bindingSet.Bind(view.StockIconAsset2).For(v => v.AssetName).To(vm => vm.StockIconAsset2).OneWay();
            bindingSet.Bind(view.StockIconBundle2).For(v => v.BundleName).To(vm => vm.StockIconBundle2).OneWay();
            bindingSet.Bind(view.StockIconShow2).For(v => v.activeSelf).To(vm => vm.StockIconShow2).OneWay();
            bindingSet.Bind(view.UpperRailSlotShow3).For(v => v.activeSelf).To(vm => vm.UpperRailSlotShow3).OneWay();
            bindingSet.Bind(view.MuzzleSlotShow3).For(v => v.activeSelf).To(vm => vm.MuzzleSlotShow3).OneWay();
            bindingSet.Bind(view.LowerRailSlotShow3).For(v => v.activeSelf).To(vm => vm.LowerRailSlotShow3).OneWay();
            bindingSet.Bind(view.MagazineSlotShow3).For(v => v.activeSelf).To(vm => vm.MagazineSlotShow3).OneWay();
            bindingSet.Bind(view.StockSlotShow3).For(v => v.activeSelf).To(vm => vm.StockSlotShow3).OneWay();
            bindingSet.Bind(view.UpperRailIconAsset3).For(v => v.AssetName).To(vm => vm.UpperRailIconAsset3).OneWay();
            bindingSet.Bind(view.UpperRailIconBundle3).For(v => v.BundleName).To(vm => vm.UpperRailIconBundle3).OneWay();
            bindingSet.Bind(view.UpperRailIconShow3).For(v => v.activeSelf).To(vm => vm.UpperRailIconShow3).OneWay();
            bindingSet.Bind(view.MuzzleIconAsset3).For(v => v.AssetName).To(vm => vm.MuzzleIconAsset3).OneWay();
            bindingSet.Bind(view.MuzzleIconBundle3).For(v => v.BundleName).To(vm => vm.MuzzleIconBundle3).OneWay();
            bindingSet.Bind(view.MuzzleIconShow3).For(v => v.activeSelf).To(vm => vm.MuzzleIconShow3).OneWay();
            bindingSet.Bind(view.LowerRailIconAsset3).For(v => v.AssetName).To(vm => vm.LowerRailIconAsset3).OneWay();
            bindingSet.Bind(view.LowerRailIconBundle3).For(v => v.BundleName).To(vm => vm.LowerRailIconBundle3).OneWay();
            bindingSet.Bind(view.LowerRailIconShow3).For(v => v.activeSelf).To(vm => vm.LowerRailIconShow3).OneWay();
            bindingSet.Bind(view.MagazineIconAsset3).For(v => v.AssetName).To(vm => vm.MagazineIconAsset3).OneWay();
            bindingSet.Bind(view.MagazineIconBundle3).For(v => v.BundleName).To(vm => vm.MagazineIconBundle3).OneWay();
            bindingSet.Bind(view.MagazineIconShow3).For(v => v.activeSelf).To(vm => vm.MagazineIconShow3).OneWay();
            bindingSet.Bind(view.StockIconAsset3).For(v => v.AssetName).To(vm => vm.StockIconAsset3).OneWay();
            bindingSet.Bind(view.StockIconBundle3).For(v => v.BundleName).To(vm => vm.StockIconBundle3).OneWay();
            bindingSet.Bind(view.StockIconShow3).For(v => v.activeSelf).To(vm => vm.StockIconShow3).OneWay();
            bindingSet.Bind(view.WeaponIconSprite1).For(v => v.sprite).To(vm => vm.WeaponIconSprite1).OneWay();
            bindingSet.Bind(view.WeaponIconAsset1).For(v => v.AssetName).To(vm => vm.WeaponIconAsset1).OneWay();
            bindingSet.Bind(view.WeaponIconBundle1).For(v => v.BundleName).To(vm => vm.WeaponIconBundle1).OneWay();
            bindingSet.Bind(view.WeaponGroupShow1).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow1).OneWay();
            bindingSet.Bind(view.WeaponNameText1).For(v => v.text).To(vm => vm.WeaponNameText1).OneWay();
            bindingSet.Bind(view.WeaponIconSprite2).For(v => v.sprite).To(vm => vm.WeaponIconSprite2).OneWay();
            bindingSet.Bind(view.WeaponIconAsset2).For(v => v.AssetName).To(vm => vm.WeaponIconAsset2).OneWay();
            bindingSet.Bind(view.WeaponIconBundle2).For(v => v.BundleName).To(vm => vm.WeaponIconBundle2).OneWay();
            bindingSet.Bind(view.WeaponGroupShow2).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow2).OneWay();
            bindingSet.Bind(view.WeaponNameText2).For(v => v.text).To(vm => vm.WeaponNameText2).OneWay();
            bindingSet.Bind(view.WeaponIconSprite3).For(v => v.sprite).To(vm => vm.WeaponIconSprite3).OneWay();
            bindingSet.Bind(view.WeaponIconAsset3).For(v => v.AssetName).To(vm => vm.WeaponIconAsset3).OneWay();
            bindingSet.Bind(view.WeaponIconBundle3).For(v => v.BundleName).To(vm => vm.WeaponIconBundle3).OneWay();
            bindingSet.Bind(view.WeaponGroupShow3).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow3).OneWay();
            bindingSet.Bind(view.WeaponNameText3).For(v => v.text).To(vm => vm.WeaponNameText3).OneWay();
            bindingSet.Bind(view.WeaponIconSprite4).For(v => v.sprite).To(vm => vm.WeaponIconSprite4).OneWay();
            bindingSet.Bind(view.WeaponIconAsset4).For(v => v.AssetName).To(vm => vm.WeaponIconAsset4).OneWay();
            bindingSet.Bind(view.WeaponIconBundle4).For(v => v.BundleName).To(vm => vm.WeaponIconBundle4).OneWay();
            bindingSet.Bind(view.WeaponGroupShow4).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow4).OneWay();
            bindingSet.Bind(view.WeaponNameText4).For(v => v.text).To(vm => vm.WeaponNameText4).OneWay();
            bindingSet.Bind(view.WeaponIconSprite5).For(v => v.sprite).To(vm => vm.WeaponIconSprite5).OneWay();
            bindingSet.Bind(view.WeaponIconAsset5).For(v => v.AssetName).To(vm => vm.WeaponIconAsset5).OneWay();
            bindingSet.Bind(view.WeaponIconBundle5).For(v => v.BundleName).To(vm => vm.WeaponIconBundle5).OneWay();
            bindingSet.Bind(view.WeaponGroupShow5).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow5).OneWay();
            bindingSet.Bind(view.WeaponNameText5).For(v => v.text).To(vm => vm.WeaponNameText5).OneWay();
            bindingSet.Bind(view.WearIconAsset1).For(v => v.AssetName).To(vm => vm.WearIconAsset1).OneWay();
            bindingSet.Bind(view.WearIconBundle1).For(v => v.BundleName).To(vm => vm.WearIconBundle1).OneWay();
            bindingSet.Bind(view.WearNameText1).For(v => v.text).To(vm => vm.WearNameText1).OneWay();
            bindingSet.Bind(view.WearGroupShow1).For(v => v.activeSelf).To(vm => vm.WearGroupShow1).OneWay();
            bindingSet.Bind(view.WearIconAsset2).For(v => v.AssetName).To(vm => vm.WearIconAsset2).OneWay();
            bindingSet.Bind(view.WearIconBundle2).For(v => v.BundleName).To(vm => vm.WearIconBundle2).OneWay();
            bindingSet.Bind(view.WearNameText2).For(v => v.text).To(vm => vm.WearNameText2).OneWay();
            bindingSet.Bind(view.WearGroupShow2).For(v => v.activeSelf).To(vm => vm.WearGroupShow2).OneWay();
            bindingSet.Bind(view.WearIconAsset3).For(v => v.AssetName).To(vm => vm.WearIconAsset3).OneWay();
            bindingSet.Bind(view.WearIconBundle3).For(v => v.BundleName).To(vm => vm.WearIconBundle3).OneWay();
            bindingSet.Bind(view.WearNameText3).For(v => v.text).To(vm => vm.WearNameText3).OneWay();
            bindingSet.Bind(view.WearGroupShow3).For(v => v.activeSelf).To(vm => vm.WearGroupShow3).OneWay();
            bindingSet.Bind(view.WearIconAsset4).For(v => v.AssetName).To(vm => vm.WearIconAsset4).OneWay();
            bindingSet.Bind(view.WearIconBundle4).For(v => v.BundleName).To(vm => vm.WearIconBundle4).OneWay();
            bindingSet.Bind(view.WearNameText4).For(v => v.text).To(vm => vm.WearNameText4).OneWay();
            bindingSet.Bind(view.WearGroupShow4).For(v => v.activeSelf).To(vm => vm.WearGroupShow4).OneWay();
            bindingSet.Bind(view.WearIconAsset5).For(v => v.AssetName).To(vm => vm.WearIconAsset5).OneWay();
            bindingSet.Bind(view.WearIconBundle5).For(v => v.BundleName).To(vm => vm.WearIconBundle5).OneWay();
            bindingSet.Bind(view.WearNameText5).For(v => v.text).To(vm => vm.WearNameText5).OneWay();
            bindingSet.Bind(view.WearGroupShow5).For(v => v.activeSelf).To(vm => vm.WearGroupShow5).OneWay();
            bindingSet.Bind(view.WearIconAsset6).For(v => v.AssetName).To(vm => vm.WearIconAsset6).OneWay();
            bindingSet.Bind(view.WearIconBundle6).For(v => v.BundleName).To(vm => vm.WearIconBundle6).OneWay();
            bindingSet.Bind(view.WearNameText6).For(v => v.text).To(vm => vm.WearNameText6).OneWay();
            bindingSet.Bind(view.WearGroupShow6).For(v => v.activeSelf).To(vm => vm.WearGroupShow6).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(ChickenBagView view)
		{
            _weightText = view.WeightText.text;
		}


		void SaveOriData(ChickenBagView view)
		{
            view.oriWeightText = _weightText;
		}




		private void SpriteReset()
		{
			GearIconSprite1 = ViewModelUtil.EmptySprite;
			GearIconSprite2 = ViewModelUtil.EmptySprite;
			GearIconSprite3 = ViewModelUtil.EmptySprite;
			GearIconSprite4 = ViewModelUtil.EmptySprite;
			WeaponIconSprite1 = ViewModelUtil.EmptySprite;
			WeaponIconSprite2 = ViewModelUtil.EmptySprite;
			WeaponIconSprite3 = ViewModelUtil.EmptySprite;
			WeaponIconSprite4 = ViewModelUtil.EmptySprite;
			WeaponIconSprite5 = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			WeightText = _view.oriWeightText;
			SpriteReset();
		}

		public void CallFunction(string functionName)
        {
            if (MethodSetter.ContainsKey(functionName))
            {
                MethodSetter[functionName].Invoke(this, null);
            }
        }

		public bool IsPropertyExist(string propertyId)
        {
            return PropertySetter.ContainsKey(propertyId);
        }

		public Transform GetParentLinkNode()
		{
			return null;
		}

		public Transform GetChildLinkNode()
		{
			return null;
		}

        public const string GearLastingLayerShow = "GearLastingLayerShow";
        public const int GearLastingLayerShowCount = 4;
        public const string GearLastingLayerValue = "GearLastingLayerValue";
        public const int GearLastingLayerValueCount = 4;
        public const string GearIconSprite = "GearIconSprite";
        public const int GearIconSpriteCount = 4;
        public const string GearIconAsset = "GearIconAsset";
        public const int GearIconAssetCount = 4;
        public const string GearIconBundle = "GearIconBundle";
        public const int GearIconBundleCount = 4;
        public const string GearIconGroupShow = "GearIconGroupShow";
        public const int GearIconGroupShowCount = 4;
        public const string GearLastingBgShow = "GearLastingBgShow";
        public const int GearLastingBgShowCount = 4;
        public const string GearLastingNumText = "GearLastingNumText";
        public const int GearLastingNumTextCount = 4;
        public const string GearNameText = "GearNameText";
        public const int GearNameTextCount = 4;
        public const string UpperRailSlotShow = "UpperRailSlotShow";
        public const int UpperRailSlotShowCount = 3;
        public const string MuzzleSlotShow = "MuzzleSlotShow";
        public const int MuzzleSlotShowCount = 3;
        public const string LowerRailSlotShow = "LowerRailSlotShow";
        public const int LowerRailSlotShowCount = 3;
        public const string MagazineSlotShow = "MagazineSlotShow";
        public const int MagazineSlotShowCount = 3;
        public const string StockSlotShow = "StockSlotShow";
        public const int StockSlotShowCount = 3;
        public const string UpperRailIconAsset = "UpperRailIconAsset";
        public const int UpperRailIconAssetCount = 3;
        public const string UpperRailIconBundle = "UpperRailIconBundle";
        public const int UpperRailIconBundleCount = 3;
        public const string UpperRailIconShow = "UpperRailIconShow";
        public const int UpperRailIconShowCount = 3;
        public const string MuzzleIconAsset = "MuzzleIconAsset";
        public const int MuzzleIconAssetCount = 3;
        public const string MuzzleIconBundle = "MuzzleIconBundle";
        public const int MuzzleIconBundleCount = 3;
        public const string MuzzleIconShow = "MuzzleIconShow";
        public const int MuzzleIconShowCount = 3;
        public const string LowerRailIconAsset = "LowerRailIconAsset";
        public const int LowerRailIconAssetCount = 3;
        public const string LowerRailIconBundle = "LowerRailIconBundle";
        public const int LowerRailIconBundleCount = 3;
        public const string LowerRailIconShow = "LowerRailIconShow";
        public const int LowerRailIconShowCount = 3;
        public const string MagazineIconAsset = "MagazineIconAsset";
        public const int MagazineIconAssetCount = 3;
        public const string MagazineIconBundle = "MagazineIconBundle";
        public const int MagazineIconBundleCount = 3;
        public const string MagazineIconShow = "MagazineIconShow";
        public const int MagazineIconShowCount = 3;
        public const string StockIconAsset = "StockIconAsset";
        public const int StockIconAssetCount = 3;
        public const string StockIconBundle = "StockIconBundle";
        public const int StockIconBundleCount = 3;
        public const string StockIconShow = "StockIconShow";
        public const int StockIconShowCount = 3;
        public const string WeaponIconSprite = "WeaponIconSprite";
        public const int WeaponIconSpriteCount = 5;
        public const string WeaponIconAsset = "WeaponIconAsset";
        public const int WeaponIconAssetCount = 5;
        public const string WeaponIconBundle = "WeaponIconBundle";
        public const int WeaponIconBundleCount = 5;
        public const string WeaponGroupShow = "WeaponGroupShow";
        public const int WeaponGroupShowCount = 5;
        public const string WeaponNameText = "WeaponNameText";
        public const int WeaponNameTextCount = 5;
        public const string WearIconAsset = "WearIconAsset";
        public const int WearIconAssetCount = 6;
        public const string WearIconBundle = "WearIconBundle";
        public const int WearIconBundleCount = 6;
        public const string WearNameText = "WearNameText";
        public const int WearNameTextCount = 6;
        public const string WearGroupShow = "WearGroupShow";
        public const int WearGroupShowCount = 6;
        public bool SetGearLastingLayerShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		GearLastingLayerShow1 = val;
        		break;
        	case 2:
        		GearLastingLayerShow2 = val;
        		break;
        	case 3:
        		GearLastingLayerShow3 = val;
        		break;
        	case 4:
        		GearLastingLayerShow4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetGearLastingLayerShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearLastingLayerShow1;
        	case 2:
        		return GearLastingLayerShow2;
        	case 3:
        		return GearLastingLayerShow3;
        	case 4:
        		return GearLastingLayerShow4;
        	default:
        		return default(bool);
        	}
        }
        public bool SetGearLastingLayerValue (int index, float val)
        {
        	switch(index)
        	{
        	case 1:
        		GearLastingLayerValue1 = val;
        		break;
        	case 2:
        		GearLastingLayerValue2 = val;
        		break;
        	case 3:
        		GearLastingLayerValue3 = val;
        		break;
        	case 4:
        		GearLastingLayerValue4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public float GetGearLastingLayerValue (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearLastingLayerValue1;
        	case 2:
        		return GearLastingLayerValue2;
        	case 3:
        		return GearLastingLayerValue3;
        	case 4:
        		return GearLastingLayerValue4;
        	default:
        		return default(float);
        	}
        }
        public bool SetGearIconSprite (int index, Sprite val)
        {
        	switch(index)
        	{
        	case 1:
        		GearIconSprite1 = val;
        		break;
        	case 2:
        		GearIconSprite2 = val;
        		break;
        	case 3:
        		GearIconSprite3 = val;
        		break;
        	case 4:
        		GearIconSprite4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Sprite GetGearIconSprite (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearIconSprite1;
        	case 2:
        		return GearIconSprite2;
        	case 3:
        		return GearIconSprite3;
        	case 4:
        		return GearIconSprite4;
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetGearIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		GearIconAsset1 = val;
        		break;
        	case 2:
        		GearIconAsset2 = val;
        		break;
        	case 3:
        		GearIconAsset3 = val;
        		break;
        	case 4:
        		GearIconAsset4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetGearIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearIconAsset1;
        	case 2:
        		return GearIconAsset2;
        	case 3:
        		return GearIconAsset3;
        	case 4:
        		return GearIconAsset4;
        	default:
        		return default(string);
        	}
        }
        public bool SetGearIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		GearIconBundle1 = val;
        		break;
        	case 2:
        		GearIconBundle2 = val;
        		break;
        	case 3:
        		GearIconBundle3 = val;
        		break;
        	case 4:
        		GearIconBundle4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetGearIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearIconBundle1;
        	case 2:
        		return GearIconBundle2;
        	case 3:
        		return GearIconBundle3;
        	case 4:
        		return GearIconBundle4;
        	default:
        		return default(string);
        	}
        }
        public bool SetGearIconGroupShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		GearIconGroupShow1 = val;
        		break;
        	case 2:
        		GearIconGroupShow2 = val;
        		break;
        	case 3:
        		GearIconGroupShow3 = val;
        		break;
        	case 4:
        		GearIconGroupShow4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetGearIconGroupShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearIconGroupShow1;
        	case 2:
        		return GearIconGroupShow2;
        	case 3:
        		return GearIconGroupShow3;
        	case 4:
        		return GearIconGroupShow4;
        	default:
        		return default(bool);
        	}
        }
        public bool SetGearLastingBgShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		GearLastingBgShow1 = val;
        		break;
        	case 2:
        		GearLastingBgShow2 = val;
        		break;
        	case 3:
        		GearLastingBgShow3 = val;
        		break;
        	case 4:
        		GearLastingBgShow4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetGearLastingBgShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearLastingBgShow1;
        	case 2:
        		return GearLastingBgShow2;
        	case 3:
        		return GearLastingBgShow3;
        	case 4:
        		return GearLastingBgShow4;
        	default:
        		return default(bool);
        	}
        }
        public bool SetGearLastingNumText (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		GearLastingNumText1 = val;
        		break;
        	case 2:
        		GearLastingNumText2 = val;
        		break;
        	case 3:
        		GearLastingNumText3 = val;
        		break;
        	case 4:
        		GearLastingNumText4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetGearLastingNumText (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearLastingNumText1;
        	case 2:
        		return GearLastingNumText2;
        	case 3:
        		return GearLastingNumText3;
        	case 4:
        		return GearLastingNumText4;
        	default:
        		return default(string);
        	}
        }
        public bool SetGearNameText (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		GearNameText1 = val;
        		break;
        	case 2:
        		GearNameText2 = val;
        		break;
        	case 3:
        		GearNameText3 = val;
        		break;
        	case 4:
        		GearNameText4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetGearNameText (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GearNameText1;
        	case 2:
        		return GearNameText2;
        	case 3:
        		return GearNameText3;
        	case 4:
        		return GearNameText4;
        	default:
        		return default(string);
        	}
        }
        public bool SetUpperRailSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		UpperRailSlotShow1 = val;
        		break;
        	case 2:
        		UpperRailSlotShow2 = val;
        		break;
        	case 3:
        		UpperRailSlotShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetUpperRailSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpperRailSlotShow1;
        	case 2:
        		return UpperRailSlotShow2;
        	case 3:
        		return UpperRailSlotShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetMuzzleSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		MuzzleSlotShow1 = val;
        		break;
        	case 2:
        		MuzzleSlotShow2 = val;
        		break;
        	case 3:
        		MuzzleSlotShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetMuzzleSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MuzzleSlotShow1;
        	case 2:
        		return MuzzleSlotShow2;
        	case 3:
        		return MuzzleSlotShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetLowerRailSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		LowerRailSlotShow1 = val;
        		break;
        	case 2:
        		LowerRailSlotShow2 = val;
        		break;
        	case 3:
        		LowerRailSlotShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetLowerRailSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LowerRailSlotShow1;
        	case 2:
        		return LowerRailSlotShow2;
        	case 3:
        		return LowerRailSlotShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetMagazineSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		MagazineSlotShow1 = val;
        		break;
        	case 2:
        		MagazineSlotShow2 = val;
        		break;
        	case 3:
        		MagazineSlotShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetMagazineSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MagazineSlotShow1;
        	case 2:
        		return MagazineSlotShow2;
        	case 3:
        		return MagazineSlotShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetStockSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		StockSlotShow1 = val;
        		break;
        	case 2:
        		StockSlotShow2 = val;
        		break;
        	case 3:
        		StockSlotShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetStockSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return StockSlotShow1;
        	case 2:
        		return StockSlotShow2;
        	case 3:
        		return StockSlotShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetUpperRailIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		UpperRailIconAsset1 = val;
        		break;
        	case 2:
        		UpperRailIconAsset2 = val;
        		break;
        	case 3:
        		UpperRailIconAsset3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetUpperRailIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpperRailIconAsset1;
        	case 2:
        		return UpperRailIconAsset2;
        	case 3:
        		return UpperRailIconAsset3;
        	default:
        		return default(string);
        	}
        }
        public bool SetUpperRailIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		UpperRailIconBundle1 = val;
        		break;
        	case 2:
        		UpperRailIconBundle2 = val;
        		break;
        	case 3:
        		UpperRailIconBundle3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetUpperRailIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpperRailIconBundle1;
        	case 2:
        		return UpperRailIconBundle2;
        	case 3:
        		return UpperRailIconBundle3;
        	default:
        		return default(string);
        	}
        }
        public bool SetUpperRailIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		UpperRailIconShow1 = val;
        		break;
        	case 2:
        		UpperRailIconShow2 = val;
        		break;
        	case 3:
        		UpperRailIconShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetUpperRailIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return UpperRailIconShow1;
        	case 2:
        		return UpperRailIconShow2;
        	case 3:
        		return UpperRailIconShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetMuzzleIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		MuzzleIconAsset1 = val;
        		break;
        	case 2:
        		MuzzleIconAsset2 = val;
        		break;
        	case 3:
        		MuzzleIconAsset3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetMuzzleIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MuzzleIconAsset1;
        	case 2:
        		return MuzzleIconAsset2;
        	case 3:
        		return MuzzleIconAsset3;
        	default:
        		return default(string);
        	}
        }
        public bool SetMuzzleIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		MuzzleIconBundle1 = val;
        		break;
        	case 2:
        		MuzzleIconBundle2 = val;
        		break;
        	case 3:
        		MuzzleIconBundle3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetMuzzleIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MuzzleIconBundle1;
        	case 2:
        		return MuzzleIconBundle2;
        	case 3:
        		return MuzzleIconBundle3;
        	default:
        		return default(string);
        	}
        }
        public bool SetMuzzleIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		MuzzleIconShow1 = val;
        		break;
        	case 2:
        		MuzzleIconShow2 = val;
        		break;
        	case 3:
        		MuzzleIconShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetMuzzleIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MuzzleIconShow1;
        	case 2:
        		return MuzzleIconShow2;
        	case 3:
        		return MuzzleIconShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetLowerRailIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		LowerRailIconAsset1 = val;
        		break;
        	case 2:
        		LowerRailIconAsset2 = val;
        		break;
        	case 3:
        		LowerRailIconAsset3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetLowerRailIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LowerRailIconAsset1;
        	case 2:
        		return LowerRailIconAsset2;
        	case 3:
        		return LowerRailIconAsset3;
        	default:
        		return default(string);
        	}
        }
        public bool SetLowerRailIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		LowerRailIconBundle1 = val;
        		break;
        	case 2:
        		LowerRailIconBundle2 = val;
        		break;
        	case 3:
        		LowerRailIconBundle3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetLowerRailIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LowerRailIconBundle1;
        	case 2:
        		return LowerRailIconBundle2;
        	case 3:
        		return LowerRailIconBundle3;
        	default:
        		return default(string);
        	}
        }
        public bool SetLowerRailIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		LowerRailIconShow1 = val;
        		break;
        	case 2:
        		LowerRailIconShow2 = val;
        		break;
        	case 3:
        		LowerRailIconShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetLowerRailIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return LowerRailIconShow1;
        	case 2:
        		return LowerRailIconShow2;
        	case 3:
        		return LowerRailIconShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetMagazineIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		MagazineIconAsset1 = val;
        		break;
        	case 2:
        		MagazineIconAsset2 = val;
        		break;
        	case 3:
        		MagazineIconAsset3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetMagazineIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MagazineIconAsset1;
        	case 2:
        		return MagazineIconAsset2;
        	case 3:
        		return MagazineIconAsset3;
        	default:
        		return default(string);
        	}
        }
        public bool SetMagazineIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		MagazineIconBundle1 = val;
        		break;
        	case 2:
        		MagazineIconBundle2 = val;
        		break;
        	case 3:
        		MagazineIconBundle3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetMagazineIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MagazineIconBundle1;
        	case 2:
        		return MagazineIconBundle2;
        	case 3:
        		return MagazineIconBundle3;
        	default:
        		return default(string);
        	}
        }
        public bool SetMagazineIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		MagazineIconShow1 = val;
        		break;
        	case 2:
        		MagazineIconShow2 = val;
        		break;
        	case 3:
        		MagazineIconShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetMagazineIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MagazineIconShow1;
        	case 2:
        		return MagazineIconShow2;
        	case 3:
        		return MagazineIconShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetStockIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		StockIconAsset1 = val;
        		break;
        	case 2:
        		StockIconAsset2 = val;
        		break;
        	case 3:
        		StockIconAsset3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetStockIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return StockIconAsset1;
        	case 2:
        		return StockIconAsset2;
        	case 3:
        		return StockIconAsset3;
        	default:
        		return default(string);
        	}
        }
        public bool SetStockIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		StockIconBundle1 = val;
        		break;
        	case 2:
        		StockIconBundle2 = val;
        		break;
        	case 3:
        		StockIconBundle3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetStockIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return StockIconBundle1;
        	case 2:
        		return StockIconBundle2;
        	case 3:
        		return StockIconBundle3;
        	default:
        		return default(string);
        	}
        }
        public bool SetStockIconShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		StockIconShow1 = val;
        		break;
        	case 2:
        		StockIconShow2 = val;
        		break;
        	case 3:
        		StockIconShow3 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetStockIconShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return StockIconShow1;
        	case 2:
        		return StockIconShow2;
        	case 3:
        		return StockIconShow3;
        	default:
        		return default(bool);
        	}
        }
        public bool SetWeaponIconSprite (int index, Sprite val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconSprite1 = val;
        		break;
        	case 2:
        		WeaponIconSprite2 = val;
        		break;
        	case 3:
        		WeaponIconSprite3 = val;
        		break;
        	case 4:
        		WeaponIconSprite4 = val;
        		break;
        	case 5:
        		WeaponIconSprite5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Sprite GetWeaponIconSprite (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconSprite1;
        	case 2:
        		return WeaponIconSprite2;
        	case 3:
        		return WeaponIconSprite3;
        	case 4:
        		return WeaponIconSprite4;
        	case 5:
        		return WeaponIconSprite5;
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetWeaponIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconAsset1 = val;
        		break;
        	case 2:
        		WeaponIconAsset2 = val;
        		break;
        	case 3:
        		WeaponIconAsset3 = val;
        		break;
        	case 4:
        		WeaponIconAsset4 = val;
        		break;
        	case 5:
        		WeaponIconAsset5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWeaponIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconAsset1;
        	case 2:
        		return WeaponIconAsset2;
        	case 3:
        		return WeaponIconAsset3;
        	case 4:
        		return WeaponIconAsset4;
        	case 5:
        		return WeaponIconAsset5;
        	default:
        		return default(string);
        	}
        }
        public bool SetWeaponIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconBundle1 = val;
        		break;
        	case 2:
        		WeaponIconBundle2 = val;
        		break;
        	case 3:
        		WeaponIconBundle3 = val;
        		break;
        	case 4:
        		WeaponIconBundle4 = val;
        		break;
        	case 5:
        		WeaponIconBundle5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWeaponIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconBundle1;
        	case 2:
        		return WeaponIconBundle2;
        	case 3:
        		return WeaponIconBundle3;
        	case 4:
        		return WeaponIconBundle4;
        	case 5:
        		return WeaponIconBundle5;
        	default:
        		return default(string);
        	}
        }
        public bool SetWeaponGroupShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponGroupShow1 = val;
        		break;
        	case 2:
        		WeaponGroupShow2 = val;
        		break;
        	case 3:
        		WeaponGroupShow3 = val;
        		break;
        	case 4:
        		WeaponGroupShow4 = val;
        		break;
        	case 5:
        		WeaponGroupShow5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetWeaponGroupShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponGroupShow1;
        	case 2:
        		return WeaponGroupShow2;
        	case 3:
        		return WeaponGroupShow3;
        	case 4:
        		return WeaponGroupShow4;
        	case 5:
        		return WeaponGroupShow5;
        	default:
        		return default(bool);
        	}
        }
        public bool SetWeaponNameText (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponNameText1 = val;
        		break;
        	case 2:
        		WeaponNameText2 = val;
        		break;
        	case 3:
        		WeaponNameText3 = val;
        		break;
        	case 4:
        		WeaponNameText4 = val;
        		break;
        	case 5:
        		WeaponNameText5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWeaponNameText (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponNameText1;
        	case 2:
        		return WeaponNameText2;
        	case 3:
        		return WeaponNameText3;
        	case 4:
        		return WeaponNameText4;
        	case 5:
        		return WeaponNameText5;
        	default:
        		return default(string);
        	}
        }
        public bool SetWearIconAsset (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WearIconAsset1 = val;
        		break;
        	case 2:
        		WearIconAsset2 = val;
        		break;
        	case 3:
        		WearIconAsset3 = val;
        		break;
        	case 4:
        		WearIconAsset4 = val;
        		break;
        	case 5:
        		WearIconAsset5 = val;
        		break;
        	case 6:
        		WearIconAsset6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWearIconAsset (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WearIconAsset1;
        	case 2:
        		return WearIconAsset2;
        	case 3:
        		return WearIconAsset3;
        	case 4:
        		return WearIconAsset4;
        	case 5:
        		return WearIconAsset5;
        	case 6:
        		return WearIconAsset6;
        	default:
        		return default(string);
        	}
        }
        public bool SetWearIconBundle (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WearIconBundle1 = val;
        		break;
        	case 2:
        		WearIconBundle2 = val;
        		break;
        	case 3:
        		WearIconBundle3 = val;
        		break;
        	case 4:
        		WearIconBundle4 = val;
        		break;
        	case 5:
        		WearIconBundle5 = val;
        		break;
        	case 6:
        		WearIconBundle6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWearIconBundle (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WearIconBundle1;
        	case 2:
        		return WearIconBundle2;
        	case 3:
        		return WearIconBundle3;
        	case 4:
        		return WearIconBundle4;
        	case 5:
        		return WearIconBundle5;
        	case 6:
        		return WearIconBundle6;
        	default:
        		return default(string);
        	}
        }
        public bool SetWearNameText (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		WearNameText1 = val;
        		break;
        	case 2:
        		WearNameText2 = val;
        		break;
        	case 3:
        		WearNameText3 = val;
        		break;
        	case 4:
        		WearNameText4 = val;
        		break;
        	case 5:
        		WearNameText5 = val;
        		break;
        	case 6:
        		WearNameText6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetWearNameText (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WearNameText1;
        	case 2:
        		return WearNameText2;
        	case 3:
        		return WearNameText3;
        	case 4:
        		return WearNameText4;
        	case 5:
        		return WearNameText5;
        	case 6:
        		return WearNameText6;
        	default:
        		return default(string);
        	}
        }
        public bool SetWearGroupShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		WearGroupShow1 = val;
        		break;
        	case 2:
        		WearGroupShow2 = val;
        		break;
        	case 3:
        		WearGroupShow3 = val;
        		break;
        	case 4:
        		WearGroupShow4 = val;
        		break;
        	case 5:
        		WearGroupShow5 = val;
        		break;
        	case 6:
        		WearGroupShow6 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetWearGroupShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WearGroupShow1;
        	case 2:
        		return WearGroupShow2;
        	case 3:
        		return WearGroupShow3;
        	case 4:
        		return WearGroupShow4;
        	case 5:
        		return WearGroupShow5;
        	case 6:
        		return WearGroupShow6;
        	default:
        		return default(bool);
        	}
        }
        public string ResourceBundleName { get { return "ui/client/prefab/chicken"; } }
        public string ResourceAssetName { get { return "ChickenBag"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
