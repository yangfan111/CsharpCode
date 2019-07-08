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
    public class PosEditSelector : AbstractPosSelector, IRule
    {
        public string type;
        public string index;
        private bool notSame;
        private bool birth;

        private UnitPosition tempPosition;

        private Dictionary<int, RandomBit> bitDic;

        public override UnitPosition Select(IEventArgs args)
        {
            if (tempPosition == null)
            {
                tempPosition = new UnitPosition();
                bitDic = new Dictionary<int, RandomBit>();
            }

            if (MapConfigPoints.current != null)
            {
                int realType = FreeUtil.ReplaceInt(type, args);
                int realIndex = FreeUtil.ReplaceInt(index, args);
                int mapId = args.GameContext.session.commonSession.RoomInfo.MapId;
                foreach (MapConfigPoints.ID_Point p in FreeMapPosition.GetPositions(mapId).IDPints)
                {
                    if (p.ID == realType)
                    {

                        if (!bitDic.ContainsKey(p.ID))
                        {
                            bitDic.Add(p.ID, new RandomBit(p.points.Count));
                        }
                        Vector3 v = Vector3.zero;
                        if (realIndex == 0)
                        {
                            v = p.points[Random(p.ID, p.points.Count)].pos;
                            if (birth)
                            {
                                if (HasPlayerNearBy(v, args))
                                {
                                    for (int i = 0; i < p.points.Count; i++)
                                    {
                                        realIndex = Random(p.ID, p.points.Count);
                                        v = p.points[realIndex].pos;
                                        if (!HasPlayerNearBy(v, args))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (realIndex < 0 || realIndex > p.points.Count)
                            {
                                realIndex = Math.Min(1, p.points.Count);
                            }
                            realIndex--;
                            v = p.points[realIndex].pos;
                        }
                        tempPosition.SetX(v.x);
                        tempPosition.SetY(v.y);
                        tempPosition.SetZ(v.z);
                        tempPosition.SetYaw(p.points[realIndex].dir);
                        tempPosition.SetCylinderVolR(p.points[realIndex].cylinderVolR);
                        tempPosition.SetCylinderVolH(p.points[realIndex].cylinderVolH);
                        break;
                    }
                }
            }

            return tempPosition;
        }

        private bool HasPlayerNearBy(Vector3 v, IEventArgs args)
        {
            PlayerEntity[] players = args.GameContext.player.GetInitializedPlayerEntities();
            for (int i = 0; i < players.Length; i++)
            {
                if (Math.Abs(players[i].position.Value.x - v.x) > 1)
                {
                    continue;
                }
                else if (Math.Abs(players[i].position.Value.z - v.z) > 1)
                {
                    continue;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private int Random(int type, int len)
        {
            if (notSame)
            {
                return bitDic[type].Random();
            }
            else
            {
                return RandomUtil.Random(0, len - 1);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PosEditSelector;
        }
    }

    public class RandomBit
    {
        public int index;
        public int[] array;

        public RandomBit(int len)
        {
            index = len - 1;
            array = new int[len];
            for (int i = 0; i < len; i++)
            {
                array[i] = i;
            }
        }

        public int Random()
        {
            if (index < 0)
            {
                index = array.Length - 1;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = i;
                }
            }
            int currentIndex = RandomUtil.Random(0, index);

            int r = array[currentIndex];

            array[currentIndex] = array[index];
            // 支持还原操作
            array[index] = r;

            index--;

            return r;
        }
    }
}
