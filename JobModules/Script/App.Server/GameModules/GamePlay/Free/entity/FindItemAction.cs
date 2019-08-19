using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.chicken;
using App.Server.GameModules.GamePlay.Free.item.config;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.item;
using com.wd.free.para;
using Core.EntityComponent;
using Core.Free;
using gameplay.gamerule.free.item;
using System;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class FindItemAction : AbstractPlayerAction, IRule
    {
        private int radius;

        [NonSerialized] private FreeRuleEventArgs fr;
        [NonSerialized] private PlayerEntity _playerEntity;
        [NonSerialized] private ItemInventory _ground;

        public override void DoAction(IEventArgs args)
        {
            fr = (FreeRuleEventArgs) args;
            PlayerEntity playerEntity = (PlayerEntity) fr.GetEntity(player);

            _playerEntity = playerEntity;

            if (playerEntity != null)
            {
                FreeData fd = (FreeData) playerEntity.freeData.FreeData;
                _ground = fd.freeInventory.GetInventoryManager().GetInventory(ChickenConstant.BagGround);
                _ground.Clear();

                var bin2Ds = args.GameContext.session.serverSessionObjects.Bin2dManager.GetBin2Ds();
                foreach (var bin2DState in bin2Ds)
                {
                    bin2DState.Bin2D.Retrieve(
                        new Rect(playerEntity.position.Value.x - 100, playerEntity.position.Value.z - 100, 200, 200),
                        onFind);
                }


                _ground.GetInventoryUI().ReDraw(fr, _ground, true);
            }
        }

        private void onFind(IGameEntity localEntity)
        {
            if (localEntity.EntityType != 6)
            {
                return;
            }

            SceneObjectEntity entity = fr.GameContext.sceneObject.GetEntityWithEntityKey(localEntity.EntityKey);
            if (entity != null && entity.hasSimpleItem && entity.simpleItem != null)
            {
                if (IsNear(entity.position.Value, _playerEntity.position.Value))
                {
                    CreateItemToPlayerAction action = new CreateItemToPlayerAction();
                    action.key = FreeItemConfig.GetItemKey(entity.simpleItem.Category, entity.simpleItem.Id);
                    action.name = ChickenConstant.BagGround;

                    if (!string.IsNullOrEmpty(action.key))
                    {
                        action.count = entity.simpleItem.Count.ToString();
                        action.SetPlayer("current");
                        fr.TempUse("current", (FreeData) _playerEntity.freeData.FreeData);

                        action.Act(fr);

                        if (_ground.posList.Count > 0)
                        {
                            ItemPosition lastItem = _ground.posList[_ground.posList.Count - 1];
                            lastItem.GetKey().GetParameters()
                                .AddPara(new IntPara("entityId", entity.entityKey.Value.EntityId));
                        }

                        fr.Resume("current");
                    }
                }
            }
        }

        private bool IsNear(Vector3 v1, Vector3 v2)
        {
            return Math.Abs(v1.x - v2.x) < radius && Math.Abs(v1.z - v2.z) < radius;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.FindItemAction;
        }
    }
}