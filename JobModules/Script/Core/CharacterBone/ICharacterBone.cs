using Core.Appearance;
using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Appearance;
using Utils.CharacterState;

namespace Core.CharacterBone
{
    public interface ICharacterBone : IBoneRigging, IFollowRot, IWeaponRot
    {
        void CurrentWeaponChanged(GameObject objP1, GameObject objP3);

        void Execute(Action<FsmOutput> addOutput);

        void Reborn();
        void Dead();

        void SetStablePelvisRotation();
        void SetThirdPersonCharacter(GameObject obj);
        void SetFirstPersonCharacter(GameObject obj);
        void SetFirstPerson();
        void SetThridPerson();
        void SetCharacterRoot(GameObject characterRoot);
        void SetWardrobeController(WardrobeControllerBase value);
        void SetWeaponController(WeaponControllerBase value);

        Transform FastGetBoneTransform(string boneName, CharacterView view);

        Transform GetLocation(SpecialLocation location, CharacterView view);

        int LastHeadRotSlerpTime { get; set; }  //+-180度之间转头
        float LastHeadRotAngle { get; set; }
        bool IsHeadRotCW { get; set; }  //顺时针
        bool ForbidRot { get; set; }

        bool IsIKActive { set; get; }
        void EnableIK();
        void EndIK();
    }
}
