using System.Collections.Generic;
using Utils.AssetManager;
using Core.GameModule.System;
using Core.GameTime;
using Core.Sound;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.Sound
{
    public class SoundResourcesLoadSystem : ReactiveResourceLoadSystem<SoundEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundResourcesLoadSystem));
        private ISoundPlayer _soundPlayer;
        private ITimeManager _timeManager;
        private PlayerContext _playerContext; 
        private BulletContext _bulletContext;
        private SoundParentController _soundParentController;

        public SoundResourcesLoadSystem(Contexts contexts, SoundParentController soundParentController) :base(contexts.sound)
        {
            _soundPlayer = contexts.session.clientSessionObjects.SoundPlayer;
            _timeManager = contexts.session.clientSessionObjects.TimeManager;
            _playerContext = contexts.player;
            _bulletContext = contexts.bullet;
            _soundParentController = soundParentController;
        }

        protected override ICollector<SoundEntity> GetTrigger(IContext<SoundEntity> context)
        {
            return context.CreateCollector(SoundMatcher.AssetInfo.Added());
        }

        protected override bool Filter(SoundEntity entity)
        {
            return true;
        }

        public override void SingleExecute(SoundEntity entity)
        {
            AssetManager.LoadAssetAsync(entity, new AssetInfo(entity.assetInfo.BundleName, entity.assetInfo.AssetName), OnLoadSucc);
        }

        public void OnLoadSucc(SoundEntity entity, UnityObject unityObject)
        {
            if (null == entity)
            {
                Logger.Error("Entity is null or not sound entity!");
                return;
            }
            var go = unityObject.AsGameObject;
            if (null != go)
            {
                var audioSource = go.GetComponent<AudioSource>();
               // go.name = entity.entityKey.ToString();
                entity.AddUnityObj(audioSource, unityObject);
                var key = _soundPlayer.Preload(audioSource);
                if (!entity.hasAudioSourceKey)
                {
                    entity.AddAudioSourceKey(key);
                }
                else
                {
                    entity.audioSourceKey.Value = key;
                }
                if(entity.hasParent)
                {
                    _soundParentController.AttachParent(entity);
                }
            }
            else
            {
                Logger.Error("Asset load error : null or type missmatch, GameObject needed");
            }
        }
    }
}
