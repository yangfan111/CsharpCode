using UnityEngine;
using Utils.CharacterState;
using XmlConfig;
using Utils.Appearance;
using System;
using Core.Fsm;
using System.Collections.Generic;
using Core.CharacterController;
using Core.EntityComponent;

namespace Core.Appearance
{
    public interface ICharacterAppearance : ICharacterLoadResource, IInputScheme
    {
        void SetThirdPersonCharacter(GameObject obj);
        void SetFirstPersonCharacter(GameObject obj);
        void SetRoleModelIdAndInitAvatar(int roleModelId,List<int> avatarIds);
        void SetAnimatorP1(Animator animator);
        void SetAnimatorP3(Animator animator);

        void SetFirstPerson();
        void SetThridPerson();
        bool IsFirstPerson { get; }
        GameObject CharacterP3 { get;}
        GameObject CharacterP1 { get;}

        void PlayerDead();
        void PlayerReborn();

        void UpdateAvatar();
        void ChangeAvatar(int id);
        void ClearAvatar(Wardrobe pos);
        // prop
        void AddProp(int id);
        void RemoveProp();

        // 所有武器要先挂载到1~5号位，再挂载到手上
        void MountWeaponToHand(WeaponInPackage pos);
        void UnmountWeaponFromHand();
        void UnmountWeaponFromHandAtOnce();
        void MountWeaponInPackage(WeaponInPackage pos, int id);
        void UnmountWeaponInPackage(WeaponInPackage pos);
        void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id);
        void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location);
        GameObject GetWeaponP3InHand();
        GameObject GetWeaponP1InHand();
        GameObject GetP3CurrentAttachmentByType(int type);
        // 获取当前武器上镜ID
        int GetScopeIdInCurrentWeapon();
        // 获取当前武器ID
        int GetWeaponIdInHand();
        bool IsEmptyHand();
        //将武器挂载到替代绑点上
        void MountWeaponOnAlternativeLocator();
        //武器重新绑到右手绑点上
        void RemountWeaponOnRightHand();
        // 仅三人称切换绑点
        void MountP3WeaponOnAlternativeLocator();
        void RemountP3WeaponOnRightHand();

        void StartReload();
        void DropMagazine();
        void AddMagazine();
        void EndReload();

        WardrobeControllerBase GetWardrobeController();
        WeaponControllerBase GetController<PlayerWeaponController>();

        void Execute();

        // 同步用
        void SyncLatestFrom(IGameComponent playerLatestAppearance);
        void SyncPredictedFrom(IGameComponent playerPredictedAppearance);
        void SyncLatestTo(IGameComponent playerLatestAppearance);
        void SyncPredictedTo(IGameComponent playerPredictedAppearance);
        void TryRewind();
    }
}
