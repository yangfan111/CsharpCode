using System.Text;
using App.Protobuf;
using App.Shared.Components.Player;
using App.Shared.Components.Serializer;
using App.Shared.Network.SerializeInfo;
using Core.Network;
using Core.ObjectPool;
using Core.SnapshotReplication;
using Core.UpdateLatest;
using Entitas.Utils;
using Google.Protobuf;
using Free.framework;

namespace App.Shared.Network
{
    public class AppMessageTypeInfo: IMessageTypeInfo
    {
        public void PrintDebugInfo(StringBuilder sb)
        {
            sb.Append("full:").Append(_replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.FullCount)
                .Append(", diff").Append(_replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.DiffCount)
                .Append(", skip").Append(_replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.SkipCount)
                .Append("<br>");
            sb.Append("<table width='400px' border='1' align='center' cellpadding='2' cellspacing='1'>");
            sb.Append("<thead>");
            sb.Append("<td>name</td>");
            sb.Append("<td>write count</td>");
            sb.Append("<td>write size</td>");
            sb.Append("<td>avg write size</td>");
            sb.Append("<td>last write size</td>");
            sb.Append("<td>read count</td>");
            sb.Append("<td>read size</td>");
            sb.Append("<td>avg read size</td>");
            sb.Append("<td>last read size</td>");
            sb.Append("</thead>");

            foreach (var info in _serializeInfo)
            {
                if (info != null)
                {
                    SerializationStatistics st = info.Statistics;
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(st.Name);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.TotalSerializeCount);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.TotalSerializeSize);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.TotalSerializeSize/(st.TotalSerializeCount+1));
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(st.LatestSerializeSize);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(st.TotalDeserializeCount);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.TotalDeserializeSize);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.TotalDeserializeSize / (st.TotalDeserializeCount + 1));
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(st.LatestDeserializeSize);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }
            sb.Append("</table>");
        }

       

        private ISerializeInfo[] _serializeInfo;

        private ReplicatedSnapshotSerializeInfo _replicatedSnapshotSerializeInfo;
        private ReplicatedUpdateEntitySerializeInfo _replicatedUpdateEntitySerializeInfo;
        public AppMessageTypeInfo()
        {
            _serializeInfo = new ISerializeInfo[(int) EServer2ClientMessage.Max];
            _serializeInfo[(int) EClient2ServerMessage.Login] = new ProtoBufSerializeInfo<LoginMessage>(Protobuf.LoginMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.LoginSucc] = new ProtoBufSerializeInfo<LoginSuccMessage>(Protobuf.LoginSuccMessage.Parser);
            _serializeInfo[(int) EClient2ServerMessage.UserCmd] = new UserCmdSerializeInfo();
            _serializeInfo[(int) EClient2ServerMessage.SimulationTimeSync] = new SimulationTimeMessageSerializeInfo();
            _serializeInfo[(int) EClient2ServerMessage.VehicleCmd] = new VehicleCmdSerializeInfo();
            _serializeInfo[(int)EClient2ServerMessage.VehicleEvent] = new VehicleEventSerializeInfo();
            _serializeInfo[(int)EClient2ServerMessage.TriggerObjectEvent] = new TriggerObjectEventSerializeInfo();
            _serializeInfo[(int) EClient2ServerMessage.LocalDisconnect] = null;
            _serializeInfo[(int) EClient2ServerMessage.LocalLogin] = null;
            _serializeInfo[(int)EClient2ServerMessage.ClothChange] = new ProtoBufSerializeInfo<ClothChangeMessage>(Protobuf.ClothChangeMessage.Parser);
            _serializeInfo[(int)EClient2ServerMessage.DebugCommand] = new ProtoBufSerializeInfo<DebugCommandMessage>(Protobuf.DebugCommandMessage.Parser);
            _serializeInfo[(int) EClient2ServerMessage.UpdateMsg] = _replicatedUpdateEntitySerializeInfo=new ReplicatedUpdateEntitySerializeInfo(ComponentSerializerManager.Instance, new UpdateMessagePool());
            _serializeInfo[(int) EClient2ServerMessage.FireInfo] = new ProtoBufSerializeInfo<FireInfoMessage>(Protobuf.DebugCommandMessage.Parser);
            _serializeInfo[(int) EClient2ServerMessage.DebugScriptInfo] =  new  ProtoBufSerializeInfo<DebugScriptInfo>(Protobuf.DebugScriptInfo.Parser);
            _serializeInfo[(int) EClient2ServerMessage.GameOver ] = new ProtoBufSerializeInfo<GameOverMesssage>(Protobuf.GameOverMesssage.Parser);
            _replicatedSnapshotSerializeInfo = new ReplicatedSnapshotSerializeInfo(new SnapshotReplicator(ComponentSerializerManager.Instance));
            _serializeInfo[(int) EServer2ClientMessage.Snapshot] = _replicatedSnapshotSerializeInfo;

            //            _replicatedUserCmdSerializeInfo = new ReplicatedCmddSerializeInfo(new SnapshotReplicator(ComponentSerializerManager.Instance));


            _serializeInfo[(int) EServer2ClientMessage.SimulationTimeSync] = new SimulationTimeMessageSerializeInfo();
            _serializeInfo[(int)EServer2ClientMessage.UdpId] = new ProtoBufSerializeInfo<UdpIdMessage>(Protobuf.UdpIdMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.FreeData] = new ProtoBufSerializeInfo<SimpleProto>(SimpleProto.Parser);
            _serializeInfo[(int)EClient2ServerMessage.FreeEvent] = new ProtoBufSerializeInfo<SimpleProto>(SimpleProto.Parser);
            _serializeInfo[(int)EServer2ClientMessage.Statistics] = new ProtoBufSerializeInfo<BattleStatisticsMessage>(Protobuf.BattleStatisticsMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.Ping] = new ProtoBufSerializeInfo<PingMessage>(Protobuf.PingMessage.Parser);
            _serializeInfo[(int)EClient2ServerMessage.Ping] = new ProtoBufSerializeInfo<PingRequestMessage>(Protobuf.PingRequestMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.DamageInfo] = new ProtoBufSerializeInfo<PlayerDamageInfoMessage>(Protobuf.PlayerDamageInfoMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.UpdateAck] = new ProtoBufSerializeInfo<UpdateMessageAck>(Protobuf.UpdateMessageAck.Parser);
            _serializeInfo[(int)EServer2ClientMessage.PlayerInfo] = new ProtoBufSerializeInfo<PlayerInfoMessage>(Protobuf.PlayerInfoMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.FireInfoAck] = new ProtoBufSerializeInfo<FireInfoAckMessage>(Protobuf.UpdateMessageAck.Parser);
            _serializeInfo[(int)EServer2ClientMessage.DebugMessage] = new ProtoBufSerializeInfo<ServerDebugMessage>(Protobuf.ServerDebugMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.ClearScene] = new ProtoBufSerializeInfo<ClearSceneMessage>(Protobuf.ClearSceneMessage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.GameOver] = new ProtoBufSerializeInfo<GameOverMesssage>(Protobuf.GameOverMesssage.Parser);
            _serializeInfo[(int)EServer2ClientMessage.HeartBeat] = new ProtoBufSerializeInfo<HeartBeatMessage>(Protobuf.HeartBeatMessage.Parser);
        }



        public ISerializeInfo this[int messageType]
        {
            get { return _serializeInfo[messageType]; }
            set { _serializeInfo[messageType] = value; }
        }




        public ISerializeInfo GetSerializeInfo(int messageType)
        {
            return this[messageType];
        }

        public void SetReplicationAckId(int id)
        {
            _replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.SnapshotReceived(id);
        }

        public bool SkipSendSnapShot(int serverTime)
        {
            return _replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.SkipSendSnapShot(serverTime);
        }

        public void IncSendSnapShot()
        {
            _replicatedSnapshotSerializeInfo.SnapshotReplicator.SendChannel.SkipCount++;
        }

        public int LatestUpdateMessageSeq
        {
            get { return _replicatedUpdateEntitySerializeInfo.MessagePool.LatestMessageSeq; }
        }


        public void Dispose()
	    {
		    for (int i = 0; i < _serializeInfo.Length; i++)
		    {
			    if (_serializeInfo[i] != null)
			    {
				    _serializeInfo[i].Dispose();
				    _serializeInfo[i] = null;
			    }
		    }
	    }

        public IUpdateMessagePool GetUpdateMessagePool()
        {
            return _replicatedUpdateEntitySerializeInfo.MessagePool;
        }
    }
}
