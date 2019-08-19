using System;
using Core.Components;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;

// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Player
{
  

    [Player]
    [Serializable]
    
    public class TimeComponent : IUserPredictionComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.PlayerTime; } }

        [NetworkProperty]
        public int ClientTime { get; set; }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as TimeComponent;
            ClientTime = r.ClientTime;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as TimeComponent;
            return ClientTime == r.ClientTime;
        }

	    public override string ToString()
	    {
		    return string.Format("ClientTime: {0}", ClientTime);
	    }

        public void RewindTo(object rightComponent)
        {
           CopyFrom(rightComponent);
        }
    }
}