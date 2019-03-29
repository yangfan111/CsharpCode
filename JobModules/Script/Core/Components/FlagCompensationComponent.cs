using Core.Compensation;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;

namespace Core.Components
{
    
    public class FlagCompensationComponent :  ICompensationComponent,IPlaybackComponent
    {
        public void CopyFrom(object rightComponent)
        {
        }

        public bool IsApproximatelyEqual(object right)
        {
            return true;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
        }

        public bool IsInterpolateEveryFrame(){ return false; }

        public int GetComponentId()
        {
            {
                return (int) ECoreComponentIds.FlagCompensation;
            }
        }

        public void RewindTo(object rightComponent)
        {
        }
    }
}