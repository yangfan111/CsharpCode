namespace Core.GameModule.Interface
{
    public interface IPhysicsUpdateSystem
    {
        void Update();
    }

    public interface IPhysicsPostUpdateSystem
    {
        void PostUpdate();
    }
}
