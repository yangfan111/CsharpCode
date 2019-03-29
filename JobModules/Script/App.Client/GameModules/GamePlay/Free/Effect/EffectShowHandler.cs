using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{
    public class EffectShowHandler : ISimpleMesssageHandler
    {

        private const string DEBUG_ID = "debug_effect";

        private static FreeRenderObject debug;

        public EffectShowHandler()
        {
        }

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_EFFECT_SHOW;
        }

        public void Handle(SimpleProto sp)
        {
            var totalTime = sp.Ks[0];

            var key = sp.Ss[0];

            var effect = SingletonManager.Get<FreeEffectManager>().GetEffect(key);

            if (effect != null)
            {
                if (sp.Fs.Count == 3)
                {
                    effect.Move(sp.Fs[0], sp.Fs[1], sp.Fs[2]);
                }
                if (totalTime < 0)
                {
                    effect.Visible = false;
                }
                else
                {
                    effect.Visible = true;
                    effect.Show(totalTime);
                }
            }
        }
    }
}
