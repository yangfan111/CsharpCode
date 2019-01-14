using System;
using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using System.Threading;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public interface IConfigInit
    {
        void ParseConfig();
    }

    public class DefaultConfigInitSystem<T> : IResourceLoadSystem, IConfigInit
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultConfigInitSystem<T>));
        private bool _isLoding;
        private ISessionState _sessionState;
        private AssetInfo _assetInfo;
        private IConfigParser _parser;
        private volatile bool _isDone = false;
        private volatile bool _isExit = false;
        private string _cfg;

        private SubResourceLoadSystem _subSystem;

        private string GetConditionId()
        {
            return string.Format("DefaultConfigInitSystem-{0}-{1}", typeof(T).Name, _assetInfo);
        }

        public DefaultConfigInitSystem(ISessionState sessionState, AssetInfo asset, IConfigParser parser, SubReousourcesHandler subResourceHandler)
        {
            _sessionState = sessionState;
            _assetInfo = asset;
            _sessionState.CreateExitCondition(GetConditionId());
            _parser = parser;

            _subSystem = new SubResourceLoadSystem(subResourceHandler);
            SingletonManager.Get<SubProgressBlackBoard>().Add();
        }

        public bool IsDone
        {
            get { return _isDone && _subSystem.IsDone; }
            set { _isDone = value; }
        }

        public void OnLoadResources(ILoadRequestManager loadRequestManager)
        {
            if (!_isLoding)
            {
                _isLoding = true;
                loadRequestManager.AppendLoadRequest(null, _assetInfo, OnLoadSucc);
            }

            if (IsDone && !_isExit)
            {
                _sessionState.FullfillExitCondition(GetConditionId());
                _isExit = true;
                Logger.InfoFormat("Exit  {0}", GetConditionId());
            }
        }

        public void OnLoadSucc(object source, AssetInfo assetInfo, UnityEngine.Object obj)
        {
            var asset = obj as TextAsset;
            if (null == asset)
            {
                Logger.ErrorFormat("Asset {0}:{1} Load Fialed ", assetInfo.BundleName, assetInfo.AssetName);
                return;
            }

            _cfg = asset.text;
            if (null == _parser)
            {
                Logger.ErrorFormat("instance to parse config {0} is null ", typeof(T));
                return;
            }

            Logger.InfoFormat("ParseConfig {0}:{1}", assetInfo.BundleName, assetInfo.AssetName);

            ThreadPool.QueueUserWorkItem(ParseConfig, this);

            _subSystem.OnLoadSucc(assetInfo, obj);
            SingletonManager.Get<SubProgressBlackBoard>().Step();
        }

        public void ParseConfig()
        {
            _parser.ParseConfig(_cfg);
            IsDone = true;
            Logger.InfoFormat("ParseConfig {0}", GetConditionId());
        }

        public static void ParseConfig(object o)
        {
            var system = o as IConfigInit;
            system.ParseConfig();
        }
    }
}