using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Shared.Scripts;
using XmlConfig;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IChickenBagUiAdapter : IAbstractUiAdapter
    {
        int GetWeaponIdBySlotIndex(int index);
        int GetWeaponPartIdBySlotIndexAndWeaponPartType(int index,EWeaponPartType type);
        List<IBaseChickenBagItemData> BagItemDataList { get; }
        List<IChickenBagItemUiData> GroundItemDataList { get;}
        int GetEquipmentIdByWardrobeType(Wardrobe type, out int count);
        bool RefreshGround();
        void SendRightClickUseItem(string key);
        void SetCrossVisible(bool isVisible);
        void SendDragItem(string beginKey, string endKey);
        void SendSplitItem(string splitKey);
        int CurWeight { get; }
        int TotalWeight { get; }
    }
}
