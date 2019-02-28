using App.Shared;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using Core;
using System;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.para;
using Core.Enums;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerWeaponDropAction : AbstractPlayerAction
    {
        /// <summary>
        /// 0 是当前武器
        /// </summary>
        public int slotIndex;
        public IPosSelector pos;
        public string lifeTime;

        public override void DoAction(IEventArgs args)
        {
            var contexts = args.GameContext;
            var player = GetPlayerEntity(args);

            var factory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            var slot = (EWeaponSlotType)slotIndex;
            if (slot == EWeaponSlotType.None)
            {
                slot = player.GetController<PlayerWeaponController>().CurrSlotType;
            }
            var weaponInfo = player.GetController<PlayerWeaponController>().GetSlotWeaponInfo(args.GameContext, slot);
            if(weaponInfo.Id > 0)
            {
                var unitPos = pos.Select(args);
                var weapon = factory.CreateDropWeaponEntity(weaponInfo, new UnityEngine.Vector3(unitPos.GetX(), unitPos.GetY(), unitPos.GetZ()), args.GetInt(lifeTime));
            }
            player.GetController<PlayerWeaponController>().DropSlotWeapon(args.GameContext, slot);
            TriggerArgs ta = new TriggerArgs();
            ta.AddPara(new IntPara("weaponId", weaponInfo.Id));
            ta.AddUnit("current", (FreeData) player.freeData.FreeData);
            args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
        }
    }
}