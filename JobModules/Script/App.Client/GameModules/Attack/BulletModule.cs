using App.Client.ClientGameModules.Bullet;
using App.Shared.GameModules.Attack;
using Core.GameModule.Module;

namespace App.Client.GameModules.Attack
{
    public class BulletModule : GameModule
    {
        public BulletModule(Contexts contexts) 
        {
            AddSystem(new BulletEntityInitSytem(contexts));
            AddSystem(new BulletRenderSystem(contexts));
            AddSystem(new ClientBulletRemoteCollectSystem(contexts.bullet, 
                contexts.session.clientSessionObjects.NetworkChannel));
        }
    }
}