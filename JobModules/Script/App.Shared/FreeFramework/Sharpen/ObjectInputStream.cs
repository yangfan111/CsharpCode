using System.Text;

namespace Sharpen
{
	using System;
	using System.IO;

	public class ObjectInputStream : InputStream
	{
		private BinaryReader reader;

		public ObjectInputStream (InputStream s)
		{
			this.reader = new BinaryReader (s.GetWrappedStream (),Encoding.UTF8);
		}

		public int ReadInt ()
		{
			return this.reader.ReadInt32 ();
		}

		public object ReadObject ()
		{
			throw new NotImplementedException ();
		}
	}
}
