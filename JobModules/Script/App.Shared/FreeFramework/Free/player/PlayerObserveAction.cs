using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Sharpen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerObserveAction : AbstractPlayerAction, IRule
    {
        private string observeEnemy;
        private IGameAction noOneAction;
        private string wise;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);

            /*if (!fd.Player.gamePlay.IsDead())
            {
                fd.Player.gamePlay.CameraEntityId = 0;
                return;
            }*/


            if (DateTime.Now.Ticks / 10000 - fd.Player.statisticsData.Statistics.LastDeadTime <= 2990L) return;

            bool success = ObservePlayer(args, fd, args.GetBool(observeEnemy), args.GetBool(wise));
            if (!success)
            {
                if (noOneAction != null)
                {
                    noOneAction.Act(args);
                }
            }
        }

        public static bool ObservePlayer(IEventArgs args, FreeData fd, bool observeEnemyFlag, bool wiseFlag)
        {
            List<PlayerEntity> teammateEntities = new List<PlayerEntity>();
            List<PlayerEntity> enemyEntities = new List<PlayerEntity>();
            PlayerEntity revenge = null;

            foreach (PlayerEntity playerEntity in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (!playerEntity.gamePlay.IsDead() && playerEntity.playerInfo.PlayerId != fd.Player.playerInfo.PlayerId)
                {
                    if (playerEntity.playerInfo.TeamId == fd.Player.playerInfo.TeamId)
                    {
                        teammateEntities.Add(playerEntity);
                    }
                    else
                    {
                        enemyEntities.Add(playerEntity);
                    }
                    if (playerEntity.playerInfo.PlayerId == fd.Player.statisticsData.Statistics.RevengeKillerId)
                    {
                        revenge = playerEntity;
                    }
                }
            }

            if (!teammateEntities.IsEmpty())
            {
                if (FindObserveObject(teammateEntities, fd, false, wiseFlag))
                {
                    return true;
                }
            }
            else if (revenge != null && observeEnemyFlag)
            {
                fd.Player.gamePlay.CameraEntityId = revenge.playerInfo.EntityId;
                return true;
            }
            else if (!enemyEntities.IsEmpty() && observeEnemyFlag)
            {
                if (FindObserveObject(enemyEntities, fd, true, wiseFlag))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool FindObserveObject(List<PlayerEntity> playerEntities, FreeData fd, bool observeEnemyFlag, bool wiseFlag)
        {
            if (playerEntities.Count == 1)
            {
                fd.Player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
                return true;
            }

            playerEntities.Sort((x, y) =>
            {
                if (observeEnemyFlag)
                {
                    if (Vector3.Distance(fd.Player.position.Value, x.position.Value) > Vector3.Distance(fd.Player.position.Value, y.position.Value))
                        return 1;
                    return -1;
                }
                if (x.playerInfo.Num > y.playerInfo.Num)
                    return 1;
                return -1;
            });

            if (fd.Player.gamePlay.CameraEntityId == 0)
            {
                fd.Player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
                return true;
            }

            for (int i = 0; i < playerEntities.Count; i++)
            {
                if (playerEntities[i].playerInfo.EntityId == fd.Player.gamePlay.CameraEntityId)
                {
                    if (wiseFlag)
                    {
                        fd.Player.gamePlay.CameraEntityId = i == playerEntities.Count - 1 ? playerEntities[0].playerInfo.EntityId : playerEntities[i + 1].playerInfo.EntityId;
                    }
                    else
                    {
                        fd.Player.gamePlay.CameraEntityId = i == 0 ? playerEntities[playerEntities.Count - 1].playerInfo.EntityId : playerEntities[i - 1].playerInfo.EntityId;
                    }
                    return true;
                }
            }
            fd.Player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
            return true;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerObserveAction;
        }
    }
}
