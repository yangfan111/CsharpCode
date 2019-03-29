using App.Client.ClientGameModules.Bullet;
using App.Shared.GameModules.Bullet;
using Core.GameModule.Module;

namespace App.Client.GameModules.Bullet
{
    public class BulletModule : GameModule
    {
        public BulletModule(Contexts contexts) 
        {
            AddSystem(new BulletEntityInitSytem(contexts));
            AddSystem(new BulletRenderSystem(contexts));
            AddSystem(new ClientBulletInfoCollectSystem(contexts.bullet, 
                contexts.session.clientSessionObjects.NetworkChannel, 
                contexts.session.commonSession.BulletInfoCollector));
        }
    }
}