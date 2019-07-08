using App.Client.GameModules.ClientInit;
using App.Shared.Components;
using App.Shared.GameModules.Weapon;
using App.Shared.GameModules.Weapon.Behavior;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.GameModule.Module;
using Core.SessionState;

using Entitas;
using I2.Loc;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.SessionStates
{
    public class RequestSceneInfoState : AbstractSessionState
    {
        private CompositeGameModule _gameModule;

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = new Entitas.Systems();
            systems.Add(new RequestSceneInfoSystem(_contexts, this));
            /*MakeWeaponLogicManager(_contexts);*/
            return systems;
        }

        private void MakeWeaponLogicManager(Contexts contexts)
        {
            var commonSession = contexts.session.commonSession;
            //commonSession.PlayerWeaponResourceConfigManager = new PlayerWeaponResourceConfigManager(SingletonManager.Get<WeaponPartsConfigManager>(),
            //    SingletonManager.Get<WeaponConfigManagement>());

           // var initializer = new WeaponFireScriptsInitializer(contexts,
           //     contexts.session.commonSession.EntityIdGenerator);
            var fireLogicCreator = new WeaponFireScriptsProvider(contexts);
            commonSession.WeaponFireUpdateManager = new WeaponFireUpdateManagaer(fireLogicCreator, contexts.session.commonSession.FreeArgs);
        }

        public RequestSceneInfoState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
        {
        }

        public override int LoadingProgressNum
        {
            get { return 1; }
        }

        public override string LoadingTip
        {
            get { return ScriptLocalization.client_loadtip.preloginrequest; }
        }

    }
}