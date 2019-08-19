using System;


namespace StatsMonitor
{
    
    //StatsMonitor.Instance.RegistProfiler(IProfiler);
    //StatsMonitor.Instance.UnRegistProfiler(IProfiler);
    public interface IProfiler
    {
        string Name
        {
            get;
        }

        float SampleValue();



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

       

        int timesToChange = 20;
        int count;
        int maxValue = 500;
        int minValue = 100;
        public float SampleValue()
        {
            count++;
            if(count>timesToChange)
            {
                if(UnityEngine.Random.Range(0, 10)%2==0)
                {
                    maxValue = 1000;
                    minValue = 300;
                }
                else
                {
                    maxValue = 350;
                    minValue = 0;
                }

                count = 0;
            }
            return UnityEngine.Random.Range(minValue, maxValue);
        }
    }
}
