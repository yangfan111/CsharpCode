using App.Shared.Components;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Core.Room;
using System;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    [Serializable]
    public class CampChangeAction : AbstractGameAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            foreach (PlayerEntity player in args.GameContext.player.GetInitializedPlayerEntities())
            {
                player.playerInfo.Camp = 3 - player.playerInfo.Camp;
                player.gamePlay.CastState = 6 - player.gamePlay.CastState;

                if (player.playerInfo.CampInfo == null) continue;

                switch (GameRules.ClothesType(args.GameContext.session.commonSession.RoomInfo.ModeId))
                {
                    case (int) EGameModeClothes.SingleCamp:
                    case (int) EGameModeClothes.DualCamp:
                        if (player.playerInfo.CampInfo.CurrCamp != player.playerInfo.Camp)
                        {
                            player.playerInfo.CampInfo.CurrCamp = player.playerInfo.Camp;
                            foreach (Preset p in player.playerInfo.CampInfo.Preset)
                            {
                                if (p.camp == player.playerInfo.Camp)
                                {
                                    player.gamePlay.NewRoleId = p.roleModelId;
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public int GetRuleID()
        {
            return (int) ERuleIds.CampChangeAction;
        }
    }
}
