using System;
using Core.EntityComponent;
using Core.Utils;

namespace Core.UpdateLatest
{
    [Serializable]
    public class ClientUpdateLatestManager
    {
        private int lastAckUserCmdSeq = -1;
        private IGameContexts gameContexts;

        public int LastLocalUserCmd { private get; set; }
        public int LastAckSnapshotId {private get; set; }
   

        public ClientUpdateLatestManager(IGameContexts gameContexts)
        {
            this.gameContexts = gameContexts;
        }


        public int LastAckUserCmdSeq
        {
            get { return lastAckUserCmdSeq; }
            set { lastAckUserCmdSeq = Math.Max(lastAckUserCmdSeq, value); }
        }

        public UpdateLatestPacakge CreateClientUpdateLatestPacakge(EntityKey selfKey)
        {
            var entity = gameContexts.GetGameEntity(selfKey);
            if (entity == null) return null;
#pragma warning disable RefCounter001,RefCounter002
            UpdateLatestPacakge pacakge = UpdateLatestPacakge.Allocate();
#pragma warning restore RefCounter001,RefCounter002

            pacakge.Head.LastAckUserCmdSeq = LastAckUserCmdSeq;
            pacakge.Head.LastUserCmdSeq     = LastLocalUserCmd;
            pacakge.Head.LastSnapshotId = LastAckSnapshotId;
            var components = entity.GetUpdateLatestComponents();
            AssertUtility.Assert(components.Count < 255);
            pacakge.Head.ComponentCount = (byte) components.Count;
            foreach (var gameComponent in components)
            {
                IUpdateComponent copy =
                                (IUpdateComponent) GameComponentInfo.Instance.Allocate(gameComponent.GetComponentId());
                copy.CopyFrom(gameComponent);
                pacakge.UpdateComponents.Add(copy);
            }

            return pacakge;
        }
    }
}