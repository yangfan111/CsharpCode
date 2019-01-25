namespace Core.Replicaton
{
    public class SnapshotPair 
    {
        public SnapshotPair(ISnapshot leftSnapshot, ISnapshot rightSnapshot, int renderTime)
        {
            LeftSnapshot = leftSnapshot;
            RightSnapshot = rightSnapshot;
            RenderTime = renderTime;
        }

        public ISnapshot LeftSnapshot;
        public ISnapshot RightSnapshot;
        public int RenderTime;
    }
}