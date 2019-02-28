using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.CharacterState;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using Core.Animation;
using App.Shared.GameModules.Player;
using App.Shared.Components.Player;
using App.Shared.Player;
using Core.WeaponAnimation;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Player.PlayerShowPackage
{
    class WeaponAnimationPlaybackSystem : AbstractPlayerBackSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(WeaponAnimationPlaybackSystem));
        private CustomProfileInfo _info;
        private WeaponAnimationController _weaponAnimation = new WeaponAnimationController();
        
        public WeaponAnimationPlaybackSystem(Contexts contexts) : base(contexts)
        {
            _info = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("WeaponAnimationPlayback");
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.AppearanceInterface,
                PlayerMatcher.GamePlay,
                PlayerMatcher.NetworkWeaponAnimation).NoneOf(PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return !entity.isFlagPlayBackFilter;
        }

        protected override void OnPlayBack(PlayerEntity player)
        {
            if (!player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                try
                {
                    _info.BeginProfileOnlyEnableProfile();
                    
                    _weaponAnimation.FromWeaponAnimProgressToWeaponAnim(
                        null,
                        player.appearanceInterface.Appearance.GetWeaponP3InHand(),
                        player.networkWeaponAnimation);
                }
                finally
                {
                    _info.EndProfileOnlyEnableProfile();
                }
            }
        }
    }
}