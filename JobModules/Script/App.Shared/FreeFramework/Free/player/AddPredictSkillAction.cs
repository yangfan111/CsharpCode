using Assets.App.Server.GameModules.GamePlay.Free;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.config;
using com.wd.free.@event;
using com.wd.free.skill;
using com.wd.free.xml;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class AddPredictSkillAction : AbstractPlayerAction
    {
        private List<ISkill> skills;

        public AddPredictSkillAction()
        {
            this.skills = new List<ISkill>();
        }

        public void AddSkill(ISkill skill)
        {
            this.skills.Add(skill);
        }

        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(this.player))
            {
                this.player = "current";
            }
            var unit = GetPlayer(args);
            if (skills != null && unit != null)
            {
                foreach (var skill in skills)
                {
                    unit.GetUnitSkill().AddSkill((ISkill)SerializeUtil.Clone(skill));
                }

                byte[] bs = SerializeUtil.ObjectToByte(skills);

                SimpleProto msg = FreePool.Allocate();
                msg.Key = FreeMessageConstant.ClientSkill;
                for(int i = 0; i < bs.Length; i++)
                {
                    msg.Ins.Add(bs[i]);
                }

                FreeMessageSender.SendMessage(unit.Player, msg);
            }
        }
    }
}
