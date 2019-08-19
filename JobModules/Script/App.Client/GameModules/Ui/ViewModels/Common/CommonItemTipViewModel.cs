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
    public class CommonItemTipViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonItemTipView : UIView
        {
            public Image ItemTipQuality;
            public Image SymbolSprite;
            public Image SymbolBorderSprite;
            public GameObject SymbolBorderActive;
            [HideInInspector]
            public bool oriSymbolBorderActive;
            public Text NameText;
            [HideInInspector]
            public string oriNameText;
            public GameObject LevelGroupActive;
            [HideInInspector]
            public bool oriLevelGroupActive;
            public Text LevelText;
            [HideInInspector]
            public string oriLevelText;
            public Text MaxLevelText;
            [HideInInspector]
            public string oriMaxLevelText;
            public GameObject MaxLevelGroupActive;
            [HideInInspector]
            public bool oriMaxLevelGroupActive;
            public GameObject ExpGroupActive;
            [HideInInspector]
            public bool oriExpGroupActive;
            public Image ExpBar;
            [HideInInspector]
            public float oriExpBar;
            public GameObject PartsDescActive;
            [HideInInspector]
            public bool oriPartsDescActive;
            public Text EquipNum;
            [HideInInspector]
            public string oriEquipNum;
            public Text AllNum;
            [HideInInspector]
            public string oriAllNum;
            public GameObject PartsGroupActive;
            [HideInInspector]
            public bool oriPartsGroupActive;
            public GameObject AttrGroupActive;
            [HideInInspector]
            public bool oriAttrGroupActive;
            public GameObject SuperGroupActive;
            [HideInInspector]
            public bool oriSuperGroupActive;
            public Image SuperIcon;
            public Text SuperName;
            [HideInInspector]
            public string oriSuperName;
            public Text SuperDesc;
            [HideInInspector]
            public string oriSuperDesc;
            public Text Desc;
            [HideInInspector]
            public string oriDesc;
            
            public void FillField()
            {
                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "BattleItemTip":
                            ItemTipQuality = v;
                            break;
                        case "Symbol":
                            SymbolSprite = v;
                            break;
                        case "SymbolBorder":
                            SymbolBorderSprite = v;
                            break;
                        case "ExpBar":
                            ExpBar = v;
                            break;
                        case "SuperIcon":
                            SuperIcon = v;
                            break;
                    }
                }

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "SymbolBorder":
                            SymbolBorderActive = v.gameObject;
                            break;
                        case "LevelGroup":
                            LevelGroupActive = v.gameObject;
                            break;
                        case "MaxLevelGroup":
                            MaxLevelGroupActive = v.gameObject;
                            break;
                        case "ExpGroup":
                            ExpGroupActive = v.gameObject;
                            break;
                        case "PartsDesc":
                            PartsDescActive = v.gameObject;
                            break;
                        case "PartsGroup":
                            PartsGroupActive = v.gameObject;
                            break;
                        case "AttrGroup":
                            AttrGroupActive = v.gameObject;
                            break;
                        case "SuperGroup":
                            SuperGroupActive = v.gameObject;
                            break;
                    }
                }

                Text[] texts = gameObject.GetComponentsInChildren<Text>(true);
                foreach (var v in texts)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "Name":
                            NameText = v;
                            break;
                        case "Level":
                            LevelText = v;
                            break;
                        case "MaxLevel":
                            MaxLevelText = v;
                            break;
                        case "EquipNum":
                            EquipNum = v;
                            break;
                        case "AllNum":
                            AllNum = v;
                            break;
                        case "SuperName":
                            SuperName = v;
                            break;
                        case "SuperDesc":
                            SuperDesc = v;
                            break;
                        case "Desc":
                            Desc = v;
                            break;
                    }
                }

            }
        }


        private Sprite _itemTipQuality;
        private Sprite _symbolSprite;
        private Sprite _symbolBorderSprite;
        private bool _symbolBorderActive;
        private string _nameText;
        private bool _levelGroupActive;
        private string _levelText;
        private string _maxLevelText;
        private bool _maxLevelGroupActive;
        private bool _expGroupActive;
        private float _expBar;
        private bool _partsDescActive;
        private string _equipNum;
        private string _allNum;
        private bool _partsGroupActive;
        private bool _attrGroupActive;
        private bool _superGroupActive;
        private Sprite _superIcon;
        private string _superName;
        private string _superDesc;
        private string _desc;
        public Sprite ItemTipQuality { get { return _itemTipQuality; } set {if(_itemTipQuality != value) Set(ref _itemTipQuality, value, "ItemTipQuality"); if(null != _view && null != _view.ItemTipQuality && null == value) _view.ItemTipQuality.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite SymbolSprite { get { return _symbolSprite; } set {if(_symbolSprite != value) Set(ref _symbolSprite, value, "SymbolSprite"); if(null != _view && null != _view.SymbolSprite && null == value) _view.SymbolSprite.sprite = ViewModelUtil.EmptySprite; } }
        public Sprite SymbolBorderSprite { get { return _symbolBorderSprite; } set {if(_symbolBorderSprite != value) Set(ref _symbolBorderSprite, value, "SymbolBorderSprite"); if(null != _view && null != _view.SymbolBorderSprite && null == value) _view.SymbolBorderSprite.sprite = ViewModelUtil.EmptySprite; } }
        public bool SymbolBorderActive { get { return _symbolBorderActive; } set {if(_symbolBorderActive != value) Set(ref _symbolBorderActive, value, "SymbolBorderActive"); } }
        public string NameText { get { return _nameText; } set {if(_nameText != value) Set(ref _nameText, value, "NameText"); } }
        public bool LevelGroupActive { get { return _levelGroupActive; } set {if(_levelGroupActive != value) Set(ref _levelGroupActive, value, "LevelGroupActive"); } }
        public string LevelText { get { return _levelText; } set {if(_levelText != value) Set(ref _levelText, value, "LevelText"); } }
        public string MaxLevelText { get { return _maxLevelText; } set {if(_maxLevelText != value) Set(ref _maxLevelText, value, "MaxLevelText"); } }
        public bool MaxLevelGroupActive { get { return _maxLevelGroupActive; } set {if(_maxLevelGroupActive != value) Set(ref _maxLevelGroupActive, value, "MaxLevelGroupActive"); } }
        public bool ExpGroupActive { get { return _expGroupActive; } set {if(_expGroupActive != value) Set(ref _expGroupActive, value, "ExpGroupActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float ExpBar { get { return _expBar; } set {if(_expBar != value) Set(ref _expBar, value, "ExpBar"); } }
        public bool PartsDescActive { get { return _partsDescActive; } set {if(_partsDescActive != value) Set(ref _partsDescActive, value, "PartsDescActive"); } }
        public string EquipNum { get { return _equipNum; } set {if(_equipNum != value) Set(ref _equipNum, value, "EquipNum"); } }
        public string AllNum { get { return _allNum; } set {if(_allNum != value) Set(ref _allNum, value, "AllNum"); } }
        public bool PartsGroupActive { get { return _partsGroupActive; } set {if(_partsGroupActive != value) Set(ref _partsGroupActive, value, "PartsGroupActive"); } }
        public bool AttrGroupActive { get { return _attrGroupActive; } set {if(_attrGroupActive != value) Set(ref _attrGroupActive, value, "AttrGroupActive"); } }
        public bool SuperGroupActive { get { return _superGroupActive; } set {if(_superGroupActive != value) Set(ref _superGroupActive, value, "SuperGroupActive"); } }
        public Sprite SuperIcon { get { return _superIcon; } set {if(_superIcon != value) Set(ref _superIcon, value, "SuperIcon"); if(null != _view && null != _view.SuperIcon && null == value) _view.SuperIcon.sprite = ViewModelUtil.EmptySprite; } }
        public string SuperName { get { return _superName; } set {if(_superName != value) Set(ref _superName, value, "SuperName"); } }
        public string SuperDesc { get { return _superDesc; } set {if(_superDesc != value) Set(ref _superDesc, value, "SuperDesc"); } }
        public string Desc { get { return _desc; } set {if(_desc != value) Set(ref _desc, value, "Desc"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonItemTipView _view;
		
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
			var view = obj.GetComponent<CommonItemTipView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonItemTipView>();
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
		private void EventTriggerBind(CommonItemTipView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonItemTipViewModel()
        {
            Type type = typeof(CommonItemTipViewModel);
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

		void ViewBind(CommonItemTipView view)
		{
		     BindingSet<CommonItemTipView, CommonItemTipViewModel> bindingSet =
                view.CreateBindingSet<CommonItemTipView, CommonItemTipViewModel>();
            bindingSet.Bind(view.ItemTipQuality).For(v => v.sprite).To(vm => vm.ItemTipQuality).OneWay();
            bindingSet.Bind(view.SymbolSprite).For(v => v.sprite).To(vm => vm.SymbolSprite).OneWay();
            bindingSet.Bind(view.SymbolBorderSprite).For(v => v.sprite).To(vm => vm.SymbolBorderSprite).OneWay();
            bindingSet.Bind(view.SymbolBorderActive).For(v => v.activeSelf).To(vm => vm.SymbolBorderActive).OneWay();
            bindingSet.Bind(view.NameText).For(v => v.text).To(vm => vm.NameText).OneWay();
            bindingSet.Bind(view.LevelGroupActive).For(v => v.activeSelf).To(vm => vm.LevelGroupActive).OneWay();
            bindingSet.Bind(view.LevelText).For(v => v.text).To(vm => vm.LevelText).OneWay();
            bindingSet.Bind(view.MaxLevelText).For(v => v.text).To(vm => vm.MaxLevelText).OneWay();
            bindingSet.Bind(view.MaxLevelGroupActive).For(v => v.activeSelf).To(vm => vm.MaxLevelGroupActive).OneWay();
            bindingSet.Bind(view.ExpGroupActive).For(v => v.activeSelf).To(vm => vm.ExpGroupActive).OneWay();
            bindingSet.Bind(view.ExpBar).For(v => v.fillAmount).To(vm => vm.ExpBar).OneWay();
            bindingSet.Bind(view.PartsDescActive).For(v => v.activeSelf).To(vm => vm.PartsDescActive).OneWay();
            bindingSet.Bind(view.EquipNum).For(v => v.text).To(vm => vm.EquipNum).OneWay();
            bindingSet.Bind(view.AllNum).For(v => v.text).To(vm => vm.AllNum).OneWay();
            bindingSet.Bind(view.PartsGroupActive).For(v => v.activeSelf).To(vm => vm.PartsGroupActive).OneWay();
            bindingSet.Bind(view.AttrGroupActive).For(v => v.activeSelf).To(vm => vm.AttrGroupActive).OneWay();
            bindingSet.Bind(view.SuperGroupActive).For(v => v.activeSelf).To(vm => vm.SuperGroupActive).OneWay();
            bindingSet.Bind(view.SuperIcon).For(v => v.sprite).To(vm => vm.SuperIcon).OneWay();
            bindingSet.Bind(view.SuperName).For(v => v.text).To(vm => vm.SuperName).OneWay();
            bindingSet.Bind(view.SuperDesc).For(v => v.text).To(vm => vm.SuperDesc).OneWay();
            bindingSet.Bind(view.Desc).For(v => v.text).To(vm => vm.Desc).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonItemTipView view)
		{
            _symbolBorderActive = view.SymbolBorderActive.activeSelf;
            _nameText = view.NameText.text;
            _levelGroupActive = view.LevelGroupActive.activeSelf;
            _levelText = view.LevelText.text;
            _maxLevelText = view.MaxLevelText.text;
            _maxLevelGroupActive = view.MaxLevelGroupActive.activeSelf;
            _expGroupActive = view.ExpGroupActive.activeSelf;
            _expBar = view.ExpBar.fillAmount;
            _partsDescActive = view.PartsDescActive.activeSelf;
            _equipNum = view.EquipNum.text;
            _allNum = view.AllNum.text;
            _partsGroupActive = view.PartsGroupActive.activeSelf;
            _attrGroupActive = view.AttrGroupActive.activeSelf;
            _superGroupActive = view.SuperGroupActive.activeSelf;
            _superName = view.SuperName.text;
            _superDesc = view.SuperDesc.text;
            _desc = view.Desc.text;
		}


		void SaveOriData(CommonItemTipView view)
		{
            view.oriSymbolBorderActive = _symbolBorderActive;
            view.oriNameText = _nameText;
            view.oriLevelGroupActive = _levelGroupActive;
            view.oriLevelText = _levelText;
            view.oriMaxLevelText = _maxLevelText;
            view.oriMaxLevelGroupActive = _maxLevelGroupActive;
            view.oriExpGroupActive = _expGroupActive;
            view.oriExpBar = _expBar;
            view.oriPartsDescActive = _partsDescActive;
            view.oriEquipNum = _equipNum;
            view.oriAllNum = _allNum;
            view.oriPartsGroupActive = _partsGroupActive;
            view.oriAttrGroupActive = _attrGroupActive;
            view.oriSuperGroupActive = _superGroupActive;
            view.oriSuperName = _superName;
            view.oriSuperDesc = _superDesc;
            view.oriDesc = _desc;
		}




		private void SpriteReset()
		{
			ItemTipQuality = ViewModelUtil.EmptySprite;
			SymbolSprite = ViewModelUtil.EmptySprite;
			SymbolBorderSprite = ViewModelUtil.EmptySprite;
			SuperIcon = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			SymbolBorderActive = _view.oriSymbolBorderActive;
			NameText = _view.oriNameText;
			LevelGroupActive = _view.oriLevelGroupActive;
			LevelText = _view.oriLevelText;
			MaxLevelText = _view.oriMaxLevelText;
			MaxLevelGroupActive = _view.oriMaxLevelGroupActive;
			ExpGroupActive = _view.oriExpGroupActive;
			ExpBar = _view.oriExpBar;
			PartsDescActive = _view.oriPartsDescActive;
			EquipNum = _view.oriEquipNum;
			AllNum = _view.oriAllNum;
			PartsGroupActive = _view.oriPartsGroupActive;
			AttrGroupActive = _view.oriAttrGroupActive;
			SuperGroupActive = _view.oriSuperGroupActive;
			SuperName = _view.oriSuperName;
			SuperDesc = _view.oriSuperDesc;
			Desc = _view.oriDesc;
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

        public string ResourceBundleName { get { return "ui/client/prefab/common"; } }
        public string ResourceAssetName { get { return "BattleItemTip"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
