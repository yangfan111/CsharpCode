using System;
using UnityEngine;

namespace App.Client.SceneManagement.DistanceCulling
{
    class Constants
    {
        public static readonly DistCullingParam[] DistCullingCats = 
        {
            new DistCullingParam { Cat = DistCullingCat.Detail, Dist = 20 * 20, RedundantDist = 25 * 25 },
            new DistCullingParam { Cat = DistCullingCat.Near, Dist = 50 * 50, RedundantDist = 55 * 55 },
            new DistCullingParam { Cat = DistCullingCat.Median, Dist = 150 * 150, RedundantDist = 155 * 155 }, 
            new DistCullingParam { Cat = DistCullingCat.Far, Dist = 1000 * 1000, RedundantDist = 1005 * 1005 }, 
        };

        public static readonly int DistCullingCatCount;
        
        static Constants()
        {
            DistCullingCatCount = DistCullingCats.Length;
        }

        /// <summary>
        /// 大小最接近的节点，作为判断相机是否靠近物体密集处的基础
        /// </summary>
        public const float CloseToArticlesStandard = 32;
        /// <summary>
        /// 满足上面要求的节点，包含的物体个数的需要满足的标准
        /// </summary>
        public const float ArticlesCountStandard = 20f;
        
        private const float NearMedian = 0.5f;
        private const float MedianFar = 1f;   
        
        public static void SetCullingDistance(DistCullingCat cat, float value)
        {
            for (int i = 0; i < DistCullingCats.Length; i++)
            {
                if (DistCullingCats[i].Cat == cat)
                    DistCullingCats[i].Dist = value * value;
            }
        }

        public static DistCullingCat GetDistCullingCatForStreamingGo(Vector3 size)
        {
            var paramLength = Math.Max(Math.Max(size.x, size.y), size.z);

            if (paramLength < NearMedian)
                return DistCullingCat.Near;

            if (paramLength < MedianFar)
                return DistCullingCat.Median;

            return DistCullingCat.Far;
        }

        public static DistCullingCat GetDistCullingCatForTag(int tagValue)
        {
            if (MultiTagHelper.InDoor(tagValue))
                return DistCullingCat.Near;

            return DistCullingCat.Far;
        }
    }

    enum DistCullingCat
    {
        Detail,
        Near,
        Median,
        Far,
        EndOfTheWorld
    }

    class DistCullingParam
    {
        public DistCullingCat Cat;
        public float Dist;
        public float RedundantDist;
    }
}