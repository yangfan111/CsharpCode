using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance.PropItem;
using Utils.Appearance.WardrobePackage;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Appearance.ManagerPackage
{
    public class AppearanceManagerBase
    {
        protected readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AppearanceManagerBase));

        protected WeaponControllerBase WeaponControllerBaseImpl;
        protected WardrobeControllerBase WardrobeControllerBaseImpl;
        protected PropControllerBase PropControllerBaseImpl;
        protected ReplaceMaterialShaderBase ReplaceMaterialShaderBaseImpl;

        private GameObject _characterP3;
        private GameObject _characterP1;

        private CharacterView _view = CharacterView.ThirdPerson;

        private readonly List<UnityObject> _recycleRequestBatch = new List<UnityObject>();
        private readonly List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();

        private HitBoxCache _hitboxHandler;
        
        protected AppearanceManagerBase()
        {
        }
        
        #region ICharacterAppearance

        public GameObject CharacterP3
        {
            get { return _characterP3; }
        }

        public GameObject CharacterP1
        {
            get { return _characterP1; }
        }

        public void SetThirdPersonCharacter(GameObject obj)
        {
            WeaponControllerBaseImpl.SetThirdPersonCharacter(obj);
            WardrobeControllerBaseImpl.SetThirdPersonCharacter(obj);
            PropControllerBaseImpl.SetThirdPersonCharacter(obj);
            _characterP3 = obj;
            _hitboxHandler = obj.GetComponent<HitBoxCache>();
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            WeaponControllerBaseImpl.SetFirstPersonCharacter(obj);
            WardrobeControllerBaseImpl.SetFirstPersonCharacter(obj);
            PropControllerBaseImpl.SetFirstPersonCharacter(obj);

            _characterP1 = obj;
            AppearanceUtils.DisableRender(_characterP1);
        }
        
        public void SetAnimatorP1(Animator animator)
        {
            WeaponControllerBaseImpl.SetAnimatorP1(animator);
            WardrobeControllerBaseImpl.SetAnimatorP1(animator);
        }

        public void SetAnimatorP3(Animator animator)
        {
            WeaponControllerBaseImpl.SetAnimatorP3(animator);
            WardrobeControllerBaseImpl.SetAnimatorP3(animator);
        }

        public void SetRoleModelIdAndInitAvatar(int roleModelId, List<int> avatarIds)
        {
            var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModelId).Sex;
            var unique = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModelId).Unique;
            List<int> initAvatars = new List<int>();
            // 根据modelId获取默认avatarIds
            var arr = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModelId).Res;
            
            if(null != arr)
            {
                for (var i = 0; i < arr.Count; ++i)
                {
                    initAvatars.Add(arr[i]);
                }
            }
            // 大厅带入的装扮
            if (null != avatarIds)
            {
                for (var i = 0; i < avatarIds.Count; ++i)
                {
                    var avatarId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(avatarIds[i], (Sex)sex);
                    var pos = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarType(avatarId);
                    if(SingletonManager.Get<AvatarAssetConfigManager>().IsModelPart(pos))
                        initAvatars.Add(avatarId);
                }
            }
            SetRoleSexAndAvatars(sex, unique, initAvatars);
        }

        private void SetRoleSexAndAvatars(int sex, bool unique, List<int> initAvatars)
        {
            SetSex((Sex) sex);
            SetUnique(unique);
            SetInitAvatar(initAvatars);
            SetInitWeapon();
        }
		
        private void SetSex(Sex sex)
        {
            WeaponControllerBaseImpl.SetSex(sex);
            WardrobeControllerBaseImpl.SetSex(sex);
        }

        private void SetUnique(bool unique)
        {
            WeaponControllerBaseImpl.SetUnique(unique);
        }

        public void SetFirstPerson()
        {
            WeaponControllerBaseImpl.SetFirstPerson();
            WardrobeControllerBaseImpl.SetFirstPerson();
            PropControllerBaseImpl.SetFirstPerson();

            _view = CharacterView.FirstPerson;
        }

        public void SetThridPerson()
        {
            WeaponControllerBaseImpl.SetThirdPerson();
            WardrobeControllerBaseImpl.SetThirdPerson();
            PropControllerBaseImpl.SetThirdPerson();

            _view = CharacterView.ThirdPerson;
        }

        public bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        public virtual void PlayerDead()
        {
            ControlRagDoll(true);
            UnmountWeaponFromHand();
            SetThridPerson();
            Logger.DebugFormat("Player Dead");
        }

        public void PlayerReborn()
        {
            ControlRagDoll(false);
            Logger.DebugFormat("Player Reborn");
        }

        public virtual void Execute()
        {
            WeaponControllerBaseImpl.Execute();
            WardrobeControllerBaseImpl.Execute();
            WardrobeControllerBaseImpl.TryRewind();
            PropControllerBaseImpl.Execute();
            PropControllerBaseImpl.TryRewind();
        }

        #region changeAvatar

        private void SetInitAvatar(List<int> avatars)
        {
            WardrobeControllerBaseImpl.DefaultModelParts = avatars;
        }

        private void SetInitWeapon()
        {
            WeaponControllerBaseImpl.ClearInitWeaponData();
        }

        public virtual void UpdateAvatar()
        {
            WardrobeControllerBaseImpl.Update();
        }

        public void ChangeAvatar(int id)
        {
            WardrobeControllerBaseImpl.Dress(id);
        }

        public void ClearAvatar(Wardrobe pos)
        {
            WardrobeControllerBaseImpl.Undress(pos);
        }
        #endregion

        #region AddProps
        public void AddProp(int id)
        {
            PropControllerBaseImpl.AddProps(id);
        }

        public void RemoveProp()
        {
            PropControllerBaseImpl.RemoveProps();
        }

        #endregion

        public void SetP1ObjTopLayerShader()
        {
            WardrobeControllerBaseImpl.SetP1WardrobeTopLayerShader();
            WeaponControllerBaseImpl.SetP1WeaponTopLayerShader();
        }

        public void ResetP1ObjShader()
        {
            WardrobeControllerBaseImpl.ResetP1WardrobeShader();
            WeaponControllerBaseImpl.ResetP1WeaponShader();
        }
        
        public GameObject GetWeaponP3InHand()
        {
            return WeaponControllerBaseImpl.GetWeaponP3ObjInHand();
        }

        public GameObject GetWeaponP1InHand()
        {
            return WeaponControllerBaseImpl.GetWeaponP1ObjInHand();
        }

        public GameObject GetP3CurrentAttachmentByType(int type)
        {
            return WeaponControllerBaseImpl.GetP3CurrentAttachmentByType(type);
        }

        public int GetScopeIdInCurrentWeapon()
        {
            return WeaponControllerBaseImpl.GetCurrentScopeId();
        }

        public int GetWeaponIdInHand()
        {
            return WeaponControllerBaseImpl.GetWeaponIdInHand();
        }

        public bool IsPrimaryWeaponOrSideArm()
        {
            return WeaponControllerBaseImpl.IsPrimaryWeaponOrSideArm();
        }

        public bool IsEmptyHand()
        {
            return WeaponControllerBaseImpl.IsEmptyHand();
        }

        public void MountWeaponInPackage(WeaponInPackage pos, int id)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.MountWeaponToPackage(pos, id);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void UnmountWeaponInPackage(WeaponInPackage pos)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.UnmountWeaponInPackage(pos);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void MountWeaponToHand(WeaponInPackage pos)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.MountWeaponToHand(pos);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void UnmountWeaponFromHand()
        {
            WeaponControllerBaseImpl.UnmountWeaponFromHand();
        }

        public void UnmountWeaponFromHandAtOnce()        //仅人物死亡时使用
        {
            WeaponControllerBaseImpl.UnmountWeaponFromHandAtOnce();
        }

        public void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.MountAttachment(pos, location, id);
            }
        }

        public void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                WeaponControllerBaseImpl.UnmountAttachment(pos, location);
            }
        }

        public void MountWeaponOnAlternativeLocator()
        {
            WeaponControllerBaseImpl.MountWeaponOnAlternativeLocator();
        }

        public void RemountWeaponOnRightHand()
        {
            WeaponControllerBaseImpl.RemountWeaponOnRightHand();
        }

        public void MountP3WeaponOnAlternativeLocator()
        {
            WeaponControllerBaseImpl.MountP3WeaponOnAlternativeLocator();
        }

        public void RemountP3WeaponOnRightHand()
        {
            WeaponControllerBaseImpl.RemountP3WeaponOnRightHand();
        }

        public void StartReload()
        {
            WeaponControllerBaseImpl.StartReload();
        }

        public void DropMagazine()
        {
            WeaponControllerBaseImpl.DropMagazine();
        }

        public void AddMagazine()
        {
            WeaponControllerBaseImpl.AddMagazine();
        }

        public void EndReload()
        {
            WeaponControllerBaseImpl.EndReload();
        }

        #endregion
        
        #region ICharacterLoadResource

        public List<AbstractLoadRequest> GetLoadRequests()
        {
            _loadRequestBatch.AddRange(WeaponControllerBaseImpl.GetLoadRequests());
            _loadRequestBatch.AddRange(WardrobeControllerBaseImpl.GetLoadRequests());
            _loadRequestBatch.AddRange(PropControllerBaseImpl.GetLoadRequests());
            _loadRequestBatch.AddRange(ReplaceMaterialShaderBaseImpl.GetLoadRequests());
            return _loadRequestBatch;
        }

        public List<UnityObject> GetRecycleRequests()
        {
            var weaponRecycle = WeaponControllerBaseImpl.GetRecycleRequests();
            foreach (var v in weaponRecycle)
            {
                if (v == null)
                {
                    Logger.WarnFormat("weapon recycle null");
                }
            }
            _recycleRequestBatch.AddRange(weaponRecycle);
            var wardrobeRecycle = WardrobeControllerBaseImpl.GetRecycleRequests();
            foreach (var v in wardrobeRecycle)
            {
                if (v == null)
                {
                    Logger.WarnFormat("wardrobe recycle null");
                }
            }
            _recycleRequestBatch.AddRange(wardrobeRecycle);
            var propRecycle = PropControllerBaseImpl.GetRecycleRequests();
            _recycleRequestBatch.AddRange(propRecycle);
            foreach (var v in propRecycle)
            {
                if (v == null)
                {
                    Logger.WarnFormat("prop recycle null");
                }
            }
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();
            WeaponControllerBaseImpl.ClearRequests();
            WardrobeControllerBaseImpl.ClearRequests();
            PropControllerBaseImpl.ClearRequests();
            ReplaceMaterialShaderBaseImpl.ClearRequests();
        }

        #endregion
        
        private static readonly List<Rigidbody> RagDollList = new List<Rigidbody>(128);
        private static readonly List<Collider> RagDollColliderList = new List<Collider>(1024);

        private void ControlRagDoll(bool enable)
        {
            RagDollList.Clear();
            
            _characterP3.GetComponentsInChildren(RagDollList);
            foreach (var v in RagDollList)
            {
                v.detectCollisions = enable;
                v.isKinematic = !enable;
            }
            
            RagDollColliderList.Clear();
            _characterP3.GetComponentsInChildren(RagDollColliderList);
            var hitboxList = _hitboxHandler.GetHitBox().Values;
            foreach (var v in RagDollColliderList)
            {
                v.enabled = hitboxList.Contains(v) ? !enable : enable;
            }
            
            if (!enable)
                ResetRagDollRootBoneTransform();
        }
        
        private void ResetRagDollRootBoneTransform()
        {
            if (null == _characterP3) return;
            var rootBone = BoneMount.FindChildBoneFromCache(_characterP3, BoneName.CharacterBipPelvisBoneName);
            if(null == rootBone) return;
            rootBone.localPosition = Vector3.zero;
            rootBone.localRotation = Quaternion.identity;
        }
    }
}
