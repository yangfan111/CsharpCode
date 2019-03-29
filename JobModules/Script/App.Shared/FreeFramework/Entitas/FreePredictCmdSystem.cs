using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Player;
using com.wd.free.@event;
using com.wd.free.skill;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Util;
using UnityEngine;
using com.wd.free.action;

namespace App.Shared.FreeFramework.Entitas
{
    public class FreePredictCmdSystem : AbstractUserCmdExecuteSystem
    {
        private Contexts contexts;

        public FreePredictCmdSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        protected override bool filter(PlayerEntity player)
        {
            return player.hasGamePlay && player.hasFreeData;
        }

        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {
            ISkillArgs args = (ISkillArgs)contexts.session.commonSession.FreeArgs;
            if (SharedConfig.IsServer)
            {
                FreeLog.Reset();  
            }
            args.GetInput().SetUserCmd(cmd);
            //if(player.gamePlay.LifeState != (int)EPlayerLifeState.Dead)
            //{
                FreeData fd = ((FreeData)player.freeData.FreeData);
                args.TempUse("current", fd);

                /*if (cmd.IsPDown)
                {
                    Debug.LogFormat("p down {0}, is server {1}", cmd.Seq, SharedConfig.IsServer);
                }*/
                fd.GetUnitSkill().Frame(args);

                args.Resume("current");
            //}

            if (SharedConfig.IsServer)
            {
                FreeLog.Print();
            }
        }

    }
}
