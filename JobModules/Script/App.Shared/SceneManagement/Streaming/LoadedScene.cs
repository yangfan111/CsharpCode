using System.Collections.Generic;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;
using Object = UnityEngine.Object;

namespace App.Shared.SceneManagement.Streaming
{
    class LoadedScene
    {
        public Scene Scene { get; private set; }
        public readonly GameObject StreamingRoot;
        private Dictionary<int, UnityObject> _goInScene = new Dictionary<int, UnityObject>();
        private readonly StreamingScene _sceneDesc;

        public LoadedScene(Scene scene, StreamingScene sceneDesc)
        {
            Scene = scene;
            _sceneDesc = sceneDesc;
            StreamingRoot = new GameObject("StreamingRoot");
            SceneManager.MoveGameObjectToScene(StreamingRoot, Scene);
        }
    }
}