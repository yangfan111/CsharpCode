using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
    public interface IFreeComponent
    {
        int GetKey(IEventArgs args);

        string GetWidth(IEventArgs args);

        string GetHeight(IEventArgs args);

        string GetX(IEventArgs args);

        string GetY(IEventArgs args);

        string GetRelative(IEventArgs args);

        string GetParent(IEventArgs args);

        string GetEvent(IEventArgs args);

        IList<IFreeUIAuto> GetAutos(IEventArgs args);

        IFreeUIValue GetValue();

        string GetStyle(IEventArgs args);

        SimpleProto CreateChildren(IEventArgs args);
    }

    public static class IFreeComponentConstants
    {
        public const int IMAGE = 1;

        public const int TEXT = 2;

        public const int NUMBER = 3;

        public const int LIST = 4;

        public const int RADER = 5;

        public const int EXP = 6;

        public const int SMAP = 7;

        public const int RIMAGE = 8;
    }
}
