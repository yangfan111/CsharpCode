using System;
using Core.Event;
using Core.Utils;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.Event
{
    public static class PlayerEventsExtensions
    {
        public static bool []EventSwitch;
        private static CustomProfileInfo []_infos;
        static PlayerEventsExtensions()
        {
            EventSwitch = new bool[(int)EEventType.End];
            _infos = new CustomProfileInfo[(int)EEventType.End];
            for (int i = 0; i < EventSwitch.Length; i++)
            {
                EventSwitch[i] = true;
                _infos[i] = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo(string.Format("event{0}", (EEventType) i));
            }
        }

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerEventsExtensions));
        public static  void DoAllEvent(this PlayerEvents events, Contexts contexts, PlayerEntity entity,  bool isServer)
        {
            foreach (var v in events.Events.Values)
            {
                var handler = EventInfos.Instance.GetEventHandler(v.EventType);

                try
                {
                    _infos[(int) v.EventType].BeginProfileOnlyEnableProfile();
                    if (isServer)
                    {
                        if (handler.ServerFilter(entity, v))
                        {
                            handler.DoEventServer(contexts, entity, v);
                        }
                        else
                        {
                            _logger.DebugFormat("Skip Handler:{0} {1}", v.EventType, v.IsRemote);
                        }
                    }
                    else
                    {
                        if (handler.ClientFilter(entity, v) && EventSwitch[(int) v.EventType])
                        {
                            handler.DoEventClient(contexts, entity, v);
                        }
                        else
                        {
                            _logger.DebugFormat("Skip Handler:{0} {1}", v.EventType, v.IsRemote);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("Handler:{0} {1}", v.EventType, e);
                }
                finally
                {
                    _infos[(int) v.EventType].EndProfileOnlyEnableProfile();
                }
            }
        }
    }
}