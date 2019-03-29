using System;
using System.Collections.Generic;
using Core.Common;
using Core.Utils;
using Shared.Scripts;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.GameModules.Effect
{
    public class GlobalEffectManager : IGlobalEffectManager
    {
        private readonly LoggerAdapter _logger = new LoggerAdapter(typeof(GlobalEffectManager));
        private readonly Dictionary<string, IEffectController> _effects = new Dictionary<string, IEffectController>();
        private readonly DummyEffectController _dummyEffectController = new DummyEffectController();
        private readonly Dictionary<string, AssetInfo> _assetBundles = new Dictionary<string, AssetInfo>();
        public const string GlobalGroundPropFlash = "GlobalGroundPropFlash";

        public GlobalEffectManager()
        {
            _assetBundles[GlobalGroundPropFlash] = new AssetInfo("effect/common", "GlobalGroundPropFlash".ToLower());
        }

        public void AddGameObject(string effectName, GameObject obj)
        {
            IEffectController effect;
            if (_effects.TryGetValue(effectName, out effect))
            {
                effect.AddGameObject(obj);
            }
        }

        public void RemoveGameObject(string effectName, GameObject obj)
        {
            IEffectController effect;
            if (_effects.TryGetValue(effectName, out effect))
            {
                effect.RemoveGameObject(obj);
            }
        }

        public IEffectController GetEffectController(string effectName)
        {
            IEffectController ret;
            if (_effects.TryGetValue(effectName, out ret))
            {
                return ret;
            }

            return _dummyEffectController;
        }

        private Action _allLoadSucc;
        public void LoadAllGlobalEffect(IUnityAssetManager assetManager, Action allLoadSucc)
        {
            foreach (var kv in _assetBundles)
            {
                assetManager.LoadAssetAsync(kv.Key, kv.Value, OnLoadSucc);
            }

            _allLoadSucc = allLoadSucc;

        }

        private void OnLoadSucc(string effectKey, UnityObject unityObj)
        {
            _logger.InfoFormat("OnLoadSucc {0}", effectKey);
            if (unityObj.AsGameObject!=null)
            {
                var effect = unityObj.AsGameObject.GetComponent<AbstractEffectMonoBehaviour>();
                _logger.InfoFormat("OnLoadSucc {0} {1}", effectKey, effect);
                if (effect != null)
                {
                    _effects[effectKey] = effect;
                }
                else
                {
                    _logger.ErrorFormat("OnLoad Failed {0} {1}", effectKey, effect);
                    _effects[effectKey] =new DummyEffectController();
                }
            }
            else
            {
                _logger.ErrorFormat("OnLoad Failed {0} null", effectKey);
                _effects[effectKey] =new DummyEffectController();
            }
            
            if (_effects.Count == _assetBundles.Count)
            {
                _allLoadSucc();
            }
        }
    }
}