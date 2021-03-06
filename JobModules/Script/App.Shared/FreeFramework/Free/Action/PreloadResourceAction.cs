using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using Core.Room;
using Core.Utils;

namespace App.Shared.FreeFramework.Free.Action
{
    [Serializable]
    public class PreloadResourceAction : AbstractGameAction, IRule
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PreloadResourceAction));

        public string ui;
        public string weapon;
        public string car;
        public string avatar;
        public string item;

        public override void DoAction(IEventArgs args)
        {
            RoomInfo info = args.GameContext.session.commonSession.RoomInfo;

            PreloadUI(args, info);
        }

        private void PreloadUI(IEventArgs args, RoomInfo info)
        {
            HashSet<string> set = new HashSet<string>();

            if (!string.IsNullOrEmpty(info.PreLoadUI) && info.PreLoadUI != "null")
            {
                string[] uis = info.PreLoadUI.Split(',');
                foreach (string u in uis)
                {
                    if (!string.IsNullOrEmpty(u))
                    {
                        set.Add(u.Trim());
                    }
                }
            }

            if (!string.IsNullOrEmpty(ui))
            {
                string[] uis = ui.Split(',');
                foreach (string u in uis)
                {
                    if (!string.IsNullOrEmpty(u))
                    {
                        set.Add(u.Trim());
                    }
                }
            }

            info.PreLoadUI = string.Join(",", set.ToArray());

            _logger.Info("已预加载资源：" + ui);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PreloadResourceAction;
        }
    }
}
