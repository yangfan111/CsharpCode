using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils.AssetManager
{
    public class DefaultGo
    {
        private static readonly string[] TargetScene = {"Hall", "ClientScene", "ServerScene"};

        private static GameObject[] _defaultParents = new GameObject[(int) GameRunningStage.EndOfTheWorld];

        private static GameRunningStage _stage = GameRunningStage.Hall;

        public static void SetStage(GameRunningStage stage, bool clearOthers)
        {
            _stage = stage;
            if (clearOthers)
            {
                for (int i = 0; i < _defaultParents.Length; i++)
                {
                    if (i != (int) stage)
                    {
                        _defaultParents[i] = null;
                    }
                }
            }
        }

        public static void Clear(GameRunningStage stage)
        {
            _defaultParents[(int) stage] = null;
        }

        public static GameObject CreateGameObject(string name)
        {
            var go = new GameObject(name);
            go.transform.parent = DefaultParent.transform;
            return go;
        }

        public static void SetParentToDefaultGo(GameObject go)
        {
            //go.transform.parent = DefaultParent.transform;
            go.transform.SetParent(DefaultParent.transform, false);
        }

        public static GameObject DefaultParent
        {
            get
            {
                if (_defaultParents[(int) _stage] == null)
                {
                    CreateDefaultGo(_stage);
                }

                return _defaultParents[(int) _stage];
            }
        }

        private static void CreateDefaultGo(GameRunningStage stage)
        {
            var go = new GameObject("DefaultGo" + stage);

//		   go.AddComponent<Canvas>().enabled = false;
            MoveToStageScene(stage, go);

            _defaultParents[(int) _stage] = go;
        }

        private static void MoveToStageScene(GameRunningStage stage, GameObject go)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name.StartsWith(TargetScene[(int) stage]))
                {
                    SceneManager.MoveGameObjectToScene(go, scene);

                    break;
                }
            }
        }

        private static GameObject _streamingRoot;

        public static GameObject StreamingRoot
        {
            get
            {
                if (_streamingRoot == null)
                {
                    _streamingRoot = new GameObject("StreamingRoot");
                    MoveToStageScene(_stage, _streamingRoot);
                }

                return _streamingRoot;
            }
        }

        public static void DisposeStreamGo()
        {
           GameObject.Destroy(_streamingRoot);
           _streamingRoot = null;
        }
    }

    public enum GameRunningStage
    {
        Hall,
        BattleClient,
        BattleServer,
        EndOfTheWorld
    }
}