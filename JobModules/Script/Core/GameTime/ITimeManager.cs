namespace Core.GameTime
{
    public interface ITimeManager
    {
        int ClientTime { get; }

        int FrameInterval { get; }

        int RenderTime { get; set; }

        float FrameInterpolation { get; }
        
        void Tick(float now);
        void SyncWithServer(int serverTime);
        void UpdateFrameInterpolation(int leftServerTime, int rightServerTime);
       
    }
}