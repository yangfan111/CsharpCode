using App.Protobuf;
using App.Shared;
using App.Shared.Network;
using Core.BulletSimulation;
using Core.Network;
using Core.Utils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Client.Bullet
{
    public class ClientBulletInfoCollector : IBulletInfoCollector
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientBulletInfoCollector));
        private INetworkChannel _networkChannel;
        private int _total;
        private int _miss;
        private string _serverFirInfo;

        public ClientBulletInfoCollector(INetworkChannel networkChannel)
        {
            _networkChannel = networkChannel;
        }

        public string GetStatisticData(int type)
        {
            switch(type)
            {
                case -1:
                    return _serverFirInfo;
                default:
                    return string.Format("total {0}, miss {1}, missPercent {2:0.00%}", _total, _miss, (float)_miss / Mathf.Max(1, _total));
            }
        }

        public void OnMissMatch(int seq)
        {
            Debug.LogFormat("OnMissMatch {0}", seq);
            _miss++; 
        }

        public void AddBulletData(int seq, Vector3 startPoint, Vector3 emitPoint, Vector3 startDir, Vector3 hitPoint, int hitType, INetworkChannel networkChannel)
        {
            _total += 1;
            var msg = FireInfoMessage.Allocate();
            msg.Seq = seq;
            msg.StartPoint = Vector3Converter.UnityToProtobufVector3(startPoint);
            msg.StartDir = Vector3Converter.UnityToProtobufVector3(startDir);
            msg.EmitPoint = Vector3Converter.UnityToProtobufVector3(emitPoint);
            msg.HitPoint = Vector3Converter.UnityToProtobufVector3(hitPoint);
            msg.HitType = hitType;
            if(null != _networkChannel)
            {
                _networkChannel.SendReliable((int)EClient2ServerMessage.FireInfo, msg);
            }
            else
            {
                Logger.Error("NetworkChannel is null");
            }
            msg.ReleaseReference();
        }

        public void SetServerFireInfo(string content)
        {
            _serverFirInfo = content;
        }

        public string GetServerFireInfo()
        {
            return _serverFirInfo;
        }
    }
}
