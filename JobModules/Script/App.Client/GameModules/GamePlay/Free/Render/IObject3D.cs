using UnityEngine;

namespace Assets.Sources.Free.Render
{
    public interface IObject3D
    {
        float x { get; set; }
        float y { get; set; }
        float z { get; set; }

        float scaleX { get; set; }
        float scaleY { get; set; }
        float scaleZ { get; set; }

        float rotationX { get; set; }
        float rotationY { get; set; }
        float rotationZ { get; set; }

        float alpha { get; set; }
        bool visible { get; set; }

        GameObject GameObject { get; }

        void AddChild(IObject3D child);
    }
}
