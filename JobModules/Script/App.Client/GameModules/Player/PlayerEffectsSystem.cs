using System.Collections.Generic;
using App.Client.GameModules.Effect;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using com.wd.free.skill;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using UnityEngine;
using WeaponConfigNs;

namespace App.Client.GameModules.Player
{
    public class PlayerEffectsSystem:AbstractGamePlaySystem<PlayerEntity>
    {
        private Contexts _contexts;
        private LoggerAdapter logger = new LoggerAdapter(typeof(PlayerEffectsSystem));

        public PlayerEffectsSystem(Contexts contexts):base(contexts)
        {
            _contexts = contexts;
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.Effects);
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnGamePlay(PlayerEntity player)
        {
            var effect = player.effects.GetEffect("GodModeEffect");
            if (effect != null)
            {
                if (PlayerStateUtil.HasPlayerState(EPlayerGameState.Invincible, player.gamePlay))
                {
                    effect.SetParam("Enable", true);
                }
                else effect.SetParam("Enable", false);
            }
        }
    }
}