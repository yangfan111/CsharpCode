using Assets.Sources.Free.Utility;
using System;

namespace Assets.Sources.Free.Auto
{
    public class AutoTimeValue : IAutoValue
    {

        public int from;
        public int to;
        public float step;
        private bool reverse;

        private int currentValue;
        private int ms;

        private long lastTime;

        private bool started;

        public AutoTimeValue()
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
                        if (reverse)
                        {
                            var temp = from;
                            from = to;
                            to = temp;
                        }
                        else
                        {
                            ms = (int)(ms + (from - to + 1) * step);
                        }
                    }
                    currentValue = (int)(ms / step);
                }
                else if (from < to)
                {
                    ms = ms + frame;
                    if (ms > (to + 1) * step)
                    {
                        if (reverse)
                        {
                            var temp = from;
                            from = to;
                            to = temp;
                        }
                        else
                        {
                            ms = (int)(ms - (to - from + 1) * step);
                        }
                    }
                    currentValue = (int)(ms / step);
                }
                else
                {
                    currentValue = from;
                }
            }

            lastTime = DateTime.Now.Ticks;

            return currentValue;
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if (ss.Length >= 4 && ss[0] == "time")
            {
                var at = new AutoTimeValue();
                at.from = Convert.ToInt32(ss[2]);
                at.to = Convert.ToInt32(ss[3]);

                at.step = Convert.ToSingle(ss[1]) / Convert.ToSingle(Math.Abs(at.from - at.to));

                at.reverse = "true" == ss[4];

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

            this.started = true;
            this.lastTime = DateTime.Now.Ticks;
        }

        public void Stop()
        {

            this.started = false;

        }

        public void SetValue(params object[] v)
        {

            this.currentValue = (int)Convert.ToSingle(v[0]);

            this.ms = (int)(currentValue * step);

        }

    }
}
