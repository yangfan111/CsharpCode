using System;

namespace App.Shared.Components.ClientSession
{
    [Serializable]
    public class ServerStatus
    {
        public int TcpPing;
        public int UdpPing;
        public long LastTcpPing;
        public long LastUdpPing;
        public float AvgDelta;
        public float MaxDelta;
        public int Fps5;
        public int Fps30;
        public int Fps60;
        public int RecvCmdCount;
        public int GcCount;

        public override string ToString()
        {
            return string.Format("ad: {0}, md: {1}, fps: {2} {3} {4}, Gc:{5}", AvgDelta, MaxDelta, Fps5, Fps30, Fps60,  GcCount);
        }
    }
}