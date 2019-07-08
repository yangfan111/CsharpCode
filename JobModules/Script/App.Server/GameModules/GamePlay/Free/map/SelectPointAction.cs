using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.exception;
using com.wd.free.map.position;
using com.wd.free.para;
using com.wd.free.unit;
using Core.Free;

namespace gameplay.gamerule.free.map
{
    [System.Serializable]
    public class SelectPointAction : AbstractGameAction, IRule
    {
        private const long serialVersionUID = 3615114353547135092L;

        private IPosSelector pos;

        private IGameAction action;

        private int count;

        public override void DoAction(IEventArgs args)
        {
            if (count == 0)
            {
                count = 1;
            }
            UnitPosition[] ups = null;
            if (count == 1)
            {
                UnitPosition sub = pos.Select(args);
                if (sub == null)
                {
                    throw new GameActionExpception("找不到pos点[" + pos.ToString() + "]");
                }

                ups = new UnitPosition[] { sub };
            }
            else
            {
                ups = pos.Select(args, count);
            }
            int index = 0;
            foreach (UnitPosition up in ups)
            {
                args.GetDefault().GetParameters().TempUse(new FloatPara("x", up.GetX()));
                args.GetDefault().GetParameters().TempUse(new FloatPara("y", up.GetY()));
                args.GetDefault().GetParameters().TempUse(new FloatPara("z", up.GetZ()));
                args.GetDefault().GetParameters().TempUse(new IntPara("index", ++index));
                args.GetDefault().GetParameters().TempUse(new BoolPara("invalid", up.GetInvalid()));
                args.GetDefault().GetParameters().TempUse(new IntPara("randomindex", up.GetRandomindex()));
                action.Act(args);
                args.GetDefault().GetParameters().Resume("x");
                args.GetDefault().GetParameters().Resume("y");
                args.GetDefault().GetParameters().Resume("z");
                args.GetDefault().GetParameters().Resume("index");
                args.GetDefault().GetParameters().Resume("invalid");
                args.GetDefault().GetParameters().Resume("randomindex");
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.SelectPointAction;
        }
    }
}
