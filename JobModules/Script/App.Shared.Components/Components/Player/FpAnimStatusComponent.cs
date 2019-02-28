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
        

        public bool IsApproximatelyEqual(object right)
        {
            bool ret = true;
            var rightComp = right as FpAnimStatusComponent;
            if (rightComp != null)
            {
//                for (int i = 0; i < AnimatorLayers.Count; i++)
//                {
//                    var equal = AnimatorLayers[i].IsSimilar(rightComp.AnimatorLayers[i]);
//                    if (!equal)
//                    {
//                        _logger.Debug(AnimatorLayers[i].ToString());
//                        _logger.Debug(rightComp.AnimatorLayers[i].ToString());
//                    }
//
//                    ret = ret && equal;
//                }

                for (int i = 0; i < AnimatorParameters.Count; i++)
                {
                    var equal = AnimatorParameters[i].IsSimilar(rightComp.AnimatorParameters[i]);
                    if (!equal)
                    {
                        _logger.Debug(AnimatorParameters[i].ToString());
                        _logger.Debug(rightComp.AnimatorParameters[i].ToString());
                    }

                    ret = ret && equal;
                }
                
                ret = ret && CompareUtility.IsApproximatelyEqual(BaseClientTime, rightComp.BaseClientTime);
            }

            return ret;
        }

        public override string ToString()
        {
            return string.Format("FpAnimStatusComponent:{0}", base.ToString());
        }
    }
}