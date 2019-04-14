using App.Shared;
using App.Client.GameModules.Ui.UiAdapter;
using Core.Enums;
using Core.GameModule.System;
using Entitas;
using UnityEngine;
using Core.IFactory;
using Utils.AssetManager;

namespace App.Client.ClientGameModules.Bullet
{
    public class BulletEntityInitSytem : ReactiveResourceLoadSystem<BulletEntity>
    {
        private Contexts _contexts;
        private ISoundEntityFactory _soundEntityFactory;

        public BulletEntityInitSytem(Contexts contexts) : base(contexts.bullet)
        {
            _contexts = contexts;
            _soundEntityFactory = contexts.session.entityFactoryObject.SoundEntityFactory;
        }

        protected override ICollector<BulletEntity> GetTrigger(IContext<BulletEntity> context)
        {
            return context.CreateCollector(BulletMatcher.Position.Added());
        }

        protected override bool Filter(BulletEntity entity)
        {
            return entity.hasEntityKey;
        }

        public override void SingleExecute(BulletEntity entity)
        {
            AssetManager.LoadAssetAsync(entity, AssetConfig.GetBulletAssetInfo(EBulletType.Light), OnLoadSucc);
        }

        public void OnLoadSucc(BulletEntity bullet, UnityObject unityObj)
        {
            if(null != bullet)
            {
                bullet.AddBulletGameObject(unityObj);
                GameAudioMedia.PlayBulletFlyAudio(unityObj.AsGameObject);
            }
        }
    }
}