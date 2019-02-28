using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.free.player;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.para;
using com.wd.free.unit;
using com.wd.free.util;
using UnityEngine;
using Core.Utils;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core.Enums;
using UltimateFracturing;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class BombHurtAction : AbstractGameAction
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BombHurtAction));
        private static float CeilCheckDistance = 20f;
        private IPosSelector pos;

        private string damage;
        private string type;
        private string radius;

        [NonSerialized]
        private FloatPara damagePara;
        [NonSerialized]
        private StringPara typePara;
        [NonSerialized]
        private FloatPara disPara;

        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);

            float realRadius = FreeUtil.ReplaceFloat(radius, args);
            float realDamage = FreeUtil.ReplaceFloat(damage, args);

            if (damagePara == null)
            {
                damagePara = new FloatPara("damage", realDamage);
                disPara = new FloatPara("dis", 0f);
                typePara = new StringPara("type", FreeUtil.ReplaceVar(type, args));
            }

            if (up != null)
            {
                var bombPos = new Vector3(up.GetX(), up.GetY(), up.GetZ());
                var colliders = Physics.OverlapSphere(bombPos, realRadius, UnityLayerManager.GetLayerMask(EUnityLayerName.Player) | UnityLayerManager.GetLayerMask(EUnityLayerName.UserInputRaycast) | UnityLayerManager.GetLayerMask(EUnityLayerName.Vehicle));
                foreach (var collider in colliders)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("Process {0}", collider.name);
                    }
                    if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Player))
                    {
                        HandlePlayer(collider, args, args.GameContext, realDamage, bombPos);
                    }
                    else if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast))
                    {
                        HandleFracturedObjects(collider.transform, bombPos);
                    }
                    else if (collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle))
                    {
                        HandleVehicle(collider.transform, realDamage, bombPos);
                    }
                }

            }
        }

        private bool HasObstacle(Vector3 colPosition, Vector3 bombPosition, Func<Transform, bool> exclude = null)
        {
            RaycastHit hitInfo;
            if (null == exclude)
            {
                Debug.DrawLine(bombPosition, colPosition, Color.red, 10f);
                if (Physics.Linecast(bombPosition, colPosition, out hitInfo, UnityLayerManager.GetLayerMask(EUnityLayerName.Default)))
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("has obstacle {0}", hitInfo.transform.name);
                    }
                    return true;
                }
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("no obstacle");
                }
                return false;
            }
            var dir = colPosition - bombPosition;
            var obstacles = Physics.RaycastAll(bombPosition, dir, dir.magnitude, UnityLayerManager.GetLayerMask(EUnityLayerName.Default));
            foreach (var obstacle in obstacles)
            {
                if (!exclude(obstacle.transform))
                {
                    return true;
                }
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("has exculde fun and no obstacle ");
            }
            return false;
        }

        private void HandleVehicle(Transform trans, float damage, Vector3 bombPos)
        {
            var parent = trans;
            while (null != parent)
            {
                var entityRef = parent.GetComponent<EntityReference>();
                if (null != entityRef)
                {
                    var vehicle = entityRef.Reference as VehicleEntity;
                    if (null != vehicle)
                    {
                        vehicle.GetGameData().DecreaseHp(Core.Prediction.VehiclePrediction.Cmd.VehiclePartIndex.Body, damage, EUIDeadType.Bombing);
                    }
                    else
                    {
                        Logger.ErrorFormat("entity {0} is not vehicle ", trans.name);
                    }
                    break;
                }
                parent = parent.parent;
            }
        }

        private void HandleFracturedObjects(Transform trans, Vector3 bombPos)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("HandleFracturedObjects {0}", trans.name);
            }
            var parent = trans;
            while (null != parent)
            {
                var fractured = parent.GetComponent<FracturedObject>();
                if (null != fractured)
                {
                    if (!HasObstacle(trans.position, bombPos, (obstacleTrans) =>
                    {
                        var obstacleParent = obstacleTrans.parent;
                        while (null != obstacleParent)
                        {
                            if (obstacleParent == fractured.transform)
                            {
                                return true;
                            }
                            obstacleParent = obstacleParent.parent;
                        }
                        return false;
                    }))
                    {
                        if (Logger.IsDebugEnabled)
                        {
                            Logger.DebugFormat("do fractured explode ");
                        }
                        fractured.Explode(Vector3.zero, 50);
                    }
                    break;
                }
                parent = parent.parent;
            }
        }

        private void HandlePlayer(Collider collider, IEventArgs fr, Contexts contexts, float damage, Vector3 bombPos)
        {
            if (HasObstacle(collider.bounds.center, collider.bounds.center + Vector3.up * CeilCheckDistance))
            {
                return;
            }
            var entityReference = collider.transform.GetComponent<EntityReference>();
            var player = entityReference.Reference as PlayerEntity;
            if (null == player)
            {
                Logger.ErrorFormat("player {0} has no player reference ", collider.name);
                return;
            }
            if (player.IsOnVehicle())
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("player in vehicle ");
                }
                var vehicleEntity = contexts.vehicle.GetEntityWithEntityKey(player.controlledVehicle.EntityKey);
                vehicleEntity.GetGameData().DecreaseHp(Core.Prediction.VehiclePrediction.Cmd.VehiclePartIndex.Body, damage);
            }
            else
            {
                disPara.SetValue(Math.Max(0, (fr.GetFloat(radius) - Vector3.Distance(bombPos, player.position.Value)) / fr.GetFloat(radius)));
                fr.TempUsePara(damagePara);
                fr.TempUsePara(disPara);
                fr.TempUsePara(typePara);
                fr.TempUse("current", (FreeData)player.freeData.FreeData);
                action.Act(fr);

                fr.ResumePara("damage");
                fr.ResumePara("dis");
                fr.ResumePara("type");
                fr.Resume("current");
            }
        }
    }
}