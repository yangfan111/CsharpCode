using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.SessionState;
using Core.WeaponLogic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Shared
{
    public class ClientWeaponConfigInitSystem : IResourceLoadSystem
    {
        private Contexts _contexts;
        private ISessionState _sessionState;
        
        public ClientWeaponConfigInitSystem(Contexts contexts,
            ISessionState sessionState)
        {
            _contexts = contexts;
            this._sessionState = sessionState;
            _sessionState.CreateExitCondition(typeof(ClientWeaponConfigInitSystem));
        }

        public void OnLoadResources(ILoadRequestManager loadRequestManager)
        {
            if (_contexts.session.entityFactoryObject.WeaponLogicFactory == null)
                loadRequestManager.AppendLoadRequest(null, AssetConfig.GetWeaponConfigAssetInfo(), OnLoadSucc);
        }
        

        public void OnLoadSucc(object source, AssetInfo assetInfo, Object obj)
        {
            var ta = obj as TextAsset;
            _sessionState.FullfillExitCondition(typeof(ClientWeaponConfigInitSystem));
        }

    }
}