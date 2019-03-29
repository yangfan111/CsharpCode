namespace Core.Playback
{
    public interface IInterpolationInfo
    {
        int LeftServerTime { get; }
        int RightServerTime { get; }
        int CurrentRenderTime { get; }
        float Ratio { get; }
        float RatioWithOutClamp { get; }
    }
}