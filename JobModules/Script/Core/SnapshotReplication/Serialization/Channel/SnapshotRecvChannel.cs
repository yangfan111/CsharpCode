using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Clone;
using Core.SnapshotReplication.Serialization.Patch;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using UnityEngine;

namespace Core.SnapshotReplication.Serialization.Channel
{
    public class SnapshotRecvChannel :ISnapshotRecvChannel, IDisposable
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SnapshotRecvChannel));
        private Dictionary<int, ISnapshot> _receivedSnapMap;
        private ISnapshotSerializer _snapSerializer;
        private ISnapshot _emptySnapshot = Snapshot.Allocate();
        public SnapshotRecvChannel(SnapshotSerializer serializer)
        {
            _receivedSnapMap = new Dictionary<int, ISnapshot>();
            _snapSerializer = serializer;
        }

	    public void Dispose()
	    {
		    foreach (var id in _receivedSnapMap.Values)
		    {
			    id.ReleaseReference();
		    }
		    _receivedSnapMap.Clear();
			_emptySnapshot.ReleaseReference();
		    _emptySnapshot = null;
		}

	    public ISnapshot DeSerializeSnapshot(BinaryReader reader)
        {
            SnapshotHeader header;
            SnapshotPatch patch = _snapSerializer.DeSerialize(reader, out header);
            ClearOldSnapshot(patch.BaseSnapshotSeq);
            var baseSnap = SnapshotCloner.Clone(GetBaseSnapshot(patch.BaseSnapshotSeq));
            baseSnap.Header = header;
            patch.ApplyPatchTo(baseSnap, _snapSerializer.GetSerializerManager());
            patch.ReleaseReference();
            if (_receivedSnapMap.ContainsKey(baseSnap.SnapshotSeq))
            {
                Debug.LogErrorFormat("SnapSHotSeq {0} exist", baseSnap.SnapshotSeq);
                Logger.ErrorFormat("SnapSHotSeq {0} exist", baseSnap.SnapshotSeq);
            }
            else
            {
                _receivedSnapMap.Add(baseSnap.SnapshotSeq,baseSnap);
            }
            baseSnap.AcquireReference();
            return baseSnap;
        }

        private void ClearOldSnapshot(int baseId)
        {
            foreach(var id in _receivedSnapMap.Keys.ToArray())
            {
                if (id < baseId)
                {
                    _receivedSnapMap[id].ReleaseReference();
                    _receivedSnapMap.Remove(id);
                }
            }
        }
        private ISnapshot GetBaseSnapshot(int id)
        {
#pragma warning disable RefCounter002
            ISnapshot snap;
#pragma warning restore RefCounter002
            if (_receivedSnapMap.TryGetValue(id, out snap) == false)
            {
                snap = _emptySnapshot;
            }
            return snap;
        }

	    
    }
}
