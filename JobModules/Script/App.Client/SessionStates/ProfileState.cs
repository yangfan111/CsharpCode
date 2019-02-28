using App.Client.GameModules.SceneManagement;
using App.Client.Tools;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.Configuration;
using App.Shared.SceneManagement;
using Core.GameModule.Module;
using Core.SessionState;
using Entitas;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.SessionStates
{
    public class ProfileState : AbstractSessionState
    {
        public ProfileState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
        }

        

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            var ctx = (Contexts) contexts;
            var systems = new Feature("ProfileState");

            ctx.player.flagSelfEntity.cameraObj.MainCamera = Camera.main;
            ctx.player.flagSelfEntity.cameraObj.MainCamera.transform.position =
                ctx.player.flagSelfEntity.position.Value;
            
            CompositeGameModule module = new CompositeGameModule();
            module.AddSystem(new VisionCenterUpdateSystem(ctx));
            
            systems.Add(new AutoTerrainNavigatorSystem((Contexts) contexts));
            systems.Add(new ProfileFeature(module, ctx.session.commonSession));
            
            return systems;
        }
    }
}