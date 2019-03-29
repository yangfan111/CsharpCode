using System.Collections.Generic;
using App.Shared.CommonResource.Base;
using Core.CommonResource;
using Core.EntityComponent;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.CommonResource
{
    public abstract class AbstractCommonResourceLoadSystem<TEntity> : ICommonResourceUpdate<TEntity> where TEntity : class, IEntity
    {
        private readonly IGroup<TEntity> _initializedGroup;
        private readonly Queue<RemoveAssetStatus> _oldResource;
        private readonly IGroup<TEntity> _uninitializedGroup;
        private readonly AssetLoadOption _assetLoadOption = new AssetLoadOption(true, null, 0);
        private readonly IContexts _contexts;
        private IUnityAssetManager _loadRequestManager;

        protected AbstractCommonResourceLoadSystem(IContexts contexts, Queue<RemoveAssetStatus> oldResource)
        {
            _oldResource = oldResource;
            _initializedGroup = GetInitializedGroup(contexts);
            _uninitializedGroup = GetUninitializedGroup(contexts);
            _contexts = contexts;
        }

        public void Update(IUnityAssetManager assetManager)
        {
            _loadRequestManager = assetManager;
            ProcessInitializedEntitys(assetManager);
            ProcessUnInitializedEntitys(assetManager);

            ProcessOldResources(_oldResource, assetManager);
        }

        public void LoadResource(TEntity entity, int index, AssetInfo assetInfo, bool forceReload)
        {
            var time = (int) (Time.time * 1000);
            AssertUtility.Assert(index < GetActionsLength());
            var comp = GetCommonResource(entity);
            var old = LoadResource(comp, index, assetInfo, time, forceReload);

            PushToOldResource(old, GetActions(index));
        }


        public void UnloadResource(TEntity entity, int index)
        {
            AssertUtility.Assert(index < GetActionsLength());
            var comp = GetCommonResource(entity);
            var old = UnloadResource(comp, index);

            PushToOldResource(old, GetActions(index));
        }

        private void ProcessUnInitializedEntitys(IUnityAssetManager assetManager)
        {
            var entities = _initializedGroup.GetEntities();
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                InitComponent(entity, _contexts);
            }
        }

        private void ProcessInitializedEntitys(IUnityAssetManager assetManager)
        {
            var entities = _initializedGroup.GetEntities();
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var res = GetCommonResource(entity);
                if (res != null)
                    if (res.Resources != null)
                        ProcessCommonResources(entity, res, assetManager);
            }
        }

        protected abstract IGroup<TEntity> GetInitializedGroup(IContexts contexts);
        protected abstract IGroup<TEntity> GetUninitializedGroup(IContexts contexts);
        protected abstract ICommonResourceComponent GetCommonResource(TEntity entity);
        protected abstract TEntity GetEntity(EntityKey key);
        protected abstract EntityKey GetEntityKey(TEntity entity);
        protected abstract ICommonResourceActions GetActions(int index);
        protected abstract int GetActionsLength();
        protected abstract void InitComponent(TEntity playerEntity, IContexts contexts);

        private static AssetStatus LoadResource(ICommonResourceComponent comp, int index, AssetInfo assetInfo, int time, bool forceReload = false)
        {
            AssertUtility.Assert(index < comp.Resources.Length);
            var oldStatus = comp.Resources[index];
            if (oldStatus.AssetInfo.Equals(assetInfo) && !forceReload) return null;

            var newStatus = comp.Resources[index] = ObjectAllocatorHolder<AssetStatus>.Allocate();
            newStatus.LastRequestTime = time;
            newStatus.Status = EAssetLoadStatus.UnLoad;
            newStatus.Object = null;
            newStatus.AssetInfo = assetInfo;
            newStatus.ResIndex = index;
            return oldStatus;
        }

        public static AssetStatus UnloadResource(ICommonResourceComponent comp, int index)
        {
            AssertUtility.Assert(index < comp.Resources.Length);
            var oldStatus = comp.Resources[index];
            if (oldStatus.Status == EAssetLoadStatus.None) return null;

            var newStatus = comp.Resources[index] = ObjectAllocatorHolder<AssetStatus>.Allocate();
            newStatus.LastRequestTime = 0;
            newStatus.Status = EAssetLoadStatus.None;
            newStatus.Object = null;
            newStatus.AssetInfo = AssetInfo.EmptyInstance;
            newStatus.ResIndex = index;
            return oldStatus;
        }

        protected void PushToOldResource(AssetStatus old, ICommonResourceActions action)
        {
            if (old != null)
                _oldResource.Enqueue(RemoveAssetStatus.Allocate(old, action));
        }

        private static void ProcessOldResources(Queue<RemoveAssetStatus> resOldResource, IUnityAssetManager assetManager)
        {
            while (resOldResource.Count > 0)
            {
                var removeAssetStatus = resOldResource.Dequeue();
                var res = removeAssetStatus.Status;

                if (res.Status == EAssetLoadStatus.Loaded)
                {
                    var actions = removeAssetStatus.Actions;
                    actions.Recycle(res);
                    assetManager.Recycle(res.Object);
                }
                else if (res.Status == EAssetLoadStatus.UnInit)
                {
                    assetManager.Recycle(res.Object);
                }

                res.ReleaseReference();
            }
        }


        private void ProcessCommonResources(TEntity entity, ICommonResourceComponent res, IUnityAssetManager loadRequestManager)
        {
            var time = (int) (Time.time * 1000);
            var resourceLength = res.Resources.Length;
            for (var i = 0; i < resourceLength; i++)
            {
                var asset = res.Resources[i];
                var actions = GetActions(i);
                if (asset == null || actions == null) continue;
                if (asset.Status == EAssetLoadStatus.UnLoad)
                {
                    asset.Status = EAssetLoadStatus.Loading;
                    asset.LastRequestTime = time;
                    loadRequestManager.LoadAssetAsync(
                        new AssetRequestSource(GetEntityKey(entity), res.GetComponentId(), asset.ResIndex, asset.LastRequestTime),
                        asset.AssetInfo, OnLoadSucc, _assetLoadOption);
                }
                else if (asset.Status == EAssetLoadStatus.UnInit)
                {
                    if (actions.CanInit(entity, asset))
                    {
                        actions.Init(entity, asset);
                        asset.Status = EAssetLoadStatus.Loaded;
                    }
                }
            }
        }

        private ICommonResourceComponent GetComponent(AssetRequestSource assetRequestSource)
        {
            var entity = GetEntity(assetRequestSource.EntityKey);
            if (entity != null && entity.HasComponent(assetRequestSource.ComponentId)) return entity.GetComponent(assetRequestSource.ComponentId) as ICommonResourceComponent;

            return null;
        }

        private void OnLoadSucc(AssetRequestSource assetRequestSource, UnityObject u)
        {
            var obj = u.AsGameObject;
            var isSourceExist = false;


            var comp = GetComponent(assetRequestSource);
            var entity = GetEntity(assetRequestSource.EntityKey);
            if (comp != null)
            {
                var assetStatus = comp.Resources[assetRequestSource.ResIndex];
                if (assetStatus.LastRequestTime == assetRequestSource.TimeLine)
                {
                    isSourceExist = true;
                    var actions = GetActions(assetRequestSource.ResIndex);
                    if (obj == null)
                    {
                        actions.OnLoadFailed(entity, assetStatus);
                        assetStatus.Status = EAssetLoadStatus.Failed;
                    }
                    else
                    {
                        assetStatus.Object = u;

                        if (actions.CanInit(entity, assetStatus))
                        {
                            actions.Init(entity, assetStatus);
                            assetStatus.Status = EAssetLoadStatus.Loaded;
                        }
                        else
                        {
                            assetStatus.Status = EAssetLoadStatus.UnInit;
                        }
                    }
                }
            }

            if (!isSourceExist) _loadRequestManager.Recycle(u);
        }
    }
}