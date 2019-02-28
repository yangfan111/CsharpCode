using App.Shared.GameModules.Bullet;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.player
{
    public class SimplePlayerDamager : IPlayerDamager
    {
        private ServerRoom _room;

        public SimplePlayerDamager(ServerRoom room)
        {
            _room = room;
        }

        public IEventArgs FreeArgs { get { return _room.FreeArgs; } }

        public IGameRule GameRule
        {
            get { return _room.GameRule; }
        }

        public float HandleDamage(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage)
        {
            return _room.GameRule.HandleDamage(source, target, damage);
        }

        public void KillPlayer(PlayerEntity source, PlayerEntity target, PlayerDamageInfo damage)
        {
            _room.GameRule.KillPlayer(source, target, damage);
        }
    }
}
