using System.Collections.Generic;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Common;
using Assets.UiFramework.Libs;
using Core.GameModule.Interface;
using Core.Utils;
using DG.Tweening;
using UIComponent.UI;
using UnityEngine;
using WeaponConfigNs;

namespace App.Client.GameModules.Ui.Models.Common
{
    public class BaseWeaponHudModel : ClientAbstractModel, IUiHfrSystem
    {
        protected IWeaponStateUiAdapter _adapter;

        public BaseWeaponHudModel(IWeaponStateUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
        }

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BaseWeaponHudModel));
        private BaseWeaponHudViewModel _viewModel = new BaseWeaponHudViewModel();
        private int _weaponSlotChoice = 0;
        private int _grenadeSlotChoice = 0;
        private Dictionary<int, Sprite> _weaponIdPool = new Dictionary<int, Sprite>();

        private const string GrenadeSlotName = "GrenadeIconGroup";

        private const string WeaponSlotName = "WeaponSlot";

        private const int WeaponSlotNum = (int) WeaponHudStyleType.Down;

        private const int GrenadeSlotNum = 4;

        private const int TotalSlotNum = (int)WeaponHudStyleType.Bottom; 
  
        private const int GrenadeSlotPos = (int)WeaponHudStyleType.Bottom;

        private Transform _weaponModeTf;
        private Vector2 _origWeaponModePos;
        private int _lastBulletCount = -1;
        private int _curBulletCount = -1;


        private Sequence _bulletChangeAnime;

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
                UnSelectWeaponStateByIndex(i);
            }

            for (int i = 1; i <= GrenadeSlotNum; i++)
            {
                UnSelectGrenadeSlotByIndex(i);
            }
        }

        public override void Update(float interval)
        {
            if (!UpdateVisible())
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
            for (int i = 1; i <= TotalSlotNum; i++)
            {
                if (_adapter.HasWeaponByIndex(i))
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
                UnSelectGrenadeSlotByIndex(_grenadeSlotChoice);
                _grenadeSlotChoice = _adapter.CurrentGrenadeIndex;
                SelectGrenadeSlotByIndex(_grenadeSlotChoice);
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
            InitTweener();
            InitWeaponModeView();
            IntiUiSelectable();

            _viewModel.Show = false;
            _viewModel.WeaponGroupShow = true;

            InitWeaponSlotScale();
        }

        private UISelectable[] weaponSlotUISelectables = new UISelectable[TotalSlotNum + 1];
        private UISelectable[] grenadeSlotUISelectables = new UISelectable[GrenadeSlotNum + 1];

        private void IntiUiSelectable()
        {
            _weaponModeTf = FindChildGo("WeaponMode");
            if (_weaponModeTf != null) _origWeaponModePos = (_weaponModeTf as RectTransform).anchoredPosition;
            for (int i = 1; i < weaponSlotUISelectables.Length; i++)
            {
                weaponSlotUISelectables[i] = FindComponent<UISelectable>(WeaponSlotName + i);
            }
            for (int i = 1; i < grenadeSlotUISelectables.Length; i++)
            {
                grenadeSlotUISelectables[i] = FindComponent<UISelectable>(GrenadeSlotName + i);
            }
        }

        private void InitTweener()
        {
            InitBulletChangeAnime();
        }

        private void InitBulletChangeAnime()
        {
            float toAlpha = 0.5f;
            float toScale = 0.9f;
            float time = 0.1f;
            var alphaTweener = DOTween.To(() => _viewModel.BulletAlapha, x => _viewModel.BulletAlapha = x, toAlpha,
                time);

            float scale = 1f;
            var scaleTweener = DOTween.To(() => scale, x => _viewModel.BulletScale = x * Vector3.one, toScale, time);

            _bulletChangeAnime = DOTween.Sequence();
            _bulletChangeAnime.Insert(0, alphaTweener);
            _bulletChangeAnime.Insert(0, scaleTweener);
            _bulletChangeAnime.Pause();
            _bulletChangeAnime.SetAutoKill(false);
            _bulletChangeAnime.onComplete = () => { _bulletChangeAnime.Rewind(); };
            _bulletChangeAnime.onRewind = () =>
            {
                _viewModel.BulletCountString = FormatBulletCount(_lastBulletCount);
                _lastBulletCount = _curBulletCount;
            };
        }

        private string FormatBulletCount(int count)
        {
            if (count < 10) return "0" + count.ToString();
            return count.ToString();
        }

        private void InitSlotIndex()
        {
            var list = _adapter.SlotIndexList;
            for (int i = 0; i < TotalSlotNum && i < list.Count; i++)
            {
                _viewModel.SetMarkText(i + 1, list[i].ToString());
            }
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

            UpdateWeaponColorByIndex(GrenadeSlotPos);
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
            if (weaponId < 1) //没有武器
            {
                return;
            }

            Sprite image;
            if (_weaponIdPool.TryGetValue(weaponId, out image)) //直接从缓存里取
            {
                _viewModel.SetGrenadeIconSprite(index, image);
            }
            else //缓存没有，再请求ab
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
            UpdateWeaponMode(_weaponSlotChoice);
            for (int i = 1; i <= WeaponSlotNum; i++)
            {
                UpdateWeaponSlotByIndex(i);
            }
        }


        private void UpdateCurGrenade()
        {
            var curIndex = _adapter.CurrentGrenadeIndex;
            if (!NeedUpdateGrenade(curIndex))
            {
                return;
            }

            UpdateGrenadeIconByIndex(curIndex);
        }

        private bool NeedUpdateGrenade(int index)
        {
            return _viewModel.GetWeaponSlotShow(GrenadeSlotPos) || _adapter.HasGrenadByIndex(index);
        }

        private void UpdateAllGrenade()
        {
            for (int index = 1; index <= GrenadeSlotNum; index++)
            {
                if (!NeedUpdateGrenade(index))
                {
                    return;
                }

                UpdateGrenadeIconByIndex(index);
                UpdateGrenadeSlotShowByIndex(index);
            }
        }

        private void SelectGrenadeSlotByIndex(int index)
        {
            if (index <= 0) return;
            var selectable = grenadeSlotUISelectables[index];
            if (selectable != null) selectable.Selected = true;
        }

        private void UnSelectGrenadeSlotByIndex(int index)
        {
            if (index <= 0) return;
            var selectable = grenadeSlotUISelectables[index];
            if (selectable != null) selectable.Selected = false;
        }

        /// <summary>
        /// 更新武器槽的选择情况
        /// </summary>
        private void UpdateWeaponSlotChoice()
        {
            int curSlotIndex = _adapter.HoldWeaponSlotIndex;

            if (_weaponSlotChoice == curSlotIndex)
            {
                return; //判断武器槽的选择是否发生改变
            }

            UnSelectWeaponStateByIndex(_weaponSlotChoice);
            SelectWeaponSlotByIndex(curSlotIndex);
            SetWeaponSlotChoice(curSlotIndex);
         
        }

        private void SetWeaponSlotChoice(int curSlotIndex)
        {
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

        private bool NeedUpdateWeapon(int index)
        {
            return _viewModel.GetWeaponSlotShow(index) || _adapter.HasWeaponByIndex(index);
        }

        private void UpdateWeaponSlotByIndex(int index)
        {
            if (!NeedUpdateWeapon(index))
            {
                return;
            }

            UpdateWeaponIconByIndex(index);
            UpdateWeaponColorByIndex(index);
            UpdateWeaponSlotShowByIndex(index);
        }

        private void UpdateReservedBulletByIndex(int index)
        {
            if ((WeaponHudWeaponType) _adapter.WeaponTypeByIndex(index) != WeaponHudWeaponType.Gun)
            {
                _viewModel.ReservedBulletCountShow = false; //非一般枪类武器没有弹夹
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
            var type = (WeaponHudWeaponType) _adapter.WeaponTypeByIndex(index);
            if (type != WeaponHudWeaponType.Gun && type != WeaponHudWeaponType.Grenade)
            {
                _viewModel.BulletCountShow = false; //只有枪类和投掷类有数量
                return;
            }
            _viewModel.BulletCountShow = true;

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
                _viewModel.BulletCountString = FormatBulletCount(count);
                _lastBulletCount = count;
                _curBulletCount = count;
            }

        }

        private void PlayBulletCountDecreaseAnime()
        {
            _bulletChangeAnime.Restart();
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
            switch ((EFireMode) atkMode)
            {
                case EFireMode.Auto:
                    _viewModel.AtkModeString = I2.Loc.ScriptLocalization.client_common.word39;
                    _viewModel.AtkModeShow = true;
                    break;
                case EFireMode.Manual:
                    _viewModel.AtkModeString = I2.Loc.ScriptLocalization.client_common.word40;
                    _viewModel.AtkModeShow = true;
                    break;
                case EFireMode.Burst:
                    _viewModel.AtkModeString = I2.Loc.ScriptLocalization.client_common.word41;
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
        private void UnSelectWeaponStateByIndex(int index)
        {
            if (index <= 0)
            {
                return;
            }

            var selectable = weaponSlotUISelectables[index];
            if (selectable != null) selectable.Selected = false;

            if (index == GrenadeSlotPos)
            {
                var curIndex = _adapter.CurrentGrenadeIndex;
                for (int i = 1; i <= GrenadeSlotNum; i++)
                {
                    if (i != curIndex)
                    {
                        _viewModel.SetGrenadeIconGroupShow(i, false);
                    }
                }

                UnSelectGrenadeSlotByIndex(curIndex);
            }

        }

        /// <summary>
        /// 将选中的武器设置为选中状态
        /// </summary>
        /// <param name="index"></param>
        private void SelectWeaponSlotByIndex(int index)
        {
            if (index <= 0)
            {
                _viewModel.WeaponModeShow = false; //未选中时关闭显示
                return;
            }

            var selectable = weaponSlotUISelectables[index];
            if (selectable != null)
            {
                SetWeaponModePos(selectable.transform);
                selectable.Selected = true;
            }

            if (index == GrenadeSlotPos)
            {
                var curIndex = _adapter.CurrentGrenadeIndex;
                for (int i = 1; i <= GrenadeSlotNum; i++)
                {
                    if (i != curIndex)
                    {
                        UnSelectGrenadeSlotByIndex(i);
                        UpdateGrenadeSlotShowByIndex(i);
                    }
                }

                SelectGrenadeSlotByIndex(curIndex);
            }

        }

        private void SetWeaponModePos(Transform root)
        {
            if (_weaponModeTf == null) return;
            _weaponModeTf.parent = root;
            var rtf = _weaponModeTf.transform as RectTransform;
            var pos = _origWeaponModePos;
            rtf.anchoredPosition = new Vector2(pos.x, 0);
        }

        /// <summary>
        /// 更新武器剪影的颜色
        /// </summary>
        /// <param name="index"></param>
        private void UpdateWeaponColorByIndex(int index)
        {
            if ((WeaponHudWeaponType) _adapter.WeaponTypeByIndex(index) == WeaponHudWeaponType.Gun
                && _adapter.WeaponBulletCountByIndex(index) == 0
            ) //一般枪类武器没有子弹时，武器剪影变红
            {
                _viewModel.SetWeaponIconColor(index, NoBulletColor);
                if (index == _weaponSlotChoice)
                {
                    _viewModel.BulletCountColor = NoBulletColor;
                }
            }
            else
            {
                _viewModel.SetWeaponIconColor(index, HaveBulletColor);
                if (index == _weaponSlotChoice)
                {
                    _viewModel.BulletCountColor = HaveBulletColor;
                }
            }
        }

        private Color NoBulletColor = new Color32(0xff, 0x54, 0x54, 0xff);
        private Color HaveBulletColor = Color.white;

        /// <summary>
        /// 加载武器剪影图标
        /// </summary>
        /// <param name="index"></param>
        private void UpdateWeaponIconByIndex(int index)
        {
            int weaponId = _adapter.WeaponIdByIndex(index);

            Sprite image;
            if (_weaponIdPool.TryGetValue(weaponId, out image)) //直接从缓存里取
            {
                _viewModel.SetWeaponIconSprite(index, image);
            }
            else //缓存没有，再请求ab
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
            if (index == GrenadeSlotPos && _adapter.WeaponBulletCountByIndex(index) == 0)
            {
                _viewModel.SetWeaponSlotShow(index, false); //如果当前武器是投掷类且用完投掷数量，关闭显示
                if (index == _weaponSlotChoice)
                    _viewModel.WeaponModeShow = false; //如果当前武器是投掷类且用完投掷数量，关闭显示
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