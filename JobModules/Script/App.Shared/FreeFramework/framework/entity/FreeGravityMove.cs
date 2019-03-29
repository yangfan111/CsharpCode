using App.Server.GameModules.GamePlay.Free.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay;
using com.wd.free.map.position;
using com.wd.free.unit;
using UnityEngine;

namespace App.Shared.FreeFramework.framework.entity
{
    public class FreeGravityMove : IFreeMove
    {
        private string speed;
        private IPosSelector startPos;
        private float VelocityDecay;
        private float Gravity;
        private float CollisionVelocityDecay;

        public bool Frame(FreeRuleEventArgs args, FreeMoveEntity entity, int interval)
        {
            return false;
        }

        public void Start(FreeRuleEventArgs args, FreeMoveEntity entity)
        {
            UnitPosition up = startPos.Select(args);
            entity.position.Value = new Vector3(up.GetX(), up.GetY(), up.GetZ());
        }
    }
}
