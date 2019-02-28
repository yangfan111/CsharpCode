using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui;
using App.Shared.Components;
using App.Shared.GameModules.Preparation;
using Core.AssetManager;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.ClientGameModules
{
    public class InitUiModuleState : AbstractSessionState
    {
        public InitUiModuleState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
            
        }

       
        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            var systems = new Feature("MapLoadState");
            var contextsImpl = contexts as Contexts;
            var commonSession = contextsImpl.session.commonSession;

            systems.Add(new SceneResourceLoadingFeature("SceneResourceLoading", CreateSystems(contextsImpl),
                commonSession));

            return systems;
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.sceneresourceload; }
        }

        private IGameModule CreateSystems(Contexts contexts)
        {
            var module = new CompositeGameModule();

            module.AddModule(new UiLoadModule(this, contexts));

            return module;
        }
        
        class SceneResourceLoadingFeature : Feature
        {
            public SceneResourceLoadingFeature(string name,
                IGameModule topLevelGameModule,
                ICommonSessionObjects commonSessionObjects) : base(name)
            {
                topLevelGameModule.Init();
            
                Add(new UnityAssetManangerSystem(commonSessionObjects));
                Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            }
        } 
    }
}
