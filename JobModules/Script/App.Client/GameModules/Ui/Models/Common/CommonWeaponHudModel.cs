using System.Collections.Generic;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Profiling;
using Utils.Utils;
using WeaponConfigNs;
using AssetInfo = Utils.AssetManager.AssetInfo;
using UnityEngine.UI;

namespace App.Client.GameModules.Ui.Models.Common
{

    enum WeaponHudStyleType
	{
		Top = 1,
		Up,
		Center,
		Down,
		Bottom,
	}

	enum WeaponHudWeaponType
	{
		Gun = 1,
		Grenade,
		Melee
	}
	class WeaponHudStyle
	{
		public float alpha;
		public Vector3 scale;
		public int index;

		public WeaponHudStyle(float v1, Vector3 vector3, int v2)
		{
			alpha = v1;
			scale = vector3;
			index = v2;
		}
	}

	public partial class CommonWeaponHudModel : ClientAbstractModel, IUiHfrSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonWeaponHudModel));
        private CommonWeaponHudViewModel _viewModel = new CommonWeaponHudViewModel();
        private int _weaponSlotChoice = 0;
	    private int _grenadeSlotChoice = 0;
        private Dictionary<int, Sprite> _weaponIdPool = new Dictionary<int, Sprite>();
		private int WeaponSlotNum
	    {
		    get { return (int) WeaponHudStyleType.Down; }
	    }

	    private int GrenadeSlotNum
	    {
		    get { return 4; }
	    } 

	    private int TotalSlotNum
	    {
		    get { return (int)WeaponHudStyleType.Bottom; }
	    }

	    private int GrenadeSlotPos
	    {
		    get { return TotalSlotNum; }
	    }
		
		private Vector3[] _slotPosList;
	    private Transform[] _slotTransforms;
        private Transform[] _grenadeSlotTransforms;

	    private Image[] _weaponIcons;
	    private Image[] _grenadeIcons;
	    private Dictionary<WeaponHudStyleType, WeaponHudStyle> StyleDict;
        private IWeaponStateUiAdapter _adapter;
		private float _unSelectedAlpha = UiCommonColor.ColorGammaCorrect(0.4f);
	    private int _lastBulletCount = -1;
        private int _curBulletCount = -1;
        

	    private Sequence _bulletChangeAnime;
        private Sequence _slotChangeAnime;
		protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

	    protected override void OnGameobjectInitialized()
	    {
		    base.OnGameobjectInitialized();
		    InitVariable();
        }
		
		private void InitWeaponModeView()
		{
			_viewModel.WeaponModeShow = false;
			_viewModel.BulletScale = Vector3.one;
			_viewModel.BulletAlapha = 1f;
		    for (int i = 1; i <= WeaponSlotNum; i++)
		    {
		        ResetWeaponStateByIndex(i);
		    }

		    for (int i = 1; i <= GrenadeSlotNum; i++)
		    {
		        SetGrenadeSlotToUnSelectedState(i);
		    }
        }

		public CommonWeaponHudModel(IWeaponStateUiAdapter adapter):base(adapter)
        {
            _adapter = adapter;
        }

        public override void Update(float interval)
        {
            _slotChangePosAnimeTime = 14 * interval;
            if(!UpdateVisible())
            {
                return;
            }

            UpdateWeaponSlotChoice();
	        UpdateGrenadeSlotChoice();
            UpdateWeaponChanged();
	        UpdateGrenadeChanged();
        }

		private bool UpdateVisible()
		{
			for (int i = 1; i <= WeaponSlotNum; i++)
			{
				if (_adapter.HasWeaponByIndex(i))
				{
                    _viewModel.Show = true;
                    return true;
				}
			
			}

			for (int i = 1; i <= GrenadeSlotNum; i++)
			{
				if (_adapter.HasGrenadByIndex(i))
				{
				    _viewModel.Show = true;
                    return true;
				}
			}
		    _viewModel.Show = false;
            return false;
		}

        private void UpdateGrenadeSlotChoice()
		{
			if (_grenadeSlotChoice == _adapter.CurrentGrenadeIndex)
			{
				return;
			}

			if (_weaponSlotChoice == GrenadeSlotPos)
			{
				SetGrenadeSlotToUnSelectedState(_grenadeSlotChoice);
				_grenadeSlotChoice = _adapter.CurrentGrenadeIndex;
				SetGrenadeSlotToSelectedState(_grenadeSlotChoice);
				
			}
			else
			{
				if (_grenadeSlotChoice > 0)
				{
					_viewModel.SetGrenadeIconGroupShow(_grenadeSlotChoice, false);
				}
				_grenadeSlotChoice = _adapter.CurrentGrenadeIndex;
				if (_grenadeSlotChoice > 0)
				{
					_viewModel.SetGrenadeIconGroupShow(_grenadeSlotChoice, true);
				}
			}

			_lastBulletCount = -1;
		    _curBulletCount = -1;

		}

		private void InitVariable()
		{
		    InitSlotIndex();
			InitStyleDict();
			InitTransList();
			InitTweener();
			InitWeaponModeView();

			_viewModel.Show = false;
			_viewModel.WeaponGroupShow = true;

			InitWeaponSlotScale();
		    UpdateSlotStyle();
		}

        private void InitSlotIndex()
        {
            var list = _adapter.SlotIndexList;
            for (int i = 0; i < TotalSlotNum && i < list.Count; i++)
            {
                _viewModel.SetMarkText(i + 1, list[i].ToString());
            }
        }

        private void InitTransList()
		{
			_slotTransforms = new Transform[TotalSlotNum + 1];
			_slotPosList = new Vector3[TotalSlotNum + 1];
			_weaponIcons = new Image[WeaponSlotNum + 1];
			_grenadeIcons = new Image[GrenadeSlotNum + 1];
		    _grenadeSlotTransforms = new Transform[GrenadeSlotNum + 1];

            for (int i = 1; i <= TotalSlotNum; i++)
			{
				_slotTransforms[i] = FindChildGo(WeaponSlotName + i);
				_slotPosList[i] = _slotTransforms[i].localPosition;
			}

			for (int i = 1; i <= WeaponSlotNum; i++)
			{
				_weaponIcons[i] = FindChildGo(WeaponIconName + i).GetComponent<Image>();
			}
			for (int i = 1; i <= GrenadeSlotNum; i++)
			{
				_grenadeIcons[i] = FindChildGo(GrenadeIconName + i).GetComponent<Image>();
			    _grenadeSlotTransforms[i] = FindChildGo(GrenadeSlotName + i);
			}

		}

        private string GrenadeSlotName
        {
            get { return "GrenadeIconGroup"; }
        }

        private string WeaponSlotName
	    {
		    get { return "WeaponSlot"; }
	    }

	    private string WeaponIconName
	    {
		    get { return "WeaponBg"; }
	    }

	    private string GrenadeIconName
	    {
		    get { return "GrenadeIcon"; }
	    }

        private void InitStyleDict()
		{
            StyleDict = new Dictionary<WeaponHudStyleType, WeaponHudStyle>(CommonIntEnumEqualityComparer<WeaponHudStyleType>.Instance);
            StyleDict.Add(WeaponHudStyleType.Top, new WeaponHudStyle(UiCommonColor.ColorGammaCorrect(0.4f), Vector3.one * 0.7f, 1));
			StyleDict.Add(WeaponHudStyleType.Up, new WeaponHudStyle(UiCommonColor.ColorGammaCorrect(0.7f), Vector3.one * 0.85f, 2));
			StyleDict.Add(WeaponHudStyleType.Center, new WeaponHudStyle(1f, Vector3.one, 3));
			StyleDict.Add(WeaponHudStyleType.Down, new WeaponHudStyle(UiCommonColor.ColorGammaCorrect(0.7f), Vector3.one * 0.85f, 4));
			StyleDict.Add(WeaponHudStyleType.Bottom, new WeaponHudStyle(UiCommonColor.ColorGammaCorrect(0.4f), Vector3.one * 0.7f, 5));
		}

	    private void InitWeaponSlotScale()
	    {
		    for (int i = 1; i <= TotalSlotNum; i++)
		    {
			    _viewModel.SetWeaponIconScale(i, Vector3.one);
		    }
	    }

	    private void UpdateGrenadeChanged()
	    {

		    if (_weaponSlotChoice != GrenadeSlotPos)
		    {
			    UpdateCurGrenade();
		    }
		    else
		    {
			    UpdateAllGrenade();
			    UpdateGrenadeCount();
			}
		    UpdateWeaponSlotShowByIndex(GrenadeSlotPos);
		}

		private void UpdateGrenadeCount()
		{
			UpdateBulletCountByIndex(GrenadeSlotPos);
		}

		private void UpdateGrenadeSlotShowByIndex(int index)
	    {
		    _viewModel.SetGrenadeIconGroupShow(index, _adapter.HasGrenadByIndex(index));
	    }

	    private void UpdateGrenadeIconByIndex(int index)
	    {
	        int weaponId = _adapter.GrenadeIdByIndex(index);
	        if (weaponId < 1)//没有武器
	        {
	            return;
	        }

	        Sprite image;
	        if (_weaponIdPool.TryGetValue(weaponId, out image))//直接从缓存里取
	        {
	            _viewModel.SetGrenadeIconSprite(index, image);
	        }
	        else//缓存没有，再请求ab
	        {
	            var weaponName = _adapter.GetAssetInfoById(weaponId);

	            _weaponIdPool.Add(weaponId, new Sprite());
	            Loader.RetriveSpriteAsync(weaponName.BundleName, weaponName.AssetName,
	                (sprite) =>
	                {
	                    _viewModel.SetGrenadeIconSprite(index, sprite);
	                    _weaponIdPool[weaponId] = sprite;
	                });

	        }

        }

        /// <summary>
        /// 更新武器相关信息
        /// </summary>
        private void UpdateWeaponChanged()
		{ 
			UpdateSlotStyle();
		    UpdateWeaponMode(_weaponSlotChoice);
            for (int i = 1; i <= WeaponSlotNum; i++)
            {
                UpdateWeaponSlotByIndex(i);
            }
        }

		
		private void UpdateCurGrenade()
		{
            
            var curIndex = _adapter.CurrentGrenadeIndex;
		    if (!_slotTransforms[GrenadeSlotPos].gameObject.activeSelf && !_adapter.HasGrenadByIndex(curIndex))
		    {
		        return;
		    }
            UpdateGrenadeIconByIndex(curIndex);
		}

		private void UpdateAllGrenade()
		{

			for (int index = 1; index <= GrenadeSlotNum; index++)
			{
                if (!_grenadeSlotTransforms[index].gameObject.activeSelf && !_adapter.HasGrenadByIndex(index))
                {
                    return;
                }
                UpdateGrenadeIconByIndex(index);
                UpdateGrenadeSlotShowByIndex(index);
            }
        }

		private void SetGrenadeSlotToSelectedState(int index)
		{
			if (index <= 0) return;
			_viewModel.SetGrenadeIconAlpha(index, 1);
			_viewModel.SetGrenadeIconScale(index, Vector3.one);
		}

		private void SetGrenadeSlotToUnSelectedState(int index)
		{
			if (index <= 0) return;
			_viewModel.SetGrenadeIconAlpha(index, _unSelectedAlpha);
			_viewModel.SetGrenadeIconScale(index, 0.7f * Vector3.one);
		}

		private void UpdateSlotStyle()
		{
		    if (_slotChangeAnime.IsPlaying())
		    {
		        return;
		    }
		    UpdateStyle();
            
		}

        private void UpdateStyle()
        {
            UpdateSlotPos();
            for (int i = 1; i <= TotalSlotNum; i++)
            {
                var style = StyleDict[(WeaponHudStyleType)i];

                SetSlotScale((WeaponHudStyleType)i, style.scale);
                SetSlotAlpha((WeaponHudStyleType)i, style.alpha);
            }
        }

        private void UpdateSlotPos()
		{

		    if (GetSlotTf(WeaponHudStyleType.Bottom).gameObject.activeSelf)
		    {
		        if (!GetSlotTf(WeaponHudStyleType.Down).gameObject.activeSelf)
		        {
		            StyleDict[WeaponHudStyleType.Down].index = StyleDict[WeaponHudStyleType.Center].index + 2;
		            StyleDict[WeaponHudStyleType.Bottom].index = StyleDict[WeaponHudStyleType.Center].index + 1;
                }
		        else
		        {
		            StyleDict[WeaponHudStyleType.Bottom].index = StyleDict[WeaponHudStyleType.Center].index + 2;
		            StyleDict[WeaponHudStyleType.Down].index = StyleDict[WeaponHudStyleType.Center].index + 1;
                }
		    }
		    

		    if (GetSlotTf(WeaponHudStyleType.Top).gameObject.activeSelf)
		    {
		        if (!GetSlotTf(WeaponHudStyleType.Up).gameObject.activeSelf)
		        {
		            StyleDict[WeaponHudStyleType.Up].index = StyleDict[WeaponHudStyleType.Center].index - 2;
		            StyleDict[WeaponHudStyleType.Top].index = StyleDict[WeaponHudStyleType.Center].index - 1;
                }
		        else
		        {
		            StyleDict[WeaponHudStyleType.Up].index = StyleDict[WeaponHudStyleType.Center].index - 1;
		            StyleDict[WeaponHudStyleType.Top].index = StyleDict[WeaponHudStyleType.Center].index - 2;
                }
            }

		    ClipIndex();
            SetSlotPos(WeaponHudStyleType.Center, WeaponHudStyleType.Center);
		    SetSlotPos(WeaponHudStyleType.Up, WeaponHudStyleType.Up);
		    SetSlotPos(WeaponHudStyleType.Down, WeaponHudStyleType.Down);
		    SetSlotPos(WeaponHudStyleType.Top, WeaponHudStyleType.Top);
		    SetSlotPos(WeaponHudStyleType.Bottom, WeaponHudStyleType.Bottom);

        }

        #region Setter And Getter
        private Transform GetSlotTf(WeaponHudStyleType slot)
        {
            return _slotTransforms[StyleDict[slot].index];
        }

        private Vector3 GetSlotLocalPos(WeaponHudStyleType slot)
        {
            return GetSlotTf(slot).localPosition;
        }

        private void SetSlotPos(WeaponHudStyleType targetSlot, WeaponHudStyleType posSlot)
        {
            _slotTransforms[StyleDict[targetSlot].index].localPosition = _slotPosList[(int)posSlot];
        }

        private void SetSlotPos(WeaponHudStyleType targetSlot, Vector3 pos)
        {
            _slotTransforms[StyleDict[targetSlot].index].localPosition = pos;
        }

        private void SetSlotAlpha(WeaponHudStyleType slot, float alpha)
        {
            GetSlotTf(slot).GetComponent<CanvasGroup>().alpha = alpha;
        }

        private float GetSlotLocalAlpha(WeaponHudStyleType slot)
        {
            return GetSlotTf(slot).GetComponent<CanvasGroup>().alpha;
        }

        private void SetSlotScale(WeaponHudStyleType slot, Vector3 scale)
        {
            GetSlotTf(slot).localScale = scale;
        }

        private Vector3 GetSlotLocalScale(WeaponHudStyleType slot)
        {
            return GetSlotTf(slot).localScale;
        }

        private int GetSlotByIndex(int index)
        {
            foreach (var item in StyleDict)
            {
                if (item.Value.index == index)
                {
                    return (int)item.Key;
                }
            }

            return 0;
        }
        #endregion

        /// <summary>
        /// 更新武器槽的选择情况
        /// </summary>
        private void UpdateWeaponSlotChoice()
        {
            int curSlotIndex = _adapter.HoldWeaponSlotIndex;

            if (_weaponSlotChoice == curSlotIndex)
            {
                return;//判断武器槽的选择是否发生改变
            }
            
            ResetWeaponStateByIndex(_weaponSlotChoice);
            UpdateWeaponStateByIndex(curSlotIndex);
            PlaySlotChangeAnime(_weaponSlotChoice, curSlotIndex);
            _weaponSlotChoice = curSlotIndex;
            _lastBulletCount = -1;
            _curBulletCount = -1;

        }

        private void UpdateWeaponMode(int index)
		{
			UpdateBulletCountByIndex(index);
			UpdateReservedBulletByIndex(index);
			UpdateAtkModeByIndex(index);
		}

		private void UpdateSlotPosIndex(int index)
		{
			if (index <= 0)
			{
				return;
			}
          
			StyleDict[WeaponHudStyleType.Center].index = index;
			StyleDict[WeaponHudStyleType.Down].index = index + 1;
			StyleDict[WeaponHudStyleType.Bottom].index = index + 2;
			StyleDict[WeaponHudStyleType.Up].index = index - 1;
			StyleDict[WeaponHudStyleType.Top].index = index - 2;

		    ClipIndex();
            
			_viewModel.SetWeaponSlotShow(index, true);
        }

        private void ClipIndex()
        {
            foreach (var it in StyleDict)
            {
                ClipIndex(ref it.Value.index);          
            }
        }

        private void ClipIndex(ref int index)
        {
            if (index > TotalSlotNum)
            {
                index -= TotalSlotNum;
            }
            else if (index < (int)WeaponHudStyleType.Top)
            {
                index += TotalSlotNum;
            }
        }

        private void UpdateWeaponSlotByIndex(int index)
        {
            if (!_slotTransforms[index].gameObject.activeSelf && !_adapter.HasWeaponByIndex(index))
            {
                return;
            }

            UpdateWeaponIconByIndex(index);
            UpdateWeaponColorByIndex(index);
            UpdateWeaponSlotShowByIndex(index);
        }

        private void UpdateReservedBulletByIndex(int index)
        {
            if ((WeaponHudWeaponType)_adapter.WeaponTypeByIndex(index) != WeaponHudWeaponType.Gun)
            {
                _viewModel.ReservedBulletCountShow = false;//非一般枪类武器没有弹夹
                return;
            }
            if (_adapter.WeaponReservedBulletByIndex(index) == 0)
            {
                _viewModel.ReservedBulletCountShow = false;
            }
            else
            {
                _viewModel.ReservedBulletCountShow = true;
                _viewModel.ReservedBulletCountString = 
                    "." + _adapter.WeaponReservedBulletByIndex(index);
            }   
        }

        private void UpdateBulletCountByIndex(int index)
        { 
            var type = (WeaponHudWeaponType)_adapter.WeaponTypeByIndex(index);
            if (type != WeaponHudWeaponType.Gun && type != WeaponHudWeaponType.Grenade)
            {
                _viewModel.BulletCountShow =  false;//只有枪类和投掷类有数量
                return;
            }

            var count = _adapter.WeaponBulletCountByIndex(index);
            
	        if (count == _lastBulletCount)
	        {
		        return;
	        }
            if (count < _lastBulletCount)
            {
                _curBulletCount = count;
                PlayBulletCountDecreaseAnime();
            }
            else
            {
                _viewModel.BulletCountString = count.ToString();
                _lastBulletCount = count;
                _curBulletCount = count;
            }

            _viewModel.BulletCountShow =  true;
	        
		}

		private void UpdateAtkModeByIndex(int index)
        {
            var atkModeNumber = _adapter.WeaponAtkModeNumberByIndex(index);
            if (atkModeNumber <= 1)
            {
                _viewModel.AtkModeShow = false;
                return;
            }
            var atkMode = _adapter.WeaponAtkModeByIndex(index);
            switch ((EFireMode)atkMode)
            {
                case EFireMode.Auto:
					_viewModel.AtkModeString =  I2.Loc.ScriptLocalization.client_common.word39;
                    _viewModel.AtkModeShow = true;
                    break;
                case EFireMode.Manual:
                    _viewModel.AtkModeString =  I2.Loc.ScriptLocalization.client_common.word40;
                    _viewModel.AtkModeShow = true;
                    break;
                case EFireMode.Burst:
                    _viewModel.AtkModeString =  I2.Loc.ScriptLocalization.client_common.word41;
                    _viewModel.AtkModeShow = true;
                    break;
                default:
                    _viewModel.AtkModeShow = false;
                    break;
            }
        }
       
        /// <summary>
        /// 将未选中的武器重置为原始状态
        /// </summary>
        /// <param name="index"></param>
        private void ResetWeaponStateByIndex(int index)
        {
	        if (index <= 0)
	        {
		        return;
	        }
            _viewModel.WeaponModeShow =  false;
	        if (index != GrenadeSlotPos)
	        {
		        _viewModel.SetWeaponIconAlpha(index, _unSelectedAlpha);
	        }
	        else
	        {
		        var curIndex = _adapter.CurrentGrenadeIndex;
		        for (int i = 1; i <= GrenadeSlotNum; i++)
		        {
			        if (i != curIndex)
			        {
				        _viewModel.SetGrenadeIconGroupShow(i, false);
			        }
		        }
				SetGrenadeSlotToUnSelectedState(curIndex);
	        }
        }

	    /// <summary>
	    /// 将选中的武器设置为选中状态
	    /// </summary>
	    /// <param name="index"></param>
	    private void UpdateWeaponStateByIndex(int index)
	    {
		    if (index <= 0)
		    {
			    return;
		    }
		    _viewModel.WeaponModeShow =  true;
		    if (index != GrenadeSlotPos)
		    {
			    _viewModel.SetWeaponIconAlpha(index, 1);
		    }
		    else
		    {
				_viewModel.BulletCountColor = Color.white;
			    var curIndex = _adapter.CurrentGrenadeIndex;
			    for (int i = 1; i <= GrenadeSlotNum; i++)
			    {
				    if (i != curIndex)
				    {
					    SetGrenadeSlotToUnSelectedState(i);
						UpdateGrenadeSlotShowByIndex(i);
				    }
			    }
			    SetGrenadeSlotToSelectedState(curIndex);
			}
	    }

		/// <summary>
		/// 更新武器剪影的颜色
		/// </summary>
		/// <param name="index"></param>
		private void UpdateWeaponColorByIndex(int index)
        {            
            if ((WeaponHudWeaponType)_adapter.WeaponTypeByIndex(index) == WeaponHudWeaponType.Gun
                && _adapter.WeaponBulletCountByIndex(index) == 0              
                )//一般枪类武器没有子弹时，武器剪影变红
            {
                _viewModel.SetWeaponIconColor(index, Color.red);
	            if (index == _weaponSlotChoice)
	            {
		            _viewModel.BulletCountColor = Color.red;
				}
            }
            else
            {
                _viewModel.SetWeaponIconColor(index, Color.white);
	            if (index == _weaponSlotChoice)
	            {
		            _viewModel.BulletCountColor = Color.white;
				}	
            }
        }

        /// <summary>
        /// 加载武器剪影图标
        /// </summary>
        /// <param name="index"></param>
        private void UpdateWeaponIconByIndex(int index)
        {
            if (!_adapter.HasWeaponByIndex(index))
            {
                return;
            }
            int weaponId = _adapter.WeaponIdByIndex(index);

            Sprite image;
            if(_weaponIdPool.TryGetValue(weaponId, out image))//直接从缓存里取
	        {
		        _viewModel.SetWeaponIconSprite(index, image);
	        }
	        else//缓存没有，再请求ab
            {
                var weaponName = _adapter.GetAssetInfoById(weaponId);

                _weaponIdPool.Add(weaponId, new Sprite());
                Loader.RetriveSpriteAsync(weaponName.BundleName, weaponName.AssetName,
			        (sprite) =>
			        {
				        _viewModel.SetWeaponIconSprite(index, sprite);
                        _weaponIdPool[weaponId] = sprite;
			        });

			}

        }

        private void UpdateWeaponSlotShowByIndex(int index)
        {
            //if ((WeaponHudWeaponType)_adapter.WeaponTypeByIndex(index) == WeaponHudWeaponType.Grenade && _adapter.WeaponBulletCountByIndex(index) == 0)
            if(index == GrenadeSlotPos && _adapter.WeaponBulletCountByIndex(index) == 0)
            {
                _viewModel.SetWeaponSlotShow(index, false);//如果当前武器是投掷类且用完投掷数量，关闭显示
                if(index == _weaponSlotChoice)
	            _viewModel.WeaponModeShow = false;//如果当前武器是投掷类且用完投掷数量，关闭显示
			}
            else
            {
                _viewModel.SetWeaponSlotShow(index, _adapter.HasWeaponByIndex(index));
                if (index == _weaponSlotChoice)
                    _viewModel.WeaponModeShow = _adapter.HasWeaponByIndex(index);
			}

            if (_adapter.IsSwitchWeapon)
            {
                _viewModel.WeaponModeShow = false;
            }
        }

    }
}
