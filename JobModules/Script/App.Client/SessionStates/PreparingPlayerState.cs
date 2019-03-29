using App.Client.ClientSystems;
using App.Client.GameModules.ClientInit;
using App.Shared.Components;
using App.Shared.Components.ClientSession;
using App.Shared.GameModules;
using Core.GameModule.Module;

using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.SessionStates
{
    public class PreparingPlayerState : AbstractSessionState
    {
       
        


        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = new Feature("PreparingPlayer");
            
           
            systems.Add(new RequestPlayerInfoSystem(_contexts, this));
            systems.Add(new ClientPreparePlayerMainFeature("MainFeature",
                GameModuleFactory.CreatePreparePlayerGameModule(_contexts), _contexts.session.commonSession));
            systems.Add(new CheckPreparingPlayerSystem(_contexts, this));
            return systems;
        }



        public PreparingPlayerState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.preparingplayer; }
        }
    }
}
