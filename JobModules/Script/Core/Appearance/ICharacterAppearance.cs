using UnityEngine;
using XmlConfig;
using Utils.Appearance;
using System.Collections.Generic;
using Core.EntityComponent;
using Shared.Scripts;
using Utils.Appearance.WardrobePackage;
using Utils.Appearance.Weapon.WeaponShowPackage;

namespace Core.Appearance
{
    public interface ICharacterAppearance : ICharacterLoadResource, IInputScheme
    {
        void SetRootGo(GameObject obj);
        void SetThirdPersonCharacter(GameObject obj);
        void SetFirstPersonCharacter(GameObject obj);
        void SetRoleModelIdAndInitAvatar(int roleModelId, List<int> avatarIds);
        void SetAnimatorP1(Animator animator);
        void SetAnimatorP3(Animator animator);
        void SetRagDollComponent(IGameComponent component);

        void SetFirstPerson();
        void SetThridPerson();
        bool IsFirstPerson { get; }
        GameObject CharacterP3 { get;}
        GameObject CharacterP1 { get;}

        void PlayerDead();
        void PlayerReborn();
        void PlayerVisibility(bool value);

        void UpdateAvatar();
        void ChangeAvatar(int id);
        void ClearAvatar(Wardrobe pos);
        void SetForceLodLevel(int level);
        // prop
        void AddProp(int id);
        void RemoveProp();

        // 所有武器要先挂载到1~5号位，再挂载到手上
        void MountWeaponToHand(WeaponInPackage pos);
        void UnmountWeaponFromHand();
        void JustUnMountWeaponFromHand();
        void JustClearOverrideController();
        void UnmountWeaponFromHandAtOnce();
        void MountWeaponInPackage(WeaponInPackage pos, int id);
        void UnmountWeaponInPackage(WeaponInPackage pos);
        void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id);
        void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location);
        void SetP1ObjTopLayerShader();
        void ResetP1ObjShader();
        GameObject GetWeaponP3InHand();
        GameObject GetWeaponP1InHand();
        GameObject GetP3CurrentAttachmentByType(int type);
        // 获取当前武器上镜ID
        int GetScopeIdInCurrentWeapon();
        // 获取当前武器ID
        int GetWeaponIdInHand();
        bool IsPrimaryWeaponOrSideArm();
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
        NewWeaponControllerBase GetController<TPlayerWeaponController>();

        void ClearThirdPersonCharacter();
        void CheckP3HaveInit(bool value);

        void Execute();
        void Update();
        void Init();

        // 同步用
        void CreateComponentData(IGameComponent playerLatestAppearance, IGameComponent playerPredictedAppearance);
        void SyncLatestFrom(IGameComponent playerLatestAppearance);
        void SyncPredictedFrom(IGameComponent playerPredictedAppearance);
        void SyncClientFrom(IGameComponent playerClientAppearance);
        void SyncLatestTo(IGameComponent playerLatestAppearance);
        void SyncPredictedTo(IGameComponent playerPredictedAppearance);
        void SyncClientTo(IGameComponent playerClientAppearance);
        void TryRewind();
        void SetVisibility(bool value);
    }
}
