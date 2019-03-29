using App.Client.GameModules.Player;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.Throwing
{
    public class ThrowingRenderSystem : AbstractRenderSystem<ThrowingEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingRenderSystem));
        private int _maxId;


        public ThrowingRenderSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<ThrowingEntity> GetIGroup(Contexts contexts)
        {
            return contexts.throwing.GetGroup(ThrowingMatcher.AllOf(ThrowingMatcher.ThrowingGameObject));
        }

        protected override bool Filter(ThrowingEntity entity)
        {
            return entity.throwingGameObject.UnityObject.AsGameObject != null;
        }

        protected override void OnRender(ThrowingEntity throwing)
        {
            var throwingGo = throwing.throwingGameObject.UnityObject.AsGameObject; 
            throwingGo.transform.position = throwing.position.Value;
            throwingGo.transform.rotation = Quaternion.LookRotation(throwing.throwingData.Velocity);
        }

        
    }
}