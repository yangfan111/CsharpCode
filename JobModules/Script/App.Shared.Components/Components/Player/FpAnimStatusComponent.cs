using System.Collections.Generic;
using System.Text;
using Core.Animation;
using Core.Compare;
using Core.Compensation;
using Core.Components;
using Core.Enums;
using Core.Playback;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    
    public class FpAnimStatusComponent : AbstractNetworkAnimator, IUpdateComponent, ICompensationComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(NetworkAnimatorComponent));
        
        public int GetComponentId() { return (int) EComponentIds.FpAnimData; }
       
        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override string ToString()
        {
            return string.Format("FpAnimStatusComponent:{0}", base.ToString());
        }
    }
}