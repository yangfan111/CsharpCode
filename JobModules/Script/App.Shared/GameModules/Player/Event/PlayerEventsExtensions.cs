using System;
using Core.Event;
using Core.Utils;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.Event
{
    public static class PlayerEventsExtensions
    {
        public static bool[] EventSwitch;
        private static CustomProfileInfo[] _infos;

        static PlayerEventsExtensions()
        {
            EventSwitch = new bool[(int) EEventType.End];
            _infos = new CustomProfileInfo[(int) EEventType.End];
            for (int i = 0; i < EventSwitch.Length; i++)
            {
                EventSwitch[i] = true;
                _infos[i] = SingletonManager.Get<DurationHelp>()
                    .GetCustomProfileInfo(string.Format("event{0}", (EEventType) i));
            }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerEventsExtensions));

        public static void DoAllEvent(this PlayerEvents events, Contexts contexts, PlayerEntity entity, bool isServer)
        {
            foreach (var evt in events.Events)
            {
                var handler = EventInfos.Instance.GetEventHandler(evt.EventType);
                try
                {
                    _infos[(int) evt.EventType].BeginProfileOnlyEnableProfile();
                    if (isServer)
                    {
                        if (handler.ServerFilter(entity, evt))
                        {
                            _logger.DebugFormat("Do Handler:{0} {1}", evt.EventType, evt.IsRemote);
                            handler.DoEventServer(contexts, entity, evt);
                        }
                        else
                        {
                            _logger.DebugFormat("Skip Handler:{0} {1}", evt.EventType, evt.IsRemote);
                        }
                    }
                    else
                    {
                        if (handler.ClientFilter(entity, evt) && EventSwitch[(int) evt.EventType])
                        {
                            _logger.DebugFormat("Do Handler:{0} {1}", evt.EventType, evt.IsRemote);
                            handler.DoEventClient(contexts, entity, evt);
                        }
                        else
                        {
                            _logger.DebugFormat("Skip Handler:{0} {1}", evt.EventType, evt.IsRemote);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Handler:{0} {1}", evt.EventType, e);
                }
                finally
                {
                    _infos[(int) evt.EventType].EndProfileOnlyEnableProfile();
                }
            }
        }
    }
}