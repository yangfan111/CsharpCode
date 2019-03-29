using Core.Free;
using Core.Utils;
using Free.framework;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.Scene
{
    public class FogHandler : ISimpleMesssageHandler
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(FogHandler));

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.FROG_EFFECT;
        }

        public void Handle(SimpleProto simpleUI)
        {
            var near = simpleUI.Fs[0];
            var far = simpleUI.Fs[1];
            var colorString = simpleUI.Ss[0];
            var density = simpleUI.Fs[2];

            var fogId = simpleUI.Ins[0];

            var close = simpleUI.Bs[0];

            Color color;
            ColorUtility.TryParseHtmlString(string.Format("#{0}", colorString), out color);

            _logger.DebugFormat("Fog close {0} fogId {1} near {2} far {3} color {4} colorString {6} density {5}", close, fogId, near, far, color, density, colorString);

            var fog = SingletonManager.Get<FogManager>().GetFog(fogId);
            if (close && fog != null)
            {
                SingletonManager.Get<FogManager>().RemoveFog(fogId);
                return;
            }

            if (!close)
            {
                
            }
        }
    }
}
