using App.Server.GameModules.GamePlay.player;
using App.Server.GameModules.Player;
using App.Shared.GameModules.Player;
using Core.GameModule.Module;

namespace App.Server
{
    public class ServerPlayerModule : GameModule
    {
        public ServerPlayerModule(Contexts contexts)
        {
            AddSystem(new ServerRemoteEventInitSystem(contexts));
            AddSystem(new PlayerEntityInitSystem(contexts));
            AddSystem(new ServerPlayerCameraInitSystem(contexts.player));

            AddSystem(new PlayerResourceLoadSystem(contexts));
            
            AddSystem(new PlayerDeadAnimSystem(contexts));
            AddSystem(new PlayerSoundPlaySystem(contexts));

            AddSystem(new PlayerDebugDrawSystem(contexts));
            AddSystem(new ServerPlayerWeaponInitSystem(contexts.player, contexts.session.commonSession));
            AddSystem(new PlayerEquipPickAndDropSystem(null));
        }
    }
}