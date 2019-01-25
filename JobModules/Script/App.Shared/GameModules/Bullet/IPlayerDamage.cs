using App.Server.GameModules.GamePlay;
using com.wd.free.@event;

namespace App.Shared.GameModules.Bullet
{
    public interface IPlayerDamager
    {
        IGameRule GameRule { get; }

        IEventArgs FreeArgs { get; }

        float HandleDamage(PlayerEntity source, PlayerEntity target, PlayerDamageInfo info);

        void KillPlayer(PlayerEntity source, PlayerEntity target, PlayerDamageInfo info);
    }
}
