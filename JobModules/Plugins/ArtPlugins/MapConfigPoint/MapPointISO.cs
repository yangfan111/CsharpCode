using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtPlugins.MapConfig
{
    public class MapPointISO : MonoBehaviour
    {
         public int PointID = 0;
        [Header("启用方向数据")]
        public bool enableDir = false;
        public float dir = 0;
        [Header("启用圆柱体范围数据")]
        public bool enableCylinderVol = false;
        public float cylinderRadius = 0.5f;
        public float cylinderHeight = 1;
        void OnDrawGizmos()
        {
            if (enableDir)
            {
                Gizmos.color = Color.yellow;

                float rot = dir * Mathf.Deg2Rad + Mathf.PI / 2;
                Vector3 endPos = transform.position + new Vector3(-Mathf.Cos(rot), 0, Mathf.Sin(rot)) * 2;
                Gizmos.DrawLine(transform.position, endPos);
                Gizmos.DrawLine(endPos, transform.position + new Vector3(-Mathf.Cos(rot + Mathf.PI / 8), 0, Mathf.Sin(rot + Mathf.PI / 8)) * 1.6f);
                Gizmos.DrawLine(endPos, transform.position + new Vector3(-Mathf.Cos(rot - Mathf.PI / 8), 0, Mathf.Sin(rot - Mathf.PI / 8)) * 1.6f);
            }
            if (enableCylinderVol)
            {
                Gizmos.color = Color.yellow;
                Vector3 lastPos=Vector3.zero;
                Vector3 addHei = new Vector3(0, cylinderHeight, 0);
                for (int i = 0; i <= 12; i++)
                {
                    float rot = i * Mathf.PI * 2 / 12;
                    Vector3 pt = transform.position + new Vector3(-Mathf.Cos(rot), 0, Mathf.Sin(rot)) * cylinderRadius;
                    if (i > 0)
                    {
                        Gizmos.DrawLine(pt, pt + addHei);
                        Gizmos.DrawLine(pt, lastPos);
                        Gizmos.DrawLine(pt + addHei, lastPos + addHei);
                    }

                    lastPos = pt;
                }
            }
        }
    }
 
}
