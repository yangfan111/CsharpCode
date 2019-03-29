using App.Server.GameModules.GamePlay.free.player;
using App.Shared.DebugSystem;
using App.Shared.FreeFramework.Entitas;
using App.Shared.GameModules.GamePlay;
using App.Shared.GameModules.GamePlay.SimpleTest;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.Event;
using App.Shared.GameModules.Weapon;
using Core.GameModule.Module;

namespace App.Server.GameModules.GamePlay
{
    public class ServerGamePlayModule : GameModule
    {
        public ServerGamePlayModule(Contexts contexts, ServerRoom room)
        {
            var gameRule = contexts.session.serverSessionObjects.GameRule;

            //AddSystem(new SimplePlayerLifeSystem(contexts, contexts.serverSession.sessionObjects));
            AddSystem(new SimpleLoadBulletSystem(contexts, contexts.session.commonSession));
            AddSystem(new FreePlayerCmdSystem(contexts, room));
            AddSystem(new RigidbodyDebugInfoSystem(contexts));
            AddSystem(new MapObjectDebugInfoSystem(contexts));
            AddSystem(new LocalEventPlaySystem(contexts, true));
            AddSystem(new FreePredictCmdSystem(contexts));
            AddSystem(new WeaponCleanupSystem(contexts));
            AddSystem(new ObserveSystem(contexts));
        }
    }
}