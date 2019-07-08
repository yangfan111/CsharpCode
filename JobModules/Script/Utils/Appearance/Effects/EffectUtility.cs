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
        private static Dictionary<string, AbstractEffectMonoBehaviour> _monoDict = new Dictionary<string, AbstractEffectMonoBehaviour>();
        private static Transform _effectNode;
        private static readonly string GodModeEffectName = "GodModeEffect";
        
        public static bool HasEffectNode(GameObject rootGo)
        {
            if (_effectNode != null) return true;
            for(int i=0;i<rootGo.transform.childCount;i++)
            {
                var child = rootGo.transform.GetChild(i);
                if (child.name == "Effects")
                {
                    _effectNode = child;
                    return true;
                }
            }
            return false;
        }
        public static void ReflushGodModeEffect(GameObject rootGo, GameObject item)
        {
            if (rootGo == null || item == null) return;
            if (!HasEffectNode(rootGo)) return;
            
            var script = GetEffect(GodModeEffectName);
            if (script == null) return;
            script.AddGameObject(item);
            _logger.DebugFormat("player:{0}, get GodModeEffect of obj: {1}", rootGo, item);
        }
        
        public static void RegistEffect(GameObject rootGo, UnityObject obj)
        {
            if (!HasEffectNode(rootGo)) return;
            var script = obj.AsGameObject.transform.GetComponentInChildren<AbstractEffectMonoBehaviour>();
            if (script != null)
            {
                if(!_monoDict.ContainsKey(script.GetEffectName()))
                    _monoDict.Add(script.GetEffectName(), script);
            }
        }

        public static void DeleteGodModeEffect(GameObject rootGo, GameObject item)
        {
            if (rootGo == null || item == null) return;
            if (!HasEffectNode(rootGo)) return;

            var script = GetEffect(GodModeEffectName);
            if (script == null) return;
            script.RemoveGameObject(item);
            _logger.DebugFormat("player:{0}, delete GodModeEffect of obj: {1}", rootGo, item);
        }

        public static AbstractEffectMonoBehaviour GetEffect(string name)
        {
            if (!_monoDict.ContainsKey(name))
                return null;
            return _monoDict[name];
        }
    }
}