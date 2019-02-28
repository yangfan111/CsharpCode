using Core.Free;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{
    public class EffectDeleteHandler : ISimpleMesssageHandler
    {

        private const string DEBUG_ID = "debug_effect";

        private static FreeRenderObject debug;


        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_EFFECT_DELETE;
        }

        public void Handle(SimpleProto sp)
        {
            var key = sp.Ss[0];

            var effect = SingletonManager.Get<FreeEffectManager>().GetEffect(key);

            if(effect != null)
            {
                Object.Destroy(effect.gameObject);

                SingletonManager.Get<FreeEffectManager>().RemoveEffect(effect);
            }
        }
    }
}
