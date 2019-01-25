using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;

namespace com.wd.free.util
{
    public class FreeRandomReplacer : IFreeReplacer
    {
        private const string RANDOM = "random:";

        public virtual bool CanHandle(string exp, IEventArgs args)
        {
            return exp.StartsWith(RANDOM);
        }

        public virtual string Replace(string exp, IEventArgs args)
        {
            string value = exp.Replace(RANDOM, FreeUtil.EMPTY_STRING);
            string[] vs = StringUtil.Split(value, "_");
            if (vs.Length == 2 && StringUtil.IsNumberString(vs[0]) && StringUtil.IsNumberString(vs[1]))
            {
                return RandomUtil.Random(NumberUtil.GetInt(vs[0]), NumberUtil.GetInt(vs[1])).ToString();
            }
            else
            {
                vs = StringUtil.Split(value, "-");
                if (vs.Length == 2 && StringUtil.IsNumberString(vs[0]) && StringUtil.IsNumberString(vs[1]))
                {
                    return RandomUtil.Random(NumberUtil.GetInt(vs[0]), NumberUtil.GetInt(vs[1])).ToString();
                }
                else
                {
                    return vs[RandomUtil.Random(0, vs.Length - 1)];
                }
            }
        }
    }
}
