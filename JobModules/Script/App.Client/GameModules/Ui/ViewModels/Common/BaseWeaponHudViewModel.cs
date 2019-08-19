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
    public class BaseWeaponHudViewModel : ViewModelBase, IUiViewModel
    {
        private class BaseWeaponHudView : UIView
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
            public GameObject WeaponSlotShow2;
            public GameObject WeaponSlotShow3;
            public GameObject WeaponSlotShow4;
            public GameObject WeaponSlotShow5;
            public Text MarkText1;
            public Text MarkText2;
            public Text MarkText3;
            public Text MarkText4;
            public Text MarkText5;
            public RectTransform WeaponIconScale1;
            public Image WeaponIconSprite1;
            public Image WeaponIconColor1;
            public RectTransform WeaponIconScale2;
            public Image WeaponIconSprite2;
            public Image WeaponIconColor2;
            public RectTransform WeaponIconScale3;
            public Image WeaponIconSprite3;
            public Image WeaponIconColor3;
            public RectTransform WeaponIconScale4;
            public Image WeaponIconSprite4;
            public Image WeaponIconColor4;
            public GameObject GrenadeIconGroupShow1;
            public GameObject GrenadeIconGroupShow2;
            public GameObject GrenadeIconGroupShow3;
            public GameObject GrenadeIconGroupShow4;
            public Image GrenadeIconSprite1;
            public Image GrenadeIconSprite2;
            public Image GrenadeIconSprite3;
            public Image GrenadeIconSprite4;
            
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
        private bool _weaponSlotShow2;
        private bool _weaponSlotShow3;
        private bool _weaponSlotShow4;
        private bool _weaponSlotShow5;
        private string _markText1;
        private string _markText2;
        private string _markText3;
        private string _markText4;
        private string _markText5;
        private Vector3 _weaponIconScale1;
        private Sprite _weaponIconSprite1;
        private Color _weaponIconColor1;
        private Vector3 _weaponIconScale2;
        private Sprite _weaponIconSprite2;
        private Color _weaponIconColor2;
        private Vector3 _weaponIconScale3;
        private Sprite _weaponIconSprite3;
        private Color _weaponIconColor3;
        private Vector3 _weaponIconScale4;
        private Sprite _weaponIconSprite4;
        private Color _weaponIconColor4;
        private bool _grenadeIconGroupShow1;
        private bool _grenadeIconGroupShow2;
        private bool _grenadeIconGroupShow3;
        private bool _grenadeIconGroupShow4;
        private Sprite _grenadeIconSprite1;
        private Sprite _grenadeIconSprite2;
        private Sprite _grenadeIconSprite3;
        private Sprite _grenadeIconSprite4;
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
        public bool WeaponSlotShow2 { get { return _weaponSlotShow2; } set {if(_weaponSlotShow2 != value) Set(ref _weaponSlotShow2, value, "WeaponSlotShow2"); } }
        public bool WeaponSlotShow3 { get { return _weaponSlotShow3; } set {if(_weaponSlotShow3 != value) Set(ref _weaponSlotShow3, value, "WeaponSlotShow3"); } }
        public bool WeaponSlotShow4 { get { return _weaponSlotShow4; } set {if(_weaponSlotShow4 != value) Set(ref _weaponSlotShow4, value, "WeaponSlotShow4"); } }
        public bool WeaponSlotShow5 { get { return _weaponSlotShow5; } set {if(_weaponSlotShow5 != value) Set(ref _weaponSlotShow5, value, "WeaponSlotShow5"); } }
        public string MarkText1 { get { return _markText1; } set {if(_markText1 != value) Set(ref _markText1, value, "MarkText1"); } }
        public string MarkText2 { get { return _markText2; } set {if(_markText2 != value) Set(ref _markText2, value, "MarkText2"); } }
        public string MarkText3 { get { return _markText3; } set {if(_markText3 != value) Set(ref _markText3, value, "MarkText3"); } }
        public string MarkText4 { get { return _markText4; } set {if(_markText4 != value) Set(ref _markText4, value, "MarkText4"); } }
        public string MarkText5 { get { return _markText5; } set {if(_markText5 != value) Set(ref _markText5, value, "MarkText5"); } }
        public Vector3 WeaponIconScale1 { get { return _weaponIconScale1; } set {if(_weaponIconScale1 != value) Set(ref _weaponIconScale1, value, "WeaponIconScale1"); } }
        public Sprite WeaponIconSprite1 { get { return _weaponIconSprite1; } set {if(_weaponIconSprite1 != value) Set(ref _weaponIconSprite1, value, "WeaponIconSprite1"); if(null != _view && null != _view.WeaponIconSprite1 && null == value) _view.WeaponIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor1 { get { return _weaponIconColor1; } set {if(_weaponIconColor1 != value) Set(ref _weaponIconColor1, value, "WeaponIconColor1"); } }
        public Vector3 WeaponIconScale2 { get { return _weaponIconScale2; } set {if(_weaponIconScale2 != value) Set(ref _weaponIconScale2, value, "WeaponIconScale2"); } }
        public Sprite WeaponIconSprite2 { get { return _weaponIconSprite2; } set {if(_weaponIconSprite2 != value) Set(ref _weaponIconSprite2, value, "WeaponIconSprite2"); if(null != _view && null != _view.WeaponIconSprite2 && null == value) _view.WeaponIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor2 { get { return _weaponIconColor2; } set {if(_weaponIconColor2 != value) Set(ref _weaponIconColor2, value, "WeaponIconColor2"); } }
        public Vector3 WeaponIconScale3 { get { return _weaponIconScale3; } set {if(_weaponIconScale3 != value) Set(ref _weaponIconScale3, value, "WeaponIconScale3"); } }
        public Sprite WeaponIconSprite3 { get { return _weaponIconSprite3; } set {if(_weaponIconSprite3 != value) Set(ref _weaponIconSprite3, value, "WeaponIconSprite3"); if(null != _view && null != _view.WeaponIconSprite3 && null == value) _view.WeaponIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor3 { get { return _weaponIconColor3; } set {if(_weaponIconColor3 != value) Set(ref _weaponIconColor3, value, "WeaponIconColor3"); } }
        public Vector3 WeaponIconScale4 { get { return _weaponIconScale4; } set {if(_weaponIconScale4 != value) Set(ref _weaponIconScale4, value, "WeaponIconScale4"); } }
        public Sprite WeaponIconSprite4 { get { return _weaponIconSprite4; } set {if(_weaponIconSprite4 != value) Set(ref _weaponIconSprite4, value, "WeaponIconSprite4"); if(null != _view && null != _view.WeaponIconSprite4 && null == value) _view.WeaponIconSprite4.sprite = ViewModelUtil.EmptySprite; } }
        public Color WeaponIconColor4 { get { return _weaponIconColor4; } set {if(_weaponIconColor4 != value) Set(ref _weaponIconColor4, value, "WeaponIconColor4"); } }
        public bool GrenadeIconGroupShow1 { get { return _grenadeIconGroupShow1; } set {if(_grenadeIconGroupShow1 != value) Set(ref _grenadeIconGroupShow1, value, "GrenadeIconGroupShow1"); } }
        public bool GrenadeIconGroupShow2 { get { return _grenadeIconGroupShow2; } set {if(_grenadeIconGroupShow2 != value) Set(ref _grenadeIconGroupShow2, value, "GrenadeIconGroupShow2"); } }
        public bool GrenadeIconGroupShow3 { get { return _grenadeIconGroupShow3; } set {if(_grenadeIconGroupShow3 != value) Set(ref _grenadeIconGroupShow3, value, "GrenadeIconGroupShow3"); } }
        public bool GrenadeIconGroupShow4 { get { return _grenadeIconGroupShow4; } set {if(_grenadeIconGroupShow4 != value) Set(ref _grenadeIconGroupShow4, value, "GrenadeIconGroupShow4"); } }
        public Sprite GrenadeIconSprite1 { get { return _grenadeIconSprite1; } set {if(_grenadeIconSprite1 != value) Set(ref _grenadeIconSprite1, value, "GrenadeIconSprite1"); if(null != _view && null != _view.GrenadeIconSprite1 && null == value) _view.GrenadeIconSprite1.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite GrenadeIconSprite2 { get { return _grenadeIconSprite2; } set {if(_grenadeIconSprite2 != value) Set(ref _grenadeIconSprite2, value, "GrenadeIconSprite2"); if(null != _view && null != _view.GrenadeIconSprite2 && null == value) _view.GrenadeIconSprite2.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite GrenadeIconSprite3 { get { return _grenadeIconSprite3; } set {if(_grenadeIconSprite3 != value) Set(ref _grenadeIconSprite3, value, "GrenadeIconSprite3"); if(null != _view && null != _view.GrenadeIconSprite3 && null == value) _view.GrenadeIconSprite3.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite GrenadeIconSprite4 { get { return _grenadeIconSprite4; } set {if(_grenadeIconSprite4 != value) Set(ref _grenadeIconSprite4, value, "GrenadeIconSprite4"); if(null != _view && null != _view.GrenadeIconSprite4 && null == value) _view.GrenadeIconSprite4.sprite = ViewModelUtil.EmptySprite; } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private BaseWeaponHudView _view;
		
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
			var view = obj.GetComponent<BaseWeaponHudView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<BaseWeaponHudView>();
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
		private void EventTriggerBind(BaseWeaponHudView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static BaseWeaponHudViewModel()
        {
            Type type = typeof(BaseWeaponHudViewModel);
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

		void ViewBind(BaseWeaponHudView view)
		{
		     BindingSet<BaseWeaponHudView, BaseWeaponHudViewModel> bindingSet =
                view.CreateBindingSet<BaseWeaponHudView, BaseWeaponHudViewModel>();
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
            bindingSet.Bind(view.WeaponSlotShow2).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow2).OneWay();
            bindingSet.Bind(view.WeaponSlotShow3).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow3).OneWay();
            bindingSet.Bind(view.WeaponSlotShow4).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow4).OneWay();
            bindingSet.Bind(view.WeaponSlotShow5).For(v => v.activeSelf).To(vm => vm.WeaponSlotShow5).OneWay();
            bindingSet.Bind(view.MarkText1).For(v => v.text).To(vm => vm.MarkText1).OneWay();
            bindingSet.Bind(view.MarkText2).For(v => v.text).To(vm => vm.MarkText2).OneWay();
            bindingSet.Bind(view.MarkText3).For(v => v.text).To(vm => vm.MarkText3).OneWay();
            bindingSet.Bind(view.MarkText4).For(v => v.text).To(vm => vm.MarkText4).OneWay();
            bindingSet.Bind(view.MarkText5).For(v => v.text).To(vm => vm.MarkText5).OneWay();
            bindingSet.Bind(view.WeaponIconScale1).For(v => v.localScale).To(vm => vm.WeaponIconScale1).OneWay();
            bindingSet.Bind(view.WeaponIconSprite1).For(v => v.sprite).To(vm => vm.WeaponIconSprite1).OneWay();
            bindingSet.Bind(view.WeaponIconColor1).For(v => v.color).To(vm => vm.WeaponIconColor1).OneWay();
            bindingSet.Bind(view.WeaponIconScale2).For(v => v.localScale).To(vm => vm.WeaponIconScale2).OneWay();
            bindingSet.Bind(view.WeaponIconSprite2).For(v => v.sprite).To(vm => vm.WeaponIconSprite2).OneWay();
            bindingSet.Bind(view.WeaponIconColor2).For(v => v.color).To(vm => vm.WeaponIconColor2).OneWay();
            bindingSet.Bind(view.WeaponIconScale3).For(v => v.localScale).To(vm => vm.WeaponIconScale3).OneWay();
            bindingSet.Bind(view.WeaponIconSprite3).For(v => v.sprite).To(vm => vm.WeaponIconSprite3).OneWay();
            bindingSet.Bind(view.WeaponIconColor3).For(v => v.color).To(vm => vm.WeaponIconColor3).OneWay();
            bindingSet.Bind(view.WeaponIconScale4).For(v => v.localScale).To(vm => vm.WeaponIconScale4).OneWay();
            bindingSet.Bind(view.WeaponIconSprite4).For(v => v.sprite).To(vm => vm.WeaponIconSprite4).OneWay();
            bindingSet.Bind(view.WeaponIconColor4).For(v => v.color).To(vm => vm.WeaponIconColor4).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow1).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow1).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow2).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow2).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow3).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow3).OneWay();
            bindingSet.Bind(view.GrenadeIconGroupShow4).For(v => v.activeSelf).To(vm => vm.GrenadeIconGroupShow4).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite1).For(v => v.sprite).To(vm => vm.GrenadeIconSprite1).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite2).For(v => v.sprite).To(vm => vm.GrenadeIconSprite2).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite3).For(v => v.sprite).To(vm => vm.GrenadeIconSprite3).OneWay();
            bindingSet.Bind(view.GrenadeIconSprite4).For(v => v.sprite).To(vm => vm.GrenadeIconSprite4).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(BaseWeaponHudView view)
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


		void SaveOriData(BaseWeaponHudView view)
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
        public const string MarkText = "MarkText";
        public const int MarkTextCount = 5;
        public const string WeaponIconScale = "WeaponIconScale";
        public const int WeaponIconScaleCount = 4;
        public const string WeaponIconSprite = "WeaponIconSprite";
        public const int WeaponIconSpriteCount = 4;
        public const string WeaponIconColor = "WeaponIconColor";
        public const int WeaponIconColorCount = 4;
        public const string GrenadeIconGroupShow = "GrenadeIconGroupShow";
        public const int GrenadeIconGroupShowCount = 4;
        public const string GrenadeIconSprite = "GrenadeIconSprite";
        public const int GrenadeIconSpriteCount = 4;
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
        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "BaseWeaponHud"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
