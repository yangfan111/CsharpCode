using Core.EntityComponent;

namespace Core
{
    /// <summary>
    /// Defines the <see cref="ISessionMode" />
    /// </summary>
    public interface ISessionMode
    {
        int ModeId { get; }
    }
    public interface IGameWeapon
    {
        EntityKey Owner { get; }
    }
}
