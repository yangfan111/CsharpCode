using System.Collections.Generic;
using UnityEngine;

namespace Utils.Appearance
{
    public class TransformCache:MonoBehaviour
    {
        public Dictionary<string, Transform> Cache;

        public TransformCache()
        {
            Cache = new Dictionary<string, Transform>();
        }
    }
}