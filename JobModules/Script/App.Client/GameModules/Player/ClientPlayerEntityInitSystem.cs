using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;

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
            player.AddPlayerInterruptState();

            player.AddAnimationExData();
            player.AddFreeUserCmd();
            player.AddWeaponSound(new List<XmlConfig.EWeaponSoundType>());
            player.AddWeaponEffect(new List<XmlConfig.EClientEffectType>());

            player.AddPlayerIntercept();
            player.playerIntercept.InterceptKeys = new Shared.Components.Player.KeyTime();
            player.playerIntercept.PresssKeys = new Shared.Components.Player.KeyTime();
        }
    }
}