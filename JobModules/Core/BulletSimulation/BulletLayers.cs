using Core.Utils;

namespace Core.BulletSimulation
{
    public static class BulletLayers
    {
        public static int GetBulletLayerMask()
        {
            int layerMask = UnityLayers.UnHitableLayerMask;
            layerMask = ~layerMask;
            return layerMask;
        }
    }
}