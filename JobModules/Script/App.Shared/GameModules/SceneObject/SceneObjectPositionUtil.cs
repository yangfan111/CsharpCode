using App.Shared.GameModules.Player;
using Core;
using UnityEngine;

namespace App.Shared.GameModules.SceneObject
{
    public static class SceneObjectPositionUtil
    {

        public static Vector3 GetPlayerDropPos(PlayerEntity playerEntity)
        {
            return playerEntity.position.Value + playerEntity.characterContoller.Value.transform.forward * GlobalConst.WeaponDropOffset;
            
        }
        public static Vector3 GetHandObjectPosition(PlayerEntity playerEntity)
        {
            return playerEntity.GetHandWeaponPosition();
        }
    }
}