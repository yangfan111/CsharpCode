using Core.EntityComponent;

namespace Core.Prediction
{
    public interface IRewindableComponent
    {
        void RewindTo(object rightComponent);
    }

    public interface ICloneableComponent
    {
        void CopyFrom(object rightComponent);
    }
    public interface ILatestComponent 
    {
        void SyncLatestFrom(object rightComponent);
    }
}