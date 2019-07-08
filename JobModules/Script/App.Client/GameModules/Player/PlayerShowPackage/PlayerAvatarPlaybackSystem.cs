using Core.Utils;
using Entitas;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class PlayerAvatarPlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private readonly CustomProfileInfo _info;

        public PlayerAvatarPlaybackSystem(Contexts contexts) : base(contexts)
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("PlayerAvatarPlayback");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.GamePlay).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.isFlagPlayBackFilter;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            if(!player.hasAppearanceInterface) return;
            try
            {
                _info.BeginProfileOnlyEnableProfile();
                
                var appearanceInterface = player.appearanceInterface;
                appearanceInterface.Appearance.UpdateAvatar();
            }
            finally
            {
                _info.EndProfileOnlyEnableProfile();
            }
        }
    }
}
