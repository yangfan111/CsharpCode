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
using Core.HitBox;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.FreeFramework.framework.unit;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using App.Shared.GameModules.Common;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class FreePositionMove : AbstractFreeMove, IRule
    {
        public enum HitType
        {
            HitEnemy = 1, HitAll = 2, HitEnemyGone = 3, HitAnyGone = 4
        }

        private IPosSelector targetPos;
        private bool dynamic;
        private bool stayTarget;
        private IGameAction action;
        private int hitType;
        private IGameAction hitAction;

        private string useTime;

        [NonSerialized]
        private UnitPosition tempPosition;

        [NonSerialized]
        private bool firstTime;

        [NonSerialized]
        private float realTime;

        [NonSerialized]
        private long startTime;

        public override void StartMove(FreeRuleEventArgs args, FreeMoveEntity entity)
        {
            tempPosition = targetPos.Select(args);
            if (!string.IsNullOrEmpty(useTime))
            {
                realTime = FreeUtil.ReplaceFloat(useTime, args);
            }

            UnityPositionUtil.SetUnitPositionToEntity(entity.position, startPos.Select(args));

            startTime = DateTime.Now.Ticks / 10000;
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

            long nowTime = DateTime.Now.Ticks / 10000;

            int deltaTime = (int)(nowTime - startTime);

            float speedMeter = GetSpeed(args, entity).GetSpeed(args, deltaTime);

            float dis = speedMeter * (float)deltaTime / 1000f;

            startTime = nowTime;

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
                Vector3 from = entity.position.Value;
                UnitPosition ep = GetEntityPosition(entity);
                UnityPositionUtil.Move(ep, tempPosition, dis);

                entity.position.Value = new Vector3(ep.GetX(), ep.GetY(), ep.GetZ());

                if (hitType > 0)
                {
                    bool hit = CheckHit(args, (FreeEntityData)entity.freeData.FreeData, from, entity.position.Value, dis);

                    if (hit)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckHit(IEventArgs args, FreeEntityData data, Vector3 fromV, Vector3 toV, float dis)
        {
            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

            RaycastHit[] hits = Physics.RaycastAll(r, dis);

            if (hits.Length > 0)
            {
                PlayerEntity player = args.GameContext.player.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(data.CreatorId, (short)EEntityType.Player));

                int team = 0;
                FreeData source = null;
                if (player != null)
                {
                    team = player.playerInfo.Camp;
                    source = (FreeData)player.freeData.FreeData;
                }

                foreach (RaycastHit hit in hits)
                {
                    var comp = hit.collider.transform.gameObject.GetComponent<EntityReference>();
                    if (comp != null)
                    {
                        PlayerEntity hitPlayer = args.GameContext.player.GetEntityWithEntityKey(comp.EntityKey);

                        if (hitPlayer != null && hitPlayer != player && !hitPlayer.gamePlay.IsDead())
                        {
                            bool sameTeam = hitPlayer.playerInfo.Camp == team;
                            switch ((HitType)hitType)
                            {
                                case HitType.HitAll:
                                    args.Act(hitAction, new TempUnit("source", source), new TempUnit("target", (FreeData)hitPlayer.freeData.FreeData),
                                        new TempUnit("pos", new ObjectUnit(hit.point)));
                                    break;
                                case HitType.HitEnemy:
                                    if (!sameTeam)
                                    {
                                        args.Act(hitAction, new TempUnit("source", source), new TempUnit("target", (FreeData)hitPlayer.freeData.FreeData),
                                        new TempUnit("pos", new ObjectUnit(hit.point)));
                                    }
                                    break;
                                case HitType.HitAnyGone:
                                    args.Act(hitAction, new TempUnit("source", source), new TempUnit("target", (FreeData)hitPlayer.freeData.FreeData),
                                        new TempUnit("pos", new ObjectUnit(hit.point)));
                                    return true;
                                case HitType.HitEnemyGone:
                                    if (!sameTeam)
                                    {
                                        args.Act(hitAction, new TempUnit("source", source), new TempUnit("target", (FreeData)hitPlayer.freeData.FreeData),
                                        new TempUnit("pos", new ObjectUnit(hit.point)));
                                        return true;
                                    }
                                    break;
                            }
                        }
                        Debug.LogFormat(hit.collider.gameObject.GetInstanceID().ToString() + " of " + hits.Length);
                    }
                }
            }

            return false;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.FreePositionMove;
        }
    }
}
