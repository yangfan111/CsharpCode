using App.Shared;
using Assets.XmlConfig;
using Core.GameModule.System;
using Entitas;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.ClientGameModules.Throwing
{
    public class ThrowingEntityInitSytem : ReactiveResourceLoadSystem<ThrowingEntity>
    {
        public ThrowingEntityInitSytem(IContext<ThrowingEntity> context) : base(context)
        {

        }

        protected override ICollector<ThrowingEntity> GetTrigger(IContext<ThrowingEntity> context)
        {
            return context.CreateCollector(ThrowingMatcher.Position.Added());
        }

        protected override bool Filter(ThrowingEntity entity)
        {
            return entity.hasEntityKey;
        }

        public override void SingleExecute(ThrowingEntity entity)
        {
            AssetManager.LoadAssetAsync(entity, AssetConfig.GetThrowingAssetInfo((EWeaponSubType)entity.throwingData.WeaponSubType), OnLoadSucc);
        }

        public void OnLoadSucc(ThrowingEntity throwing, UnityObject unityObj)
        {
            if (null != throwing)
            {
                var go = unityObj.AsGameObject;
                go.SetActive(false);

                var renders = go.GetComponentsInChildren<MeshRenderer>();
                foreach (var rend in renders)
                {
                    rend.enabled = true;
                }

                throwing.AddThrowingGameObject(unityObj);
            }
        }
    }
}