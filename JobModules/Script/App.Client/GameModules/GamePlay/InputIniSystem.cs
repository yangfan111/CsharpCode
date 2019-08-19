using App.Client.GameMode;
using App.Shared;
using Assets.Sources;
using Core.GameModule.Interface;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay
{
    /// <summary>
    /// Defines the <see cref="InputIniSystem" />
    /// </summary>
    public class InputIniSystem : IModuleInitSystem
    {
        private Contexts _contexts;

        public InputIniSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            var cmdGenerator = _contexts.session.clientSessionObjects.UserCmdGenerator as UnityUserCmdGenerator;
            if (null != cmdGenerator)
            {
                var mode = _contexts.session.commonSession.RoomInfo.ModeId;
#if UNITY_EDITOR
                if(mode == 0)
                {
                mode = SharedConfig.ModeId;
                }
#endif
                var bagType = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(mode);
                IGlobalKeyInputMapper inputMapper = null;
                switch (bagType)
                {
                    case XmlConfig.EBagType.Group:
                        inputMapper = new GroupModeInputMapper(_contexts);
                        break;
                    default:
                        inputMapper = new DefaultModeInputMapper();
                        break;
                }
                cmdGenerator.RegisterGlobalKeyReceiver(inputMapper);
                cmdGenerator.BeginReceiveUserInput(_contexts.userInput.userInputManager.Mgr);
            }
        }
    }
}
