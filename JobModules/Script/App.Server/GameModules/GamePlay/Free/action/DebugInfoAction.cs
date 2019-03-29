using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.para;
using com.cpkf.yyjd.tools.util;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class DebugInfoAction : AbstractGameAction
    {
        private string key;

        private IGameAction action;

        public override void DoAction(IEventArgs args)
        {
            string realKey = FreeUtil.ReplaceVar(key, args);
            if (realKey == "sc")
            {
                int c = args.GameContext.sceneObject.GetEntities().Length;

                args.TempUsePara(new StringPara("info", c.ToString()));

                action.Act(args);

                args.ResumePara("info");
            }

            if (realKey == "mp")
            {
               List< MapConfigPoints.ID_Point>  ps = MapConfigPoints.current.IDPints;

                List<string> list = new List<string>();
                foreach (MapConfigPoints.ID_Point p in ps)
                {
                    list.Add(p.ID + "=" + p.points.Count);
                }

                args.TempUsePara(new StringPara("info", StringUtil.GetStringFromStrings(list, ", ")));

                action.Act(args);

                args.ResumePara("info");
            }
        }
    }
}
