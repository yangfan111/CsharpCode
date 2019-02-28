using App.Client.GameModules.ClientInit;
using App.Shared.Components;
using App.Shared.WeaponLogic;
using App.Shared.WeaponLogic.Common;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.GameModule.Module;
using Core.SessionState;
using Core.WeaponLogic.Attachment;
using Entitas;
using I2.Loc;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.SessionStates
{
    public class RequestRoomInfoState : AbstractSessionState
    {
        private CompositeGameModule _gameModule;

        public override Systems CreateUpdateSystems(IContexts contexts)
        {
            Contexts _contexts = (Contexts)contexts;
     
            var systems = new Entitas.Systems();
            systems.Add(new RequestRoomInfoSystem(_contexts, this));
            MakeWeaponLogicManager(_contexts);
            return systems;
        }

        private void MakeWeaponLogicManager(Contexts contexts)
        {
            var commonSession = contexts.session.commonSession;
            commonSession.PlayerWeaponConfigManager = new PlayerWeaponConfigManager(SingletonManager.Get<WeaponPartsConfigManager>(),
                SingletonManager.Get<WeaponDataConfigManager>());

            var weaponComponentsFacoty = new WeaponLogicComponentsFactory(contexts,
                contexts.session.commonSession.EntityIdGenerator);
            var fireLogicCreator = new FireLogicProvider(contexts, weaponComponentsFacoty);
            commonSession.WeaponLogicManager = new WeaponLogicManager(SingletonManager.Get<WeaponDataConfigManager>(),
                SingletonManager.Get<WeaponConfigManager>(), fireLogicCreator, contexts.session.commonSession.FreeArgs);
        }

        public RequestRoomInfoState(IContexts contexts, EClientSessionStates state, EClientSessionStates next) : base(contexts,(int)state, (int) next)
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