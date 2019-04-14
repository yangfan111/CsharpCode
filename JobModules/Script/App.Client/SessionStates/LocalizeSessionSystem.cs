using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using I2.Loc;
using UnityEngine;
using Utils.AssetManager;
using Entitas;
namespace App.Client.SessionStates
{
    public class LocalizeSessionSystem : Systems

    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(LocalizeSessionSystem));

        private ISessionState _sessionState;
        private bool _isStartLoad = false;
        const string ASSETBUNDLENAME = "i18nprefab";
        const string ASSETNAME = "ssjjLanguage";
        IUnityAssetManager _assetManager;
        public LocalizeSessionSystem(ISessionState sessionState,IUnityAssetManager assetManager)
        {
            _sessionState = sessionState;
            _assetManager = assetManager;
            sessionState.CreateExitCondition(typeof(LocalizeSessionSystem));
            assetManager.LoadAssetAsync("", new AssetInfo(ASSETBUNDLENAME, ASSETNAME), OnLoadSucc);
        }

     

        public void OnLoadSucc(string s,UnityObject unityObj)
        {
            var assetInfo = unityObj.Address;
            GameObject go = unityObj.AsGameObject;
            //UnityEngine.Object.DontDestroyOnLoad(go);
            I18nStarter.LoadTxt();
            _sessionState.FullfillExitCondition(typeof(LocalizeSessionSystem));
        }

    
    }
    
}
