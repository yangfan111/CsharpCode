using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Core.GameTime;
using System;

namespace Assets.Sources.Free.Auto
{
    public class AutoOneTimeValue : IAutoValue
    {

        public int from;
        public int to;
        public float step;
        public int totalTime;

        private bool ended;

        private int currentValue;
        private int ms;

        private bool started;

        private long lastTime;

        public AutoOneTimeValue()
        {
        }

        public Object Frame(int frameTime)
        {
            if (started)
            {
                int frame = (int)(DateTime.Now.Ticks - lastTime) / 10000;

                if (from > to)
                {
                    ms = ms - frame;
                    if (ms < to * step)
                    {
                        ms = (int)(to * step);
                        currentValue = to;
                    }
                    else
                    {
                        currentValue = (int)(ms / step);
                    }
                }
                else if (from < to)
                {
                    ms = ms + frame;
                    if (ms > (to + 1) * step)
                    {
                        ms = (int)((to + 1) * step);
                        currentValue = to;
                    }
                    else
                    {
                        currentValue = (int)(ms / step);
                    }
                }
                else
                {
                    currentValue = from;
                }

                lastTime = DateTime.Now.Ticks;
            }

            return currentValue;
        }

        public bool Started
        {
            get
            {
                return started;
            }
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if (ss.Length >= 4 && ss[0] == "timeonce")
            {
                var at = new AutoOneTimeValue();
                at.from = Convert.ToInt32(ss[2]);
                at.to = Convert.ToInt32(ss[3]);

                at.totalTime = Convert.ToInt32(ss[1]);
                at.step = at.totalTime / Convert.ToSingle(Math.Abs(at.from - at.to));

                at.currentValue = at.from;
                at.ms = (int)(at.step * at.from);

                return at;
            }
            return null;
        }

        public void Start()
        {
            this.started = true;
            lastTime = DateTime.Now.Ticks;
        }

        public void Stop()
        {

            this.started = false;

        }

        public void SetValue(params object[] v)
        {
            var ss = Convert.ToString(v[0]).Split(",");
            if (ss.Length == 4)
            {

                this.currentValue = Convert.ToInt32(ss[3]);

                this.from = Convert.ToInt32(ss[1]);

                this.to = Convert.ToInt32(ss[2]);

                this.totalTime = Convert.ToInt32(ss[0]);

                this.step = totalTime / Convert.ToSingle(Math.Abs(this.from - this.to));

                this.ms = (int)(currentValue * step);

            }
            else if (ss.Length == 1)
            {
                this.currentValue = (int)Convert.ToDouble(ss[0]);
                this.ms = (int)(currentValue * step);
            }
        }
    }
}
