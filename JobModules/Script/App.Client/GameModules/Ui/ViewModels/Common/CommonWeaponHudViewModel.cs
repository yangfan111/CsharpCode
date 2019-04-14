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

namespace App.Client.GameModules.Ui.ViewModels.Common
{
    public class CommonWeaponHudViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonWeaponHudView : UIView
        {
            public GameObject Show;
            [HideInInspector]
            public bool oriShow;
            public GameObject WeaponGroupShow;
            [HideInInspector]
            public bool oriWeaponGroupShow;
            public GameObject WeaponModeShow;
            [HideInInspector]
            public bool oriWeaponModeShow;
            public GameObject AtkModeShow;
            [HideInInspector]
            public bool oriAtkModeShow;
            public Text AtkModeString;
            [HideInInspector]
            public string oriAtkModeString;
            public GameObject BulletCountShow;
            [HideInInspector]
            public bool oriBulletCountShow;
            public Text BulletCountString;
            [HideInInspector]
            public string oriBulletCountString;
            public Text BulletCountColor;
            [HideInInspector]
            public Color oriBulletCountColor;
            public CanvasGroup BulletAlapha;
            [HideInInspector]
            public float oriBulletAlapha;
            public RectTransform BulletScale;
            [HideInInspector]
            public Vector3 oriBulletScale;
            public GameObject ReservedBulletCountShow;
            [HideInInspector]
            public bool oriReservedBulletCountShow;
            public Text ReservedBulletCountString;
            [HideInInspector]
            public string oriReservedBulletCountString;
            public GameObject WeaponSlotShow1;
            public RectTransform WeaponSlotScale1;
            public CanvasGroup WeaponSlotAlpha1;
            public GameObject WeaponSlotShow2;
            public RectTransform WeaponSlotScale2;
            public CanvasGroup WeaponSlotAlpha2;
            public GameObject WeaponSlotShow3;
            public RectTransform WeaponSlotScale3;
            public CanvasGroup WeaponSlotAlpha3;
            public GameObject WeaponSlotShow4;
            public RectTransform WeaponSlotScale4;
            public CanvasGroup WeaponSlotAlpha4;
            public GameObject WeaponSlotShow5;
            public RectTransform WeaponSlotScale5;
            public CanvasGroup WeaponSlotAlpha5;
            public Text MarkText1;
            public Text MarkText2;
            public Text MarkText3;
            public Text MarkText4;
            public Text MarkText5;
            public RectTransform WeaponIconScale1;
            public Image WeaponIconSprite1;
            public Image WeaponIconColor1;
            public CanvasGroup WeaponIconAlpha1;
            public RectTransform WeaponIconScale2;
            public Image WeaponIconSprite2;
            public Image WeaponIconColor2;
            public CanvasGroup WeaponIconAlpha2;
            public RectTransform WeaponIconScale3;
            public Image WeaponIconSprite3;
            public Image WeaponIconColor3;
            public CanvasGroup WeaponIconAlpha3;
            public RectTransform WeaponIconScale4;
            public Image WeaponIconSprite4;
            public Image WeaponIconColor4;
            public CanvasGroup WeaponIconAlpha4;
            public GameObject GrenadeIconGroupShow1;
            public CanvasGroup GrenadeIconAlpha1;
            public GameObject GrenadeIconGroupShow2;
            public CanvasGroup GrenadeIconAlpha2;
            public GameObject GrenadeIconGroupShow3;
            public CanvasGroup GrenadeIconAlpha3;
            public GameObject GrenadeIconGroupShow4;
            public CanvasGroup GrenadeIconAlpha4;
            public Image GrenadeIconSprite1;
            public RectTransform GrenadeIconScale1;
            public Image GrenadeIconSprite2;
            public RectTransform GrenadeIconScale2;
            public Image GrenadeIconSprite3;
            public RectTransform GrenadeIconScale3;
            public Image GrenadeIconSprite4;
            public RectTransform GrenadeIconScale4;
            
            public void FillField()
            {
                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Show":
                            Show = v.gameObject;
                            break;
                        case "WeaponGroup":
                            WeaponGroupShow = v.gameObject;
                            break;
                        case "WeaponMode":
                            WeaponModeShow = v.gameObject;
                            break;
                        case "AtkMode":
                            AtkModeShow = v.gameObject;
                            break;
                        case "BulletCount":
                            BulletCountShow = v.gameObject;
                            break;
                        case "ReservedBulletCount":
                            ReservedBulletCountShow = v.gameObject;
                            break;
                        case "WeaponSlot1":
                            WeaponSlotShow1 = v.gameObject;
                            break;
                        case "WeaponSlot2":
                            WeaponSlotShow2 = v.gameObject;
                            break;
                        case "WeaponSlot3":
                            WeaponSlotShow3 = v.gameObject;
                            break;
                        case "WeaponSlot4":
                            WeaponSlotShow4 = v.gameObject;
                            break;
                        case "WeaponSlot5":
                            WeaponSlotShow5 = v.gameObject;
                            break;
                        case "GrenadeIconGroup1":
                            GrenadeIconGroupShow1 = v.gameObject;
                            break;
                        case "GrenadeIconGroup2":
                            GrenadeIconGroupShow2 = v.gameObject;
                            break;
                        case "GrenadeIconGroup3":
                            GrenadeIconGroupShow3 = v.gameObject;
                            break;
                        case "GrenadeIconGroup4":
                            GrenadeIconGroupShow4 = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "AtkMode":
                            AtkModeString = v;
                            break;
                        case "BulletCount":
                            BulletCountString = v;
                            BulletCountColor = v;
                            break;
                        case "ReservedBulletCount":
                            ReservedBulletCountString = v;
                            break;
                        case "MarkText1":
                            MarkText1 = v;
                            break;
                        case "MarkText2":
                            MarkText2 = v;
                            break;
                        case "MarkText3":
                            MarkText3 = v;
                            break;
                        case "MarkText4":
                            MarkText4 = v;
                            break;
                        case "MarkText5":
                            MarkText5 = v;
                            break;
                    }
                }

                CanvasGroup[] canvasgroups = gameObject.GetComponentsInChildren<CanvasGroup>(true);
                foreach (var v in canvasgroups)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BulletCount":
                            BulletAlapha = v;
                            break;
                        case "WeaponSlot1":
                            WeaponSlotAlpha1 = v;
                            break;
                        case "WeaponSlot2":
                            WeaponSlotAlpha2 = v;
                            break;
                        case "WeaponSlot3":
                            WeaponSlotAlpha3 = v;
                            break;
                        case "WeaponSlot4":
                            WeaponSlotAlpha4 = v;
                            break;
                        case "WeaponSlot5":
                            WeaponSlotAlpha5 = v;
                            break;
                        case "WeaponBg1":
                            WeaponIconAlpha1 = v;
                            break;
                        case "WeaponBg2":
                            WeaponIconAlpha2 = v;
                            break;
                        case "WeaponBg3":
                            WeaponIconAlpha3 = v;
                            break;
                        case "WeaponBg4":
                            WeaponIconAlpha4 = v;
                            break;
                        case "GrenadeIconGroup1":
                            GrenadeIconAlpha1 = v;
                            break;
                        case "GrenadeIconGroup2":
                            GrenadeIconAlpha2 = v;
                            break;
                        case "GrenadeIconGroup3":
                            GrenadeIconAlpha3 = v;
                            break;
                        case "GrenadeIconGroup4":
                            GrenadeIconAlpha4 = v;
                            break;
                    }
                }

                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BulletCount":
                            BulletScale = v;
                            break;
                        case "WeaponSlot1":
                            WeaponSlotScale1 = v;
                            break;
                        case "WeaponSlot2":
                            WeaponSlotScale2 = v;
                            break;
                        case "WeaponSlot3":
                            WeaponSlotScale3 = v;
                            break;
                        case "WeaponSlot4":
                            WeaponSlotScale4 = v;
                            break;
                        case "WeaponSlot5":
                            WeaponSlotScale5 = v;
                            break;
                        case "WeaponBg1":
                            WeaponIconScale1 = v;
                            break;
                        case "WeaponBg2":
                            WeaponIconScale2 = v;
                            break;
                        case "WeaponBg3":
                            WeaponIconScale3 = v;
                            break;
                        case "WeaponBg4":
                            WeaponIconScale4 = v;
                            break;
                        case "GrenadeIcon1":
                            GrenadeIconScale1 = v;
                            break;
                        case "GrenadeIcon2":
                            GrenadeIconScale2 = v;
                            break;
                        case "GrenadeIcon3":
                            GrenadeIconScale3 = v;
                            break;
                        case "GrenadeIcon4":
                            GrenadeIconScale4 = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "WeaponBg1":
                            WeaponIconSprite1 = v;
                            WeaponIconColor1 = v;
                            break;
                        case "WeaponBg2":
                            WeaponIconSprite2 = v;
                            WeaponIconColor2 = v;
                            break;
                        case "WeaponBg3":
                            WeaponIconSprite3 = v;
                            WeaponIconColor3 = v;
                            break;
                        case "WeaponBg4":
                            WeaponIconSprite4 = v;
                            WeaponIconColor4 = v;
                            break;
                        case "GrenadeIcon1":
                            GrenadeIconSprite1 = v;
                            break;
                        case "GrenadeIcon2":
                            GrenadeIconSprite2 = v;
                            break;
                        case "GrenadeIcon3":
                            GrenadeIconSprite3 = v;
                            break;
                        case "GrenadeIcon4":
                            GrenadeIconSprite4 = v;
                            break;
                    }
                }

            }
        }


        private bool _show;
        private bool _weaponGroupShow;
        private bool _weaponModeShow;
        private bool _atkModeShow;
        private string _atkModeString;
        private bool _bulletCountShow;
        private string _bulletCountString;
        private Color _bulletCountColor;
        private float _bulletAlapha;
        private Vector3 _bulletScale;
        private bool _reservedBulletCountShow;
        private string _reservedBulletCountString;
        private bool _weaponSlotShow1;
        private Vector3 _weaponSlotScale1;
        private float _weaponSlotAlpha1;
        private bool _weaponSlotShow2;
        private Vector3 _weaponSlotScale2;
        private float _weaponSlotAlpha2;
        private bool _weaponSlotShow3;
        private Vector3 _weaponSlotScale3;
        private float _weaponSlotAlpha3;
        private bool _weaponSlotShow4;
        private Vector3 _weaponSlotScale4;
        private float _weaponSlotAlpha4;
        private bool _weaponSlotShow5;
        private Vector3 _weaponSlotScale5;
        private float _weaponSlotAlpha5;
        private string _markText1;
        private string _markText2;
        private string _markText3;
        private string _markText4;
        private string _markText5;
        private Vector3 _weaponIconScale1;
        private Sprite _weaponIconSprite1;
        private Color _weaponIconColor1;
        private float _weaponIconAlpha1;
        private Vector3 _weaponIconScale2;
        private Sprite _weaponIconSprite2;
        private Color _weaponIconColor2;
        private float _weaponIconAlpha2;
        private Vector3 _weaponIconScale3;
        private Sprite _weaponIconSprite3;
        private Color _weaponIconColor3;
        private float _weaponIconAlpha3;
        private Vector3 _weaponIconScale4;
        private Sprite _weaponIconSprite4;
        private Color _weaponIconColor4;
        private float _weaponIconAlpha4;
        private bool _grenadeIconGroupShow1;
        private float _grenadeIconAlpha1;
        private bool _grenadeIconGroupShow2;
        private float _grenadeIconAlpha2;
        private bool _grenadeIconGroupShow3;
        private float _grenadeIconAlpha3;
        private bool _grenadeIconGroupShow4;
        private float _grenadeIconAlpha4;
        private Sprite _grenadeIconSprite1;
        private Vector3 _grenadeIconScale1;
        private Sprite _grenadeIconSprite2;
        private Vector3 _grenadeIconScale2;
        private Sprite _grenadeIconSprite3;
        private Vector3 _grenadeIconScale3;
        private Sprite _grenadeIconSprite4;
        private Vector3 _grenadeIconScale4;
        public bool Show { get { return _show; } set {if(_show != value) Set(ref _show, value, "Show"); } }
        public bool WeaponGroupShow { get { return _weaponGroupShow; } set {if(_weaponGroupShow != value) Set(ref _weaponGroupShow, value, "WeaponGroupShow"); } }
        public bool WeaponModeShow { get { return _weaponModeShow; } set {if(_weaponModeShow != value) Set(ref _weaponModeShow, value, "WeaponModeShow"); } }
        public bool AtkModeShow { get { return _atkModeShow; } set {if(_atkModeShow != value) Set(ref _atkModeShow, value, "AtkModeShow"); } }
        public string AtkModeString { get { return _atkModeString; } set {if(_atkModeString != value) Set(ref _atkModeString, value, "AtkModeString"); } }
        public bool BulletCountShow { get { return _bulletCountShow; } set {if(_bulletCountShow != value) Set(ref _bulletCountShow, value, "BulletCountShow"); } }
        public string BulletCountString { get { return _bulletCountString; } set {if(_bulletCountString != value) Set(ref _bulletCountString, value, "BulletCountString"); } }
        public Color BulletCountColor { get { return _bulletCountColor; } set {if(_bulletCountColor != value) Set(ref _bulletCountColor, value, "BulletCountColor"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float BulletAlapha { get { return _bulletAlapha; } set {if(_bulletAlapha != value) Set(ref _bulletAlapha, value, "BulletAlapha"); } }
        public Vector3 BulletScale { get { return _bulletScale; } set {if(_bulletScale != value) Set(ref _bulletScale, value, "BulletScale"); } }
        public bool ReservedBulletCountShow { get { return _reservedBulletCountShow; } set {if(_reservedBulletCountShow != value) Set(ref _reservedBulletCountShow, value, "ReservedBulletCountShow"); } }
        public string ReservedBulletCountString { get { return _reservedBulletCountString; } set {if(_reservedBulletCountString != value) Set(ref _reservedBulletCountString, value, "ReservedBulletCountString"); } }
        public bool WeaponSlotShow1 { get { return _weaponSlotShow1; } set {if(_weaponSlotShow1 != value) Set(ref _weaponSlotShow1, value, "WeaponSlotShow1"); } }
        public Vector3 WeaponSlotScale1 { get { return _weaponSlotScale1; } set {if(_weaponSlotScale1 != value) Set(ref _weaponSlotScale1, value, "WeaponSlotScale1"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponSlotAlpha1 { get { return _weaponSlotAlpha1; } set {if(_weaponSlotAlpha1 != value) Set(ref _weaponSlotAlpha1, value, "WeaponSlotAlpha1"); } }
        public bool WeaponSlotShow2 { get { return _weaponSlotShow2; } set {if(_weaponSlotShow2 != value) Set(ref _weaponSlotShow2, value, "WeaponSlotShow2"); } }
        public Vector3 WeaponSlotScale2 { get { return _weaponSlotScale2; } set {if(_weaponSlotScale2 != value) Set(ref _weaponSlotScale2, value, "WeaponSlotScale2"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponSlotAlpha2 { get { return _weaponSlotAlpha2; } set {if(_weaponSlotAlpha2 != value) Set(ref _weaponSlotAlpha2, value, "WeaponSlotAlpha2"); } }
        public bool WeaponSlotShow3 { get { return _weaponSlotShow3; } set {if(_weaponSlotShow3 != value) Set(ref _weaponSlotShow3, value, "WeaponSlotShow3"); } }
        public Vector3 WeaponSlotScale3 { get { return _weaponSlotScale3; } set {if(_weaponSlotScale3 != value) Set(ref _weaponSlotScale3, value, "WeaponSlotScale3"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponSlotAlpha3 { get { return _weaponSlotAlpha3; } set {if(_weaponSlotAlpha3 != value) Set(ref _weaponSlotAlpha3, value, "WeaponSlotAlpha3"); } }
        public bool WeaponSlotShow4 { get { return _weaponSlotShow4; } set {if(_weaponSlotShow4 != value) Set(ref _weaponSlotShow4, value, "WeaponSlotShow4"); } }
        public Vector3 WeaponSlotScale4 { get { return _weaponSlotScale4; } set {if(_weaponSlotScale4 != value) Set(ref _weaponSlotScale4, value, "WeaponSlotScale4"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponSlotAlpha4 { get { return _weaponSlotAlpha4; } set {if(_weaponSlotAlpha4 != value) Set(ref _weaponSlotAlpha4, value, "WeaponSlotAlpha4"); } }
        public bool WeaponSlotShow5 { get { return _weaponSlotShow5; } set {if(_weaponSlotShow5 != value) Set(ref _weaponSlotShow5, value, "WeaponSlotShow5"); } }
        public Vector3 WeaponSlotScale5 { get { return _weaponSlotScale5; } set {if(_weaponSlotScale5 != value) Set(ref _weaponSlotScale5, value, "WeaponSlotScale5"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponSlotAlpha5 { get { return _weaponSlotAlpha5; } set {if(_weaponSlotAlpha5 != value) Set(ref _weaponSlotAlpha5, value, "WeaponSlotAlpha5"); } }
        public string MarkText1 { get { return _markText1; } set {if(_markText1 != value) Set(ref _markText1, value, "MarkText1"); } }
        public string MarkText2 { get { return _markText2; } set {if(_markText2 != value) Set(ref _markText2, value, "MarkText2"); } }
        public string MarkText3 { get { return _markText3; } set {if(_markText3 != value) Set(ref _markText3, value, "MarkText3"); } }
        public string MarkText4 { get { return _markText4; } set {if(_markText4 != value) Set(ref _markText4, value, "MarkText4"); } }
        public string MarkText5 { get { return _markText5; } set {if(_markText5 != value) Set(ref _markText5, value, "MarkText5"); } }
        public Vector3 WeaponIconScale1 { get { return _weaponIconScale1; } set {if(_weaponIconScale1 != value) Set(ref _weaponIconScale1, value, "WeaponIconScale1"); } }
        public Sprite WeaponIconSprite1 { get { return _weaponIconSprite1; } set {if(_weaponIconSprite1 != value) Set(ref _weaponIconSprite1, value, "WeaponIconSprite1"); if(null != _view && null != _view.WeaponIconSprite1 && null == value) _view.WeaponIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor1 { get { return _weaponIconColor1; } set {if(_weaponIconColor1 != value) Set(ref _weaponIconColor1, value, "WeaponIconColor1"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponIconAlpha1 { get { return _weaponIconAlpha1; } set {if(_weaponIconAlpha1 != value) Set(ref _weaponIconAlpha1, value, "WeaponIconAlpha1"); } }
        public Vector3 WeaponIconScale2 { get { return _weaponIconScale2; } set {if(_weaponIconScale2 != value) Set(ref _weaponIconScale2, value, "WeaponIconScale2"); } }
        public Sprite WeaponIconSprite2 { get { return _weaponIconSprite2; } set {if(_weaponIconSprite2 != value) Set(ref _weaponIconSprite2, value, "WeaponIconSprite2"); if(null != _view && null != _view.WeaponIconSprite2 && null == value) _view.WeaponIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor2 { get { return _weaponIconColor2; } set {if(_weaponIconColor2 != value) Set(ref _weaponIconColor2, value, "WeaponIconColor2"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponIconAlpha2 { get { return _weaponIconAlpha2; } set {if(_weaponIconAlpha2 != value) Set(ref _weaponIconAlpha2, value, "WeaponIconAlpha2"); } }
        public Vector3 WeaponIconScale3 { get { return _weaponIconScale3; } set {if(_weaponIconScale3 != value) Set(ref _weaponIconScale3, value, "WeaponIconScale3"); } }
        public Sprite WeaponIconSprite3 { get { return _weaponIconSprite3; } set {if(_weaponIconSprite3 != value) Set(ref _weaponIconSprite3, value, "WeaponIconSprite3"); if(null != _view && null != _view.WeaponIconSprite3 && null == value) _view.WeaponIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor3 { get { return _weaponIconColor3; } set {if(_weaponIconColor3 != value) Set(ref _weaponIconColor3, value, "WeaponIconColor3"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponIconAlpha3 { get { return _weaponIconAlpha3; } set {if(_weaponIconAlpha3 != value) Set(ref _weaponIconAlpha3, value, "WeaponIconAlpha3"); } }
        public Vector3 WeaponIconScale4 { get { return _weaponIconScale4; } set {if(_weaponIconScale4 != value) Set(ref _weaponIconScale4, value, "WeaponIconScale4"); } }
        public Sprite WeaponIconSprite4 { get { return _weaponIconSprite4; } set {if(_weaponIconSprite4 != value) Set(ref _weaponIconSprite4, value, "WeaponIconSprite4"); if(null != _view && null != _view.WeaponIconSprite4 && null == value) _view.WeaponIconSprite4.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor4 { get { return _weaponIconColor4; } set {if(_weaponIconColor4 != value) Set(ref _weaponIconColor4, value, "WeaponIconColor4"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float WeaponIconAlpha4 { get { return _weaponIconAlpha4; } set {if(_weaponIconAlpha4 != value) Set(ref _weaponIconAlpha4, value, "WeaponIconAlpha4"); } }
        public bool GrenadeIconGroupShow1 { get { return _grenadeIconGroupShow1; } set {if(_grenadeIconGroupShow1 != value) Set(ref _grenadeIconGroupShow1, value, "GrenadeIconGroupShow1"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GrenadeIconAlpha1 { get { return _grenadeIconAlpha1; } set {if(_grenadeIconAlpha1 != value) Set(ref _grenadeIconAlpha1, value, "GrenadeIconAlpha1"); } }
        public bool GrenadeIconGroupShow2 { get { return _grenadeIconGroupShow2; } set {if(_grenadeIconGroupShow2 != value) Set(ref _grenadeIconGroupShow2, value, "GrenadeIconGroupShow2"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GrenadeIconAlpha2 { get { return _grenadeIconAlpha2; } set {if(_grenadeIconAlpha2 != value) Set(ref _grenadeIconAlpha2, value, "GrenadeIconAlpha2"); } }
        public bool GrenadeIconGroupShow3 { get { return _grenadeIconGroupShow3; } set {if(_grenadeIconGroupShow3 != value) Set(ref _grenadeIconGroupShow3, value, "GrenadeIconGroupShow3"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GrenadeIconAlpha3 { get { return _grenadeIconAlpha3; } set {if(_grenadeIconAlpha3 != value) Set(ref _grenadeIconAlpha3, value, "GrenadeIconAlpha3"); } }
        public bool GrenadeIconGroupShow4 { get { return _grenadeIconGroupShow4; } set {if(_grenadeIconGroupShow4 != value) Set(ref _grenadeIconGroupShow4, value, "GrenadeIconGroupShow4"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float GrenadeIconAlpha4 { get { return _grenadeIconAlpha4; } set {if(_grenadeIconAlpha4 != value) Set(ref _grenadeIconAlpha4, value, "GrenadeIconAlpha4"); } }
        public Sprite GrenadeIconSprite1 { get { return _grenadeIconSprite1; } set {if(_grenadeIconSprite1 != value) Set(ref _grenadeIconSprite1, value, "GrenadeIconSprite1"); if(null != _view && null != _view.GrenadeIconSprite1 && null == value) _view.GrenadeIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public Vector3 GrenadeIconScale1 { get { return _grenadeIconScale1; } set {if(_grenadeIconScale1 != value) Set(ref _grenadeIconScale1, value, "GrenadeIconScale1"); } }
        public Sprite GrenadeIconSprite2 { get { return _grenadeIconSprite2; } set {if(_grenadeIconSprite2 != value) Set(ref _grenadeIconSprite2, value, "GrenadeIconSprite2"); if(null != _view && null != _view.GrenadeIconSprite2 && null == value) _view.GrenadeIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public Vector3 GrenadeIconScale2 { get { return _grenadeIconScale2; } set {if(_grenadeIconScale2 != value) Set(ref _grenadeIconScale2, value, "GrenadeIconScale2"); } }
        public Sprite GrenadeIconSprite3 { get { return _grenadeIconSprite3; } set {if(_grenadeIconSprite3 != value) Set(ref _grenadeIconSprite3, value, "GrenadeIconSprite3"); if(null != _view && null != _view.GrenadeIconSprite3 && null == value) _view.GrenadeIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public Vector3 GrenadeIconScale3 { get { return _grenadeIconScale3; } set {if(_grenadeIconScale3 != value) Set(ref _grenadeIconScale3, value, "GrenadeIconScale3"); } }
        public Sprite GrenadeIconSprite4 { get { return _grenadeIconSprite4; } set {if(_grenadeIconSprite4 != value) Set(ref _grenadeIconSprite4, value, "GrenadeIconSprite4"); if(null != _view && null != _view.GrenadeIconSprite4 && null == value) _view.GrenadeIconSprite4.sprite = ViewModelUtil.EmptySprite; } }
        public Vector3 GrenadeIconScale4 { get { return _grenadeIconScale4; } set {if(_grenadeIconScale4 != value) Set(ref _grenadeIconScale4, value, "GrenadeIconScale4"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonWeaponHudView _view;
		
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
			var view = obj.GetComponent<CommonWeaponHudView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonWeaponHudView>();
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
		private void EventTriggerBind(CommonWeaponHudView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonWeaponHudViewModel()
        {
            Type type = typeof(CommonWeaponHudViewModel);
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

		void ViewBind(CommonWeaponHudView view)
		{
		     BindingSet<CommonWeaponHudView, CommonWeaponHudViewModel> bindingSet =
                view.CreateBindingSet<CommonWeaponHudView, CommonWeaponHudViewModel>();
            bindingSet.Bind(view.Show).For(v => v.activeSelf).To(vm => vm.Show).OneWay();
            bindingSet.Bind(view.WeaponGroupShow).For(v => v.activeSelf).To(vm => vm.WeaponGroupShow).OneWay();
            bindingSet.Bind(view.WeaponModeShow).For(v => v.activeSelf).To(vm => vm.WeaponModeShow).OneWay();
            bindingSet.Bind(view.AtkModeShow).For(v => v.activeSelf).To(vm => vm.AtkModeShow).OneWay();
            bindingSet.Bind(view.AtkModeString).For(v => v.text).To(vm => vm.AtkModeString).OneWay();
            bindingSet.Bind(view.BulletCountShow).For(v => v.activeSelf).To(vm => vm.BulletCountShow).OneWay();
            bindingSet.Bind(view.BulletCountString).For(v => v.text).To(vm => vm.BulletCountString).OneWay();
            bindingSet.Bind(view.BulletCountColor).For(v => v.color).To(vm => vm.BulletCountColor).OneWay();
            bindingSet.Bind(view.BulletAlapha).For(v => v.alpha).To(vm => vm.BulletAlapha).OneWay();
            bindingSet.Bind(view.BulletScale).For(v => v.localScale).To(vm => vm.BulletScale).OneWay();
            bindingSet.Bind(view.ReservedBulletCountShow).For(v => v.activeSelf).To(vm => vm.ReservedBulletCountShow).OneWay();
            bindingSet.Bind(view.ReservedBulletCountString).For(v => v.text).To(vm => vm.ReservedBulletCountString).OneWay();
            bindingSet.Bind(view.WeaponSlotShow1).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow1).OneWay();
            bindingSet.Bind(view.WeaponSlotScale1).For(v => v.localScale).To(vm => vm.WeaponSlotScale1).OneWay();
            bindingSet.Bind(view.WeaponSlotAlpha1).For(v => v.alpha).To(vm => vm.WeaponSlotAlpha1).OneWay();
            bindingSet.Bind(view.WeaponSlotShow2).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow2).OneWay();
            bindingSet.Bind(view.WeaponSlotScale2).For(v => v.localScale).To(vm => vm.WeaponSlotScale2).OneWay();
            bindingSet.Bind(view.WeaponSlotAlpha2).For(v => v.alpha).To(vm => vm.WeaponSlotAlpha2).OneWay();
            bindingSet.Bind(view.WeaponSlotShow3).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow3).OneWay();
            bindingSet.Bind(view.WeaponSlotScale3).For(v => v.localScale).To(vm => vm.WeaponSlotScale3).OneWay();
            bindingSet.Bind(view.WeaponSlotAlpha3).For(v => v.alpha).To(vm => vm.WeaponSlotAlpha3).OneWay();
            bindingSet.Bind(view.WeaponSlotShow4).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow4).OneWay();
            bindingSet.Bind(view.WeaponSlotScale4).For(v => v.localScale).To(vm => vm.WeaponSlotScale4).OneWay();
            bindingSet.Bind(view.WeaponSlotAlpha4).For(v => v.alpha).To(vm => vm.WeaponSlotAlpha4).OneWay();
            bindingSet.Bind(view.WeaponSlotShow5).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow5).OneWay();
            bindingSet.Bind(view.WeaponSlotScale5).For(v => v.localScale).To(vm => vm.WeaponSlotScale5).OneWay();
            bindingSet.Bind(view.WeaponSlotAlpha5).For(v => v.alpha).To(vm => vm.WeaponSlotAlpha5).OneWay();
            bindingSet.Bind(view.MarkText1).For(v => v.text).To(vm => vm.MarkText1).OneWay();
            bindingSet.Bind(view.MarkText2).For(v => v.text).To(vm => vm.MarkText2).OneWay();
            bindingSet.Bind(view.MarkText3).For(v => v.text).To(vm => vm.MarkText3).OneWay();
            bindingSet.Bind(view.MarkText4).For(v => v.text).To(vm => vm.MarkText4).OneWay();
            bindingSet.Bind(view.MarkText5).For(v => v.text).To(vm => vm.MarkText5).OneWay();
            bindingSet.Bind(view.WeaponIconScale1).For(v => v.localScale).To(vm => vm.WeaponIconScale1).OneWay();
            bindingSet.Bind(view.WeaponIconSprite1).For(v => v.sprite).To(vm => vm.WeaponIconSprite1).OneWay();
            bindingSet.Bind(view.WeaponIconColor1).For(v => v.color).To(vm => vm.WeaponIconColor1).OneWay();
            bindingSet.Bind(view.WeaponIconAlpha1).For(v => v.alpha).To(vm => vm.WeaponIconAlpha1).OneWay();
            bindingSet.Bind(view.WeaponIconScale2).For(v => v.localScale).To(vm => vm.WeaponIconScale2).OneWay();
            bindingSet.Bind(view.WeaponIconSprite2).For(v => v.sprite).To(vm => vm.WeaponIconSprite2).OneWay();
            bindingSet.Bind(view.WeaponIconColor2).For(v => v.color).To(vm => vm.WeaponIconColor2).OneWay();
            bindingSet.Bind(view.WeaponIconAlpha2).For(v => v.alpha).To(vm => vm.WeaponIconAlpha2).OneWay();
            bindingSet.Bind(view.WeaponIconScale3).For(v => v.localScale).To(vm => vm.WeaponIconScale3).OneWay();
            bindingSet.Bind(view.WeaponIconSprite3).For(v => v.sprite).To(vm => vm.WeaponIconSprite3).OneWay();
            bindingSet.Bind(view.WeaponIconColor3).For(v => v.color).To(vm => vm.WeaponIconColor3).OneWay();
            bindingSet.Bind(view.WeaponIconAlpha3).For(v => v.alpha).To(vm => vm.WeaponIconAlpha3).OneWay();
            bindingSet.Bind(view.WeaponIconScale4).For(v => v.localScale).To(vm => vm.WeaponIconScale4).OneWay();
            bindingSet.Bind(view.WeaponIconSprite4).For(v => v.sprite).To(vm => vm.WeaponIconSprite4).OneWay();
            bindingSet.Bind(view.WeaponIconColor4).For(v => v.color).To(vm => vm.WeaponIconColor4).OneWay();
            bindingSet.Bind(view.WeaponIconAlpha4).For(v => v.alpha).To(vm => vm.WeaponIconAlpha4).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow1).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow1).OneWay();
            bindingSet.Bind(view.GrenadeIconAlpha1).For(v => v.alpha).To(vm => vm.GrenadeIconAlpha1).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow2).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow2).OneWay();
            bindingSet.Bind(view.GrenadeIconAlpha2).For(v => v.alpha).To(vm => vm.GrenadeIconAlpha2).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow3).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow3).OneWay();
            bindingSet.Bind(view.GrenadeIconAlpha3).For(v => v.alpha).To(vm => vm.GrenadeIconAlpha3).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow4).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow4).OneWay();
            bindingSet.Bind(view.GrenadeIconAlpha4).For(v => v.alpha).To(vm => vm.GrenadeIconAlpha4).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite1).For(v => v.sprite).To(vm => vm.GrenadeIconSprite1).OneWay();
            bindingSet.Bind(view.GrenadeIconScale1).For(v => v.localScale).To(vm => vm.GrenadeIconScale1).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite2).For(v => v.sprite).To(vm => vm.GrenadeIconSprite2).OneWay();
            bindingSet.Bind(view.GrenadeIconScale2).For(v => v.localScale).To(vm => vm.GrenadeIconScale2).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite3).For(v => v.sprite).To(vm => vm.GrenadeIconSprite3).OneWay();
            bindingSet.Bind(view.GrenadeIconScale3).For(v => v.localScale).To(vm => vm.GrenadeIconScale3).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite4).For(v => v.sprite).To(vm => vm.GrenadeIconSprite4).OneWay();
            bindingSet.Bind(view.GrenadeIconScale4).For(v => v.localScale).To(vm => vm.GrenadeIconScale4).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonWeaponHudView view)
		{
            _show = view.Show.activeSelf;
            _weaponGroupShow = view.WeaponGroupShow.activeSelf;
            _weaponModeShow = view.WeaponModeShow.activeSelf;
            _atkModeShow = view.AtkModeShow.activeSelf;
            _atkModeString = view.AtkModeString.text;
            _bulletCountShow = view.BulletCountShow.activeSelf;
            _bulletCountString = view.BulletCountString.text;
            _bulletCountColor = view.BulletCountColor.color;
            _bulletAlapha = view.BulletAlapha.alpha;
            _bulletScale = view.BulletScale.localScale;
            _reservedBulletCountShow = view.ReservedBulletCountShow.activeSelf;
            _reservedBulletCountString = view.ReservedBulletCountString.text;
		}


		void SaveOriData(CommonWeaponHudView view)
		{
            view.oriShow = _show;
            view.oriWeaponGroupShow = _weaponGroupShow;
            view.oriWeaponModeShow = _weaponModeShow;
            view.oriAtkModeShow = _atkModeShow;
            view.oriAtkModeString = _atkModeString;
            view.oriBulletCountShow = _bulletCountShow;
            view.oriBulletCountString = _bulletCountString;
            view.oriBulletCountColor = _bulletCountColor;
            view.oriBulletAlapha = _bulletAlapha;
            view.oriBulletScale = _bulletScale;
            view.oriReservedBulletCountShow = _reservedBulletCountShow;
            view.oriReservedBulletCountString = _reservedBulletCountString;
		}




		private void SpriteReset()
		{
			WeaponIconSprite1 = ViewModelUtil.EmptySprite;
			WeaponIconSprite2 = ViewModelUtil.EmptySprite;
			WeaponIconSprite3 = ViewModelUtil.EmptySprite;
			WeaponIconSprite4 = ViewModelUtil.EmptySprite;
			GrenadeIconSprite1 = ViewModelUtil.EmptySprite;
			GrenadeIconSprite2 = ViewModelUtil.EmptySprite;
			GrenadeIconSprite3 = ViewModelUtil.EmptySprite;
			GrenadeIconSprite4 = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			Show = _view.oriShow;
			WeaponGroupShow = _view.oriWeaponGroupShow;
			WeaponModeShow = _view.oriWeaponModeShow;
			AtkModeShow = _view.oriAtkModeShow;
			AtkModeString = _view.oriAtkModeString;
			BulletCountShow = _view.oriBulletCountShow;
			BulletCountString = _view.oriBulletCountString;
			BulletCountColor = _view.oriBulletCountColor;
			BulletAlapha = _view.oriBulletAlapha;
			BulletScale = _view.oriBulletScale;
			ReservedBulletCountShow = _view.oriReservedBulletCountShow;
			ReservedBulletCountString = _view.oriReservedBulletCountString;
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

        public const string WeaponSlotShow = "WeaponSlotShow";
        public const int WeaponSlotShowCount = 5;
        public const string WeaponSlotScale = "WeaponSlotScale";
        public const int WeaponSlotScaleCount = 5;
        public const string WeaponSlotAlpha = "WeaponSlotAlpha";
        public const int WeaponSlotAlphaCount = 5;
        public const string MarkText = "MarkText";
        public const int MarkTextCount = 5;
        public const string WeaponIconScale = "WeaponIconScale";
        public const int WeaponIconScaleCount = 4;
        public const string WeaponIconSprite = "WeaponIconSprite";
        public const int WeaponIconSpriteCount = 4;
        public const string WeaponIconColor = "WeaponIconColor";
        public const int WeaponIconColorCount = 4;
        public const string WeaponIconAlpha = "WeaponIconAlpha";
        public const int WeaponIconAlphaCount = 4;
        public const string GrenadeIconGroupShow = "GrenadeIconGroupShow";
        public const int GrenadeIconGroupShowCount = 4;
        public const string GrenadeIconAlpha = "GrenadeIconAlpha";
        public const int GrenadeIconAlphaCount = 4;
        public const string GrenadeIconSprite = "GrenadeIconSprite";
        public const int GrenadeIconSpriteCount = 4;
        public const string GrenadeIconScale = "GrenadeIconScale";
        public const int GrenadeIconScaleCount = 4;
        public bool SetWeaponSlotShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponSlotShow1 = val;
        		break;
        	case 2:
        		WeaponSlotShow2 = val;
        		break;
        	case 3:
        		WeaponSlotShow3 = val;
        		break;
        	case 4:
        		WeaponSlotShow4 = val;
        		break;
        	case 5:
        		WeaponSlotShow5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetWeaponSlotShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponSlotShow1;
        	case 2:
        		return WeaponSlotShow2;
        	case 3:
        		return WeaponSlotShow3;
        	case 4:
        		return WeaponSlotShow4;
        	case 5:
        		return WeaponSlotShow5;
        	default:
        		return default(bool);
        	}
        }
        public bool SetWeaponSlotScale (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponSlotScale1 = val;
        		break;
        	case 2:
        		WeaponSlotScale2 = val;
        		break;
        	case 3:
        		WeaponSlotScale3 = val;
        		break;
        	case 4:
        		WeaponSlotScale4 = val;
        		break;
        	case 5:
        		WeaponSlotScale5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetWeaponSlotScale (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponSlotScale1;
        	case 2:
        		return WeaponSlotScale2;
        	case 3:
        		return WeaponSlotScale3;
        	case 4:
        		return WeaponSlotScale4;
        	case 5:
        		return WeaponSlotScale5;
        	default:
        		return default(Vector3);
        	}
        }
        public bool SetWeaponSlotAlpha (int index, float val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponSlotAlpha1 = val;
        		break;
        	case 2:
        		WeaponSlotAlpha2 = val;
        		break;
        	case 3:
        		WeaponSlotAlpha3 = val;
        		break;
        	case 4:
        		WeaponSlotAlpha4 = val;
        		break;
        	case 5:
        		WeaponSlotAlpha5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public float GetWeaponSlotAlpha (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponSlotAlpha1;
        	case 2:
        		return WeaponSlotAlpha2;
        	case 3:
        		return WeaponSlotAlpha3;
        	case 4:
        		return WeaponSlotAlpha4;
        	case 5:
        		return WeaponSlotAlpha5;
        	default:
        		return default(float);
        	}
        }
        public bool SetMarkText (int index, string val)
        {
        	switch(index)
        	{
        	case 1:
        		MarkText1 = val;
        		break;
        	case 2:
        		MarkText2 = val;
        		break;
        	case 3:
        		MarkText3 = val;
        		break;
        	case 4:
        		MarkText4 = val;
        		break;
        	case 5:
        		MarkText5 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public string GetMarkText (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return MarkText1;
        	case 2:
        		return MarkText2;
        	case 3:
        		return MarkText3;
        	case 4:
        		return MarkText4;
        	case 5:
        		return MarkText5;
        	default:
        		return default(string);
        	}
        }
        public bool SetWeaponIconScale (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconScale1 = val;
        		break;
        	case 2:
        		WeaponIconScale2 = val;
        		break;
        	case 3:
        		WeaponIconScale3 = val;
        		break;
        	case 4:
        		WeaponIconScale4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetWeaponIconScale (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconScale1;
        	case 2:
        		return WeaponIconScale2;
        	case 3:
        		return WeaponIconScale3;
        	case 4:
        		return WeaponIconScale4;
        	default:
        		return default(Vector3);
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
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetWeaponIconColor (int index, Color val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconColor1 = val;
        		break;
        	case 2:
        		WeaponIconColor2 = val;
        		break;
        	case 3:
        		WeaponIconColor3 = val;
        		break;
        	case 4:
        		WeaponIconColor4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Color GetWeaponIconColor (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconColor1;
        	case 2:
        		return WeaponIconColor2;
        	case 3:
        		return WeaponIconColor3;
        	case 4:
        		return WeaponIconColor4;
        	default:
        		return default(Color);
        	}
        }
        public bool SetWeaponIconAlpha (int index, float val)
        {
        	switch(index)
        	{
        	case 1:
        		WeaponIconAlpha1 = val;
        		break;
        	case 2:
        		WeaponIconAlpha2 = val;
        		break;
        	case 3:
        		WeaponIconAlpha3 = val;
        		break;
        	case 4:
        		WeaponIconAlpha4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public float GetWeaponIconAlpha (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return WeaponIconAlpha1;
        	case 2:
        		return WeaponIconAlpha2;
        	case 3:
        		return WeaponIconAlpha3;
        	case 4:
        		return WeaponIconAlpha4;
        	default:
        		return default(float);
        	}
        }
        public bool SetGrenadeIconGroupShow (int index, bool val)
        {
        	switch(index)
        	{
        	case 1:
        		GrenadeIconGroupShow1 = val;
        		break;
        	case 2:
        		GrenadeIconGroupShow2 = val;
        		break;
        	case 3:
        		GrenadeIconGroupShow3 = val;
        		break;
        	case 4:
        		GrenadeIconGroupShow4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public bool GetGrenadeIconGroupShow (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GrenadeIconGroupShow1;
        	case 2:
        		return GrenadeIconGroupShow2;
        	case 3:
        		return GrenadeIconGroupShow3;
        	case 4:
        		return GrenadeIconGroupShow4;
        	default:
        		return default(bool);
        	}
        }
        public bool SetGrenadeIconAlpha (int index, float val)
        {
        	switch(index)
        	{
        	case 1:
        		GrenadeIconAlpha1 = val;
        		break;
        	case 2:
        		GrenadeIconAlpha2 = val;
        		break;
        	case 3:
        		GrenadeIconAlpha3 = val;
        		break;
        	case 4:
        		GrenadeIconAlpha4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public float GetGrenadeIconAlpha (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GrenadeIconAlpha1;
        	case 2:
        		return GrenadeIconAlpha2;
        	case 3:
        		return GrenadeIconAlpha3;
        	case 4:
        		return GrenadeIconAlpha4;
        	default:
        		return default(float);
        	}
        }
        public bool SetGrenadeIconSprite (int index, Sprite val)
        {
        	switch(index)
        	{
        	case 1:
        		GrenadeIconSprite1 = val;
        		break;
        	case 2:
        		GrenadeIconSprite2 = val;
        		break;
        	case 3:
        		GrenadeIconSprite3 = val;
        		break;
        	case 4:
        		GrenadeIconSprite4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Sprite GetGrenadeIconSprite (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GrenadeIconSprite1;
        	case 2:
        		return GrenadeIconSprite2;
        	case 3:
        		return GrenadeIconSprite3;
        	case 4:
        		return GrenadeIconSprite4;
        	default:
        		return default(Sprite);
        	}
        }
        public bool SetGrenadeIconScale (int index, Vector3 val)
        {
        	switch(index)
        	{
        	case 1:
        		GrenadeIconScale1 = val;
        		break;
        	case 2:
        		GrenadeIconScale2 = val;
        		break;
        	case 3:
        		GrenadeIconScale3 = val;
        		break;
        	case 4:
        		GrenadeIconScale4 = val;
        		break;
        	default:
        		return false;
        	}
        	return true;
        }
        public Vector3 GetGrenadeIconScale (int index)
        {
        	switch(index)
        	{
        	case 1:
        		return GrenadeIconScale1;
        	case 2:
        		return GrenadeIconScale2;
        	case 3:
        		return GrenadeIconScale3;
        	case 4:
        		return GrenadeIconScale4;
        	default:
        		return default(Vector3);
        	}
        }
        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "CommonWeaponHud"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
