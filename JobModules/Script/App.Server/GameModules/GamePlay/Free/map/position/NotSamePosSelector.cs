using App.Shared.GameModules.Player;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.map.position;
using Core.Free;
using com.wd.free.unit;
using com.wd.free.util;
using Shared.Scripts.MapConfigPoint;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.map.position
{
    [Serializable]
    public class NotSamePosSelector : AbstractPosSelector, IRule
    {
        public string type;

        private UnitPosition tempPosition;

        public int GetRuleID()
        {
            return (int)ERuleIds.NotSamePosSelector;
        }

        public override UnitPosition Select(IEventArgs args)
        {
            if (tempPosition == null)
            {
                tempPosition = new UnitPosition();
            }

            if (MapConfigPoints.current != null)
            {
                int realType = FreeUtil.ReplaceInt(type, args);
                int mapId = args.GameContext.session.commonSession.RoomInfo.MapId;
                var poss = args.FreeContext.Poss;
                foreach (MapConfigPoints.ID_Point p in FreeMapPosition.GetPositions(mapId).IDPints)
                {
                    if (p.ID == realType)
                    {
                        RandomBit randomBit = poss.AddRandomBit(p.ID, p.points.Count);
                        Vector3 v = Vector3.zero;
                        int index = -1;
                        bool invalid = false;
                        if (randomBit.index >= 0)
                        {
                            index = poss.GetRandomBit(p.ID, p.points.Count);
                            v = p.points[index].pos;
                        }
                        else
                        {
                            invalid = true;
                        }
                        tempPosition.SetInvalid(invalid);
                        tempPosition.SetX(v.x);
                        tempPosition.SetY(v.y);
                        tempPosition.SetZ(v.z);
                        tempPosition.SetRandomindex(index);
                        return tempPosition;
                    }
                }
            }
            return tempPosition;
        }
    }
}
