namespace Core.Replicaton
{
    public interface ISnapshotSelector
    {
        SnapshotPair SelectSnapshot(int renderTime);
        ISnapshot LatestSnapshot { get;  }
        ISnapshot OldestSnapshot { get; }
    }

    public interface ISnapshotSelectorContainer
    {
        ISnapshotSelector SnapshotSelector { get; set; }
    }

    public class SnapshotSelectorContainer : ISnapshotSelectorContainer
    {
        private ISnapshotSelector _snapshotSelector;

        public SnapshotSelectorContainer(ISnapshotSelector snapshotSelector)
        {
            _snapshotSelector = snapshotSelector;
        }

        public ISnapshotSelector SnapshotSelector
        {
            get { return _snapshotSelector; }
            set { _snapshotSelector = value; }
        }

    }
}