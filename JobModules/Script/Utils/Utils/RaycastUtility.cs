using System;
using UnityEngine;

namespace Core.Utils
{
    public struct ThicknessInfo
    {
        public Vector3 OutPoint;
        public float Thickness;
        public Vector3 Normal; 
    } 

    public static class RaycastUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(RaycastUtility));

        private const float MaxStep = 10;
        private const float Step = 0.2f;
        private const float MaxDistance = 1000;
        private const float Offset = 0.001f;

        public static bool GetColliderThickness(RaycastHit hit , Vector3 velocity, out ThicknessInfo thicknessInfo)
        {
            thicknessInfo = new ThicknessInfo();
            var dir = velocity.normalized;
            var i = 1;
            for (; i <= MaxStep; i++)
            {
                var start = hit.point + dir * Step * i;
                var ray = new Ray(start, -dir);
                //Debug.DrawLine(start, start + -dir * Step, Color.blue, 5f);
                RaycastHit nextHit;
                if (Physics.Raycast(ray, out nextHit, Step + Offset))
                {
                    thicknessInfo.Thickness = Vector3.Distance(nextHit.point, hit.point);
                    thicknessInfo.OutPoint = nextHit.point;
                    thicknessInfo.Normal = nextHit.normal;
                    return true;
                }
            }
            return false;
        }

        public static string GetMaterialByHit(RaycastHit hit)
        {
            var go = hit.transform.gameObject;
            var meshRenderer = go.GetComponent<MeshRenderer>();
            if(null == meshRenderer || !meshRenderer.enabled)
            {
                return string.Empty;
            }
            //只有一种材质不需要进行submesh的检测
            var matCount = meshRenderer.sharedMaterials.Length;
            if(matCount == 1)
            {
                return meshRenderer.sharedMaterials[0].name;
            }
            if (hit.triangleIndex < 0)
            {
                Logger.WarnFormat("illegal triagnle index {0}, mesh collder {1} should not be convex", hit.triangleIndex, hit.transform.name);
                return string.Empty;
            }

            var mesh = GetMesh(hit);
            if (null == mesh)
            {
                Logger.ErrorFormat("No mesh attached to {0}", go);
                return string.Empty;
            }

            var startIndex = hit.triangleIndex * 3;
            var hitTriangles = new int[]
            {
                mesh.triangles[startIndex],
                mesh.triangles[startIndex + 1],
                mesh.triangles[startIndex + 2],
            };
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                var subMeshTris = mesh.GetTriangles(i);
                for (int j = 0; j < subMeshTris.Length; j += 3)
                {
                    if (subMeshTris[j] == hitTriangles[0] && subMeshTris[j + 1] == hitTriangles[1] &&
                        subMeshTris[j + 2] == hitTriangles[2])
                    {
                        var mat = GetMaterial(go, i);
                        if (null != mat)
                        {
                            return mat.name;
                        }
                        else
                        {
                        Logger.Error("Get material failed ");
                        }
                    }
                }
            }
            Logger.WarnFormat("No matched triangle ");
            return string.Empty;
        }

        private static Material GetMaterial(GameObject go, int index)
        {
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (null != renderer)
            {
                return renderer.sharedMaterials[index];
            }

            foreach (Transform child in go.transform)
            {
                if (child.name.EndsWith("LOD0", StringComparison.Ordinal))
                {
                    renderer = child.GetComponent<MeshRenderer>();
                }
            }

            if (null == renderer)
            {
                return null;
            }

            return renderer.sharedMaterials[index];
        }

        public static Mesh GetMesh(RaycastHit hit)
        {
            var meshCol = hit.collider as MeshCollider;
            if (null == meshCol)
            {
                Logger.WarnFormat("No meshCollider found ");
                return null;
            }
           return meshCol.sharedMesh;
        }

        public static void GetLod()
        {
            
        }


        public static Vector3 GetLegalPosition(Vector3 pos)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 0.5f, Vector3.down, out hit))
            {
                return hit.point + Vector3.up * 0.2f;
            }
            return pos;
        }

    }
}
