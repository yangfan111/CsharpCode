using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Terrain
{
    enum TreeBufferType
    {
        Transform,
        Length
    }

    public class TreeInstancingDraw : InstancingDraw
    {
        internal TreeInstancingDraw(InstancingRenderer renderer, ComputeShader visShader) : base(renderer, visShader, null)
        {
        }

        protected override void SetMaterialPropertyBlock()
        {
            base.SetMaterialPropertyBlock();
            
            Mbp.SetBuffer(Constants.TerrainVariable.TransformData, TransformData);
        }
    }
}