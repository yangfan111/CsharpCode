﻿using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Singleton;

namespace App.Client.CastObjectUtil
{
    public static class SceneObjectGoAssemble
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectGoAssemble));

        public static RayCastTarget Assemble(GameObject model, SceneObjectEntity entity)
        {
            var needRotate = false;
            if (entity.hasSimpleEquipment)
            {
                var isWeaponPart = entity.simpleEquipment.Category == (int)ECategory.WeaponPart;
                var isWeapon = entity.simpleEquipment.Category == (int)ECategory.Weapon;
                var isC4 = SingletonManager.Get<WeaponConfigManager>().IsC4(entity.simpleEquipment.Id);
                needRotate = isWeaponPart || (isWeapon && !isC4);
            }
            else if(entity.hasWeapon)
            {
                var isC4 = SingletonManager.Get<WeaponConfigManager>().IsC4(entity.weapon.Id);
                needRotate = !isC4;
            }
            else
            {
                Logger.Error("illegal sceneobject has no weapon or equip component ");
            }
            if(needRotate)
            {
                model.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                model.transform.localRotation = Quaternion.identity;
            }
            var root = BaseGoAssemble.Assemble(model, entity.position.Value, entity.entityKey.Value.ToString());
            model.transform.localScale = Vector3.one;
            return root;
        }
    }
}
