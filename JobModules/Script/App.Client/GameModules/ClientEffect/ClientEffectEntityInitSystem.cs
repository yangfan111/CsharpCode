using System.Collections.Generic;
using App.Shared.Components.ClientEffect;
using Utils.AssetManager;
using Core.GameModule.System;
using Core.IFactory;
using Core.Utils;
using Entitas;
using Object = UnityEngine.Object;
using UnityEngine;
using App.Client.ClientSystems;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectEntityInitSystem : ReactiveResourceLoadSystem<ClientEffectEntity>
    {
        private LoggerAdapter Logger = new LoggerAdapter(typeof(ClientEffectEntityInitSystem));
        private Contexts _contexts;
        private ISoundEntityFactory _soundEntityFactory;

        public ClientEffectEntityInitSystem(Contexts contexts) : base(contexts.clientEffect)
        {
            _contexts = contexts;
            _soundEntityFactory = contexts.session.entityFactoryObject.SoundEntityFactory;
        }

        protected override ICollector<ClientEffectEntity> GetTrigger(IContext<ClientEffectEntity> context)
        {
            return context.CreateCollector(ClientEffectMatcher.EffectType.Added());
        }

        protected override bool Filter(ClientEffectEntity entity)
        {
            return entity.hasEffectType;
        }

        public override void SingleExecute(ClientEffectEntity entity)
        {
            IEffectLogic effectLogic;
            if (entity.hasEffectId)
            {
                effectLogic =
                    ClientEffectLogicFactory.CreateEffectLogic(entity.effectType.Value, _contexts,
                        entity.effectId.Value);
            }
            else if (entity.hasSprayPaint)
            {
                /*喷漆*/
                Vector3 position = entity.sprayPaint.SprayPaintPos;
                Vector3 forward = entity.sprayPaint.SprayPaintForward;
                Vector3 debugSize = entity.sprayPaint.SprayPrintSize;
                PlayerSprayPaintUtility.CreateSprayPaint(_contexts, debugSize, position, forward);
                return;
            }
            else
            {
                effectLogic = ClientEffectLogicFactory.CreateEffectLogic(entity.effectType.Value, _contexts);
            }

            if (null == effectLogic)
            {
                Logger.ErrorFormat("Effect Logic of {0} is null !", entity.effectType.Value);
                return;
            }
           
            entity.AddLogic(effectLogic);
            //entity.AddAssets(false, false);

            if (null == effectLogic.AssetInfos)
            {
                Logger.ErrorFormat("AssetInfos for Logic of {0} is null !", entity.effectType.Value);
                return;
            }

            BatchLoadHandler handler = new BatchLoadHandler(entity, effectLogic.AssetInfos);
            handler.Load(AssetManager);
            if (effectLogic.SoundId > 0)
            {
                if(entity.hasPosition)
                {
//                    _soundEntityFactory.CreateSelfOnlyMoveSound(entity.position.Value, entity.entityKey.Value,
//                        effectLogic.SoundId, false);
                }
                else
                {
                    Logger.ErrorFormat("Entity {0} has no position component ", entity.entityKey.Value);
                }
            }
        }
    }
}