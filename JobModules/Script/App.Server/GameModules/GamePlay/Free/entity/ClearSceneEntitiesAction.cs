using App.Protobuf;
using App.Shared;
using com.wd.free.action;
using com.wd.free.@event;
using System;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class ClearSceneEntitiesAction : AbstractGameAction
    {
        public override void DoAction(IEventArgs args)
        {

            var entities = args.GameContext.sceneObject.GetEntities();
            foreach(var entity in entities)
            {
                entity.isFlagDestroy = true;
            }

            var mapEntities = args.GameContext.mapObject.GetEntities();
            var currentTimer = args.GameContext.session.currentTimeObject;
            foreach (var entity in mapEntities)
            {
                if (entity.hasReset)
                {
                    entity.reset.Reset(entity);
                    entity.flagImmutability.LastModifyServerTime = currentTimer.CurrentTime;
                }
                else
                {
                    entity.isFlagDestroy = true;
                }
            }

            var clientEffectEntities = args.GameContext.clientEffect.GetEntities();
            foreach(var clientEffectEntity in clientEffectEntities)
            {
                clientEffectEntity.isFlagDestroy = true;
            }

            var throwingEntities = args.GameContext.throwing.GetEntities();
            foreach(var throwingEntity in throwingEntities)
            {
                throwingEntity.isFlagDestroy = true; 
            }

            var message = ClearSceneMessage.Allocate();
            var playerEntitiess = args.GameContext.player.GetEntities();
            foreach(var playerEntity in playerEntitiess)
            {
                playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.ClearScene, message);
            }
            message.ReleaseReference();
        }
    }
}
