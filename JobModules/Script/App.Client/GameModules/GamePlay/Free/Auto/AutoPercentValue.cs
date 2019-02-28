using Assets.Sources.Free.Utility;
using System;

namespace Assets.Sources.Free.Auto
{
    public class AutoPercentValue : IAutoValue
    {
        private bool started;

        private IAutoValue max;
        private IAutoValue current;

        private float currentValue;

        public AutoPercentValue()
        {
        }

        public Object Frame(int frameTime)
        {
            if (started)
            {
                var cu = Convert.ToSingle(current.Frame(frameTime));
                var per = cu / Convert.ToSingle(max.Frame(frameTime));

                currentValue = per;
            }
            if (float.IsNaN(currentValue))
                currentValue = 1;
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
            if (ss.Length > 2 && ss[0] == "percent")
            {
                var at = new AutoPercentValue();

                var temp = config.Substring(config.IndexOf("|") + 1);
                var ats = temp.Split("->");

                at.current = AutoValue.Parse(ats[0]);
                at.max = AutoValue.Parse(ats[1]);

                return at;
            }

            return null;
        }

        public void Start()
        {
            started = true;
            current.Start();
            max.Start();
        }

        public void Stop()
        {

            this.started = false;

            current.Stop();

            max.Stop();

        }

        public void SetValue(params object[] v)
        {
            if (v.Length >= 1)
            {

                this.current.SetValue(v[0]);

            }
            if (v.Length >= 2)
            {

                this.max.SetValue(v[1]);

            }


            this.currentValue = Convert.ToSingle(current.Frame(0)) / Convert.ToSingle(max.Frame(0));

        }
    }
}
