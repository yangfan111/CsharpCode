using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Shared.Scripts.Effect;
using UnityEngine;
using Utils.AssetManager;

namespace Utils.Appearance.Effects
{
    public class EffectUtility
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(EffectUtility));

        private static Dictionary<GameObject, Dictionary<string, AbstractEffectMonoBehaviour>> _monoDicts =
            new Dictionary<GameObject, Dictionary<string, AbstractEffectMonoBehaviour>>();

        private static readonly string GodModeEffectName = "GodModeEffect";
        private static readonly string EffectsName = "Effects";
        public static Transform GetEffectNode(GameObject rootGo)
        {
            var effects = rootGo.transform.Find(EffectsName);
            if (effects != null) return effects;
            
            var effectNode = new GameObject(EffectsName);
            effectNode.transform.SetParent(rootGo.transform);
            effectNode.transform.localPosition = new Vector3(0, 0, 0);
            effectNode.transform.localRotation = Quaternion.identity;
            effectNode.transform.localScale = Vector3.one;

            return effectNode.transform;
        }
        
        public static void ReflushGodModeEffect(GameObject rootGo, GameObject item)
        {
            if (rootGo == null || item == null) return;
            if (GetEffectNode(rootGo)==null) return;
            
            var script = GetEffect(rootGo, GodModeEffectName);
            if (script == null) return;
            script.AddGameObject(item);
            _logger.DebugFormat("player:{0}, get GodModeEffect of obj: {1}", rootGo, item);
        }

        public static void DeleteGodModeEffect(GameObject rootGo, GameObject item)
        {
            if (rootGo == null || item == null) return;
            if (GetEffectNode(rootGo)==null) return;

            var script = GetEffect(rootGo, GodModeEffectName);
            if (script == null) return;
            script.RemoveGameObject(item);
            _logger.DebugFormat("player:{0}, delete GodModeEffect of obj: {1}", rootGo, item);
        }

        public static AbstractEffectMonoBehaviour GetEffect(GameObject rootGo, string name)
        {
            if (!_monoDicts.ContainsKey(rootGo))
                return null;
            if (!_monoDicts[rootGo].ContainsKey(name))
                return null;
            return _monoDicts[rootGo][name];
        }
        
        public static void RegistEffect(GameObject rootGo, GameObject obj)
        {
            if (GetEffectNode(rootGo)==null) return;
            var script = obj.transform.GetComponentInChildren<AbstractEffectMonoBehaviour>();
            if (script != null)
            {
                if(!_monoDicts.ContainsKey(rootGo))
                    _monoDicts.Add(rootGo,new Dictionary<string, AbstractEffectMonoBehaviour>());
                if (!_monoDicts[rootGo].ContainsKey(script.GetEffectName()))
                    _monoDicts[rootGo].Add(script.GetEffectName(), script);
            }
        }
    }
}