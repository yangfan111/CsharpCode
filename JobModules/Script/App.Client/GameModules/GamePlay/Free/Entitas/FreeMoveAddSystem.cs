using System;
using Assets.Sources.Free;
using Assets.Sources.Free.Effect;
using Core.Free;
using Core.GameModule.System;
using Entitas;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class FreeMoveAddSystem : ReactiveResourceLoadSystem<FreeMoveEntity>
    {

        public FreeMoveAddSystem(FreeMoveContext sceneObjectContext) : base(sceneObjectContext)
        {

        }

        protected override ICollector<FreeMoveEntity> GetTrigger(IContext<FreeMoveEntity> context)
        {
            return context.CreateCollector(FreeMoveMatcher.FreeData.Added(), FreeMoveMatcher.Position.Added());
        }

        protected override bool Filter(FreeMoveEntity entity)
        {
            return entity.hasPosition && entity.hasFreeData;
        }

        public override void SingleExecute(FreeMoveEntity freeMove)
        {
            if(freeMove.freeData.Key == "bomb") freeMove.AddBombSound(DateTime.Now.Ticks / 10000L, 0);
            SimpleProto data = SingletonManager.Get<FreeEffectManager>().GetEffectData(freeMove.freeData.Key);
            if (data != null)
            {
                data.Ss[0] = "freemove_" + freeMove.entityKey.Value.EntityId;
                SimpleMessageManager.Instance.DoHandle(FreeMessageConstant.MSG_EFFECT_CREATE, data);

                FreeRenderObject effect = SingletonManager.Get<FreeEffectManager>().GetEffect(data.Ss[0]);
                if (effect != null)
                {
                    effect.Move(freeMove.position.Value.x, freeMove.position.Value.y, freeMove.position.Value.z);
                }
            }
        }

       
    }
}
