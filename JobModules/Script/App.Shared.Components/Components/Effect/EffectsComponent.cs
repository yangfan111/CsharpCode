using System.Collections.Generic;
using System.Security.Policy;
using Core.Components;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Shared.Scripts;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.Components.Effect
{
    [SceneObject, Player]
    public class EffectsComponent : IResetableComponent, IAssetComponent
    {
        private readonly HashSet<string> _globalEffects = new HashSet<string>();
        private readonly Dictionary<string, IEffectController> _effects = new Dictionary<string, IEffectController>();
        private readonly Dictionary<string, UnityObject> _unityObjects = new Dictionary<string, UnityObject>();
        private static DummyEffectController _dummyEffectController = new DummyEffectController();
        
        public HashSet<string> GlobalEffects 
        {
            get { return _globalEffects; }
        }

        public void Reset()
        {
            _unityObjects.Clear();
            _effects.Clear();
            _globalEffects.Clear();
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.Effects;
        }

        public void Recycle(IUnityAssetManager assetManager)
        {
            foreach (var unityObject in _unityObjects.Values)
            {
                assetManager.Recycle(unityObject); }

            _unityObjects.Clear();
            _effects.Clear();
            _globalEffects.Clear();
        }

        public void AddLocalEffect(UnityObject obj)
        {
            var effect = obj.AsGameObject.GetComponent<AbstractEffectMonoBehaviour>();
            if (effect != null)
            {
                var name = effect.GetEffectName();
                _unityObjects[name] = obj;
                _effects[name] = effect;
            }
        }

        public void RemoveLocalEffect(string effectName, IUnityAssetManager assetManager)
        {
            UnityObject ret;
            if (_unityObjects.TryGetValue(effectName, out ret))
            {
                _effects[effectName].Recycle();
                _unityObjects.Remove(effectName);
                _effects.Remove(effectName);

                assetManager.Recycle(ret);
            }
        }

        public void AddGlobalEffect(string effectName)
        {
            _globalEffects.Add(effectName);
        }

        public void RemoveEffect(string effectName)
        {
            _globalEffects.Remove(effectName);
        }
        
        public IEffectController GetEffect(string effectName)
        {
            IEffectController ret;
            if (_effects.TryGetValue(effectName, out ret))
            {
                return ret;
            }

            return _dummyEffectController;
        }

        public void ResetEffects()
        {
            foreach (var value in _effects.Values)
            {
                value.Recycle();
            }
        }
    }
}