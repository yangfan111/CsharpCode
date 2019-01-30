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
        private readonly GameObject _streamingRoot;
        private Dictionary<int, UnityObjectWrapper<GameObject>> _goInScene = new Dictionary<int, UnityObjectWrapper<GameObject>>();
        private readonly StreamingScene _sceneDesc;

        public LoadedScene(Scene scene, StreamingScene sceneDesc)
        {
            Scene = scene;
            _sceneDesc = sceneDesc;
            _streamingRoot = new GameObject("StreamingRoot");
            SceneManager.MoveGameObjectToScene(_streamingRoot, Scene);
        }

        public void AddGo(UnityObjectWrapper<GameObject> go, int index)
        {
            _goInScene.Add(index, go);
            go.Value.transform.SetParent(_streamingRoot.transform);
            go.Value.transform.localPosition = _sceneDesc.Objects[index].Position;
            go.Value.transform.localEulerAngles = _sceneDesc.Objects[index].Rotation;
            go.Value.transform.localScale = _sceneDesc.Objects[index].Scale;
        }

        public UnityObjectWrapper<GameObject> RemoveGo(int index)
        {
            UnityObjectWrapper<GameObject> go = null;

            if (_goInScene.ContainsKey(index))
            {
                go = _goInScene[index];
                _goInScene.Remove(index);
            }

            return go;
        }

        public void Clear()
        {
            _goInScene.Clear();
        }
    }
}