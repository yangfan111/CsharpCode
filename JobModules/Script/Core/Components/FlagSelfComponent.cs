using Core.EntityComponent;
using Core.Prediction.UserPrediction;

namespace Core.Components
{
    
    public class FlagSelfComponent : IGameComponent, IUserPredictionComponent
    {
        public int GetComponentId() { { return (int)ECoreComponentIds.FlagSelf; } }
        public void CopyFrom(object rightComponent)
        {
        }

        public bool IsApproximatelyEqual(object right)
        {
            return true;
        }

        public void RewindTo(object rightComponent)
        {
           
        }
    }
}