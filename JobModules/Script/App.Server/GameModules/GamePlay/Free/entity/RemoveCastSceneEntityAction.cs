using System;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [System.Serializable]
    public class RemoveCastSceneEntityAction : AbstractGameAction, IRule
    {
        private int key;

        public override void DoAction(IEventArgs args)
        {
            var contexts = args.GameContext;
            var factory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            factory.FreeCastEntityToDestoryList.Add(key);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.RemoveCastSceneEntityAction;
        }
    }
}
