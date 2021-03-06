﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.AssetManager;

namespace Core.SceneManagement
{
    public interface ITerrainRenderer
    {
        void SceneLoaded(Scene scene, LoadSceneMode mode);
        void SceneUnloaded(Scene scene);
        void GoLoaded(UnityObject obj);
        void GoUnloaded(UnityObject obj);
        void GetTerrainDataNames(List<string> names);
        void LoadedTerrainData(Object obj, UnityObject asset);
        bool IsLoadingEnd { get; }

        void SetCamera(Camera cam);
        void Draw();
    }
}