
using UnityEngine;

namespace Utils.Appearance.WardrobePackage
{
    public abstract class MyLodGroup
    {
        public static int GetLogLevel(GameObject go)
        {
            if (null == go) return -1;
            var group = go.GetComponent<LODGroup>();
            if (null == group) return -1;

            var lodTransform = group.transform;
            for (var i = 1; i < lodTransform.childCount; ++i)
            {
                var renderer = lodTransform.GetChild(i).GetComponent<Renderer>();
                if (renderer != null && renderer.isVisible)
                {
                    return i - 1;
                }
            }
            
            return -1;
        }

        public static void SetLogLevel(GameObject go, int level)
        {
            if(null == go || level < 0 || !go.activeInHierarchy) return;
            
            var group = go.GetComponent<LODGroup>();
            if (null == group) return;

            var index = level;
            if (level >= group.lodCount)
                index = group.lodCount - 1;
            
            group.ForceLOD(index);
        }

//        public static int GetLogLevel(GameObject go)
//        {
//            if (null == go) return -1;
//            var group = go.GetComponent<LODGroup>();
//            if (null == group) return -1;
//
//            return GetVisibleLod(group);
//        }
        
//        private static int GetVisibleLod(LODGroup lodGroup)
//        {
//            var lods = lodGroup.GetLODs();
//            var relativeHeight = GetRelativeHeight(lodGroup, Camera.main);
//
//            var lodIndex = GetMaxLod(lodGroup);
//            for (var i = 0; i < lods.Length; i++)
//            {
//                var lod = lods[i];
//
//                if (relativeHeight >= lod.screenRelativeTransitionHeight)
//                {
//                    lodIndex = i;
//                    break;
//                }
//            }
//
//            return lodIndex;
//        }
//        
//        private static float GetRelativeHeight(LODGroup lodGroup, Camera camera)
//        {
//            if (null == camera) return 0;
//            var distance = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
//            return DistanceToRelativeHeight(camera, (distance / QualitySettings.lodBias), GetWorldSpaceSize(lodGroup));
//        }
//        
//        private static float DistanceToRelativeHeight(Camera camera, float distance, float size)
//        {
//            if (camera.orthographic)
//                return size * 0.5F / camera.orthographicSize;
//
//            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
//            var relativeHeight = size * 0.5F / (distance * halfAngle);
//            return relativeHeight;
//        }
//        
//        private static int GetMaxLod(LODGroup lodGroup)
//        {
//            return lodGroup.lodCount - 1;
//        }
//        
//        private static float GetWorldSpaceSize(LODGroup lodGroup)
//        {
//            return GetWorldSpaceScale(lodGroup.transform)/* * lodGroup.size*/;
//        }
//        
//        private static float GetWorldSpaceScale(Transform t)
//        {
//            var scale = t.lossyScale;
//            var largestAxis = Mathf.Abs(scale.x);
//            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.y));
//            largestAxis = Mathf.Max(largestAxis, Mathf.Abs(scale.z));
//            return largestAxis;
//        }
    }
}

