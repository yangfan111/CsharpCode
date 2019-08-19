using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using App.Client.GameModules.UserInput;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

public class InputConfigManager : AbstractConfigManager<InputConfigManager>
{
    public readonly Dictionary<UserInputKey, InputConvertItem> itemsMap = new Dictionary<UserInputKey, InputConvertItem>(40);

    public InputConfig ConfigData { get; private set; }
    public override void ParseConfig(string xml)
    {
        ConfigData = XmlConfigParser<InputConfig>.Load(xml);
        foreach (var item in ConfigData.Items)
        {
            itemsMap[item.Key] = item;
        }
    }

    public InputConvertItem FindById(UserInputKey key)
    {
        InputConvertItem item;
        itemsMap.TryGetValue(key, out item);
        return item;
    }
}

namespace Assets.App.Client.GameModules.UserInput
{
    public class InputConfigInitSystem : IModuleInitSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(InputConfigInitSystem));

        private Contexts _contexts;
        private UserInputContext _inputContext;
        private ISessionState _sessionState;

        public InputConfigInitSystem(Contexts contexts, ISessionState sessionState)
        {
            _contexts = contexts;
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
            var manager = new GameInputManager(new  UserPointerProvider(_contexts));
            if (!_inputContext.hasUserInputManager)
            {
                var helper = new GameInputHelper(manager);
                _inputContext.SetUserInputManager(manager, helper);
            }
            {
                var cfg = unityObj.As<TextAsset>();
                if (null != cfg)
                {
                    string content = cfg.text;
                    SingletonManager.Get<InputConfigManager>().ParseConfig(content);
                    _inputContext.userInputManager.Mgr.Initialize(SingletonManager.Get<InputConfigManager>().ConfigData);
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
