using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Ui;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using System;
using System.Collections.Generic;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerObserveAction : AbstractPlayerAction
    {
        private string observeEnemy;
        private IGameAction noOneAction;
        private string wise;

        public override void DoAction(IEventArgs args)
        {
            FreeData fd = GetPlayer(args);

            bool success = ObservePlayer(args, fd, args.GetBool(observeEnemy), args.GetBool(wise));
            if (!success)
            {
                if (noOneAction != null)
                {
                    noOneAction.Act(args);
                }
            }
        }

        public static bool ObservePlayer(IEventArgs args, FreeData fd, bool observeEnemy, bool wiseFlag)
        {
            bool hasTeamMate = false;
            PlayerEntity revenge = null;
            bool hasEnemy = false;

            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (CanOb(fd.Player, p))
                {
                    if (p.playerInfo.Camp == fd.Player.playerInfo.Camp)
                    {
                        hasTeamMate = true;
                    }
                    if (fd.Player.statisticsData.Statistics.RevengeKillerId == p.playerInfo.PlayerId)
                    {
                        revenge = p;
                    }
                    if (p.playerInfo.Camp != fd.Player.playerInfo.Camp)
                    {
                        hasEnemy = true;
                    }
                }
            }

            if (hasTeamMate)
            {
                if (FindTeamMate(args, fd, wiseFlag))
                {
                    return true;
                }
            }
            else if (revenge != null && observeEnemy)
            {
                fd.Player.gamePlay.CameraEntityId = revenge.entityKey.Value.EntityId;
                return true;
            }
            else if (hasEnemy && observeEnemy)
            {
                if (!FindSomeOne(args, fd, wiseFlag))
                {
                    fd.Player.gamePlay.CameraEntityId = wiseFlag ? 0 : 999;
                    FindSomeOne(args, fd, wiseFlag);
                }
                return true;
            }

            return false;
        }

        private static bool FindTeamMate(IEventArgs args, FreeData fd, bool wiseFlag)
        {
            List<PlayerEntity> playerEntities = new List<PlayerEntity>();
            foreach (PlayerEntity playerEntity in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (playerEntity.playerInfo.TeamId == fd.Player.playerInfo.TeamId &&
                    playerEntity.playerInfo.PlayerId != fd.Player.playerInfo.PlayerId &&
                    !playerEntity.gamePlay.IsDead())
                {
                    playerEntities.Add(playerEntity);
                }
            }

            if (playerEntities.Count == 0)
            {
                return false;
            }
            if (playerEntities.Count == 1)
            {
                fd.Player.gamePlay.CameraEntityId = playerEntities[0].playerInfo.EntityId;
                return true;
            }
            playerEntities.Sort((x, y) =>
            {
                if (x.playerInfo.Num > y.playerInfo.Num) return 1;
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

        private static bool FindSomeOne(IEventArgs args, FreeData fd, bool wiseFlag)
        {
            foreach (PlayerEntity p in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (CanOb(fd.Player, p) && p.playerInfo.Camp != fd.Player.playerInfo.Camp)
                {
                    if (wiseFlag)
                    {
                        if (p.entityKey.Value.EntityId > fd.Player.gamePlay.CameraEntityId)
                        {
                            fd.Player.gamePlay.CameraEntityId = p.entityKey.Value.EntityId;
                            return true;
                        }
                    }
                    else
                    {
                        if (p.entityKey.Value.EntityId < fd.Player.gamePlay.CameraEntityId)
                        {
                            fd.Player.gamePlay.CameraEntityId = p.entityKey.Value.EntityId;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool CanOb(PlayerEntity p, PlayerEntity candidate)
        {
            return p != candidate && !candidate.gamePlay.IsDead();
        }
    }
}
