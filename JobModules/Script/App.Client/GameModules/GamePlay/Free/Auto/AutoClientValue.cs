using System;
using System.Collections.Generic;
using Assets.Sources.Utils;
using Assets.Sources.Free.Utility;

namespace Assets.Sources.Free.Auto
{
    public class AutoClientValue : IAutoValue
    {
        private bool _started;

        private string _field;

        public static Dictionary<string, object> roomInfo = new Dictionary<string, object>();
        private const int IndividualMatch = 1;//个人匹配
        private const int FreedomAthletics = 0;//自由竞技

        public AutoClientValue()
        {
        }

        public Object Frame(int frameTime)
        {
            return GetPlayerField();
        }

        public bool Started
        {
            get
            {
                return _started;
            }
        }

        private Object GetPlayerField()
        {

            if (_field == "roomName")
            {
                if (roomInfo != null)
                {
                    var battleType = Convert.ToInt32(roomInfo.GetOrDefault("battleType"));
                    if (battleType == IndividualMatch)
                    {
                        return "个人联赛";
                    }
                    else
                    {
                        var roomId = Convert.ToInt32(roomInfo.GetOrDefault("roomId"));
                        return roomInfo.GetOrDefault("channelName") + "({1})号房间".Replace("{1}", roomId.ToString());
                    }
                }
            }

            if (_field == "roomMax")
            {
                if (roomInfo != null)
                {
                    return Convert.ToInt32(roomInfo.GetOrDefault("limitNumber"));
                }
            }

            return null;
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");

            if (ss.Length >= 2 && ss[0] == "client")
            {
                var at = new AutoClientValue();
                at._field = ss[1];

                return at;
            }

            return null;
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {

            this._started = false;

        }

        public void SetValue(params object[] vs)
        {

        }
    }
}
