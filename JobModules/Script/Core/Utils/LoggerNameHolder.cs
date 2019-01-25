using System;
using Core.Network;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.Prediction.VehiclePrediction;
using Core.Prediction.VehiclePrediction.TimeSync;

namespace Core.Utils
{
    public static class LoggerNameHolder<TWrappingClass>
    {
        private static string _loggerName;
        public static string LoggerName
        {
            get
            {
                if (_loggerName == null)
                {
                    _loggerName = GetLoggerName(typeof(TWrappingClass));
                }
                return _loggerName;
            }
        }

        private static string GetLoggerName(Type type)
        {
            string ns = type.Namespace;
            if (ns.Contains(typeof(ISimulationTimer).Namespace))
            {
                return "prediction.vehicle.time." + type.Name;
            }
            else if (ns.Contains(typeof(IVehiclePredictionComponent).Namespace))
            {
                return "prediction.vehicle." + type.Name;
            }
            else if (ns.Contains(typeof(IUserPredictionComponent).Namespace))
            {
                return "prediction.user." + type.Name;
            }
            else if (ns.Contains(typeof(IPlaybackComponent).Namespace))
            {
                return "playback." + type.Name;
            }
            else if (ns.Contains(typeof(INetworkChannel).Namespace))
            {
                return "network." + type.Name;
            }
            
            return type.FullName;
        }
    }
}