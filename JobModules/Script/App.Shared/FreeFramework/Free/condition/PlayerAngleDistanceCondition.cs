using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.util;
using System;
using UnityEngine;
using Utils.Utils;

namespace App.Shared.FreeFramework.Free.condition
{
    [Serializable]
    public class PlayerAngleDistanceCondition : IParaCondition
    {
        public string entity;
        public string player;
        public string angle;
        public string distance;

        public bool Meet(IEventArgs args)
        {
            FreeData fd = (FreeData) args.GetUnit(player);
            object obj = ((FreeRuleEventArgs) args).GetEntity(entity);
            if (fd != null && obj != null)
            {
                PlayerEntity playerEntity = fd.Player;
                FreeMoveEntity objEntity = (FreeMoveEntity) obj;
                float yaw = CommonMathUtil.TransComAngle(playerEntity.orientation.Yaw);
                float ang = CommonMathUtil.GetAngle(new Vector2(objEntity.position.Value.x, objEntity.position.Value.z), new Vector2(playerEntity.position.Value.x, playerEntity.position.Value.z));
                return CommonMathUtil.GetDiffAngle(ang, yaw) <= FreeUtil.ReplaceFloat(angle, args) && Vector3.Distance(playerEntity.position.Value, objEntity.position.Value) <= FreeUtil.ReplaceFloat(distance, args);
            }
            return false;
        }
    }
}
