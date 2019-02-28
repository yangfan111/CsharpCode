using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.System;
using Core.SessionState;
using Core.Utils;
using Utils.AssetManager;
using Utils.Singleton;

namespace Core.AssetManager
{
    public class UnityAssetManangerSystem : AbstractStepExecuteSystem
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(UnityAssetManangerSystem));
        private IUnityAssetManager _assetManager;
        public UnityAssetManangerSystem(
            ICommonSessionObjects commonSesionObjects)
        {
            _assetManager = commonSesionObjects.AssetManager;
        }


        protected override void InternalExecute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.UnityAssetManager);
                _assetManager.Update();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("UnityAssetManangerSystem Exception:{0}", e);

            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.UnityAssetManager);
            }
        }
    }
}
