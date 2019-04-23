namespace Utils.Appearance
{
    public interface IBoneRigging
    {
        // left -> coefficient < 0
        void Peek(float amplitude);
        float PeekDegree { get; }
        void SightProgress(float progress);
        void FirstPersonFastMoveShift(float horizontal, float vertical, float sightHorizontal, float sightVertical);

        void Update(CodeRigBoneParam param);
    }
}
