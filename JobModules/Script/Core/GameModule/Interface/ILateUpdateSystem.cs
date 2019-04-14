namespace Core.GameModule.Interface
{
    public interface ILateUpdateSystem:IUserSystem
    {
        void OnLateUpdate();
    }
}