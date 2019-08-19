using App.Client.ClientGameModules;
using App.Client.SessionStates;
using App.Shared;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.SessionStates;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.SessionStates
{
    public class ClientSessionStateMachine : SessionStateMachine
    {
        public ClientSessionStateMachine(IContexts contexts) :
            base(new ClientSessionStateMachineMonitor(contexts))
        {
            BackroundloadSettings defaultSettings = BackroundloadSettings.GetClientCurrentSettings();
           
            AddState(new LoadBaseConfigureState(contexts, EClientSessionStates.LoadConfig,EClientSessionStates.LoadSubResourceConfig)
                .WithEnterAction(() =>
                {
                    BackroundloadSettings.SetCurrentSettings(BackroundloadSettings.LoadSettsings);
                    UnityAssetManager.ProcessLoadedRequestTime = 100;
                    
                })
            );
            AddState(new LoadSubResourceConfigureState(contexts, EClientSessionStates.LoadSubResourceConfig, EClientSessionStates.RequestSceneInfo));
            AddState(new RequestSceneInfoState(contexts, EClientSessionStates.RequestSceneInfo, EClientSessionStates.PreloadResource));
            AddState(new ClientPreLoadState(contexts, EClientSessionStates.PreloadResource, EClientSessionStates.LoadOptionConfig));
            AddState(new ClientOptionLoadState(contexts, EClientSessionStates.LoadOptionConfig, EClientSessionStates.LoadSceneMapConfig));

            AddState(new LoadSceneMapConfig(contexts, EClientSessionStates.LoadSceneMapConfig, EClientSessionStates.RequestRoomInfo, false, false));
            AddState(new RequestRoomInfoState(contexts, EClientSessionStates.RequestRoomInfo,EClientSessionStates.PreparingPlayer));          
           
            AddState(new PreparingPlayerState(contexts, EClientSessionStates.PreparingPlayer,EClientSessionStates.LoadOCConfig));

            AddState(new LoadOCConfigState(contexts, EClientSessionStates.LoadOCConfig, EClientSessionStates.InitUiModule));
            AddState(new InitUiModuleState(contexts, EClientSessionStates.InitUiModule,EClientSessionStates.RequestSnapshot));
            AddState(new WaitSnapshotState(contexts, EClientSessionStates.RequestSnapshot, EClientSessionStates.Running));
            AddState(new LoginSuccState(contexts, EClientSessionStates.Running, EClientSessionStates.Running)
                .WithEnterAction(() =>
                {
                    
                    UnityAssetManager.ProcessLoadedRequestTime = 4;
                    BackroundloadSettings.SetCurrentSettings(defaultSettings);
                    if (SharedConfig.DisableGc)
                    {
                        SingletonManager.Get<gc_manager>().disable_gc();
                        SingletonManager.Get<gc_manager>().gc_collect();
                    }
                    Application.targetFrameRate = 144;
                }).WithLevelAction(() =>
                {
                    SingletonManager.Get<gc_manager>().enable_gc();
                    SingletonManager.Get<gc_manager>().gc_collect();
                    Application.targetFrameRate = -1;
                })
            );

            Initialize((int) EClientSessionStates.LoadConfig);
        }
    }
}