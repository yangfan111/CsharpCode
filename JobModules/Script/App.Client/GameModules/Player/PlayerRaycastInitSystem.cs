using App.Client.CastObjectUtil;
using App.Shared.Player;
using Entitas;
using UserInputManager.Lib;
using Core.GameModule.Interface;
using UserInputManager.Utility;

namespace App.Client.GameModules.Player
{
    public class PlayerRaycastInitSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private IGroup<PlayerEntity> _thirdModelGroup;

        public PlayerRaycastInitSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.ThirdPersonModel).NoneOf(PlayerMatcher.Raycast));
        }

        protected override void OnPlayBack(PlayerEntity entity)
        {
            if(!entity.isFlagSelf)
            {
                InitRayCastTarget(entity);
            }
        }

        void InitRayCastTarget(PlayerEntity player)
        {
            var target = RayCastTargetUtil.AddRayCastTarget(player.RootGo());
            PlayerCastData.Make(target, player.entityKey.Value.EntityId);
            player.hasAddRaycast = true;
        }
    }
}