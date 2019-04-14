namespace Core.GameModule.Interface
{
    public interface IPhysicsUpdateSystem:IUserSystem
    {
        void Update();
    }

    public interface IPhysicsPostUpdateSystem:IUserSystem
    {
        void PostUpdate();
    }
}
