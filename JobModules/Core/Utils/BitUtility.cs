using Core.Utils;
using System.Collections.Generic;

namespace Core.Utils
{
    public static class BitUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BitUtility));
        public static bool ReverseBit(ref uint flag, byte pos)
        {
            if(pos > 31 || pos < 0)
            {
                Logger.ErrorFormat("illegal pos {0} to reverse", pos);
                return false;
            }
            uint mask1 = (uint)1 << pos;
            var mask2 = 0xFFFFFFFF ^ mask1;
            var tmp = flag + mask1;
            tmp &= mask1;
            flag = flag & mask2;
            flag += tmp;
            return true;
        }

        public static void Diff (uint left, uint right, List<byte> list)
        {
            if(null == list)
            {
                return;
            }
            var diff = left ^ right;
            list.Clear();
            byte i = 0;
            while(diff > 0)
            {
                if(diff % 2 > 0)
                {
                    list.Add(i);
                }
                i++;
                diff >>= 1;
            }
        }
    }
}
