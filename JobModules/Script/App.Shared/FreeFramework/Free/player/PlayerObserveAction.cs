using App.Shared.GameModules.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
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
            PlayerEntity player = GetPlayerEntity(args);
            if (player == null) return;

            if (args.Rule.ServerTime - player.statisticsData.Statistics.LastDeadTime <= 2900L) return;

            int oldObserveId = player.gamePlay.CameraEntityId;

            bool success = ObservePlayer(args, player, args.GetBool(observeEnemy), args.GetBool(wise));
            if (!success)
            {
                if (noOneAction != null)
                {
                    noOneAction.Act(args);
                }
            }

            if (oldObserveId != player.gamePlay.CameraEntityId)
            {
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlayerObserveTrigger;
                sp.Bs.Add(true);
                FreeMessageSender.SendMessage(player, sp);
            }
        }

        public static bool ObservePlayer(IEventArgs args, PlayerEntity player, bool observeEnemyFlag, bool wiseFlag)
        {
            List<PlayerEntity> teammateEntities = new List<PlayerEntity>();
            List<PlayerEntity> enemyEntities = new List<PlayerEntity>();
            PlayerEntity revenge = null;

            foreach (PlayerEntity playerEntity in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (!playerEntity.gamePlay.IsDead() && playerEntity.playerInfo.PlayerId != player.playerInfo.PlayerId)
                {
                    if (playerEntity.playerInfo.TeamId == player.playerInfo.TeamId)
                    {
                        teammateEntities.Add(playerEntity);
                    }
                    else
                    {
                        enemyEntities.Add(playerEntity);
                    }
                    if (playerEntity.playerInfo.PlayerId == player.statisticsData.Statistics.RevengeKillerId)
                    {
                        revenge = playerEntity;
                    }
                }
            }

            if (!teammateEntities.IsEmpty())
            {
                if (FindObserveObject(teammateEntities, player, false, wiseFlag))
                {
                    return true;
                }
            }
            else if (revenge != null && observeEnemyFlag)
            {
                player.gamePlay.CameraEntityId = revenge.playerInfo.EntityId;
                return true;
            }
            else if (!enemyEntities.IsEmpty() && observeEnemyFlag)
            {
                if (FindObserveObject(enemyEntities, player, true, wiseFlag))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool FindObserveObject(List<PlayerEntity> playerEntities, PlayerEntity player, bool observeEnemyFlag, bool wiseFlag)
        {
            if (playerEntities.Count == 1)
            {
                player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
                return true;
            }

            playerEntities.Sort((x, y) =>
            {
                if (observeEnemyFlag)
                {
                    if (Vector3.Distance(player.position.Value, x.position.Value) > Vector3.Distance(player.position.Value, y.position.Value))
                        return 1;
                    return -1;
                }
                if (x.playerInfo.Num > y.playerInfo.Num)
                    return 1;
                return -1;
            });

            if (player.gamePlay.CameraEntityId == 0)
            {
                player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
                return true;
            }

            for (int i = 0; i < playerEntities.Count; i++)
            {
                if (playerEntities[i].playerInfo.EntityId == player.gamePlay.CameraEntityId)
                {
                    if (wiseFlag)
                    {
                        player.gamePlay.CameraEntityId = i == playerEntities.Count - 1 ? playerEntities[0].playerInfo.EntityId : playerEntities[i + 1].playerInfo.EntityId;
                    }
                    else
                    {
                        player.gamePlay.CameraEntityId = i == 0 ? playerEntities[playerEntities.Count - 1].playerInfo.EntityId : playerEntities[i - 1].playerInfo.EntityId;
                    }
                    return true;
                }
            }
            player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
            return true;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerObserveAction;
        }
    }
}
