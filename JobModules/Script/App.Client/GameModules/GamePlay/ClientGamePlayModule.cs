using App.Client.GameModules.GamePlay.Free.UI;
using App.Client.GameModules.GamePlay.Offline;
using App.Shared.Components;
using App.Shared.DebugSystem;
using App.Shared.FreeFramework.Entitas;
using App.Shared.GameModules.GamePlay.SimpleTest;
using App.Shared.GameModules.Player.Event;
using App.Shared.GameModules.Weapon;
using Core.GameModule.Module;

namespace App.Client.GameModules.GamePlay
{
    public class ClientGamePlayModule : GameModule
    {
        public ClientGamePlayModule(Contexts contexts)
        {
            var gameRule = contexts.session.clientSessionObjects.GameRule;
            if (gameRule == GameRules.Offline)
            {
                AddSystem(new OfflineGamePlay(contexts,  contexts.session.commonSession));
              //  AddSystem(new SimplePlayerLifeSystem(contexts, contexts.clientSession.sessionObjects));
            }
            AddSystem(new SimpleLoadBulletSystem(contexts, contexts.session.commonSession));

            AddSystem(new TestFrameSystem(contexts));
            AddSystem(new FreeUiSystem());
            AddSystem(new LocalEventPlaySystem(contexts,false));
            AddSystem(new RemoteEventPlaySystem(contexts,false));
            AddSystem(new InputIniSystem(contexts));
            AddSystem(new FreePredictCmdSystem(contexts));
            AddSystem(new RigidbodyDebugInfoSystem(contexts));
            AddSystem(new MapObjectDebugInfoSystem(contexts));
          
        }
    }
}