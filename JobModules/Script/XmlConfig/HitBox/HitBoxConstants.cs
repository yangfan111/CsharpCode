using UnityEngine;

namespace XmlConfig.HitBox
{
    public static class HitBoxConstants
    {
        public static readonly string BoundingSphereGoName = "boundingsphere_hitbox";
        public static readonly string HitBoxGoName = "hitbox";

        public static GameObject FindBoundingSphereModel(GameObject modelGo)
        {
            
            foreach (Transform child in modelGo.transform)
            {
                if (IsBoundingSphereGo(child))
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public static bool IsHitBoxGo(Transform child)
        {
            var name = child.name.ToLower();
            bool isHitBoxGo = name.Contains(HitBoxConstants.HitBoxGoName) || IsBoundingSphereGo(child);
            return isHitBoxGo;
        }



        private static bool IsBoundingSphereGo(Transform child)
        {
            var name = child.name.ToLower();
            bool hasSphereCollider = child.GetComponent<SphereCollider>() != null;

            bool isHitBoxGo = name.Contains(BoundingSphereGoName);

            return isHitBoxGo && hasSphereCollider;
        }
    }
}