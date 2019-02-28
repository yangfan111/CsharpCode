using App.Shared.GameModules.Bullet;
using Core.GameModule.Module;

namespace App.Server.GameModules.Bullet
{
    public class ServerBulletModule : GameModule
    {
        public ServerBulletModule(Contexts contexts)
        {
            AddSystem(new ServerBulletInfoCollectSystem(contexts.bullet,
                contexts.player,
                contexts.session.commonSession.BulletInfoCollector));
        }
    }
}
