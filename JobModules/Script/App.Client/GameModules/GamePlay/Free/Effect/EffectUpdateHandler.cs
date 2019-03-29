using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{
    public class EffectUpdateHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_EFFECT_UPDATE;
        }

        public void Handle(SimpleProto sp)
        {
            var key = sp.Ss[0];

            var fro = SingletonManager.Get<FreeEffectManager>().GetEffect(key);
            if (fro != null)
            {
                for (var i = 0; i < sp.Ks.Count; i++)
                {
                    var index = sp.Ks[i];
                    var auto = sp.Ins[i];
                    var v = sp.Ss[i + 1];

                    if (index == 0)
                    {
                        fro.ChangeAutoValue(auto, v);
                    }
                    else
                    {
                        var ef = fro.GetEffect(index - 1);
                        if (ef != null)
                        {
                            ef.SetValue(auto, v);
                        }
                    }
                }
            }
        }

    }
}
