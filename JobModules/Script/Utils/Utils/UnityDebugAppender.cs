using Core.Utils;
using log4net.Appender;
using log4net.Core;
using UnityEngine;

namespace Loxodon.Log.Appender
{
    public class UnityDebugAppender : AppenderSkeleton
    {
#if UNITY_EDITOR
        public static bool Log4NetToUnityTo = true;
#else
        public static bool Log4NetToUnityTo = false;
#endif
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (Log4NetToUnityTo)
            {
                log4net.Core.Level level = loggingEvent.Level;
                if (log4net.Core.Level.Fatal.Equals(level) || log4net.Core.Level.Error.Equals(level))
                    Debug.LogError(RenderLoggingEvent(loggingEvent));
                else if (log4net.Core.Level.Warn.Equals(level))
                    Debug.LogWarning(RenderLoggingEvent(loggingEvent));
            }
        }
    }
}
