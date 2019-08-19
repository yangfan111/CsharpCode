namespace AssetBundleManager.Operation
{
    public abstract class PollingOperation
    {
        public abstract bool IsDone();
        public abstract void Process();
    }
}