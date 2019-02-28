using App.Client.GameModules.Player;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.SceneObject
{
    public class FreeObjectPositionUpdateSystem : AbstractRenderSystem<FreeMoveEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FreeObjectPositionUpdateSystem));

        public FreeObjectPositionUpdateSystem(Contexts context) : base(context)
        {
        }

        protected override IGroup<FreeMoveEntity> GetIGroup(Contexts contexts)
        {
            return contexts.freeMove.GetGroup(FreeMoveMatcher.AllOf(FreeMoveMatcher.Position,
                FreeMoveMatcher.UnityGameObject));
        }

        protected override bool Filter(FreeMoveEntity entity)
        {
            return entity.unityGameObject.UnityObject.AsGameObject != null;
        }

        protected override void OnRender(FreeMoveEntity entity)
        {
            entity.unityGameObject.UnityObject.AsGameObject.transform.position = entity.position.Value;
        }
    }
}