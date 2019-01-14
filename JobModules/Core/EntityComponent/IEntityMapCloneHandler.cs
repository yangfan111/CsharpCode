namespace Core.EntityComponent
{
    public interface IEntityMapCloneHandler
    {
        bool IsExcludeComponent(IGameComponent component);
        void CloneComponent(IGameComponent dst, IGameComponent component);
    }
}