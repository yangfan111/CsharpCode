using Sharpen;

namespace gameplay.gamerule.free.ui
{
	/// <summary>
	/// 如果数据为1,2,3,4,5,6,7,8每四个bit保存一位数据
	/// 则字节为 0001 0010 0011 0100 0101 0110 0111 1000
	/// </summary>
	/// <author>Administrator</author>
	public class BitUtil
	{
		// values的长度 = 32 / bitCount, 此条件必须满足，
		// 仅能正确返回value为2的bitCount次方一下的数据
		public static int SetValue(int[] values, int bitCount)
		{
			int v = 0;
			for (int i = 0; i < values.Length; i++)
			{
				v |= SetBit(bitCount * i, values[i], bitCount);
			}
			return v;
		}

		// values的长度 = 32 / bitCount, 此条件必须满足，
		// 仅能正确返回value为2的bitCount次方一下的数据
		public static int[] GetValues(int v, int bitCount)
		{
			int[] vs = new int[32 / bitCount];
			for (int i = vs.Length - 1; i >= 0; i--)
			{
				vs[i] = GetBits(v, bitCount * (i + 1) - 1, bitCount);
			}
			return vs;
		}

		public static int SetBit(int startPos, int v, int bitCount)
		{
			bool[] bs = new bool[bitCount];
			for (int i = 0; i < bs.Length; i++)
			{
				bs[bs.Length - i - 1] = GetBit(v, i) == 1;
			}
			return SetBit(startPos, bs);
		}

		public static int SetBit(int startPos, bool[] bits)
		{
			int r = 0;
			for (int i = startPos; i < startPos + bits.Length; i++)
			{
				r |= SetBit(i, bits[startPos + bits.Length - i - 1]);
			}
			return r;
		}

		public static int SetBit(int pos, bool bit)
		{
			if (bit)
			{
				return 1 << pos;
			}
			else
			{
				return 0;
			}
		}

		public static int GetBit(int v, int pos)
		{
			return GetBits(v, pos, 1);
		}

		public static int GetBits(int v, int startPos, int len)
		{
			return (int)(((uint)(v << (31 - startPos))) >> (32 - len));
		}
	}
}
