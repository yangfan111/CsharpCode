using System.Collections.Generic;
using Assets.Sources.Free.Utility;

namespace Assets.Sources.Free.Auto
{
    public class AutoTimeStringValue : IAutoValue
    {
        private IList<string> values;
        public int totalTime;

        private int currentTime;
        private string currentValue;

        private bool started;

        public AutoTimeStringValue()
        {
            values = new List<string>();
        }

        public string GetUrl(int index)
        {
            return values[index];
        }

        public int Size()
        {
            return values.Count;
        }

        public object Frame(int frameTime)
        {
            if (started)
            {
                if (currentTime >= totalTime)
                    currentTime = currentTime - totalTime;

                var index = currentTime * values.Count / totalTime;

                currentTime = currentTime + frameTime;

                return values[index];
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
            var ss = config.Split("_$$$_");
            if (ss.Length > 2 && ss[0] == "string")
            {
                var at = new AutoTimeStringValue();
                at.totalTime = int.Parse(ss[1]);
                for (var i = 2; i < ss.Length; i++)
                    at.values.Add(ss[i]);

                return at;
            }
            return null;
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
            currentValue = v[0] as string;
        }
    }
}
