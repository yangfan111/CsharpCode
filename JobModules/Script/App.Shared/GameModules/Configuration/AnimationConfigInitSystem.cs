using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.AssetManager;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Shared.GameModules.Configuration
{
    public class AnimationConfigInitSystem : IModuleInitSystem
    {
        private static readonly  LoggerAdapter _logger = new LoggerAdapter(typeof(AnimationConfigInitSystem));


        private ISessionState _sessionState;

        public AnimationConfigInitSystem(Contexts contexts, ISessionState sessionState)
        {
            _sessionState = sessionState;
            _logger.DebugFormat("CreateExitCondition AnimationConfigInitSystem");
            _sessionState.CreateExitCondition(typeof(AnimationConfigInitSystem));
        }

        /// <summary>
        /// 客户端加载
        /// </summary>
        /// <param name="manager"></param>
        public void OnInitModule(IUnityAssetManager assetManager)
        {
            assetManager.LoadAssetAsync(GetType().ToString(), AssetConfig.GetAnimationConfigAssetInfo(), OnLoadSucc);
        }

        private void ParseComplete()
        {
            _sessionState.FullfillExitCondition(typeof(AnimationConfigInitSystem));
        }

        private void ParseAnimationConfig(string configText)
        {
//            StateMachineSpeedConfig.ParseFromString(configText);
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            _logger.DebugFormat("OnLoadSucc {0} ", unityObj.Address);
            TextAsset asset = unityObj.As<TextAsset>();
            if (asset == null)
            {
                _logger.Error("Null animation config for equipment");
            }
            else
            {
                ParseAnimationConfig(asset.text);
            }
            ParseComplete();
        }
    }
}
