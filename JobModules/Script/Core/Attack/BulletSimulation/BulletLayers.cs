using Core.Utils;

namespace Core.Attack
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