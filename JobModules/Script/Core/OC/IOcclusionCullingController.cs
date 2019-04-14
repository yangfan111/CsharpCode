using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Core.OC
{
    public abstract class OCParam
    {
        public byte[] OCData;
    }

    public class OcclusionRunningState
    {
        public static bool HasPVSData;
        public static bool OcclusionEnabled;
    }

    public interface IOcclusionCullingController : IDisposable
    {
        void DoCulling(Vector3 position);
    }
}
