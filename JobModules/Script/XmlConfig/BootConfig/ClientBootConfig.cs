using System;
using System.Xml.Serialization;

namespace XmlConfig.BootConfig
{
    

    [Serializable]
    public class ClientInfo
    {
        public string IP;
        public int TcpPort;
        public int UdpPort;

        public override string ToString()
        {
            return string.Format("Ip: {0}, TcpPort: {1}, UdpPort: {2}", IP, TcpPort, UdpPort);
        }
    }
    [XmlType("config")]
    [Serializable]
    public class ClientBootConfig:AbstractBootConfig
    {
       

        public ClientInfo BattleServer;

        public ClientInfo HallServer;

        public int HttpPort;

        public int MapId;// ֻ��offline����

        public bool ArtMode;

        public override string ToString()
        {
            return string.Format("Resource: {0}, BattleServer: {1}, HallServer: {2}, HttpPort: {3}, MapId: {4}", Resource, BattleServer, HallServer, HttpPort, MapId);
        }
    }
}