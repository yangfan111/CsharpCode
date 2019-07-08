using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Player;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.para;
using Core;
using Core.Free;
using System;

namespace App.Server.GameModules.GamePlay.Free.player
{
    /// <summary>
    /// Defines the <see cref="PlayerWeaponDropAction" />
    /// </summary>
    [Serializable]
    public class PlayerWeaponDropAction : AbstractPlayerAction, IRule
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
            var lastWeaponScan = player.WeaponController().GetWeaponAgent(slot).ComponentScan;
            bool generateSceneObj = player.WeaponController().DropWeapon(slot);
            //     var lastWeaponScan = player.WeaponController().HeldWeaponAgent.ComponentScan;
            if (!generateSceneObj || lastWeaponScan.IsUnSafeOrEmpty()) return;
            var unitPos = pos.Select(args);
            var weapon = factory.CreateDropSceneWeaponObjectEntity(lastWeaponScan, 
                new UnityEngine.Vector3(unitPos.GetX(), unitPos.GetY(), unitPos.GetZ()), args.GetInt(lifeTime)) as SceneObjectEntity;
            if (null != weapon)
            {
                TriggerArgs ta = new TriggerArgs();
                ta.AddPara(new IntPara("weaponId", lastWeaponScan.ConfigId));
                ta.AddPara(new FloatPara("weaponx", weapon.position.Value.x));
                ta.AddPara(new FloatPara("weapony", weapon.position.Value.y));
                ta.AddPara(new FloatPara("weaponz", weapon.position.Value.z));
                ta.AddUnit("current", (FreeData)player.freeData.FreeData);
                args.Trigger(FreeTriggerConstant.WEAPON_DROP, ta);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerWeaponDropAction;
        }
    }
}
