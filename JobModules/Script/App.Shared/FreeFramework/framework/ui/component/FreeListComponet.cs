using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui;

namespace gameplay.gamerule.free.ui.component
{
    [System.Serializable]
    public class FreeListComponet : AbstractFreeComponent
    {
        private const long serialVersionUID = -7699020163820046853L;

        private IList<IFreeComponent> list;

        private string column;

        private string row;

        public override int GetKey(IEventArgs args)
        {
            return LIST;
        }

        public override string GetStyle(IEventArgs args)
        {
            return FreeUtil.ReplaceNumber(column, args) + "_" + FreeUtil.ReplaceNumber(row, args);
        }

        public override IFreeUIValue GetValue()
        {
            return null;
        }

        public override SimpleProto CreateChildren(IEventArgs args)
        {
            SimpleProto b = FreePool.Allocate();
            FreeUICreateAction.Build(b, args, "0", "", false, false, list.AsIterable());

            return b;
        }
    }
}
