using System;
using Shared.Scripts;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.AssetManager;

namespace Core.Common
{
    public interface IGlobalEffectManager
    {
        void AddGameObject(string effectName, GameObject obj);
        void RemoveGameObject(string effectName, GameObject obj);
        IEffectController GetEffectController(string effectName);
        void LoadAllGlobalEffect(IUnityAssetManager assetManager, Action allLoadSucc);
    }
}