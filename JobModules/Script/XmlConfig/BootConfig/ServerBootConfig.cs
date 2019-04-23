using System;
using System.Xml.Serialization;

namespace XmlConfig.BootConfig
{
    [Serializable]
    public class ServerInfo
    {
        public int TcpPort;
        public int UdpPort;

        public override string ToString()
        {
            return string.Format("TcpPort: {0}, UdpPort: {1}", TcpPort, UdpPort);
        }
    }

    [Serializable]
    public class RoomServerInfo
    {
        public int ListenPort;

        public override string ToString()
        {
            return string.Format("ListenPort: {0}", ListenPort);
        }
    }

    [Serializable]
    public class AllocationServerInfo
    {
        public int Open;
        public string ConnectIp;
        public int ConnectPort;

        public override string ToString()
        {
            return string.Format("Open: {0}, ConnectIp: {1}, ConnectPort: {2}", Open, ConnectIp, ConnectPort);
        }
    }

    [XmlType("config")]
    [Serializable]
    public class ServerBootConfig:AbstractBootConfig
    {
        

        public int Id;

        public string Ip;

        public ServerInfo BattleServer;

        public RoomServerInfo HallRoomServer;

        public AllocationServerInfo AllocationServer;

        public int HttpPort;

        public int MapId;

        public string Rule;

        public bool Mysql;

        public override string ToString()
        {
            return string.Format("Resource: {0}, Id: {1}, Ip: {2}, BattleServer: {3}, HallRoomServer: {4}, AllocationServer: {5}, HttpPort: {6}, MapId: {7}, Rule: {8}",
                Resource, Id, Ip, BattleServer, HallRoomServer, AllocationServer, HttpPort, MapId, Rule);
        }
    }
}