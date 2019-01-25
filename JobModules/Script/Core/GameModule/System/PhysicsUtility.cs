using UnityEngine;

namespace Core.GameModule.System
{
    public class PhysicsUtility
    {
#if UNITY_2017_1_OR_NEWER || UNITY_SOURCE_MODIFIED
        public static bool IsAutoSimulation = Physics.autoSimulation;
#else
        public static bool IsAutoSimulation = false;
#endif

        public static void SetAutoSimulation(bool enabled)
        {
#if UNITY_2017_1_OR_NEWER || UNITY_SOURCE_MODIFIED
            Physics.autoSimulation = enabled;
            IsAutoSimulation = enabled;
#endif
        }


        public static void Simulate(float step)
        {
#if UNITY_2017_1_OR_NEWER || UNITY_SOURCE_MODIFIED
            Physics.Simulate(step);
#endif
        }

    }
}
