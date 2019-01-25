using System;
using System.Collections.Generic;
using System.IO;
using Core.Replicaton;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using UnityEngine.Profiling;

namespace Core.SnapshotReplication.Serialization.Channel
{
    public class SnapshotSendChannel : ISnapshotSendChannel
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(SnapshotSendChannel));
        private Dictionary<int, ISnapshot> _sentSnapshot;
        private Queue<int> _snapshotQueue;
        private ISnapshotSerializer _snapSerializer;
        private const int MaxSnapshotCount = 40;
        private ISnapshot _emptySnapshot = Snapshot.Allocate();
        public SnapshotSendChannel(SnapshotSerializer snapshotSerializer)
        {
            _sentSnapshot = new Dictionary<int, ISnapshot>();
            _snapshotQueue = new Queue<int>();
            _snapSerializer = snapshotSerializer;
            AckedSnapshotId = -1;
        }
	    public void Dispose()
	    {
		    foreach (var snapshot in _sentSnapshot.Values) {
			    snapshot.ReleaseReference();
		    }
		    _sentSnapshot.Clear();
		    _sentSnapshot = null;

		    _snapshotQueue.Clear();
			_snapshotQueue = null;


		    _emptySnapshot.ReleaseReference();
		    _emptySnapshot = null;
	    }
		private volatile  int _ackedSnapshotId;
        public int AckedSnapshotId { get { return _ackedSnapshotId; } set { _ackedSnapshotId = value; } }

        public int FullCount;

        public int DiffCount;
        
        public int SkipCount;

        private int _lastAllSnapShotId;
        private int _lastAllSnapShotTime;
        private bool _isSendAll;

        public void SerializeSnapshot(ISnapshot snap, Stream stream)
        {
            snap.AcquireReference();
            try
            {
                _sentSnapshot.Add(snap.SnapshotSeq, snap);
            }
            catch (Exception e)
            {
                int i = 0;
                i++;
            }
            _snapshotQueue.Enqueue(snap.SnapshotSeq);
            ISnapshot baseSnap = GetBaseSnapshot(AckedSnapshotId);
            if (baseSnap == null)
            {
                _emptySnapshot.SnapshotSeq = -1;
                _snapSerializer.Serialize(_emptySnapshot, snap, stream);
                _lastAllSnapShotId = snap.SnapshotSeq;
                _lastAllSnapShotTime = snap.ServerTime;
                _isSendAll = true;
                _logger.Warn("send full snapshot!");
                FullCount++;
            }
            else
            {
                _snapSerializer.Serialize(baseSnap, snap, stream);
                _isSendAll = false;
                DiffCount++;
            }

            ClearOldSnapshot(AckedSnapshotId);
			ClearSnapshotWhenLimitExceeded();
        }


        private void ClearSnapshotWhenLimitExceeded()
        {
            while(_snapshotQueue.Count > MaxSnapshotCount)
            {   
				var key = _snapshotQueue.Dequeue();
#pragma warning disable RefCounter001
                ISnapshot snap;
#pragma warning restore RefCounter001
                if (_sentSnapshot.TryGetValue(key, out snap))
                {
                    snap.ReleaseReference();
                    _sentSnapshot.Remove(key);
                }
            }
        }

        private ISnapshot GetBaseSnapshot(int id)
        {
            ISnapshot snap;
            if (_sentSnapshot.TryGetValue(id, out snap) == false)
            {
            }
            return snap;
        }
        

        public void SnapshotReceived(int id)
        {
            AckedSnapshotId = id;
            if (_ackedSnapshotId > _lastAllSnapShotId) _isSendAll = false;
        }

        private List<int> _removeCandidates = new List<int>();
        private char _fullCount;
        private char _diffCount;

        private void ClearOldSnapshot(int baseId)
        {
            _removeCandidates.Clear();
            foreach (var id in _sentSnapshot.Keys)
            {
                if (id < baseId)
                {
                    _removeCandidates.Add(id);
                    
                }
            }
            foreach (var id in _removeCandidates)
            {
                _sentSnapshot[id].ReleaseReference();
                _sentSnapshot.Remove(id);
            }
        }


        public bool SkipSendSnapShot(int serverTime)
        {
            return _isSendAll && serverTime < _lastAllSnapShotTime +1000;
        }
    }
}
