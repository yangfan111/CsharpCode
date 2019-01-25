using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Appearance;

namespace Core.CharacterBone
{
    public interface IWeaponRot
    {
        void SetWeaponPitch(Action<FsmOutput> addOutput, float pitch);
        void WeaponRotUpdate(CodeRigBoneParam param);
        void WeaponRotPlayback(CodeRigBoneParam param);
    }
}
