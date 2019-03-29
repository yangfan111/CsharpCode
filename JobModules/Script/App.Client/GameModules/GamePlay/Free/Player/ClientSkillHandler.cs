using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Free.framework;
using Core.Free;
using Assets.Sources.Free.UI;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.skill;
using com.cpkf.yyjd.tools.util;
using com.wd.free.config;
using com.wd.free.xml;
using Core.Utils;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Entity
{
    public class ClientSkillHandler : ISimpleMesssageHandler
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientSkillHandler));


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.ClientSkill;
        }

        public void Handle(SimpleProto data)
        {
            PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            if (player != null)
            {
                FreeData fd = (FreeData)player.freeData.FreeData;
                if (fd != null)
                {
                    byte[] bs = new byte[data.Ins.Count];
                    for (int i = 0; i < bs.Length; i++)
                    {
                        bs[i] = (byte)data.Ins[i];
                    }
                    try
                    {
                        List<ISkill> skills = (List<ISkill>)SerializeUtil.ByteToObject(bs);
                        foreach (var skill in skills)
                        {
                            fd.GetUnitSkill().AddSkill(skill);
                        }
                    }catch(Exception e)
                    {
                        _logger.ErrorFormat("client skill initial failed.\n{0}", e.StackTrace);
                    }
                   
                }
            }
        }
    }
}
