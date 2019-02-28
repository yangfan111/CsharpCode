using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class CommonParachuteModel : ClientAbstractModel, IUiSystem
    {
       
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonParachuteModel));
        private CommonParachuteViewModel _viewModel = new CommonParachuteViewModel();

        private float _actualTotalHeight = 1170;
	    private float _totalViewHeight = 1170;
        private float _parachuteViewHeight = 500;
        private float _parachuteActualHeight = 500;//可额外配置
        private float _parachuteViewScale = 1.0f;

	    private bool _isHeightUpdate = false;
	    private Transform _handleImage;


        private IParachuteStateUiAdapter _adapter;
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public CommonParachuteModel(IParachuteStateUiAdapter adapter):base(adapter)
        {
            _adapter = adapter;
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
	        _totalViewHeight = FindChildGo("Background").GetComponent<RectTransform>().sizeDelta.y;
	        _parachuteViewHeight = FindChildGo("Handle").GetComponent<RectTransform>().sizeDelta.y;
	        _handleImage = FindChildGo("Handle");
        }

       public override void Update(float interval)
        {

            //if (!_adapter.IsDrop)//落地时，停止降落伞UI的显示
            //{
            //    _viewModel.Show = false;
            //    return;
            //}

	        if (!_isHeightUpdate && _adapter.IsDrop)//只在跳伞一瞬间更新一次
	        {
		        UpdateJumpHeight();
		        _isHeightUpdate = true;
	        }

            //_viewModel.Show = true;
            UpdateHeight();
            UpdateSpeed();
            UpdateTerrainHeight();
        }

		private void UpdateJumpHeight()
		{
			_actualTotalHeight = _adapter.TotalHeight;
			_parachuteActualHeight = _adapter.ForcedHeight;
			_parachuteViewScale = (_parachuteActualHeight / _parachuteViewHeight) / (_actualTotalHeight / _totalViewHeight);//处理配置了自动跳伞高度的情况
			_handleImage.localScale = new Vector3(1, _parachuteViewScale, 1);
		}

		/// <summary>
		/// 更新玩家当前坐标对应地形的高度
		/// </summary>
		private void UpdateTerrainHeight()
        {
            var terrainHeight = _adapter.TerrainHeight;

			//调整地形黑条的长度
	        _viewModel.HeightSliderValue = 1.0f - (terrainHeight / _actualTotalHeight) * (_totalViewHeight) / (_totalViewHeight - _parachuteViewHeight);
        }

        /// <summary>
        /// 更新玩家当前下降速度，单位KM/H
        /// </summary>
        private void UpdateSpeed()
        {
            var speed = _adapter.DropSpeed;
            _viewModel.SpeedString = ((int)(speed + 0.5f)).ToString();
        }

        /// <summary>
        /// 更新玩家当前距离地面的垂直高度
        /// </summary>
        private void UpdateHeight()
        {
            var height = _adapter.CurHeight;
            float posY = (height - _actualTotalHeight) * _totalViewHeight / _actualTotalHeight ;
            Vector3 origPos = _viewModel.HeightTipGroupPosition;
            origPos.y = posY;
            _viewModel.HeightTipGroupPosition = origPos;   
        }
    }
}


