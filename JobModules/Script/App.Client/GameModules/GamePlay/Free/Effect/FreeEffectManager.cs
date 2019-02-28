using System.Collections.Generic;
using Entitas.VisualDebugging.Unity;
using Core.Utils.System46;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{
    public class FreeEffectManager : DisposableSingleton<FreeEffectManager>
    {
        private IDictionary<string, FreeRenderObject> effects;

        private MyDictionary<string, SimpleProto> effectCache;

        public FreeEffectManager()
        {
            effects = new Dictionary<string, FreeRenderObject>();
            effectCache = new MyDictionary<string, SimpleProto>();
        }

        public void CacheEffect(string key, SimpleProto effect)
        {
            effectCache[key] = effect;
        }

        public SimpleProto GetEffectData(string key)
        {
            if (effectCache.ContainsKey(key))
            {
                return effectCache[key];
            }
            else
            {
                return null;
            }
            
        }

        public FreeRenderObject GetEffect(string key)
        {
            FreeRenderObject ret;
            effects.TryGetValue(key, out ret);
            return ret;
        }

        public void RemoveEffect(FreeRenderObject effect)
        {
            if (effect != null)
            {
                effects.Remove(effect.key);
            }
        }

        public void AddEffect(FreeRenderObject effect)
        {
            var old = GetEffect(effect.key);
            if (old != null)
            {
                old.Visible = false;
                old.gameObject.DestroyGameObject();
            }
            effects[effect.key] = effect;
        }

        protected override void OnDispose()
        {
            foreach (var effect in effects.Values)
            {
                effect.Dispose();
            }

            effects.Clear();
            effectCache.Clear();
        }

        public IDictionary<string, FreeRenderObject> FreeEffects
        {
            get { return effects; }


        }

    }
}
