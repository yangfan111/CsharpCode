using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Shared.Scripts;
using XmlConfig;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public interface IChickenBagUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 返回武器槽索引对应的武器Id
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetWeaponIdBySlotIndex(int index);
        /// <summary>
        /// 返回武器槽索引和武器配件槽类型对应的武器配件Id
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetWeaponPartIdBySlotIndexAndWeaponPartType(int index,EWeaponPartType type);
        /// <summary>
        /// 返回背包中的道具数据
        /// </summary>
        List<IBaseChickenBagItemData> BagItemDataList { get; }
        /// <summary>
        /// 返回地面的道具数据
        /// </summary>
        List<IChickenBagItemUiData> GroundItemDataList { get;}
        /// <summary>
        /// 返回装备类型对应的装备Id,空槽位则返回0
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int GetEquipmentIdByWardrobeType(Wardrobe type, out int count);
        /// <summary>
        /// 刷新地面物品
        /// </summary>
        /// <returns></returns>
        bool RefreshGround();
        /// <summary>
        /// 发送右键使用道具事件,key结构{(EChickenBagType|index),
        /// index对于不同的EChickenBagType有不同的含义,
        /// Ground:EntityId,Bag:templateId,
        /// Equipment:Wardrobe,如3|7表示防弹衣槽位
        /// Weapon:武器槽索引(1-5,0表示整个武器面板),如4|2表示2号主武器，4|0表示武器面板
        /// WeaponPart:武器槽索引*10+EWeqponPartType,如5|31表示3号副武器的Magazine弹夹
        /// </summary>
        /// <param name="key"></param>
        void SendRightClickUseItem(string key);
        /// <summary>
        /// 发送拖拽道具事件
        /// </summary>
        /// <param name="beginKey"></param>
        /// <param name="endKey"></param>
        void SendDragItem(string beginKey, string endKey);
        /// <summary>
        /// 发送拆分道具事件
        /// </summary>
        /// <param name="splitKey"></param>
        void SendSplitItem(string splitKey);
        /// <summary>
        /// 背包中当前道具重量和
        /// </summary>
        int CurWeight { get; }
        /// <summary>
        /// 背包的最大重量
        /// </summary>
        int TotalWeight { get; }
        /// <summary>
        /// 当前持有的武器槽索引
        /// </summary>
        int HoldWeaponSlotIndex { get; }

        void SetCrossVisible(bool isVisible);

    }
}
