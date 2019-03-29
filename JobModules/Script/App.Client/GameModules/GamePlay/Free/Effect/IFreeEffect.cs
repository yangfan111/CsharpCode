using Assets.Sources.Free.Render;

namespace Assets.Sources.Free.Effect
{
    public interface IFreeEffect
    {
        EffectFixInfo FixInfo { get; }

        void SetPos(float x, float y, float z, float scaleX, float scaleY, float scaleZ, float rotationX, float rotationY, float rotationZ);

        int Type { get; }

        void SetValue(params object[] vs);

        void Initial(params object[] ini);

        void Frame(int frameTime, ISceneManage scene, FreeRenderObject freeRender);

        IEffectModel3D EffectModel3D { get; }

        IObject3D OriObject3D { get; }

        IFreeEffect Clone();

        void Recycle();

        void Destroy();
    }
}
