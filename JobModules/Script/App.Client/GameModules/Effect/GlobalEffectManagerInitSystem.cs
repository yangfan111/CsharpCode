using App.Client.GameModules.UserInput;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.GameModules.Effect
{
    public class GlobalEffectManagerInitSystem : IModuleInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GlobalEffectManagerInitSystem));
        private Contexts _contexts;
        private ISessionState _sessionState;

        public GlobalEffectManagerInitSystem(Contexts contexts, ISessionState sessionState)
        {
            _contexts = contexts;
            _sessionState = sessionState;
            _sessionState.CreateExitCondition(typeof(GlobalEffectManagerInitSystem));
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            _contexts.session.clientSessionObjects.GlobalEffectManager.LoadAllGlobalEffect(assetManager,  AllLoadSucc);
        }

        private void AllLoadSucc()
        {
            _sessionState.FullfillExitCondition(typeof(GlobalEffectManagerInitSystem));
        }
    }
}