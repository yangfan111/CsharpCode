namespace AssetBundleManager.Operation
{
    abstract class PollingOperation
    {
        public abstract bool IsDone();
        public abstract void Process();
    }
}