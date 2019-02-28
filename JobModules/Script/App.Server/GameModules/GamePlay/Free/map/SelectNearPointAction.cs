using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using UnityEngine;
using com.wd.free.para;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.map.position;
using com.wd.free.unit;

namespace App.Server.GameModules.GamePlay.Free.map
{
    [Serializable]
    public class SelectNearPointAction : AbstractGameAction
    {
        private string type;
        private string distance;

        private IPosSelector pos;
        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            int realType = args.GetInt(type);
            int dis = args.GetInt(distance);

            int i = 1;

            UnitPosition up = pos.Select(args);
            Vector3 target = new Vector3(up.GetX(), up.GetY(), up.GetZ());
            foreach (MapConfigPoints.ID_Point p in FreeMapPosition.GetPositions(args.GameContext.session.commonSession.RoomInfo.MapId).IDPints)
            {
                if (p.ID == realType)
                {
                    foreach (Vector3 v in p.points)
                    {
                        if (IsNear(v, target, dis))
                        {
                            TriggerArgs ta = new TriggerArgs();
                            ta.AddPara(new FloatPara("x", v.x));
                            ta.AddPara(new FloatPara("y", v.y));
                            ta.AddPara(new FloatPara("z", v.z));
                            ta.AddPara(new FloatPara("index", i++));

                            args.Act(action, ta);
                        }
                    }
                }
            }
        }

        private bool IsNear(Vector3 v1, Vector3 v2, int dis)
        {
            return (v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z) < dis * dis;
        }
    }
}
