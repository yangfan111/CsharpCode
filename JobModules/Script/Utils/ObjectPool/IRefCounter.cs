namespace Core.ObjectPool
{
    public interface IRefCounter: IReusableObject
    {
        void AcquireReference();
        void ReleaseReference();
        
        int RefCount { get; }
    }
}