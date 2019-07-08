using System;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace App.Client.GPUInstancing.Core.Data
{
    class LodActualRenderer
    {
        public Mesh Mesh;
        public Material[] Materials;
        public int SubMeshCount;
        public int[] IndexCount;
    }

    class LodRenderer
    {
        public LodActualRenderer[] Renderers;
    }

    public class InstancingRenderer
    {
        private float[] _lodRatios;

        // prototype -> several lod level -> several renderers -> several submesh
        private LodRenderer[] _lodRenderers;

        private Vector3 _sphereCenter;
        private float _sphereRadius;
        private float _lodSize;

        public Vector3 SphereCenter { get { return _sphereCenter; } }
        public float SphereRadius { get { return _sphereRadius; } }

        public float LodSize { get { return _lodSize; } }
        public float[] LodRatios { get { return _lodRatios; } }

        public bool ReceiveShadow { get; private set; }
        public ShadowCastingMode CastShadow { get; private set; }
        
        public InstancingRenderer(DetailPrototype prototype)
        {
            if (prototype.usePrototypeMesh)
            {
                CreateFromPrefab(prototype.prototype, Constants.Rendering.GetGrassMaterial());
            }
            else
            {
                var nonLodMesh = Constants.Rendering.QuadMesh;
                var nonLodMaterials = new [] { Object.Instantiate(Constants.Rendering.GetGrassMaterial()) };

                foreach (var nonLodMaterial in nonLodMaterials)
                {
                    nonLodMaterial.mainTexture = prototype.prototypeTexture;
                    nonLodMaterial.mainTexture.wrapMode = TextureWrapMode.Clamp;
                }
                
                CreateNonLodRenderer(nonLodMesh, nonLodMaterials);
            }

            ReceiveShadow = true;
            CastShadow = ShadowCastingMode.Off;
        }

        public InstancingRenderer(GameObject go)
        {
            ReceiveShadow = false;
            CastShadow = ShadowCastingMode.Off;

            CreateFromPrefab(go, null);
        }

        private void CreateFromPrefab(GameObject go, Material replaceMaterial)
        {
            var lodGroup = go.GetComponent<LODGroup>();
            if (lodGroup != null)
            {
                var lods = lodGroup.GetLODs();

                _lodRatios = new float[lods.Length];
                _lodRenderers = new LodRenderer[lods.Length];

                for (int i = 0; i < lods.Length; ++i)
                {
                    var lod = lods[i];

                    _lodRatios[i] = lod.screenRelativeTransitionHeight;
                    _lodRenderers[i] = new LodRenderer
                    {
                        Renderers = new LodActualRenderer[lod.renderers.Length]
                    };

                    var singleLod = _lodRenderers[i];

                    for (int j = 0; j < lod.renderers.Length; ++j)
                    {
                        var meshFilter = lod.renderers[j].GetComponent<MeshFilter>();
                        // plain meshfiler&meshrenderer
                        if (meshFilter != null)
                        {
                            if (replaceMaterial == null)
                                singleLod.Renderers[j] = BuildRenderer(meshFilter.sharedMesh,
                                    lod.renderers[j].GetComponent<MeshRenderer>().sharedMaterials);
                            else
                                singleLod.Renderers[j] = BuildRenderer(meshFilter.sharedMesh,
                                    GetDefaultInstanceMaterial(meshFilter.sharedMesh.subMeshCount, null,
                                        replaceMaterial));
                        }
                        else
                        {
                            // Billboard Renderer&Billboard Asset
                        }
                    }
                }

                SetBoundingVolume(lods[0]);
                _lodSize = lodGroup.size;
            }
            else
            {
                var nonLodMesh = go.GetComponent<MeshFilter>().sharedMesh;
                var nonLodMaterials = GetDefaultInstanceMaterial(nonLodMesh.subMeshCount,
                    go.GetComponent<MeshRenderer>().sharedMaterials, replaceMaterial);

                CreateNonLodRenderer(nonLodMesh, nonLodMaterials);
            }
        }

        private void CreateNonLodRenderer(Mesh nonLodMesh, Material[] nonLodMaterials)
        {
            var renderer = BuildRenderer(nonLodMesh, nonLodMaterials);
            
            _lodRenderers = new LodRenderer[1];
            _lodRenderers[0] = new LodRenderer
            {
                Renderers = new []{ renderer }
            };
            
            var bounds = renderer.Mesh.bounds;
            _sphereCenter = bounds.center;
            _sphereRadius = bounds.extents.magnitude;
            
            var size = bounds.size;
            _lodSize = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
        }

        internal LodRenderer GetLodRenderer(int lodLevel)
        {
            return _lodRenderers[lodLevel];
        }

        public int LodLevelCount { get { return _lodRenderers.GetLength(0); } }

        private LodActualRenderer BuildRenderer(Mesh mesh, Material[] materials)
        {
            if (mesh == null || materials == null)
                throw new ArgumentNullException();

            LodActualRenderer ret = new LodActualRenderer
            {
                Mesh = mesh,
                Materials = materials,
                SubMeshCount = mesh.subMeshCount,
                IndexCount = new int[mesh.subMeshCount]
            };

            for (int i = 0; i < ret.SubMeshCount; ++i)
                ret.IndexCount[i] = (int) mesh.GetIndexCount(i);

            return ret;
        }

        private void SetBoundingVolume(LOD lod)
        {
            Vector3 volumeMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 volumeMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var renderer in lod.renderers)
            {
                var bounds = renderer.bounds;
                var min = bounds.center - bounds.extents;
                var max = bounds.center + bounds.extents;

                for (int i = 0; i < 3; ++i)
                {
                    volumeMin[i] = Mathf.Min(volumeMin[i], min[i]);
                    volumeMax[i] = Mathf.Max(volumeMax[i], max[i]);
                }
            }

            _sphereCenter = new Vector3(
                (volumeMin[0] + volumeMax[0]) * 0.5f,
                (volumeMin[1] + volumeMax[1]) * 0.5f,
                (volumeMin[2] + volumeMax[2]) * 0.5f);

            _sphereRadius = Vector3.Distance(_sphereCenter, volumeMin);
        }

        private Material[] GetDefaultInstanceMaterial(int subMeshCount, Material[] originMaterials, Material replaceMaterial)
        {
            Material[] instanceMaterials = new Material[subMeshCount];

            for (int i = 0; i < subMeshCount; ++i)
            {
                instanceMaterials[i] = Object.Instantiate(replaceMaterial);
                if (originMaterials != null)
                {
                    instanceMaterials[i].mainTexture = originMaterials[i].mainTexture;
                    instanceMaterials[i].mainTextureOffset = originMaterials[i].mainTextureOffset;
                    instanceMaterials[i].mainTextureScale = originMaterials[i].mainTextureScale;
                }
            }

            return instanceMaterials;
        }
    }
}
