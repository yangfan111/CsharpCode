using Core.AssetManager;
using Core.GameModule.Module;
using Core.GameModule.Step;
using Core.GameModule.System;

namespace App.Client.Tools
{
    public class ProfileFeature : Feature
    {
        public ProfileFeature(IGameModule topLevelGameModule,
            ICommonSessionObjects commonSessionObjects)
        {
            topLevelGameModule.Init();

            Add(new RenderSystem(topLevelGameModule).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new UnityAssetManangerSystem(commonSessionObjects).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
            Add(new ResourceLoadSystem(topLevelGameModule, commonSessionObjects.AssetManager).WithExecFrameStep(EEcecuteStep.NormalFrameStep));
        }
    }
}