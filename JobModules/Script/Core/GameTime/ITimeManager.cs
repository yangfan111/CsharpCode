namespace Core.GameTime
{
    public interface ITimeManager
    {
        int ClientTime { get; }

        int FrameInterval { get; }

        int ServerTime { get; set; }

        int RenderTime { get; }

        float FrameInterpolation { get; }
        
        int Delta { get; }

        void Tick(float now);
        void SyncWithServer(int latestServerTime);
        void UpdateFrameInterpolation(int leftServerTime, int rightServerTime);
       
    }
}