using System.Threading;

#if NET_4_6 && UNITY_2017
#else
namespace Core.Utils
{
    public class SpinWaitUtils
    {
        
#if (NET_4_6 && UNITY_2017)
public static System.Threading.SpinWait GetSpinWait()
        {
            return   System.Threading.SpinWait spin = new System.Threading.SpinWait();;
        }
           
#else
        public static Core.Utils.SpinWait GetSpinWait()
        {
            return  new Core.Utils.SpinWait();
        }

#endif
    }
    public struct SpinWait
    {
        internal const int YieldThreshold = 10; 
        private int _count;
        public void SpinOnce()
        {
            if (_count++ > YieldThreshold)
                Thread.Sleep(0);
            else
            {
                Thread.SpinWait(4 << _count);
            }
        }

        public void Reset()
        {
            _count = 0;
        }
    }
}
#endif