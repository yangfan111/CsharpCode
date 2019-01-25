using Sharpen;

namespace com.wd.free.util
{
	public class BytesUtil
	{
		public static byte[] IntTobyte(int res)
		{
			byte[] targets = new byte[4];
			targets[3] = unchecked((byte)(res & unchecked((int)(0xff))));
			// 最低位
			targets[2] = unchecked((byte)((res >> 8) & unchecked((int)(0xff))));
			// 次低位
			targets[1] = unchecked((byte)((res >> 16) & unchecked((int)(0xff))));
			// 次高位
			targets[0] = unchecked((byte)((int)(((uint)res) >> 24)));
			// 最高位,无符号右移。
			return targets;
		}

		public static int ByteToint(byte[] res)
		{
			int targets = (res[3] & unchecked((int)(0xff))) | ((res[2] << 8) & unchecked((int)(0xff00))) | ((int)(((uint)(res[1] << 24)) >> 8)) | (res[0] << 24);
			// | 表示安位或
			return targets;
		}
	}
}
