using System;
using App.Shared.Components.Ui;
using System.Collections.Generic;
using App.Client.CastObjectUtil;
using App.Client.GameModules.GamePlay.Free.Auto.Prefab;
using App.Shared;
using App.Shared.GameModules.GamePlay.Free;
using App.Shared.GameModules.GamePlay.Free.Map;
using Assets.XmlConfig;
using Shared.Scripts;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;
using App.Client.GameModules.GamePlay.Free.App;
using App.Client.GameModules.Ui.Models.Chicken;
using App.Shared.GameModules.Player;
using Core.Utils;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public enum EChickenBagType
    {
        None,
        Ground,
        Bag,
        Equipment,
        Weapon,
        WeaponPart
    }

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


    public class ChickenBagUiAdapter : UIAdapter, IChickenBagUiAdapter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ChickenBagUiAdapter));

        private Contexts _contexts;

        public ChickenBagUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }

        public int GetWeaponIdBySlotIndex(int index)
        {
            var list = _contexts.ui.uI.WeaponIdList;
            if (index < 0 || index >= list.Length)
            {
                Logger.Error("error index" + index);
                return 0;
            }
            return list[index];
        }

        private List<int> weaponPartsIdList = new List<int>();
        public List<int> GetWeaponPartIdsBySlotIndex(int index)
        {
            var list = _contexts.ui.uI.WeaponPartList;
            int len = list.GetLength(1);
            weaponPartsIdList.Clear();
            for (int i = 0; i < len; i++)
            {
                weaponPartsIdList.Add(list[index, i]);
            }

            return weaponPartsIdList;

        }

        public int GetWeaponPartIdBySlotIndexAndWeaponPartType(int index, EWeaponPartType type)
        {
            var list = _contexts.ui.uI.WeaponPartList;
            return list[index, (int)type];
        }

        public int GetEquipmentIdByWardrobeType(Wardrobe type, out int count)
        {
            var list = _contexts.ui.uI.EquipIdList;
            var pair = list[(int)type];
            count = pair.Value;
            return pair.Key;
        }

        #region refreshGround
        HashSet<int> current = new HashSet<int>();

        private List<IChickenBagItemUiData> _groundItemDataList = new List<IChickenBagItemUiData>();
        public List<IChickenBagItemUiData> GroundItemDataList
        {
            get { return _groundItemDataList; }
        }

        private int radius = 3;
        private bool IsNear(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.x - v2.x) < radius && Math.Abs(v1.z - v2.z) < radius && Math.Abs(v1.y - v2.y) < 2;
        }

        private bool HasNoObstacle(FreeMoveEntity item, PlayerEntity player)
        {
            if (item.hasUnityGameObject)
            {
                var noObstacle = !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.unityGameObject.UnityObject);
                return noObstacle;
            }
            else
            {
                var noObstacle = !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, null);
                return noObstacle;
            }
        }

        private bool HasNoObstacle(SceneObjectEntity item, PlayerEntity player)
        {
            if (item.hasUnityObject)
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.unityObject.UnityObject);
            }
            else if (item.hasMultiUnityObject)
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, item.multiUnityObject.FirstAsset);
            }
            else
            {
                return !CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, item.position.Value, null);
            }
        }
        private HashSet<int> groundEntitySet = new HashSet<int>();

        public bool RefreshGround()
        {
            HashSet<int> current = new HashSet<int>();
            foreach (var item in _contexts.sceneObject.GetEntities())
            {
                if (item.hasPosition
                    && IsNear(item.position.Value, Player.position.Value)
                    && item.hasSimpleItem && item.simpleItem.Category > 0
                    && HasNoObstacle(item, Player))
                {
                    current.Add(item.entityKey.Value.EntityId);
                }
            }

            foreach (var item in _contexts.freeMove.GetEntities())
            {
                if (item.hasPosition
                    && item.hasFreeData
                    && IsNear(item.position.Value, Player.position.Value)
                    && HasNoObstacle(item, Player))
                {
                    current.Add(item.entityKey.Value.EntityId);
                }
            }

            if (current.SetEquals(groundEntitySet))
            {
                return false;
            }

            List<SceneObjectEntity> list = new List<SceneObjectEntity>();
            Dictionary<string, List<FreeMoveEntity>> deadList = new Dictionary<string, List<FreeMoveEntity>>();
            Dictionary<string, List<FreeMoveEntity>> dropList = new Dictionary<string, List<FreeMoveEntity>>();

            foreach (var id in current)
            {
                var sceneEntity =
                    _contexts.sceneObject.GetEntityWithEntityKey(
                        new Core.EntityComponent.EntityKey(id, (short)EEntityType.SceneObject));
                if (null != sceneEntity)
                {
                    list.Add(sceneEntity);
                    continue;
                }

                var freeMoveEntity =
                    _contexts.freeMove.GetEntityWithEntityKey(
                        new Core.EntityComponent.EntityKey(id, (short)EEntityType.FreeMove));
                {
                    if (freeMoveEntity.freeData.Cat == FreeEntityConstant.DeadBox)
                    {
                        if (!deadList.ContainsKey(freeMoveEntity.freeData.Key))
                        {
                            deadList.Add(freeMoveEntity.freeData.Key, new List<FreeMoveEntity>());
                        }

                        deadList[freeMoveEntity.freeData.Key].Add(freeMoveEntity);
                    }

                    if (freeMoveEntity.freeData.Cat == FreeEntityConstant.DropBox)
                    {
                        if (!dropList.ContainsKey(freeMoveEntity.freeData.Key))
                        {
                            dropList.Add(freeMoveEntity.freeData.Key, new List<FreeMoveEntity>());
                        }

                        dropList[freeMoveEntity.freeData.Key].Add(freeMoveEntity);
                    }
                }
            }

            _groundItemDataList.Clear();
            FillBox(dropList, false);
            FillBox(deadList, true);
            list.Sort(new SceneObjectSorter());
            if (list.Count > 0)
            {
                var titleData = new ChickenBagItemUiData { isBagTitle = true, title = "地面" };
                _groundItemDataList.Add(titleData);

            }
            foreach (var item in list)
            {
                var data = new ChickenBagItemUiData
                {
                    cat = item.simpleItem.Category,
                    id = item.simpleItem.Id,
                    count = item.simpleItem.Category == (int)ECategory.Weapon ? 1:
                        item.simpleItem.Count,
                    key = "1|" + item.entityKey.Value.EntityId
                };
                _groundItemDataList.Add(data);

            }
            groundEntitySet = current;

            return true;
        }


        private void FillBox(Dictionary<string, List<FreeMoveEntity>> dic, bool dead)
        {
            int index = 0;
            foreach (var name in dic.Keys)
            {
                var titleData = new ChickenBagItemUiData { isBagTitle = true, title = name };
                _groundItemDataList.Add(titleData);
                List<SimpleItemInfo> infos = new List<SimpleItemInfo>();
                foreach (var item in dic[name])
                {
                    object obj = SingletonManager.Get<DeadBoxParser>().FromString(item.freeData.Value);
                    if (obj == null)
                    {
                        continue;
                    }

                    SimpleItemInfo info = (SimpleItemInfo)obj;
                    info.entityId = item.entityKey.Value.EntityId;

                    infos.Add(info);
                }

                infos.Sort(new ItemInfoSorter());

                foreach (SimpleItemInfo info in infos)
                {
                    int count = info.count;

                    var data = new ChickenBagItemUiData
                    {
                        cat = info.cat,
                        id = info.id,
                        count = info.cat == (int)ECategory.Weapon ? 1 :
                            count,
                        key = "1|" + info.entityId
                    };
                    _groundItemDataList.Add(data);
                }
            }
        }
        #endregion

        #region keyEvent

        public void SendRightClickUseItem(string key)
        {
            Debug.Log("SendRightClickUseItem" + key);
            ChickenBagUtil.ClickItem(key, true);
        }

        public void SendDragItem(string beginKey, string endKey)
        {
            Debug.Log("SendDragItem beginKey:" + beginKey + "endKey: " + endKey);
            ChickenBagUtil.DragItem(beginKey, endKey);
        }

        public void SendSplitItem(string splitKey)
        {
            Debug.Log("SendSplitItem" + splitKey);
            ChickenBagUtil.ClickItem(splitKey, true);
        }

        #endregion

        public int HoldWeaponSlotIndex
        {
            get { return _contexts.ui.uI.HoldWeaponSlotIndex; }
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }

        public int CurWeight
        {
            get { return _contexts.ui.uI.CurBagWeight; }
        }

        public int TotalWeight
        {
            get { return _contexts.ui.uI.TotalBagWeight; }
        }

        public List<IBaseChickenBagItemData> BagItemDataList
        {
            get { return _contexts.ui.uI.ChickenBagItemDataList; }
        }


        private PlayerEntity _player;
        public PlayerEntity Player
        {
            get { return _player ?? (_player = _contexts.player.flagSelfEntity); }
        }

        public override bool Enable
        {
            set
            {
                base.Enable = value;
                if (value)
                    PlayerStateUtil.AddUIState(EPlayerUIState.BagOpen, Player.gamePlay);
                else
                    PlayerStateUtil.RemoveUIState(EPlayerUIState.BagOpen, Player.gamePlay);
            }
            get { return base.Enable && !IsDead(); }
        }

        private bool IsDead()
        {
            return Player.gamePlay.IsDead();
        }
    }

}
