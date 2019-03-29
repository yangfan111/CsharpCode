using App.Client.ClientGameModules;
using App.Client.SessionStates;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.SessionStates;
using Core.SessionState;
using Entitas;

namespace App.Client.SessionStates
{
    public class ClientSessionStateMachine : SessionStateMachine
    {
        public ClientSessionStateMachine(IContexts contexts) :
            base(new ClientSessionStateMachineMonitor(contexts))
        {
            AddState(new LoadBaseConfigureState(contexts, EClientSessionStates.LoadConfig,EClientSessionStates.LoadSubResourceConfig));
            AddState(new LoadSubResourceConfigureState(contexts, EClientSessionStates.LoadSubResourceConfig, EClientSessionStates.RequestRoomInfo));
            AddState(new RequestRoomInfoState(contexts, EClientSessionStates.RequestRoomInfo,EClientSessionStates.PreloadResource));
            AddState(new ClientPreLoadState(contexts, EClientSessionStates.PreloadResource, EClientSessionStates.PreparingPlayer));
            AddState(new PreparingPlayerState(contexts, EClientSessionStates.PreparingPlayer,EClientSessionStates.LoadSceneMapConfig));
            AddState(new LoadSceneMapConfig(contexts, EClientSessionStates.LoadSceneMapConfig, EClientSessionStates.InitUiModule, false, false));
            AddState(new InitUiModuleState(contexts, EClientSessionStates.InitUiModule,EClientSessionStates.RequestSnapshot));
            AddState(new WaitSnapshotState(contexts, EClientSessionStates.RequestSnapshot, EClientSessionStates.Running));
            AddState(new LoginSuccState(contexts, EClientSessionStates.Running, EClientSessionStates.Running));

            Initialize((int) EClientSessionStates.LoadConfig);
        }
    }
}