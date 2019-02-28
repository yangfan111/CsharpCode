using Core.EntityComponent;

namespace App.Shared.CommonResource
{
    public class AssetRequestSource
    {
        public readonly int ComponentId;
        public readonly EntityKey EntityKey;
        public readonly int ResIndex;
        public readonly int TimeLine;

        public AssetRequestSource(EntityKey entityKey, int componentId, int resIndex, int timeLine)
        {
            EntityKey = entityKey;
            ComponentId = componentId;
            ResIndex = resIndex;
            TimeLine = timeLine;
        }
    }
}