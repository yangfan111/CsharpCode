using UnityEngine;

namespace Core.Utils
{
    public class UnityLogCapturer
    {
#if !UNITY_EDITOR
        public static bool UnityToLog4Net = true;
#else
        public static bool UnityToLog4Net = true;
#endif
        private static LoggerAdapter _logger;
        public static void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (UnityToLog4Net)
            {
                if (_logger == null)
                {
                    _logger = new LoggerAdapter("Unity");
                }

                if (type == LogType.Log && _logger.IsDebugEnabled)
                {
                    _logger.DebugFormat("{0} {1} {2}", type, condition, stackTrace);
                }
                else if (type == LogType.Warning && _logger.IsWarnEnabled)
                {
                    if(!condition.Contains("The referenced script on this Behaviour"))
                        _logger.WarnFormat("{0} {1} {2}", type, condition, stackTrace);
                }
                else if (_logger.IsErrorEnabled)
                {
                    _logger.ErrorFormat("{0} {1} {2}", type, condition, stackTrace);
                }
            }
        }
    }
}