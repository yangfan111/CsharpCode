using System;
using System.Collections.Generic;
using System.Linq;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Chicken;
using App.Shared;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Assets.Utils.Configuration;
using Core.GameModule.Interface;
using Core.Utils;
using Shared.Scripts;
using UIComponent.UI;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Chicken
{
    public class ChickenBagItemUiData : IChickenBagItemUiData
    {
        public int cat { get; set; }
        public int id { get; set; }
        public int count { get; set; }
        public string key { get; set; }
        public bool isBagTitle { get; set; }
        public string title { get; set; }
    }

    public class BaseChickenBagItemData : IBaseChickenBagItemData
    {
        public int cat { get; set; }
        public int id { get; set; }
        public int count { get; set; }
        public string key { get; set; }
    }

    public class ChickenBagModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenBagModel));
        private ChickenBagViewModel _viewModel = new ChickenBagViewModel();

        private IChickenBagUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitKey();
        }

        private void InitKey()
        {
            openKeyReceiver = new KeyReceiver(UiConstant.userCmdKeyLayer, BlockType.All);
            openKeyReceiver.AddAction(UserInputKey.OpenBag, (data) => { _adapter.Enable = true; });
            _adapter.RegisterKeyReceive(openKeyReceiver);
            keyReveiver = new KeyReceiver(UiConstant.userCmdUIKeyLayer, BlockType.All);
            keyReveiver.AddAction(UserInputKey.OpenBag, (data) => { _adapter.Enable = false; });
            pointerReceiver = new PointerReceiver(UiConstant.userCmdUIKeyLayer, BlockType.All);
        }


        UIScrollRect groundScrollRect, bagScrollRect;
        ReactiveListData<IChickenBagItemUiData> groundItemDataList;

        ReactiveListData<IBaseChickenBagItemData> bagItemDataList;

        private KeyReceiver keyReveiver, openKeyReceiver;
        private PointerReceiver pointerReceiver;

        private void InitVariable()
        {
            if (UIImageLoader.LoadSpriteAsync == null)
            {
                UIImageLoader.LoadSpriteAsync = Loader.RetriveSpriteAsync;
            }
            groundScrollRect = FindComponent<UIScrollRect>("GroundScrollView");
            bagScrollRect = FindComponent<UIScrollRect>("BagScrollView");
            groundItemDataList = new ReactiveListData<IChickenBagItemUiData>();
            bagItemDataList = new ReactiveListData<IBaseChickenBagItemData>();
            InitDragBeginEvent();     
            InitDragEndEvent();
            bagScrollRect.RegisterListData<IBaseChickenBagItemData, ChickenBagItem>(bagItemDataList);
            groundScrollRect.RegisterListData<IChickenBagItemUiData, ChickenBagItem>(groundItemDataList);
        }

        private void InitDragBeginEvent()
        {
            groundScrollRect.OnConstruct = (item) => { InitItemEvent(item, EChickenBagType.Ground); };
            bagScrollRect.OnConstruct = (item) => { InitItemEvent(item, EChickenBagType.Bag); };

            for (int i = 1; i <= 4; i++)
            {
                var dragEnd = FindComponent<UIDrag>("GearIcon" + i);
                var i1 = i - 1;
                dragEnd.OnEndDragCallback = (data) =>
                {
                    SetDragStartPos(EChickenBagType.Equipment, ((int)equipTypeList[i1]).ToString());
                };
            }

            for (int i = 1; i <= 6; i++)
            {
                var dragEnd = FindComponent<UIDrag>("WearIcon" + i);
                var i1 = i - 1;
                dragEnd.OnEndDragCallback = (data) =>
                {
                    SetDragStartPos(EChickenBagType.Equipment, ((int)equipTypeList[i1 + 4]).ToString());
                };
            }

            for (int j = 1; j <= 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    var type = weaponPartTypeList[i];
                    var dragEnd = FindComponent<UIDrag>(type.ToString() + "Icon" + j);
                    var j1 = j;
                    dragEnd.OnEndDragCallback = (data) =>
                    {
                        SetDragStartPos(EChickenBagType.WeaponPart, ((j1) * 10 + (int)type).ToString());
                    };
                }
            }

            for (int i = 1; i <= _slotNum; i++)
            {
                var dragEnd = FindComponent<UIDrag>("WeaponIcon" + i);
                var i1 = i;
                dragEnd.OnEndDragCallback = (data) => { SetDragStartPos(EChickenBagType.Weapon, i1.ToString()); };
            }
        }

        private void InitItemEvent(UILoopItem item, EChickenBagType type)
        {
            var realItem = item as ChickenBagItem;
            if (realItem == null) return;
            realItem.RightClickCallback = UseItem;
            realItem.DragCallback = (data)=>
            {
                DragBagItem(data,type);
            };
        }

        private void DragBagItem(IBaseChickenBagItemData data, EChickenBagType type)
        {
            SetDragStartPos(type, data.key);
            //dragKey = data.key;
        }


        private EChickenBagType endPos;
        private EChickenBagType startPos;
        private string dragKey;
        private int _slotNum = 5;
        private int endIndex;
        private void InitDragEndEvent()
        {

            var groundDragEnd = FindComponent<UIDragAccepted>("GrounditemPanel");
            groundDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Ground); };
            var bagDragEnd = FindComponent<UIDragAccepted>("SelfitemPanel");
            bagDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Bag); };
            var gearDragEnd = FindComponent<UIDragAccepted>("GearPanelBg");
            gearDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Equipment); };
            var wearDragEnd = FindComponent<UIDragAccepted>("WearPanelBg");
            wearDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Equipment); };
            for (int i = 1; i <= _slotNum; i++)
            {
                var dragEnd = FindComponent<UIDragAccepted>("WeaponSlot" + i);
                var i1 = i;
                dragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Weapon, i1); };
            }

            var weaponDragEnd = FindComponent<UIDragAccepted>("WeaponPanel");
            weaponDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Weapon); };
            
        }

        private void SetDragStartPos(EChickenBagType type,string key)
        {
            startPos = type;
            Debug.Log("startPos:" + startPos);
            string endKey = (int)endPos + "|" + endIndex;
            dragKey = key;
            if (startPos == EChickenBagType.Bag && endPos == EChickenBagType.Ground && IsSplitInput())
            {
                _adapter.SendSplitItem(dragKey);
                return;
            }
            _adapter.SendDragItem(dragKey, endKey);
            endPos = EChickenBagType.None;
            endIndex = 0;
        }

        private void SetDragEndPos(EChickenBagType type, int index = 0)
        {
            endPos = type;
            Debug.Log("endPos:" + endPos);
            this.endIndex = index;
        }

        private void UseItem(IBaseChickenBagItemData data)
        {
            if (IsSplitInput())
            {
                _adapter.SendSplitItem(data.key);
                return;
            }
            _adapter.SendRightClickUseItem(data.key);
        }

        private bool IsSplitInput()
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                return true;
            }

            return false;
                
        }


        public ChickenBagModel(IChickenBagUiAdapter adapter) : base(adapter)
        {
            _adapter = adapter;
            _adapter.Enable = false;
        }

        public override void Update(float interval)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            UpdateWeapon();
            UpdateWeaponPart();
            UpdateEquipment();
            UpdateBag();
            UpdateGround();
        }

        private void UpdateGround()
        {
            bool needUpdate = _adapter.RefreshGround();
            if (!needUpdate) return;
            var list = _adapter.GroundItemDataList;
            if (list == null) return;
            //Debug.Log("UpdateGround" + list.Count);
            groundItemDataList.SetList(list);
        }

        List<IBaseChickenBagItemData> oldBagLit;

        private void UpdateBag()
        {
            var list = _adapter.BagItemDataList;
            if (oldBagLit == list) return;
            oldBagLit = list;
            bagItemDataList.SetList(list);
            _viewModel.WeightText = _adapter.CurWeight + "/" + _adapter.TotalWeight;
        }

        private void UpdateEquipment()
        {
            int cat = (int) EItemCategory.RoleAvatar;

            foreach (Wardrobe type in equipTypeList)
            {
                int count = 0;
                int id = _adapter.GetEquipmentIdByWardrobeType(type, out count);
                UpdateEquip(type, cat, id, count.ToString(), id > 0);
            }
        }


        private void UpdateWeaponPart()
        {
            int cat = (int) EItemCategory.WeaponPart;
            for (int i = 1; i <= 3; i++)
            {
                foreach (EWeaponPartType type in weaponPartTypeList)
                {
                    var id = _adapter.GetWeaponPartIdBySlotIndexAndWeaponPartType(i, type);
                    UpdateWeaponPart(i, type, cat, id, id > 0);
                }
            }
        }

        private List<EWeaponPartType> weaponPartTypeList = new List<EWeaponPartType>
        {
            EWeaponPartType.UpperRail, EWeaponPartType.Muzzle, EWeaponPartType.LowerRail, EWeaponPartType.Magazine,
            EWeaponPartType.Stock
        };

        private void UpdateWeapon()
        {
            int cat = (int) EItemCategory.Weapon;
            for (int i = 1; i <= 5;i++)
            {
                int id = _adapter.GetWeaponIdBySlotIndex(i);
                UpdateWeapon(i, cat, id,id > 0);
            }
        }


        private List<Wardrobe> equipTypeList = new List<Wardrobe>
        {
            Wardrobe.Cap,Wardrobe.Armor, Wardrobe.Bag,  Wardrobe.Waist, Wardrobe.PendantFace, Wardrobe.Inner,
            Wardrobe.Outer,
            Wardrobe.Glove, Wardrobe.Trouser, Wardrobe.Foot
        };
        private bool _haveRegister;

        private void UpdateEquip(Wardrobe type, int cat,int id,string count,bool isShow = true)
        {
            
            switch (type)
            {
                case Wardrobe.Armor: UpdateGear(2, cat, id,count, isShow,true);break;
                case Wardrobe.Bag: UpdateGear(3, cat, id, count, isShow, false);
                    break;
                case Wardrobe.Cap: UpdateGear(1, cat, id, count, isShow, true);break;
                case Wardrobe.Waist: UpdateGear(4, cat, id, count, isShow, false);break;
                case Wardrobe.PendantFace: UpdateWear(1, cat, id, count, isShow);break;
                case Wardrobe.Inner: UpdateWear(2, cat, id, count, isShow);break;
                case Wardrobe.Outer: UpdateWear(3, cat, id, count, isShow);break;
                case Wardrobe.Glove: UpdateWear(4, cat, id, count, isShow);break;
                case Wardrobe.Trouser: UpdateWear(5, cat, id, count, isShow);break;
                case Wardrobe.Foot: UpdateWear(6, cat, id, count, isShow);break;
            }
 
        }

        private void UpdateWear(int index, int cat, int id, string count, bool isShow)
        {
            var config = GetConfig(cat, id);
            _viewModel.SetWearGroupShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetWearIconBundle(index, config.IconBundle);
            _viewModel.SetWearIconAsset(index, config.Icon);
            _viewModel.SetWearNameText(index, config.Name);
        }

        private void UpdateGear(int index, int cat, int id, string count,bool isShow,bool isLasting)
        {
            var config = GetConfig(cat, id);
            _viewModel.SetGearIconGroupShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetGearLastingBgShow(index, isLasting);
            _viewModel.SetGearNameText(index, config.Name);
            _viewModel.SetGearLastingLayerShow(index, isLasting);
            _viewModel.SetGearIconBundle(index, config.IconBundle);
            _viewModel.SetGearIconAsset(index, config.Icon);
            if (isLasting)
            {
                _viewModel.SetGearLastingNumText(index, count);
            }
        }

        private void UpdateWeaponPart(int index, EWeaponPartType type,int cat, int id, bool isShow = true)
        {

            switch (type)
            {
                case EWeaponPartType.Muzzle: UpdateMuzzle(index, cat, id, isShow); break;
                case EWeaponPartType.LowerRail: UpdateLowerRail(index, cat, id, isShow); break;
                case EWeaponPartType.Stock: UpdateStock(index, cat, id, isShow); break;
                case EWeaponPartType.UpperRail: UpdateUpperRail(index, cat, id, isShow); break;
                case EWeaponPartType.Magazine: UpdateMagazine(index, cat, id, isShow); break;
            }
        }

        private void UpdateWeaponPart(int index, int cat,int id, bool isShow = true)
        {
            int primeIndex = index / 10;
            int secondIndex = index % 10;
            var type = (EWeaponPartType)secondIndex;
            switch (type)
            {
                case EWeaponPartType.Muzzle: UpdateMuzzle(primeIndex, cat,id,isShow);break;
                case EWeaponPartType.LowerRail:UpdateLowerRail(primeIndex, cat,id, isShow); break;
                case EWeaponPartType.Stock:UpdateStock(primeIndex, cat,id, isShow); break;
                case EWeaponPartType.UpperRail: UpdateUpperRail(primeIndex, cat,id, isShow);break;
                case EWeaponPartType.Magazine: UpdateMagazine(primeIndex, cat,id, isShow);break;
            }
        }

        private void UpdateWeaponPart(int index, int cat, Dictionary<EWeaponPartType, int> dictionary)
        {
            int primeIndex = index;
            foreach (KeyValuePair<EWeaponPartType, int> pair in dictionary.ToList())
            {
                switch (pair.Key)
                {
                    case EWeaponPartType.Muzzle: UpdateMuzzle(primeIndex, cat, pair.Value); break;
                    case EWeaponPartType.LowerRail: UpdateLowerRail(primeIndex,cat, pair.Value); break;
                    case EWeaponPartType.Stock: UpdateStock(primeIndex, cat, pair.Value); break;
                    case EWeaponPartType.UpperRail: UpdateUpperRail(primeIndex, cat, pair.Value); break;
                    case EWeaponPartType.Magazine: UpdateMagazine(primeIndex, cat, pair.Value); break;
                }
            }   
        }

        private void UpdateMagazine(int index, int cat, int id, bool isShow = true)
        {
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetMagazineIconShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetMagazineIconBundle(index, config.IconBundle);
            _viewModel.SetMagazineIconAsset(index, config.Icon);
        }

        private void UpdateUpperRail(int index, int cat, int id, bool isShow = true)
        {
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetUpperRailIconShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetUpperRailIconBundle(index, config.IconBundle);
            _viewModel.SetUpperRailIconAsset(index, config.Icon);
        }

        private void UpdateStock(int index, int cat, int id, bool isShow = true)
        {
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetStockIconShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetStockIconBundle(index, config.IconBundle);
            _viewModel.SetStockIconAsset(index, config.Icon);
        }

        private void UpdateLowerRail(int index, int cat, int id, bool isShow = true)
        {
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetLowerRailIconShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetLowerRailIconBundle(index, config.IconBundle);
            _viewModel.SetLowerRailIconAsset(index, config.Icon);
        }

        private void UpdateMuzzle(int index, int cat,int id, bool isShow = true)
        {
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetMuzzleIconShow(index, isShow);
            if (!isShow) return;
            _viewModel.SetMuzzleIconBundle(index, config.IconBundle);
            _viewModel.SetMuzzleIconAsset(index, config.Icon);
        }

     
        private void UpdateWeapon(int index, int cat,int id,bool isShow = true)
        {
            var config = GetConfig(cat,id);
            _viewModel.SetWeaponGroupShow(index,isShow);
            if (!isShow) return;
            _viewModel.SetWeaponIconBundle(index,config.IconBundle);
            _viewModel.SetWeaponIconAsset(index,config.Icon);
            _viewModel.SetWeaponNameText(index, config.Name);
            UpdateWeaponPartSlot(index, id);      
        }

        private void UpdateWeaponPartSlot(int index, int id)
        {
            if (index > 3 || index < 1) return; 
            List<EWeaponPartType> list = SingletonManager.Get<WeaponPartsConfigManager>().GetAvaliablePartTypes(id);
            _viewModel.SetLowerRailSlotShow(index, false);
            _viewModel.SetMagazineSlotShow(index, false);
            _viewModel.SetMuzzleSlotShow(index, false);
            _viewModel.SetStockSlotShow(index, false);
            _viewModel.SetUpperRailSlotShow(index, false);
            foreach (EWeaponPartType type in list)
            {
                switch (type)
                {
                    case EWeaponPartType.LowerRail: _viewModel.SetLowerRailSlotShow(index, true);break;
                    case EWeaponPartType.Muzzle: _viewModel.SetMuzzleSlotShow(index, true); break;
                    case EWeaponPartType.Magazine: _viewModel.SetMagazineSlotShow(index, true); break;
                    case EWeaponPartType.UpperRail: _viewModel.SetUpperRailSlotShow(index, true);break;
                    case EWeaponPartType.Stock: _viewModel.SetStockSlotShow(index, true);break;
                }
            }
        }

        private ItemBaseConfig GetConfig(int cat,int id)
        {
            return SingletonManager.Get<ItemBaseConfigManager>().GetConfigById(cat, id);
        }
        private ItemBaseConfig GetWeaponPartConfig(int cat, int id)
        {
            var config = SingletonManager.Get<WeaponPartSurvivalConfigManager>().FindConfigBySetId(id);
            var partId = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetDefaultPartBySetId(id);
            var realConfig = GetConfig(cat, partId);
            if(config != null)
            realConfig.Name = config.Name;
            return realConfig;
        }

        private bool ParseKey(IChickenBagItemData it, out EChickenBagType type, out int index)
        {
            type = EChickenBagType.None;
            index = 0;
            string origKey = it.key;
            var list = origKey.Split('|');//type|index
            if (list.Length < 2) return false;
            type = (EChickenBagType)Int32.Parse(list[0]);
            index = Int32.Parse(list[1]);
            return true;
        }

        enum EChickenBagType
        {
            None,
            Ground,
            Bag,
            Equipment,  
            Weapon,
            WeaponPart
        }

        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.SetCanvasEnabled(enable);

            if (enable && !_haveRegister)
            {
                RegisterKeyReceiver();
            }
            else if (!enable && _haveRegister)
            {
                UnRegisterKeyReceiver();
            }

        }

        private void UnRegisterKeyReceiver()
        {
            _adapter.SetCrossVisible(true);

            if (keyReveiver == null || pointerReceiver == null)
            {
                return;
            }
            _adapter.UnRegisterKeyReceive(keyReveiver);
            _adapter.UnRegisterPointerReceive(pointerReceiver);

            _haveRegister = false;
            _adapter.ShowUiGroup(Core.Ui.UiGroup.SurvivalBagHide);

        }

        private void RegisterKeyReceiver()
        {
            _adapter.SetCrossVisible(false);

            if (keyReveiver == null || pointerReceiver == null)
            {
                return;
            }
            _adapter.RegisterKeyReceive(keyReveiver);
            _adapter.RegisterPointerReceive(pointerReceiver);
            _haveRegister = true;
            _adapter.HideUiGroup(Core.Ui.UiGroup.SurvivalBagHide);

        }
    }
}

