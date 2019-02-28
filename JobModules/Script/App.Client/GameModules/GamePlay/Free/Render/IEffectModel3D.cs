using UnityEngine;

namespace Assets.Sources.Free.Render
{
    public interface IEffectModel3D
    {
        IObject3D model3D { get; }
        GameObject gameObject { get; }
        Renderer meshRender { get; set; }
        Material material { get; set; }
        float alpha { get; set; }
        string resUrl { get; set; }
        string modelName { get; set; }
        string textureName { get; set; }
        int geometryType { get; set; }
        int depthMode { get; set; }
    }
}
