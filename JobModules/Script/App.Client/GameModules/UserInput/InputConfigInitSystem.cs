using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;
using Utils.AssetManager;
using Utils.Singleton;

namespace Assets.App.Client.GameModules.UserInput
{
    public class InputConfigInitSystem : IModuleInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputConfigInitSystem));

        private UserInputContext _inputContext;

        private ISessionState _sessionState;

        public InputConfigInitSystem(Contexts contexts, ISessionState sessionState)
        {
            _inputContext = contexts.userInput;
            _sessionState = sessionState;

            _sessionState.CreateExitCondition(typeof(InputConfigInitSystem));
            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            assetManager.LoadAssetAsync(GetType().ToString(), new AssetInfo("tables", "input"), OnLoadSucc);
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            SingletonManager.Get<SubProgressBlackBoard>().Step();
            var manager = new InputManager();
            if (!_inputContext.hasUserInputManager)
            {
                var helper = new UserInputHelper(manager);
                _inputContext.SetUserInputManager(manager, helper);
            }
            {
                var cfg = unityObj.As<TextAsset>();
                if (null != cfg)
                {
                    var content = cfg.text;
                    var inputCfg = InputConfigLoader<InputConfig>.Load(content);
                    _inputContext.userInputManager.Instance.SetConfig(inputCfg);
                }
                else
                {
                    Logger.Error("Cfg is null or not text asset");
                }
            }
            _sessionState.FullfillExitCondition(typeof(InputConfigInitSystem));
        }
    }
}
