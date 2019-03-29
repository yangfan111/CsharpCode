using Assets.Sources.Free.Auto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Client.GameModules.GamePlay.Free.Auto
{
    public class AutoServerTimeValue : IAutoValue
    {
        public int unit;
        public float scale;
        public bool desc;

        private int currentValue;

        private int ms;


        private bool started;

        private int MINUTE = 60000;
        private int HOUR = 3600000;
        private int DAY = 86400000;

        public object Frame(int frameTime)
        {
            float t = 0;
            if (started)
            {

                if (desc)
                    currentValue -= (int)(frameTime * scale);
                else
                    currentValue += (int)(frameTime * scale);
                switch (unit)
                {
                    case 1:
                        t = currentValue % 1000;
                        break;
                    case 2:
                        t = currentValue / 1000 % 60;
                        break;
                    case 3:
                        t = currentValue / MINUTE % 60;
                        break;
                    case 4:
                        t = currentValue / HOUR % 24;
                        break;
                    case 5:
                        t = currentValue / DAY;
                        break;
                    default:
                        t = 0;
                        break;
                }
            }
            return (int)t;
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split('|');
            if (ss.Length == 4 && ss[0] == "time-auto")
            {
                var at = new AutoTimeUnitValue();
                at.unit = int.Parse(ss[1]);
                at.scale = float.Parse(ss[2]);
                at.desc = "true" == ss[3];

                return at;
            }
            return null;
        }

        public bool Started
        {
            get
            {
                return started;
            }
        }

        public void Start()
        {
            started = true;
        }

        public void Stop()
        {

            started = false;

        }

        public void SetValue(params object[] v)
        {
            currentValue = Convert.ToInt32(v[0]);

        }
    }
}
