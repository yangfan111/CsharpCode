using App.Client.GameModules.ClientInit;
using App.Shared.Components;
using Core.GameModule.Module;
using Core.SessionState;
using Entitas;
using I2.Loc;

namespace App.Client.SessionStates
{
    public class WaitSnapshotState: AbstractSessionState
    {
        
        

       

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = new Entitas.Systems();
            systems.Add(new RequestSnapshotSystem(_contexts, this));
            
            return systems;
        }



        public WaitSnapshotState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.watisnapshot; }
        }
    }
}
