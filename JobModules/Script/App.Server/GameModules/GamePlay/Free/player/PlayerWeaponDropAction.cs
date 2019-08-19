using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using App.Shared.FreeFramework.framework.trigger;
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
        public int slotIndex;
        public IPosSelector pos;
        public string lifeTime;
        public bool fixDrop;

        public override void DoAction(IEventArgs args)
        {

            var contexts = args.GameContext;
            var player = GetPlayerEntity(args);

            var factory = contexts.session.entityFactoryObject.SceneObjectEntityFactory;
            var slot = (EWeaponSlotType)slotIndex;
            var lastWeaponScan = player.WeaponController().GetWeaponAgent(slot).ComponentScan;
            bool generateSceneObj = player.WeaponController().DropWeapon(slot);
            if (!generateSceneObj || lastWeaponScan.IsUnSafeOrEmpty()) return;
            var unitPos = pos.Select(args);
            SceneObjectEntity weapon;
            if (fixDrop) weapon = factory.CreateDropSceneWeaponObjectEntity(lastWeaponScan, unitPos.Vector3, args.GetInt(lifeTime)) as SceneObjectEntity;
            else weapon = factory.CreateDropSceneWeaponObjectEntity(lastWeaponScan, player, args.GetInt(lifeTime), null) as SceneObjectEntity;
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
