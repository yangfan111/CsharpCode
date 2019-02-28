using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.App.Client.GameModules.UserInput;
using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using Object = UnityEngine.Object;

namespace App.Client.GameModules.UserInput
{
    public class InputManagerConfigInitSystem: IModuleInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputManagerConfigInitSystem));
        private Contexts _contexts;
        private ISessionState _sessionState;

        public InputManagerConfigInitSystem(Contexts contexts, ISessionState sessionState)
        {
            _contexts = contexts;
            _sessionState = sessionState;
            _sessionState.CreateExitCondition(typeof(InputManagerConfigInitSystem));
            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            assetManager.LoadAssetAsync(GetType().ToString(), new AssetInfo("tables", "input_manager"), OnLoadSucc);
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            SingletonManager.Get<SubProgressBlackBoard>().Step();
            if (!Luminosity.IO.InputManager.Exists)
            {
                Logger.ErrorFormat("No Luminosity.IO.InputManager exist");
            }
            else
            {
                var cfg = unityObj.As<TextAsset>();
                if (null != cfg)
                {
                    var content = cfg.bytes;
                    Luminosity.IO.InputManager.Load(content);
                }
                else
                {
                    Logger.Error("Cfg is null or not text asset");
                }
            }
            _sessionState.FullfillExitCondition(typeof(InputManagerConfigInitSystem));
        }
    }
}
