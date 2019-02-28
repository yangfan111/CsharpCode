using Assets.Sources.Free.UI;
using Core.Free;
using Core.Utils;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.Effect
{
    public class EffectCreateHandler : ISimpleMesssageHandler
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(EffectCreateHandler));

        private const string DEBUG_ID = "debug_effect";

        private static FreeRenderObject debug;

        public EffectCreateHandler()
        {
        }

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.MSG_EFFECT_CREATE;
        }

        public void Handle(SimpleProto simpleUI)
        {
            if (simpleUI.Bs[0] == true)
            {
                var ui = build(simpleUI);

                if (ui.key == DEBUG_ID)
                {
                    if (debug != null)
                    {
                        SingletonManager.Get<FreeEffectManager>().RemoveEffect(debug);
                    }
                    debug = ui;
                }

                SingletonManager.Get<FreeEffectManager>().AddEffect(ui);
            }
            else
            {
                simpleUI.Bs[0] = true;
                SingletonManager.Get<FreeEffectManager>().CacheEffect(simpleUI.Ss[0], simpleUI);
            }

        }

        // ks ks[0]为无效字段，一个component一个值，代表类型
        // fs 一个component9个值，代表x,y,z, sx,sy,sz, rx,ry,rz
        // bs[0] 表示初始是否展示UI
        // ss ss[0]为UI的key, ss[1]为UI的 autos, 一个component2个值，代表初始化的值
        private FreeRenderObject build(SimpleProto sp)
        {
            var go = new GameObject(sp.Ss[0]);
            var ui = go.AddComponent<FreeRenderObject>();

            ui.key = sp.Ss[0];
            ui.raderImage = new RaderImage();
            ui.raderImage.img = sp.Ss[1];
            ui.Visible = sp.Bs[0];
            ui.needPvs = sp.Bs[1];

            ui.SetPos(sp.Fs[0], sp.Fs[1], sp.Fs[2],
                sp.Fs[3], sp.Fs[4], sp.Fs[5],
                sp.Fs[6], sp.Fs[7], sp.Fs[8]);

            for (var i = 1; i < sp.Ks.Count; i++)
            {
                var newPo = FreeUIUtil.GetInstance().GetEffect(sp.Ks[i]);

                if (newPo == null)
                {
                    Logger.ErrorFormat("Effect Component is not exist {0}", sp.Ks[i]);
                    continue;
                }

                newPo.Initial(sp.Ss[i * 2 + 2], sp.Ss[i * 2 + 3]);

                newPo.SetPos(sp.Fs[9 * i], sp.Fs[9 * i + 1], sp.Fs[9 * i + 2],
                    sp.Fs[9 * i + 3], sp.Fs[9 * i + 4], sp.Fs[9 * i + 5],
                    sp.Fs[9 * i + 6], sp.Fs[9 * i + 7], sp.Fs[9 * i + 8]);

                ui.AddEffect(newPo);
            }

            ui.InitialAuto(sp.Ss[2]);

            return ui;
        }

    }
}
