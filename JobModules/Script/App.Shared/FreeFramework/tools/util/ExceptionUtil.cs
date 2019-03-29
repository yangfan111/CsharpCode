using System;
using System.IO;
using Sharpen;

namespace com.cpkf.yyjd.tools.util
{
	public class ExceptionUtil
	{
		public static string GetExceptionContent(Exception e)
		{
			StringWriter sw = new StringWriter();
			Sharpen.Runtime.PrintStackTrace(e, new PrintWriter(sw));
			string content = sw.ToString();
			try
			{
				sw.Close();
			}
			catch (IOException e1)
			{
				Sharpen.Runtime.PrintStackTrace(e1);
			}
			return content;
		}
	}
}
