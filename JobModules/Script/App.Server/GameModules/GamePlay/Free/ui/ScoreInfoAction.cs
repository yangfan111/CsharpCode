using System;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;
using gameplay.gamerule.free.ui;

namespace App.Server.GameModules.GamePlay.Free.action.ui
{
    [Serializable]
    public class ScoreInfoAction : SendMessageAction
    {
        protected override void BuildMessage(IEventArgs args)
        {
            this.scope = "4";

            builder.Key = FreeMessageConstant.ScoreInfo;

            //游戏是否开始
            builder.Bs.Add(FreeUtil.ReplaceBool("{gameStart}", args));
            //总玩家数
            builder.Ins.Add(FreeUtil.ReplaceInt("{startPlayerCount}", args));
            //存活者数
            builder.Ins.Add(FreeUtil.ReplaceInt("{playerCount}", args));


            //当前玩家击杀数（每个玩家不一样）
            if (args.GetUnit("current") != null)
            {
                builder.Ins.Add(FreeUtil.ReplaceInt("{current.killNum}", args));
            }
            else
            {
                builder.Ins.Add(FreeUtil.ReplaceInt("{current.killNum}", args));
            }
            

            //击杀者姓名
            if(args.GetUnit("killer") == null)
            {
                builder.Ss.Add(FreeUtil.ReplaceVar("", args));
                builder.Ds.Add(0);
                builder.Ins.Add(0);
                builder.Ins.Add(0);
            }
            else
            {
                //击杀者姓名
                builder.Ss.Add(FreeUtil.ReplaceVar("{killer.name}", args));
                //击杀者队伍ID
                builder.Ds.Add(FreeUtil.ReplaceDouble("{killer.id}", args));
                //击杀者武器ID
                builder.Ins.Add(FreeUtil.ReplaceInt("{killer.currentWeaponId}", args));
                //击杀方式
                builder.Ins.Add(2);
            }
           
            //死者姓名
            builder.Ss.Add(FreeUtil.ReplaceVar("{killed.name}", args));
            //死者队伍ID
            builder.Ds.Add(FreeUtil.ReplaceDouble("{killed.id}", args));
            //死亡方式
            builder.Ins.Add(1);
        }
    }
}
