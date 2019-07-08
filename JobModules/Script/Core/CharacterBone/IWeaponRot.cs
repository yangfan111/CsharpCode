using Core.Fsm;
using System;
using Utils.Appearance.Bone;

namespace Core.CharacterBone
{
    public interface IWeaponRot
    {
        void SetWeaponPitch(Action<FsmOutput> addOutput, float pitch);
        void WeaponRotUpdate(CodeRigBoneParam param, float deltaTime);
        void WeaponRotPlayback(CodeRigBoneParam param);
    }
}
