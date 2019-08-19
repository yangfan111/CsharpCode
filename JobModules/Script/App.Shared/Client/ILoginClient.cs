using System;
using System.Net.Sockets;

namespace App.Shared.Client
{
    public interface ILoginClient : IDisposable
    {
        void ReConnect();
        void Update();
        void FlowTick(float time);
        void CloseConnect(ProtocolType protocolType);
    }
}