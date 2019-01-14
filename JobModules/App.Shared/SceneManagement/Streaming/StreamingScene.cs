using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace App.Shared.SceneManagement.Streaming
{
    class StreamingScene
    {
        public Scene Scene { get; private set; }
        private GameObject _streamingRoot;
        private Dictionary<int, GameObject> _goInScene = new Dictionary<int, GameObject>();

        public StreamingScene(Scene scene)
        {
            Scene = scene;
            _streamingRoot = new GameObject("StreamingRoot");
            SceneManager.MoveGameObjectToScene(_streamingRoot, Scene);
        }

        public void AddGo(GameObject go, int index)
        {
            _goInScene.Add(index, go);
            go.transform.SetParent(_streamingRoot.transform);
        }

        public void RemoveGo(int index)
        {
            _goInScene.Remove(index);
        }

        public void Clear()
        {
            _goInScene.Clear();
        }
    }
}