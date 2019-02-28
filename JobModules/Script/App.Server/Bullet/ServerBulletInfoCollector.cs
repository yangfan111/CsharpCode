using App.Protobuf;
using App.Shared;
using Core.BulletSimulation;
using Core.Network;
using Core.Utils;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Server.Bullet
{
    public class ServerBulletInfoCollector : IBulletInfoCollector
    {

        private StringBuilder _stringBuilder = new StringBuilder();

        public ServerBulletInfoCollector()
        {
        }

        class BulletInfo
        {
            public int CmdSeq;
            public Vector3 StartPoint;
            public Vector3 EmitPoint;
            public Vector3 StartDir;
            public Vector3 HitPoint;
            public EHitType HitType;

            public bool IsMatched(BulletInfo bulletInfo)
            {
                return CmdSeq == bulletInfo.CmdSeq
                    && StartPoint == bulletInfo.StartPoint
                    && EmitPoint == bulletInfo.EmitPoint
                    && StartDir == bulletInfo.StartDir
                    && HitPoint == bulletInfo.HitPoint
                    && HitType == bulletInfo.HitType;
            }

            public override string ToString()
            {
                return string.Format("Seq {0}, StartPoint {1}, EmitPoint {2} StartDir {3}, HitPoint {4}, HitType {5}",
                    CmdSeq,
                    StartPoint.ToStringExt(),
                    EmitPoint.ToStringExt(),
                    StartDir.ToStringExt(),
                    HitPoint.ToStringExt(),
                    HitType);
            }
        }
        class BulletInfoPair
        {
            public BulletInfo Client;
            public BulletInfo Server;
            public int Seq;
            public bool? Matched;
        }
        private Dictionary<int, BulletInfoPair> _infos = new Dictionary<int, BulletInfoPair>();

        public void AddBulletData(int seq, 
            Vector3 startPoint, 
            Vector3 emitPoint, 
            Vector3 startDir, 
            Vector3 hitPoint, 
            int hitType, 
            INetworkChannel networkChannel)
        {
            if(!_infos.ContainsKey(seq))
            {
                _infos[seq] = new BulletInfoPair
                {
                    Seq = seq,
                };
            }
            var pair = _infos[seq];
            pair.Seq = seq;
            pair.Server = new BulletInfo
            {
                CmdSeq = seq,
                StartPoint = startPoint,
                EmitPoint = emitPoint,
                StartDir = startDir,
                HitPoint = hitPoint,
                HitType = (EHitType)hitType,
            };
            if(null != networkChannel)
            {
                SendMismatch(seq, pair, networkChannel);
            }
        }

        public void AddClientBulletData(int seq, 
            Vector3 startPoint, 
            Vector3 emitPoint,
            Vector3 startDir, 
            Vector3 hitPoint, 
            int hitType, 
            INetworkChannel networkChannel)
        {
            if(!_infos.ContainsKey(seq))
            {
                _infos[seq] = new BulletInfoPair
                {
                    Seq = seq,
                }; 
            }
            var pair = _infos[seq];
            pair.Client = new BulletInfo
            {
                CmdSeq = seq,
                StartPoint = startPoint,
                EmitPoint = emitPoint,
                StartDir = startDir,
                HitPoint = hitPoint,
                HitType = (EHitType)hitType,
            };
            SendMismatch(seq, _infos[seq], networkChannel);
        }

        private void SendMismatch(int cmdSeq, BulletInfoPair pair, INetworkChannel networkChannel)
        {
            if(!pair.Matched.HasValue)
            {
                if(null != pair.Client && null != pair.Server)
                {
                    pair.Matched = pair.Client.IsMatched(pair.Server);
                    if (!pair.Matched.Value)
                    {
                        var msg = FireInfoAckMessage.Allocate();
                        msg.Seq = cmdSeq;
                        networkChannel.SendReliable((int)EServer2ClientMessage.FireInfoAck, msg);
                        msg.ReleaseReference();
                    }
                }
            }
        }

        public string GetStatisticData(int type)
        {
            switch(type)
            {
                case 0:
                default:
                    return DumpSummary();
                case 1:
                    return DumpPlayerMissMatch();
                case 2:
                    return DumpMismatch();
                case 3:
                    return DumpAll();
            }
        }

        public string DumpSummary()
        {
            _stringBuilder.Length = 0;
            var total = _infos.Count;
            var illegal = 0;
            int miss = 0;
            int playerMiss = 0;
            int playerHit = 0;
            foreach(var info in _infos)
            {
                var pair = info.Value;
                if(!pair.Matched.HasValue)
                {
                    illegal++;
                }
                else
                {
                    if (!pair.Matched.Value)
                    {
                        miss++;
                    }
                    if(pair.Client.HitType == EHitType.Player || pair.Server.HitType== EHitType.Player)
                    {
                        if(pair.Matched.Value)
                        {
                            playerHit++;
                        }
                        else
                        {
                            playerMiss++;
                        }
                    }
                }
            }
            return string.Format("total {0}, miss {1},  missPercent {2:0.00%}, playerHit {3}," +
                " playerMiss {4}, playerMissPercent {5:0.00%} illegal {6}", 
                total, 
                miss, 
                (float)miss / Mathf.Max(1, total), 
                playerHit + playerMiss,
                playerMiss,
                (float)playerMiss / Mathf.Max(1, (playerHit + playerMiss)),
                illegal);
        }

        public string DumpPlayerMissMatch()
        {
            _stringBuilder.Length = 0;
            foreach(var info in _infos)
            {
                var pair = info.Value;
                if(null == pair.Client || null == pair.Server)
                {
                    continue;
                }

                if(pair.Client.HitType != EHitType.Player &&
                    pair.Server.HitType != EHitType.Player)
                {
                    continue;
                }
                AppendInfoPair(info.Value);
            }
            return _stringBuilder.ToString();
        }

        private void AppendInfoPair(BulletInfoPair pair)
        {
            _stringBuilder.Append("Seq : ");
            _stringBuilder.Append(pair.Seq);
            _stringBuilder.AppendLine();
            _stringBuilder.Append("Server :");
            if(null != pair.Server)
            {
                _stringBuilder.AppendLine(pair.Server.ToString());
            }
            else
            {
                _stringBuilder.AppendLine("No Server Data");
            }
            _stringBuilder.Append("Client :");
            if(null != pair.Client)
            {
                _stringBuilder.AppendLine(pair.Client.ToString());
            }
            else
            {
                _stringBuilder.AppendLine("No Client Data");
            }
            _stringBuilder.AppendLine();
        }

        public string DumpAll()
        {
            _stringBuilder.Length = 0;
            foreach(var info in _infos)
            {
                var pair = info.Value;
                AppendInfoPair(pair);
            }
            return _stringBuilder.ToString();
        }

        public string DumpMismatch()
        {
            _stringBuilder.Length = 0;
            foreach(var info in _infos)
            {
                var pair = info.Value;
                if(pair.Matched.HasValue && pair.Matched.Value)
                {
                    continue;
                }
                AppendInfoPair(pair);
            }
            return _stringBuilder.ToString();
        }
    }
}
