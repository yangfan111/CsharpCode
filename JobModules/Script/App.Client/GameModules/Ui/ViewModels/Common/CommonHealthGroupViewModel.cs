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
    public class CommonHealthGroupViewModel : ViewModelBase, IUiViewModel
    {
        private class CommonHealthGroupView : UIView
        {
            public RectTransform rootLocation;
            [HideInInspector]
            public Vector2 orirootLocation;
            public RectTransform rootSizeDelta;
            [HideInInspector]
            public Vector2 orirootSizeDelta;
            public GameObject HpGroupGameObjectActiveSelf;
            [HideInInspector]
            public bool oriHpGroupGameObjectActiveSelf;
            public Slider currentHpValue;
            [HideInInspector]
            public float oricurrentHpValue;
            public Image currentHpFillColor;
            [HideInInspector]
            public Color oricurrentHpFillColor;
            public Slider specialHpBgValue;
            [HideInInspector]
            public float orispecialHpBgValue;
            public GameObject HpGroupInHurtGameObjectActiveSelf;
            [HideInInspector]
            public bool oriHpGroupInHurtGameObjectActiveSelf;
            public Image HpGroupHurtValue;
            [HideInInspector]
            public float oriHpGroupHurtValue;
            public GameObject ShowPoseGroupGameObjectActiveSelf;
            [HideInInspector]
            public bool oriShowPoseGroupGameObjectActiveSelf;
            public Image currentPoseImg;
            public GameObject o2BufActive;
            [HideInInspector]
            public bool orio2BufActive;
            public Image curO2FillAmount;
            [HideInInspector]
            public float oricurO2FillAmount;
            public GameObject speedBufActive;
            [HideInInspector]
            public bool orispeedBufActive;
            public GameObject retreatBufActive;
            [HideInInspector]
            public bool oriretreatBufActive;
            public GameObject PowerGroupActive;
            [HideInInspector]
            public bool oriPowerGroupActive;
            public Slider duan1;
            [HideInInspector]
            public float oriduan1;
            public Slider duan2;
            [HideInInspector]
            public float oriduan2;
            public Slider duan3;
            [HideInInspector]
            public float oriduan3;
            public Slider duan4;
            [HideInInspector]
            public float oriduan4;
            public GameObject HelmetActive;
            [HideInInspector]
            public bool oriHelmetActive;
            public GameObject BulletproofActive;
            [HideInInspector]
            public bool oriBulletproofActive;
            public Image BulletproofFillAmount;
            [HideInInspector]
            public float oriBulletproofFillAmount;
            public Image HelmetFillAmount;
            [HideInInspector]
            public float oriHelmetFillAmount;
            
            public void FillField()
            {
                RectTransform[] recttransforms = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in recttransforms)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "root":
                            rootLocation = v;
                            rootSizeDelta = v;
                            break;
                    }
                }

                RectTransform[] gameobjects = gameObject.GetComponentsInChildren<RectTransform>(true);
                foreach (var v in gameobjects)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "HpGroup":
                            HpGroupGameObjectActiveSelf = v.gameObject;
                            break;
                        case "HpGroupInHurt":
                            HpGroupInHurtGameObjectActiveSelf = v.gameObject;
                            break;
                        case "ShowPoseGroup":
                            ShowPoseGroupGameObjectActiveSelf = v.gameObject;
                            break;
                        case "o2BuffBg":
                            o2BufActive = v.gameObject;
                            break;
                        case "speedBuff":
                            speedBufActive = v.gameObject;
                            break;
                        case "retreatBuff":
                            retreatBufActive = v.gameObject;
                            break;
                        case "ShowPowerGroup":
                            PowerGroupActive = v.gameObject;
                            break;
                        case "HelmetBg":
                            HelmetActive = v.gameObject;
                            break;
                        case "BulletproofBg":
                            BulletproofActive = v.gameObject;
                            break;
                    }
                }

                Slider[] sliders = gameObject.GetComponentsInChildren<Slider>(true);
                foreach (var v in sliders)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "currentHp":
                            currentHpValue = v;
                            break;
                        case "specialHpBg":
                            specialHpBgValue = v;
                            break;
                        case "duan1":
                            duan1 = v;
                            break;
                        case "duan2":
                            duan2 = v;
                            break;
                        case "duan3":
                            duan3 = v;
                            break;
                        case "duan4":
                            duan4 = v;
                            break;
                    }
                }

                Image[] images = gameObject.GetComponentsInChildren<Image>(true);
                foreach (var v in images)
                {
                    var realName = v.gameObject.name.Replace("(Clone)","");
                    switch (realName)
                    {
                        case "currentHpFill":
                            currentHpFillColor = v;
                            break;
                        case "HpGroupInHurtFill":
                            HpGroupHurtValue = v;
                            break;
                        case "currentPose":
                            currentPoseImg = v;
                            break;
                        case "curO2":
                            curO2FillAmount = v;
                            break;
                        case "Bulletproof":
                            BulletproofFillAmount = v;
                            break;
                        case "Helmet":
                            HelmetFillAmount = v;
                            break;
                    }
                }

            }
        }


        private Vector2 _rootLocation;
        private Vector2 _rootSizeDelta;
        private bool _hpGroupGameObjectActiveSelf;
        private float _currentHpValue;
        private Color _currentHpFillColor;
        private float _specialHpBgValue;
        private bool _hpGroupInHurtGameObjectActiveSelf;
        private float _hpGroupHurtValue;
        private bool _showPoseGroupGameObjectActiveSelf;
        private Sprite _currentPoseImg;
        private bool _o2BufActive;
        private float _curO2FillAmount;
        private bool _speedBufActive;
        private bool _retreatBufActive;
        private bool _powerGroupActive;
        private float _duan1;
        private float _duan2;
        private float _duan3;
        private float _duan4;
        private bool _helmetActive;
        private bool _bulletproofActive;
        private float _bulletproofFillAmount;
        private float _helmetFillAmount;
        public Vector2 rootLocation { get { return _rootLocation; } set {if(_rootLocation != value) Set(ref _rootLocation, value, "rootLocation"); } }
        public Vector2 rootSizeDelta { get { return _rootSizeDelta; } set {if(_rootSizeDelta != value) Set(ref _rootSizeDelta, value, "rootSizeDelta"); } }
        public bool HpGroupGameObjectActiveSelf { get { return _hpGroupGameObjectActiveSelf; } set {if(_hpGroupGameObjectActiveSelf != value) Set(ref _hpGroupGameObjectActiveSelf, value, "HpGroupGameObjectActiveSelf"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float currentHpValue { get { return _currentHpValue; } set {if(_currentHpValue != value) Set(ref _currentHpValue, value, "currentHpValue"); } }
        public Color currentHpFillColor { get { return _currentHpFillColor; } set {if(_currentHpFillColor != value) Set(ref _currentHpFillColor, value, "currentHpFillColor"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float specialHpBgValue { get { return _specialHpBgValue; } set {if(_specialHpBgValue != value) Set(ref _specialHpBgValue, value, "specialHpBgValue"); } }
        public bool HpGroupInHurtGameObjectActiveSelf { get { return _hpGroupInHurtGameObjectActiveSelf; } set {if(_hpGroupInHurtGameObjectActiveSelf != value) Set(ref _hpGroupInHurtGameObjectActiveSelf, value, "HpGroupInHurtGameObjectActiveSelf"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float HpGroupHurtValue { get { return _hpGroupHurtValue; } set {if(_hpGroupHurtValue != value) Set(ref _hpGroupHurtValue, value, "HpGroupHurtValue"); } }
        public bool ShowPoseGroupGameObjectActiveSelf { get { return _showPoseGroupGameObjectActiveSelf; } set {if(_showPoseGroupGameObjectActiveSelf != value) Set(ref _showPoseGroupGameObjectActiveSelf, value, "ShowPoseGroupGameObjectActiveSelf"); } }
        public Sprite currentPoseImg { get { return _currentPoseImg; } set {if(_currentPoseImg != value) Set(ref _currentPoseImg, value, "currentPoseImg"); if(null != _view && null != _view.currentPoseImg && null == value) _view.currentPoseImg.sprite = ViewModelUtil.EmptySprite; } }
        public bool o2BufActive { get { return _o2BufActive; } set {if(_o2BufActive != value) Set(ref _o2BufActive, value, "o2BufActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float curO2FillAmount { get { return _curO2FillAmount; } set {if(_curO2FillAmount != value) Set(ref _curO2FillAmount, value, "curO2FillAmount"); } }
        public bool speedBufActive { get { return _speedBufActive; } set {if(_speedBufActive != value) Set(ref _speedBufActive, value, "speedBufActive"); } }
        public bool retreatBufActive { get { return _retreatBufActive; } set {if(_retreatBufActive != value) Set(ref _retreatBufActive, value, "retreatBufActive"); } }
        public bool PowerGroupActive { get { return _powerGroupActive; } set {if(_powerGroupActive != value) Set(ref _powerGroupActive, value, "PowerGroupActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float duan1 { get { return _duan1; } set {if(_duan1 != value) Set(ref _duan1, value, "duan1"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float duan2 { get { return _duan2; } set {if(_duan2 != value) Set(ref _duan2, value, "duan2"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float duan3 { get { return _duan3; } set {if(_duan3 != value) Set(ref _duan3, value, "duan3"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float duan4 { get { return _duan4; } set {if(_duan4 != value) Set(ref _duan4, value, "duan4"); } }
        public bool HelmetActive { get { return _helmetActive; } set {if(_helmetActive != value) Set(ref _helmetActive, value, "HelmetActive"); } }
        public bool BulletproofActive { get { return _bulletproofActive; } set {if(_bulletproofActive != value) Set(ref _bulletproofActive, value, "BulletproofActive"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float BulletproofFillAmount { get { return _bulletproofFillAmount; } set {if(_bulletproofFillAmount != value) Set(ref _bulletproofFillAmount, value, "BulletproofFillAmount"); } }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public float HelmetFillAmount { get { return _helmetFillAmount; } set {if(_helmetFillAmount != value) Set(ref _helmetFillAmount, value, "HelmetFillAmount"); } }

		private GameObject _viewGameObject;
		private Canvas _viewCanvas;
		private CommonHealthGroupView _view;
		
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
			var view = obj.GetComponent<CommonHealthGroupView>();
			if(view == null)
			{
				bFirst = true;
				view = obj.AddComponent<CommonHealthGroupView>();
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
		private void EventTriggerBind(CommonHealthGroupView view)
		{
		}

        private static readonly Dictionary<string, PropertyInfo> PropertySetter = new Dictionary<string, PropertyInfo>();
        private static readonly Dictionary<string, MethodInfo> MethodSetter = new Dictionary<string, MethodInfo>();

        static CommonHealthGroupViewModel()
        {
            Type type = typeof(CommonHealthGroupViewModel);
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

		void ViewBind(CommonHealthGroupView view)
		{
		     BindingSet<CommonHealthGroupView, CommonHealthGroupViewModel> bindingSet =
                view.CreateBindingSet<CommonHealthGroupView, CommonHealthGroupViewModel>();
            bindingSet.Bind(view.rootLocation).For(v => v.anchoredPosition).To(vm => vm.rootLocation).OneWay();
            bindingSet.Bind(view.rootSizeDelta).For(v => v.sizeDelta).To(vm => vm.rootSizeDelta).OneWay();
            bindingSet.Bind(view.HpGroupGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.HpGroupGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.currentHpValue).For(v => v.value).To(vm => vm.currentHpValue).OneWay();
            bindingSet.Bind(view.currentHpFillColor).For(v => v.color).To(vm => vm.currentHpFillColor).OneWay();
            bindingSet.Bind(view.specialHpBgValue).For(v => v.value).To(vm => vm.specialHpBgValue).OneWay();
            bindingSet.Bind(view.HpGroupInHurtGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.HpGroupInHurtGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.HpGroupHurtValue).For(v => v.fillAmount).To(vm => vm.HpGroupHurtValue).OneWay();
            bindingSet.Bind(view.ShowPoseGroupGameObjectActiveSelf).For(v => v.activeSelf).To(vm => vm.ShowPoseGroupGameObjectActiveSelf).OneWay();
            bindingSet.Bind(view.currentPoseImg).For(v => v.sprite).To(vm => vm.currentPoseImg).OneWay();
            bindingSet.Bind(view.o2BufActive).For(v => v.activeSelf).To(vm => vm.o2BufActive).OneWay();
            bindingSet.Bind(view.curO2FillAmount).For(v => v.fillAmount).To(vm => vm.curO2FillAmount).OneWay();
            bindingSet.Bind(view.speedBufActive).For(v => v.activeSelf).To(vm => vm.speedBufActive).OneWay();
            bindingSet.Bind(view.retreatBufActive).For(v => v.activeSelf).To(vm => vm.retreatBufActive).OneWay();
            bindingSet.Bind(view.PowerGroupActive).For(v => v.activeSelf).To(vm => vm.PowerGroupActive).OneWay();
            bindingSet.Bind(view.duan1).For(v => v.value).To(vm => vm.duan1).OneWay();
            bindingSet.Bind(view.duan2).For(v => v.value).To(vm => vm.duan2).OneWay();
            bindingSet.Bind(view.duan3).For(v => v.value).To(vm => vm.duan3).OneWay();
            bindingSet.Bind(view.duan4).For(v => v.value).To(vm => vm.duan4).OneWay();
            bindingSet.Bind(view.HelmetActive).For(v => v.activeSelf).To(vm => vm.HelmetActive).OneWay();
            bindingSet.Bind(view.BulletproofActive).For(v => v.activeSelf).To(vm => vm.BulletproofActive).OneWay();
            bindingSet.Bind(view.BulletproofFillAmount).For(v => v.fillAmount).To(vm => vm.BulletproofFillAmount).OneWay();
            bindingSet.Bind(view.HelmetFillAmount).For(v => v.fillAmount).To(vm => vm.HelmetFillAmount).OneWay();
		
			bindingSet.Build();
		}

		void DataInit(CommonHealthGroupView view)
		{
            _rootLocation = view.rootLocation.anchoredPosition;
            _rootSizeDelta = view.rootSizeDelta.sizeDelta;
            _hpGroupGameObjectActiveSelf = view.HpGroupGameObjectActiveSelf.activeSelf;
            _currentHpValue = view.currentHpValue.value;
            _currentHpFillColor = view.currentHpFillColor.color;
            _specialHpBgValue = view.specialHpBgValue.value;
            _hpGroupInHurtGameObjectActiveSelf = view.HpGroupInHurtGameObjectActiveSelf.activeSelf;
            _hpGroupHurtValue = view.HpGroupHurtValue.fillAmount;
            _showPoseGroupGameObjectActiveSelf = view.ShowPoseGroupGameObjectActiveSelf.activeSelf;
            _o2BufActive = view.o2BufActive.activeSelf;
            _curO2FillAmount = view.curO2FillAmount.fillAmount;
            _speedBufActive = view.speedBufActive.activeSelf;
            _retreatBufActive = view.retreatBufActive.activeSelf;
            _powerGroupActive = view.PowerGroupActive.activeSelf;
            _duan1 = view.duan1.value;
            _duan2 = view.duan2.value;
            _duan3 = view.duan3.value;
            _duan4 = view.duan4.value;
            _helmetActive = view.HelmetActive.activeSelf;
            _bulletproofActive = view.BulletproofActive.activeSelf;
            _bulletproofFillAmount = view.BulletproofFillAmount.fillAmount;
            _helmetFillAmount = view.HelmetFillAmount.fillAmount;
		}


		void SaveOriData(CommonHealthGroupView view)
		{
            view.orirootLocation = _rootLocation;
            view.orirootSizeDelta = _rootSizeDelta;
            view.oriHpGroupGameObjectActiveSelf = _hpGroupGameObjectActiveSelf;
            view.oricurrentHpValue = _currentHpValue;
            view.oricurrentHpFillColor = _currentHpFillColor;
            view.orispecialHpBgValue = _specialHpBgValue;
            view.oriHpGroupInHurtGameObjectActiveSelf = _hpGroupInHurtGameObjectActiveSelf;
            view.oriHpGroupHurtValue = _hpGroupHurtValue;
            view.oriShowPoseGroupGameObjectActiveSelf = _showPoseGroupGameObjectActiveSelf;
            view.orio2BufActive = _o2BufActive;
            view.oricurO2FillAmount = _curO2FillAmount;
            view.orispeedBufActive = _speedBufActive;
            view.oriretreatBufActive = _retreatBufActive;
            view.oriPowerGroupActive = _powerGroupActive;
            view.oriduan1 = _duan1;
            view.oriduan2 = _duan2;
            view.oriduan3 = _duan3;
            view.oriduan4 = _duan4;
            view.oriHelmetActive = _helmetActive;
            view.oriBulletproofActive = _bulletproofActive;
            view.oriBulletproofFillAmount = _bulletproofFillAmount;
            view.oriHelmetFillAmount = _helmetFillAmount;
		}




		private void SpriteReset()
		{
			currentPoseImg = ViewModelUtil.EmptySprite;
		}

		public void Reset()
		{
			if(_viewGameObject == null)
			{
				return;
			}
			rootLocation = _view.orirootLocation;
			rootSizeDelta = _view.orirootSizeDelta;
			HpGroupGameObjectActiveSelf = _view.oriHpGroupGameObjectActiveSelf;
			currentHpValue = _view.oricurrentHpValue;
			currentHpFillColor = _view.oricurrentHpFillColor;
			specialHpBgValue = _view.orispecialHpBgValue;
			HpGroupInHurtGameObjectActiveSelf = _view.oriHpGroupInHurtGameObjectActiveSelf;
			HpGroupHurtValue = _view.oriHpGroupHurtValue;
			ShowPoseGroupGameObjectActiveSelf = _view.oriShowPoseGroupGameObjectActiveSelf;
			o2BufActive = _view.orio2BufActive;
			curO2FillAmount = _view.oricurO2FillAmount;
			speedBufActive = _view.orispeedBufActive;
			retreatBufActive = _view.oriretreatBufActive;
			PowerGroupActive = _view.oriPowerGroupActive;
			duan1 = _view.oriduan1;
			duan2 = _view.oriduan2;
			duan3 = _view.oriduan3;
			duan4 = _view.oriduan4;
			HelmetActive = _view.oriHelmetActive;
			BulletproofActive = _view.oriBulletproofActive;
			BulletproofFillAmount = _view.oriBulletproofFillAmount;
			HelmetFillAmount = _view.oriHelmetFillAmount;
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
        public string ResourceAssetName { get { return "CommonHealthGroup"; } }
        public string ConfigBundleName { get { return ""; } }
        public string ConfigAssetName { get { return ""; } }
    }
}
