using App.Shared;
using App.Shared.Components.Player;
using Core.GameModule.System;
using Core.Utils;
using Entitas;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerEntityInitSystem : ReactiveEntityInitSystem<PlayerEntity>
    {
        private readonly static LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerEntityInitSystem));

      

        public ClientPlayerEntityInitSystem(PlayerContext playerContext) : base(playerContext)
        {
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.UserCmdSeq.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        public override void SingleExecute(PlayerEntity player)
        {

            player.AddAnimationExData();
            player.AddFreeUserCmd();
            //player.AddWeaponSound(new List<XmlConfig.EWeaponSoundType>());
            player.WeaponController().AddAuxEffect();

            player.AddPlayerIntercept();
            player.playerIntercept.InterceptKeys = new KeyTime();
            player.playerIntercept.PressKeys = new KeyTime();
            player.playerIntercept.RealPressKeys = new KeyTime();
        }
    }
}