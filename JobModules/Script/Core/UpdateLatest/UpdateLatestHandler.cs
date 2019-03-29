using System;
using System.Collections.Generic;
using Core.EntitasAdpater;
using Core.EntityComponent;
using Core.Utils;

namespace Core.UpdateLatest
{
    [Serializable]
    public class UpdateLatestHandler : IUpdateLatestHandler
    {
        private IGameContexts _gameContexts;

        public UpdateLatestHandler(IGameContexts gameContexts)
        {
            _gameContexts = gameContexts;
        }

        public UpdateLatestPacakge GetUpdateLatestPacakge(EntityKey selfKey)
        {
            var entity = _gameContexts.GetGameEntity(selfKey);
            if (entity == null) return null;
#pragma warning disable RefCounter001,RefCounter002
            UpdateLatestPacakge pacakge = UpdateLatestPacakge.Allocate();
#pragma warning restore RefCounter001,RefCounter002


         
            pacakge.Head.BaseUserCmdSeq = BaseUserCmdSeq;
            pacakge.Head.UserCmdSeq = UserCmd;
            pacakge.Head.LastSnapshotId = LastSnapshotId;
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

        public int UserCmd { get; set; }
        public int LastSnapshotId { get; set; }
        private int _baseUserCmdSeq = -1;
        public int BaseUserCmdSeq
        {
            get { return _baseUserCmdSeq;}
            set
            {
                if (_baseUserCmdSeq <= value) _baseUserCmdSeq = value;
            }
        }
    }
}