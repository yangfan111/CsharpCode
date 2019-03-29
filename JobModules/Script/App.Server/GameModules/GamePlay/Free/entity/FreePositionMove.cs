using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;
using UnityEngine;
using com.wd.free.para;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class FreePositionMove : AbstractFreeMove
    {
        private IPosSelector targetPos;
        private bool dynamic;
        private bool stayTarget;
        private IGameAction action;

        private string useTime;

        [NonSerialized]
        private UnitPosition tempPosition;

        [NonSerialized]
        private bool firstTime;

        [NonSerialized]
        private float realTime;

        public override void StartMove(FreeRuleEventArgs args, FreeMoveEntity entity)
        {
            tempPosition = targetPos.Select(args);
            if (!string.IsNullOrEmpty(useTime))
            {
                realTime = FreeUtil.ReplaceFloat(useTime, args);
            }

            UnityPositionUtil.SetUnitPositionToEntity(entity.position, startPos.Select(args));
        }

        protected new IMoveSpeed GetSpeed(IEventArgs args, FreeMoveEntity entity)
        {

            if (string.IsNullOrEmpty(useTime))
            {
                return base.GetSpeed(args, entity);
            }
            else
            {
                if (speed == null)
                {
                    float dis = tempPosition.Distance(GetEntityPosition(entity));
                    if (realTime > 0)
                    {
                        speed = new AlwaysOneSpeed(dis / realTime);
                    }
                    else
                    {
                        speed = new AlwaysOneSpeed(5);
                    }

                }

                return base.GetSpeed(args, entity);
            }
        }

        public override bool Frame(FreeRuleEventArgs args, FreeMoveEntity entity, int interval)
        {
            if (dynamic)
            {
                tempPosition = targetPos.Select(args);
            }

            float speedMeter = GetSpeed(args, entity).GetSpeed(args, interval);

            float dis = speedMeter * (float)interval / 1000f;

            if (tempPosition.Distance(GetEntityPosition(entity)) < dis)
            {
                UnityPositionUtil.SetUnitPositionToEntity(entity.position, tempPosition);

                if (action != null && !firstTime)
                {
                    args.TempUsePara(new FloatPara("x", entity.position.Value.x));
                    args.TempUsePara(new FloatPara("y", entity.position.Value.y));
                    args.TempUsePara(new FloatPara("z", entity.position.Value.z));

                    action.Act(args);

                    args.ResumePara("x");
                    args.ResumePara("y");
                    args.ResumePara("z");

                    firstTime = true;
                }

                if (!stayTarget)
                {
                    return true;
                }
            }
            else
            {
                UnitPosition ep = GetEntityPosition(entity);
                UnityPositionUtil.Move(ep, tempPosition, dis);

                entity.position.Value = new Vector3(ep.GetX(), ep.GetY(), ep.GetZ());
            }

            return false;
        }

    }
}
