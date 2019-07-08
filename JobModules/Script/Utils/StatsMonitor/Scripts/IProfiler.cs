using System;


namespace StatsMonitor
{
    public interface IProfiler
    {
        string Name
        {
            get;
        }

        float SampleValue();

        float MaxValue
        {
            get;
        }

    }

    public class DummyProfiler : IProfiler
    {
        public string Name
        {
            get
            {
                return "Test";
            }
        }

        public float MaxValue
        {
            get { return 200f; }
        }


        public float SampleValue()
        {
            return UnityEngine.Random.Range(0, 500);
        }
    }
}
