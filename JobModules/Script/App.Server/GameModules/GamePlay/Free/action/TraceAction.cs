using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using UnityEngine;
using App.Server.GameModules.GamePlay.Free.map.position;
using com.wd.free.para;
using App.Shared.Configuration;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class UnityTraceAction : AbstractGameAction
    {
        public enum TraceType
        {
            Ground = 0,
            Water = 1
        }

        private IPosSelector from;
        private IPosSelector to;

        private int type;

        private IGameAction action;
        private IGameAction noHitAction;

        public override void DoAction(IEventArgs args)
        {
            Vector3 fromV = UnityPositionUtil.ToVector3(from.Select(args));
            Vector3 toV = UnityPositionUtil.ToVector3(to.Select(args));

            Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

            RaycastHit hitInfo;
            bool hited = Physics.Raycast(r, out hitInfo);

            if(type == (int)TraceType.Water)
            {
                float sur = SingletonManager.Get<MapConfigManager>().DistanceAboveWater(fromV);
                if(sur > 0)
                {
                    hitInfo.point = new Vector3(fromV.x, fromV.y - sur, fromV.z);
                    hited = true;
                }
            }

            args.GetDefault().GetParameters().TempUse(new FloatPara("fx", fromV.x));
            args.GetDefault().GetParameters().TempUse(new FloatPara("fy", fromV.y));
            args.GetDefault().GetParameters().TempUse(new FloatPara("fz", fromV.z));
            args.GetDefault().GetParameters().TempUse(new FloatPara("tx", toV.x));
            args.GetDefault().GetParameters().TempUse(new FloatPara("ty", toV.y));
            args.GetDefault().GetParameters().TempUse(new FloatPara("tz", toV.z));

            if (hited)
            {
                args.GetDefault().GetParameters().TempUse(new FloatPara("x", hitInfo.point.x));
                args.GetDefault().GetParameters().TempUse(new FloatPara("y", hitInfo.point.y));
                args.GetDefault().GetParameters().TempUse(new FloatPara("z", hitInfo.point.z));

                if (action != null)
                {
                    action.Act(args);
                }

                args.GetDefault().GetParameters().Resume("x");
                args.GetDefault().GetParameters().Resume("y");
                args.GetDefault().GetParameters().Resume("z");

            }
            else
            {
                if (noHitAction != null)
                {
                    noHitAction.Act(args);
                }
            }

            args.GetDefault().GetParameters().Resume("fx");
            args.GetDefault().GetParameters().Resume("fy");
            args.GetDefault().GetParameters().Resume("fz");
            args.GetDefault().GetParameters().Resume("tx");
            args.GetDefault().GetParameters().Resume("ty");
            args.GetDefault().GetParameters().Resume("tz");
        }
    }
}
