using App.Shared.SceneManagement.Streaming;
using Core.SceneManagement;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.SceneManagement.Basic
{
    class FixedScenesManager : ISceneResourceManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FixedScenesManager));

        private readonly ISceneResourceRequestHandler _handler;
        private readonly OnceForAllParam _sceneParam;

        public FixedScenesManager(ISceneResourceRequestHandler handler, OnceForAllParam param)
        {
            _handler = handler;
            _sceneParam = param;
        }
        
        public void UpdateOrigin(Vector3 value, OriginStatus status)
        {
            // fixed scene move to levelmanager
        }

        public void SetAsapMode(bool value)
        {
            _logger.Warn("asap isn't applicable to fixedscenemanager");
        }
    }
}