using System;
using System.Collections.Generic;
using System.Linq;
using App.Client.GameModules.Ui.Models.Common;
using App.Client.GameModules.Ui.Models.Common.Tip;
using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.ViewModels.Chicken;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui;
using Assets.UiFramework.Libs;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.GameModule.Interface;
using Core.Utils;
using Shared.Scripts;
using UIComponent.UI;
using UIComponent.UI.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Chicken
{

    public class ChickenBagModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenBagModel));
        private ChickenBagViewModel _viewModel = new ChickenBagViewModel();

        private IChickenBagUiAdapter _adapter;
        private TipManager tipManager;
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
            openKeyReceiver = new KeyReceiver(UiConstant.userCmdKeyLayer, BlockType.None);
            openKeyReceiver.BindKeyAction(UserInputKey.OpenBag, (data) => { _adapter.Enable = true; });
            _adapter.RegisterOpenKey(openKeyReceiver);
            keyReveiver = new KeyReceiver(UiConstant.userCmdUIKeyLayer, BlockType.All);
            keyReveiver.BindKeyAction(UserInputKey.OpenBag, (data) => { _adapter.Enable = false; });
            keyReveiver.BindKeyAction(UserInputKey.HideWindow, (data) => { _adapter.Enable = false; });
            pointerReceiver = new PointerReceiver(UiConstant.userCmdUIKeyLayer, BlockType.All);
        }


        UIScrollRect groundScrollRect, bagScrollRect;
        ReactiveListData<IChickenBagItemUiData> groundItemDataList;

        ReactiveListData<IBaseChickenBagItemData> bagItemDataList;

        private KeyReceiver keyReveiver, openKeyReceiver;
        private PointerReceiver pointerReceiver;

        private void InitVariable()
        {
            tipManager = UiCommon.TipManager;
            groundScrollRect = FindComponent<UIScrollRect>("GroundScrollView");
            bagScrollRect = FindComponent<UIScrollRect>("BagScrollView");
            groundItemDataList = new ReactiveListData<IChickenBagItemUiData>();
            bagItemDataList = new ReactiveListData<IBaseChickenBagItemData>();
            InitTipEvent();
            InitDragBeginEvent();     
            InitDragEndEvent();
            bagScrollRect.RegisterListData<IBaseChickenBagItemData, ChickenBagItem>(bagItemDataList);
            groundScrollRect.RegisterListData<IChickenBagItemUiData, ChickenBagItem>(groundItemDataList);
        }

        private void InitTipEvent()
        {
            for (int i = 1; i <= 4; i++)
            {
                var root = FindChildGo("GearIcon" + i);
                var i1 = i - 1;
                ShowTip(EChickenBagType.Equipment, (int)equipTypeList[i1], root);
            }

            for (int i = 1; i <= 6; i++)
            {
                var root = FindChildGo("WearIcon" + i);
                var i1 = i - 1;
                ShowTip(EChickenBagType.Equipment, (int) equipTypeList[i1 + 4], root);
            }

            for (int j = 1; j <= 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    var type = weaponPartTypeList[i];
                    var root = FindChildGo(type + "Icon" + j);
                    var j1 = j;
                    ShowTip(EChickenBagType.WeaponPart, (j1) * 10 + (int) type, root);
                }
            }

            for (int i = 1; i <= _slotNum; i++)
            {
                var root = FindChildGo("WeaponIcon" + i);
                var i1 = i;
                ShowTip(EChickenBagType.Weapon, i1,root);
            }
        }

        private void ShowTip(EChickenBagType type, int key,Transform root)
        {
            switch (type)
            {
                case EChickenBagType.Equipment: ShowEquipmentTip((Wardrobe) key, root);break;
                case EChickenBagType.WeaponPart: ShowWeaponPartTip(key / 10, key - 10 * (key / 10), root);break;
                case EChickenBagType.Weapon: ShowWeaponTip(key, root);break;
            
            }
        }

        private void ShowWeaponTip(int weaponSlot, Transform root)
        {
            
            ShowTip(() => {
                var id = _adapter.GetWeaponIdBySlotIndex(weaponSlot);
                if (id <= 0) return null;
                var data = new TipShowData();
                data.CategoryId = (int)ECategory.Weapon;
                data.TemID = id;
                HandleWeaponPartDataInWeaponSlot(weaponSlot,data);
                data.IsMyWeapon = true;
                return data;
            }, root);
        }

        private void HandleWeaponPartDataInWeaponSlot(int index,TipShowData data)
        {
            var list = _adapter.GetWeaponPartIdsBySlotIndex(index);
            var partsDataList = data.WeaponPartsList;
            partsDataList.Clear();
            foreach(var it in list)
            {
                var partData = new TipPartUiData();
                var config = GetConfig((int) ECategory.WeaponPart, it);
                if (config == null) continue;
                partData.Id = it;
                partData.Name = config.Name;
                partData.Quality = config.Xlv;
                partData.Type = config.Type;
                partsDataList.Add(partData);
            }
        }

        int TypeToSlot(EWeaponType_Config type)
        {
            var slot = 0;
            switch (type)
            {
                case EWeaponType_Config.PrimeWeapon: slot = 1;break;
                case EWeaponType_Config.SubWeapon: slot = 3;break;
                case EWeaponType_Config.ThrowWeapon: slot = 5;break;
                case EWeaponType_Config.MeleeWeapon: slot = 4;break;
            }
            return slot;
        }

        private void HandleWeaponContrastId(TipShowData data)
        {
            var config = GetConfig((int)ECategory.Weapon, data.TemID);
            if (config == null)
            {
                Logger.Error("error weapon config id:" + data.TemID);
                return;
            }
            data.ContrastTemId = 0;
            var type = config.Type;
            var slot = TypeToSlot((EWeaponType_Config)type);
            int ContrastTemId = 0;
            if (type == 1)//主武器两个槽
            {
                var prime2Id = _adapter.GetWeaponIdBySlotIndex(2);
                var prime1Id = _adapter.GetWeaponIdBySlotIndex(1);
                if (prime2Id > 0) ContrastTemId = prime2Id;
                if (prime1Id > 0) ContrastTemId = prime1Id;
            }
            else
            {
                var id = _adapter.GetWeaponIdBySlotIndex(slot);
                if (id > 0)
                {
                    ContrastTemId = id;
                }
            }

            if (ContrastTemId > 0)
            {
                data.ContrastTemId = ContrastTemId;
            }

        }

        private void ShowWeaponPartTip(int weaponSlot, int weaponPartSlot, Transform root)
        {
            ShowTip(() =>
            {
                var id = _adapter.GetWeaponPartIdBySlotIndexAndWeaponPartType(weaponSlot,
                    (EWeaponPartType) weaponPartSlot);
                if (id <= 0) return null;
                var data = new TipShowData();
                data.CategoryId = (int) ECategory.WeaponPart;
                data.TemID = id;
                return data;
            }, root);
        }

        bool haveDurable(Wardrobe wardrobe)
        {
            return durableEquipTypeList.Contains(wardrobe);
        }

        private void ShowEquipmentTip(Wardrobe key,Transform root)
        {     
            ShowTip(() =>
            {
                int count;
                var id = _adapter.GetEquipmentIdByWardrobeType(key, out count);
                if (id <= 0) return null;
                var data = new TipShowData();
                data.CategoryId = haveDurable(key)?(int)ECategory.Weapon:(int) ECategory.Avatar;
                data.TemID = id;
                if (count > 0) data.Num = count;
                return data;
            },root);
        }

        private void ShowTip(Func<TipShowData> data, Transform root,bool needPassEvent = true)
        {
            UIEventTriggerListener listener = UIEventTriggerListener.Get(root);

            listener.onEnter = (arg1, arg2) =>
            {
                tipManager.RegisterTip<CommonItemTipModel>(root, data.Invoke());
            };
            if(needPassEvent)
            listener.onDrop = (arg1,arg2)=>
            {
                root.GetComponentInParent<UIDragAccepted>().OnDrop(arg2);
            };
        }

        private readonly Vector2 _copyWeaponIconSize = new Vector2(72, 72); 

        private void InitDragBeginEvent()
        {
            groundScrollRect.OnConstruct = (item) => { InitItemEvent(item, EChickenBagType.Ground); };
            bagScrollRect.OnConstruct = (item) => { InitItemEvent(item, EChickenBagType.Bag); };

            for (int i = 1; i <= 4; i++)
            {
                var dragEnd = FindComponent<UIDrag>("GearIcon" + i);
                var i1 = i - 1;
                var key = "3|" + ((int) equipTypeList[i1]).ToString();
                dragEnd.OnEndDragCallback = (data) =>
                {
                    SetDragStartPos(EChickenBagType.Equipment, key);
                };
                var listener = UIEventTriggerListener.Get(dragEnd.gameObject);
                listener.onClick += (UIEventTriggerListener arg1, PointerEventData arg2) =>
                {
                    if(arg2.button == PointerEventData.InputButton.Right)
                    UseItem(key);
                };
            }

            for (int i = 1; i <= 6; i++)
            {
                var dragEnd = FindComponent<UIDrag>("WearIcon" + i);
                var i1 = i - 1;
                var key = "3|" + ((int) equipTypeList[i1 + 4]).ToString();
                dragEnd.OnEndDragCallback = (data) =>
                {
                    SetDragStartPos(EChickenBagType.Equipment, key);
                };
                var listener = UIEventTriggerListener.Get(dragEnd.gameObject);
                listener.onClick += (UIEventTriggerListener arg1, PointerEventData arg2) =>
                {
                    if (arg2.button == PointerEventData.InputButton.Right)
                        UseItem(key);
                };
            }

            for (int j = 1; j <= 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    var type = weaponPartTypeList[i];
                    var dragEnd = FindComponent<UIDrag>(type.ToString() + "Icon" + j);
                    var j1 = j;
                    var key = "5|" + ((j1) * 10 + (int) type).ToString();
                    dragEnd.OnEndDragCallback = (data) =>
                    {
                        SetDragStartPos(EChickenBagType.WeaponPart, key);
                    };
                    var listener = UIEventTriggerListener.Get(dragEnd.gameObject);
                    listener.onClick += (UIEventTriggerListener arg1, PointerEventData arg2) =>
                    {
                        if (arg2.button == PointerEventData.InputButton.Right)
                            UseItem(key);
                    };
                }
            }

            for (int i = 1; i <= _slotNum; i++)
            {
                var dragEnd = FindComponent<UIDrag>("WeaponIcon" + i);
                var i1 = i;
                var key = "4|" + i1.ToString();
                dragEnd.OnEndDragCallback = (data) => { SetDragStartPos(EChickenBagType.Weapon, key); };
                dragEnd.CopyItemSize = _copyWeaponIconSize;
                var listener = UIEventTriggerListener.Get(dragEnd.gameObject);
                listener.onClick += (UIEventTriggerListener arg1, PointerEventData arg2) =>
                {
                    if (arg2.button == PointerEventData.InputButton.Right)
                        UseItem(key);
                };
            }
        }

        private void UseItem(string key)
        {
            _adapter.SendRightClickUseItem(key);
        }

        private void InitItemEvent(UIItem item, EChickenBagType type)
        {
            var realItem = item as ChickenBagItem;
            if (realItem == null) return;
            realItem.RightClickCallback = UseItem;
            realItem.DragCallback = (data)=>
            {
                DragBagItem(data,type);
            };
            realItem.EnterCallback = (bagItem) => { ShowTip(bagItem); };
        }

        private void ShowTip(ChickenBagItem item)
        {
            
            ShowTip(() =>
            {
                var origData = item.Data as IBaseChickenBagItemData;
                var data = new TipShowData();
                data.CategoryId = origData.cat;
                data.TemID = origData.id;
                data.Num = origData.count;
                if (data.CategoryId == (int)ECategory.Weapon)
                {
                    HandleWeaponContrastId(data);
                }
                return data;
            }, item.GetRoot());
        }

        private void DragBagItem(IBaseChickenBagItemData data, EChickenBagType type)
        {
            SetDragStartPos(type, data.key);
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
            var gearDragEnd = FindComponent<UIDragAccepted>("GearPanel");
            gearDragEnd.OnDragAcceptedCallback = () => { SetDragEndPos(EChickenBagType.Equipment); };
            var wearDragEnd = FindComponent<UIDragAccepted>("WearPanel");
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
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
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
            groundItemDataList.SetList(list);
        }

        List<IBaseChickenBagItemData> oldBagLit;

        private void UpdateBag()
        {
            _viewModel.WeightText = _adapter.CurWeight + "/" + _adapter.TotalWeight;
            var list = _adapter.BagItemDataList;
            if (oldBagLit == list) return;
            oldBagLit = list;
            bagItemDataList.SetList(list);
        }

        private void UpdateEquipment()
        {
            int cat = (int) ECategory.Avatar;
            int durableCat = (int) ECategory.Weapon;
            foreach (Wardrobe type in equipTypeList)
            {
                int count = 0;
                int id = _adapter.GetEquipmentIdByWardrobeType(type, out count);
                var realCat = haveDurable(type) ? durableCat : cat;
                UpdateEquip(type, realCat, id, count, id > 0);
            }
        }


        private void UpdateWeaponPart()
        {
            int cat = (int) ECategory.WeaponPart;
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
            int cat = (int) ECategory.Weapon;
            for (int i = 1; i <= 5;i++)
            {
                int id = _adapter.GetWeaponIdBySlotIndex(i);
                UpdateWeapon(i, cat, id,id > 0);
            }
        }

        private HashSet<Wardrobe> durableEquipTypeList = new HashSet<Wardrobe>(CommonIntEnumEqualityComparer<Wardrobe>.Instance)
        {
            Wardrobe.Cap, Wardrobe.Armor
        };

        private List<Wardrobe> equipTypeList = new List<Wardrobe>
        {
            Wardrobe.Cap,Wardrobe.Armor, Wardrobe.Bag,  Wardrobe.Waist, Wardrobe.PendantFace, Wardrobe.Inner,
            Wardrobe.Outer,
            Wardrobe.Glove, Wardrobe.Trouser, Wardrobe.Foot
        };
        private bool _haveRegister;

        private void UpdateEquip(Wardrobe type, int cat,int id,int count,bool isShow = true)
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

        private void UpdateWear(int index, int cat, int id, int count, bool isShow)
        {
            _viewModel.SetWearGroupShow(index, isShow);
            if (!isShow) return;
            var config = GetConfig(cat, id);
            _viewModel.SetWearIconBundle(index, config.IconBundle);
            _viewModel.SetWearIconAsset(index, config.Icon);
            _viewModel.SetWearNameText(index, config.Name);
        }

        private void UpdateGear(int index, int cat, int id, int count,bool isShow,bool isLasting)
        {
            _viewModel.SetGearIconGroupShow(index, isShow);
            if (!isShow) return;
            var config = GetConfig(cat, id);
            _viewModel.SetGearLastingBgShow(index, isLasting);
            _viewModel.SetGearNameText(index, config.Name);
            _viewModel.SetGearLastingLayerShow(index, isLasting);
            _viewModel.SetGearIconBundle(index, config.IconBundle);
            _viewModel.SetGearIconAsset(index, config.Icon);
            if (!isLasting) return;
            var weaponConfig = config as WeaponResConfigItem;
            _viewModel.SetGearLastingNumText(index, count.ToString());
            if (weaponConfig == null) return;
            _viewModel.SetGearLastingLayerValue(index, (float)count / weaponConfig.Durable);
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

        private void UpdateMagazine(int index, int cat, int id, bool isShow = true)
        {
            _viewModel.SetMagazineIconShow(index, isShow);
            if (!isShow) return;
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetMagazineIconBundle(index, config.IconBundle);
            _viewModel.SetMagazineIconAsset(index, config.Icon);
        }

        private void UpdateUpperRail(int index, int cat, int id, bool isShow = true)
        {
            _viewModel.SetUpperRailIconShow(index, isShow);
            if (!isShow) return;
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetUpperRailIconBundle(index, config.IconBundle);
            _viewModel.SetUpperRailIconAsset(index, config.Icon);
        }

        private void UpdateStock(int index, int cat, int id, bool isShow = true)
        {
            _viewModel.SetStockIconShow(index, isShow);
            if (!isShow) return;
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetStockIconBundle(index, config.IconBundle);
            _viewModel.SetStockIconAsset(index, config.Icon);
        }

        private void UpdateLowerRail(int index, int cat, int id, bool isShow = true)
        {
            _viewModel.SetLowerRailIconShow(index, isShow);
            if (!isShow) return;
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetLowerRailIconBundle(index, config.IconBundle);
            _viewModel.SetLowerRailIconAsset(index, config.Icon);
        }

        private void UpdateMuzzle(int index, int cat,int id, bool isShow = true)
        {
            _viewModel.SetMuzzleIconShow(index, isShow);
            if (!isShow) return;
            var config = GetWeaponPartConfig(cat, id);
            _viewModel.SetMuzzleIconBundle(index, config.IconBundle);
            _viewModel.SetMuzzleIconAsset(index, config.Icon);
        }

     
        private void UpdateWeapon(int index, int cat,int id,bool isShow = true)
        {
            _viewModel.SetWeaponGroupShow(index,isShow);
            if (!isShow)
            {
                ClearWeaponPartSlot(index);
                return;
            }
            var config = GetConfig(cat, id);
            _viewModel.SetWeaponIconBundle(index,config.IconBundle);
            _viewModel.SetWeaponIconAsset(index,config.Icon);
            _viewModel.SetWeaponNameText(index, config.Name);
            UpdateWeaponPartSlot(index, id);      
        }

        private void ClearWeaponPartSlot(int index)
        {
            _viewModel.SetLowerRailSlotShow(index, false);
            _viewModel.SetMagazineSlotShow(index, false);
            _viewModel.SetMuzzleSlotShow(index, false);
            _viewModel.SetStockSlotShow(index, false);
            _viewModel.SetUpperRailSlotShow(index, false);
        }

        private void UpdateWeaponPartSlot(int index, int id)
        {
            if (index > 3 || index < 1) return; 
            List<EWeaponPartType> list = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(id).ApplyPartsSlot;
            var differentSet =  weaponPartTypeList.Except(list);
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
            foreach (EWeaponPartType type in differentSet)
            {
                switch (type)
                {
                    case EWeaponPartType.LowerRail: _viewModel.SetLowerRailSlotShow(index, false); break;
                    case EWeaponPartType.Muzzle: _viewModel.SetMuzzleSlotShow(index, false); break;
                    case EWeaponPartType.Magazine: _viewModel.SetMagazineSlotShow(index, false); break;
                    case EWeaponPartType.UpperRail: _viewModel.SetUpperRailSlotShow(index, false); break;
                    case EWeaponPartType.Stock: _viewModel.SetStockSlotShow(index, false); break;
                }
            }
        }

        private ItemBaseConfig GetConfig(int cat,int id)
        {
            return SingletonManager.Get<ItemBaseConfigManager>().GetConfigById(cat, id,true);
        }
        private ItemBaseConfig GetWeaponPartConfig(int cat, int id)
        {
            return GetConfig(cat, id);
        }

      
        protected override void OnCanvasEnabledUpdate(bool enable)
        {
            base.OnCanvasEnabledUpdate(enable);

            if (enable && !_haveRegister)
            {
                RegisterKeyReceiver();
            }
            else if (!enable && _haveRegister)
            {
                UnRegisterKeyReceiver();
                tipManager.HideShowTip();
            }

            if (!enable)
            {
                HideUIDrag();
            }

        }

        private void HideUIDrag()
        {
            for (int i = 0; i < groundScrollRect.Count(); i++)
            {
                (groundScrollRect[i] as ChickenBagItem).CloseUIDrag();
            }
            for (int i = 0; i < bagScrollRect.Count(); i++)
            {
                (bagScrollRect[i] as ChickenBagItem).CloseUIDrag();
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

