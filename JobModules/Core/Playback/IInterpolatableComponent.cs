namespace Core.Playback
{
    public interface IInterpolatableComponent
    {
        void Interpolate(object left, object right, IInterpolationInfo interpolationInfo);
        bool IsInterpolateEveryFrame();
    }
    
   
}