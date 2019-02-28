using App.Client.CastObjectUtil;
using App.Shared.GameModules.GamePlay.Free;
using Utils.AssetManager;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using UnityEngine;

namespace App.Client.GameModules.SceneObject
{
    public class FreeObjectLoadSystem : ReactiveResourceLoadSystem<FreeMoveEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FreeObjectLoadSystem));

        public FreeObjectLoadSystem(FreeMoveContext freeMove) : base(freeMove)
        {
        }

        public void OnLoadSucc(FreeMoveEntity freeEntity, UnityObject unityObj)
        {
            if (null == freeEntity)
            {
                Logger.Error("source is not free move entity ");
                return;
            }
            var go = unityObj.AsGameObject;
            if (null == go)
            {
                Logger.Error("model is null");
                return;
            }

            var target = FreeObjectGoAssemble.Assemble(unityObj, freeEntity);
            freeEntity.AddUnityGameObject(unityObj);


            var colGo = go.transform.Find(SceneObjectConstant.NormalColliderName);
            if (null != colGo)
            {
                colGo.GetComponent<Collider>().enabled = true;
            }
            else
            {
                Logger.ErrorFormat("no normal collider in {0}", go.name);
            }

            if (null != target)
            {
                if (freeEntity.hasEntityKey)
                {
                    FreeObjectCastData.Make(target, freeEntity.entityKey.Value.EntityId);
                }
                else
                {
                    Logger.Error("free entity has no entity key");
                }
            }
            else
            {
                Logger.Error("target from FreeObjectGoAssemble is null");
            }
        }

        public override void SingleExecute(FreeMoveEntity entity)
        {
            if (entity.freeData.Cat == FreeEntityConstant.DropBoxGroup)
            {
                AssetManager.LoadAssetAsync(entity, new AssetInfo("item", "I005"), OnLoadSucc);
            }
            else if (entity.freeData.Cat == FreeEntityConstant.DeadBoxGroup)
            {
                AssetManager.LoadAssetAsync(entity, new AssetInfo("item", "I006"), OnLoadSucc);
            }
        }


        protected override ICollector<FreeMoveEntity> GetTrigger(IContext<FreeMoveEntity> context)
        {
            return context.CreateCollector(FreeMoveMatcher.FreeData.Added(), FreeMoveMatcher.Position.Added());
        }

        protected override bool Filter(FreeMoveEntity entity)
        {
            return entity.hasPosition && entity.hasFreeData;
        }
    }
}