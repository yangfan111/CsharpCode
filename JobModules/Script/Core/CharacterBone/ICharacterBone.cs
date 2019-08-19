using Core.Fsm;
using System;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Bone;
using Utils.Appearance.WardrobePackage;
using Utils.Appearance.Weapon.WeaponShowPackage;
using Utils.AssetManager;
using Utils.CharacterState;

namespace Core.CharacterBone
{
    public interface ICharacterBone : IBoneRigging, IFollowRot, IWeaponRot
    {
        void CurrentP1WeaponChanged(GameObject obj);
        void CurrentP3WeaponChanged(GameObject obj);

        void Execute(Action<FsmOutput> addOutput);

        void Reborn();
        void Dead();

        void SetStableStandPelvisRotation();
        void SetStableCrouchPelvisRotation();
        void SetThirdPersonCharacter(GameObject obj);
        void SetFirstPersonCharacter(GameObject obj);
        void SetFirstPerson();
        void SetThridPerson();
        void SetCharacterRoot(GameObject characterRoot);
        void SetWardrobeController(WardrobeControllerBase value);
        void SetWeaponController(NewWeaponControllerBase value);

        void HandleAllWeapon(Action<UnityObject> act);
        void HandleAllAttachments(Action<UnityObject> act);
        void HandleAllWardrobe(Action<UnityObject> act);
        void HandleSingleWardrobe(Wardrobe type, Action<UnityObject> act);
        
        Transform FastGetBoneTransform(string boneName, CharacterView view);

        Transform GetLocation(SpecialLocation location, CharacterView view);

        Vector3 BaseLocatorDelta { get; }
        float LastHeadRotAngle { get; set; }
        bool ForbidRot { get; set; }

        bool IsIKActive { set; get; }
        void EnableIK();
        void EndIK();
    }
}
