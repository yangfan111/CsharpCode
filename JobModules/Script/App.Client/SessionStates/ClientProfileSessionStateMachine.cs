using App.Client.ClientGameModules;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using Core.SessionState;
using Entitas;

namespace App.Client.SessionStates
{
    public class ClientProfileSessionStateMachine : SessionStateMachine
    {
        public ClientProfileSessionStateMachine(IContexts contexts):
            base(new ClientSessionStateMachineMonitor(contexts))
        {
            AddState(new LoadBaseConfigureState(contexts, EClientSessionStates.LoadConfig, EClientSessionStates.RequestRoomInfo));

            if (SharedConfig.InSamplingMode)
            {
                AddState(new RequestRoomInfoState(contexts, EClientSessionStates.RequestRoomInfo, EClientSessionStates.ProfilePreparation));
                AddState(new ProfilePreparationState(contexts, EClientSessionStates.ProfilePreparation, EClientSessionStates.LoadOptionConfig));
                AddState(new ClientOptionLoadState(contexts, EClientSessionStates.LoadOptionConfig, EClientSessionStates.LoadSceneMapConfig));
                AddState(new LoadSceneMapConfig(contexts, EClientSessionStates.LoadSceneMapConfig, EClientSessionStates.Profile, true, false));
                AddState(new ProfileState(contexts, EClientSessionStates.Profile, EClientSessionStates.Profile));
            }
            else
            {
                AddState(new RequestRoomInfoState(contexts, EClientSessionStates.RequestRoomInfo, EClientSessionStates.PreparingPlayer));
                AddState(new PreparingPlayerState(contexts, EClientSessionStates.PreparingPlayer, EClientSessionStates.LoadSceneMapConfig));
                AddState(new LoadSceneMapConfig(contexts, EClientSessionStates.LoadSceneMapConfig, EClientSessionStates.InitUiModule, true, false));
                AddState(new InitUiModuleState(contexts, EClientSessionStates.InitUiModule, EClientSessionStates.RequestSnapshot));
                AddState(new WaitSnapshotState(contexts, EClientSessionStates.RequestSnapshot, EClientSessionStates.Running));
                AddState(new LoginSuccState(contexts, EClientSessionStates.Running, EClientSessionStates.Running));
            }

            Initialize((int) EClientSessionStates.LoadConfig);
        }
    }
}