using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.System;
using Core.Sound;
using Entitas;

namespace App.Client.GameModules.Sound
{
    public class SoundCleanUpSystem : ReactiveEntityCleanUpSystem<SoundEntity>
    {
        private ISoundPlayer _soundPlayer;

        public SoundCleanUpSystem(Contexts contexts) : base(contexts.sound)
        {
            _soundPlayer = contexts.session.clientSessionObjects.SoundPlayer;
        }

        protected override ICollector<SoundEntity> GetTrigger(IContext<SoundEntity> context)
        {
            return context.CreateCollector(SoundMatcher.FlagDestroy.Added());
        }

        protected override bool Filter(SoundEntity entity)
        {
            return entity.hasAudioSourceKey;
        }

        public override void SingleExecute(SoundEntity entity)
        {
            _soundPlayer.Unload(entity.audioSourceKey.Value);
            entity.RemoveAssetInfo();
        }
    }
}