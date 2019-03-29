using System;

namespace Sharpen
{
    public static class ExceptionExtension
    {
        static StackTraceElement[] dummp = new StackTraceElement[3];
        public static StackTraceElement[] GetStackTrace(this Exception e)
        {
            return dummp;
        }
    }

    public class StackTraceElement
    {
        public string GetClassName()
        {
            return "";
        }

        public string GetMethodName()
        {
            return "";
        }
    }
}