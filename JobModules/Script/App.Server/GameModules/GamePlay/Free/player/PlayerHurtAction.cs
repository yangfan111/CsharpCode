using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Weapon;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Free;
using System;
using UnityEngine;
using WeaponConfigNs;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerHurtAction : AbstractGameAction, IRule
    {
        private string damage;
        private string type;
        private string part;
        private string source;
        private string target;
        private string dead;

        private IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            PlayerEntity player = (PlayerEntity)fr.GetEntity(target);

            if (player != null)
            {
                if (player.gamePlay.IsDead())
                    return;

                PlayerEntity sourcePlayer = null;
                if (!string.IsNullOrEmpty(source))
                {
                    sourcePlayer = (PlayerEntity)fr.GetEntity(source);
                }

                if (string.IsNullOrEmpty(part))
                {
                    part = ((int)EBodyPart.Chest).ToString();
                }

                UnitPosition up = null;
                if(pos != null) up = pos.Select(args);
                
                PlayerDamageInfo damageInfo = new PlayerDamageInfo(FreeUtil.ReplaceFloat(damage, args), FreeUtil.ReplaceInt(type, args), FreeUtil.ReplaceInt(part, args), 0,
                    false, false,FreeUtil.ReplaceBool(dead, args), up != null ? player.position.Value : Vector3.zero, up != null ? player.position.Value - up.Vector3 : Vector3.zero);

                BulletPlayerUtil.DoProcessPlayerHealthDamage(args.GameContext, (IGameRule)fr.Rule, sourcePlayer, player, damageInfo);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerHurtAction;
        }
    }
}
