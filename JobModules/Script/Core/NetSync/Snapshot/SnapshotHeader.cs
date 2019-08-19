using System;
using System.IO;
using Core.EntityComponent;
using Core.Utils;

namespace Core.Replicaton
{
    [Serializable]
    public class SnapshotHeader
    {
        public int VehicleSimulationTime;
        public int ServerTime;
        public int SnapshotSeq;
        public int LastUserCmdSeq;
        public EntityKey Self;


        public SnapshotHeader()
        {
            ServerTime = -1;
            SnapshotSeq = -1;
            VehicleSimulationTime = -1;
            LastUserCmdSeq = -1;
        }

        public void Serialize(MyBinaryWriter writer, string version)
        {
            writer.Write(version);
            writer.Write(VehicleSimulationTime);
            writer.Write(ServerTime);
            writer.Write(SnapshotSeq);
            writer.Write(LastUserCmdSeq);
            writer.Write(Self.EntityId);
            writer.Write(Self.EntityType);
        }

        public string DeSerialize(BinaryReader reader)
        {
            var version = reader.ReadString();
            VehicleSimulationTime = reader.ReadInt32();
            ServerTime = reader.ReadInt32();
            SnapshotSeq = reader.ReadInt32();
            LastUserCmdSeq = reader.ReadInt32();
            int entityId = reader.ReadInt32();
            short entityType = reader.ReadInt16();
            Self = new EntityKey(entityId: entityId, entityType: entityType);
            return version;
        }

        public void CopyFrom(SnapshotHeader src)
        {
            VehicleSimulationTime = src.VehicleSimulationTime;
            ServerTime = src.ServerTime;
            SnapshotSeq = src.SnapshotSeq;
            LastUserCmdSeq = src.LastUserCmdSeq;
            Self = src.Self;
        }
    }
}