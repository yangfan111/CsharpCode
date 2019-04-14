using App.Shared.Components;
using App.Shared.GameModules.Preparation;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;
using Entitas;
using UnityEngine;

namespace App.Client.SessionStates
{
    public class ProfilePreparationState : AbstractSessionState
    {
        public ProfilePreparationState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
            Contexts ctx = (Contexts) contexts;
            var player = ctx.player.CreateEntity();
            player.isFlagSelf = true;
            player.AddPosition();
            player.AddOrientation(0, 0, 0, 0, 0);
            player.AddCameraObj();
        }

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = new Feature("ProfilePreparationState");
            systems.Add(new ProfilePreparationFeature("ProfilePreparationState", CreateSystems(_contexts), _contexts.session.commonSession));

            return systems;
        }
        
        private IGameModule CreateSystems(Contexts contexts)
        {
            GameModule module = new GameModule();
            module.AddSystem(new InitMapIdSystem(contexts));

            return module;
        }
        
        public sealed class ProfilePreparationFeature : Feature
        {
            public ProfilePreparationFeature(string name,
                IGameModule topLevelGameModule,
                ICommonSessionObjects commonSessionObjects) : base(name)
            {
                topLevelGameModule.Init();
            
                Add(new ModuleInitSystem(topLevelGameModule, commonSessionObjects.AssetManager));
            }
        } 
    }
}