using System.Collections.Generic;
using Core.Utils;
using Shared.Scripts;
using UnityEngine;
using Utils.Appearance.Effects;
using Utils.Appearance.PropItem;
using Utils.Appearance.WardrobePackage;
using Utils.Appearance.Weapon.WeaponShowPackage;
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

        protected NewWeaponControllerBase WeaponControllerBaseImpl;
        protected WeaponDataAdapter WeaponDataBaseImpl;
        protected WardrobeControllerBase WardrobeControllerBaseImpl;
        protected PropControllerBase PropControllerBaseImpl;
        protected ReplaceMaterialShaderBase ReplaceMaterialShaderBaseImpl;

        private GameObject _characterP3;
        private GameObject _characterP1;
        private GameObject _rootGo;

        private CharacterView _view = CharacterView.ThirdPerson;

        private readonly List<UnityObject> _recycleRequestBatch = new List<UnityObject>();
        private readonly List<AbstractLoadRequest> _loadRequestBatch = new List<AbstractLoadRequest>();
        
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

        public virtual void SetThirdPersonCharacter(GameObject obj)
        {
            WeaponControllerBaseImpl.SetRootGo(_rootGo);
            
            WeaponControllerBaseImpl.SetThirdPersonCharacter(obj);
            WardrobeControllerBaseImpl.SetThirdPersonCharacter(obj);
            PropControllerBaseImpl.SetThirdPersonCharacter(obj);
            _characterP3 = obj;
            
            WardrobeControllerBaseImpl.SetRootGo(_rootGo);
            Logger.InfoFormat("CharacterLog-- third AppearanceBaseRootGo: {0}", _rootGo.name);
        }
        
        public void SetFirstPersonCharacter(GameObject obj)
        {
            WeaponControllerBaseImpl.SetRootGo(_rootGo);
            
            WeaponControllerBaseImpl.SetFirstPersonCharacter(obj);
            WardrobeControllerBaseImpl.SetFirstPersonCharacter(obj);
            PropControllerBaseImpl.SetFirstPersonCharacter(obj);

            _characterP1 = obj;
            AppearanceUtils.DisableRender(_characterP1);
            
            WardrobeControllerBaseImpl.SetRootGo(_rootGo);
            Logger.ErrorFormat("first ApperanceBaseRootGo: " + _rootGo);
        }
        
        public void SetRootGo(GameObject obj)
        {
            _rootGo = obj;
            Logger.InfoFormat("CharacterLog-- SetRootGo : {0}", obj.name);
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
            UnmountWeaponFromHand();
            Logger.InfoFormat("CharacterLog-- Player Dead");
        }

        public virtual void PlayerReborn()
        {
            Logger.InfoFormat("CharacterLog-- Player Reborn");
        }

        public void PlayerVisibility(bool value)
        {
            if (null != _characterP3)
                _characterP3.SetActive(value);
            if (null != _characterP1)
                _characterP1.SetActive(value);
        }

        public void SetVisibility(bool value)
        {
            if (null != _characterP3)
            {
                _characterP3.transform.localScale = value ? Vector3.one : 0.01f * Vector3.one;
            }
            if (null != _characterP1)
            {
                _characterP1.transform.localScale = value ? Vector3.one : 0.01f * Vector3.one;
            }
        }

        public virtual void Execute()
        {
            WardrobeControllerBaseImpl.Execute();
            WardrobeControllerBaseImpl.TryRewind();
            PropControllerBaseImpl.Execute();
            PropControllerBaseImpl.TryRewind();
        }

        public virtual void Update()
        {
            WeaponControllerBaseImpl.Update();
        }

        public void Init()
        {
            WeaponDataBaseImpl.Init();
            WeaponControllerBaseImpl.Init();
        }

        #region changeAvatar

        private void SetInitAvatar(List<int> avatars)
        {
            WardrobeControllerBaseImpl.DefaultModelParts = avatars;
        }

        private void SetInitWeapon()
        {
            //WeaponControllerBaseImpl.ClearInitWeaponData();
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

        public void SetForceLodLevel(int level)
        {
            WardrobeControllerBaseImpl.SetForceLodLevel(level);
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
            return null;
            //return WeaponControllerBaseImpl.GetP3CurrentAttachmentByType(type);
        }

        public int GetScopeIdInCurrentWeapon()
        {
            return -1;
            //return WeaponControllerBaseImpl.GetCurrentScopeId();
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
                WeaponDataBaseImpl.MountWeaponToPackage(pos, id);
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
                WeaponDataBaseImpl.UnmountWeaponInPackage(pos);
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
                WeaponDataBaseImpl.MountWeaponToHand(pos);
            }
            else
            {
                Logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void UnmountWeaponFromHand()
        {
            WeaponDataBaseImpl.UnmountWeaponFromHand();
        }
        
        public void JustUnMountWeaponFromHand()
        {
            WeaponDataBaseImpl.JustUnMountWeaponFromHand();
        }

        public void JustClearOverrideController()
        {
            WeaponDataBaseImpl.JustClearOverrideController();
        }

        public void UnmountWeaponFromHandAtOnce()        //仅人物死亡时使用
        {
            WeaponDataBaseImpl.UnmountWeaponFromHandAtOnce();
        }

        public void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                WeaponDataBaseImpl.MountAttachment(pos, location, id);
            }
        }

        public void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                WeaponDataBaseImpl.UnmountAttachment(pos, location);
            }
        }

        public void MountWeaponOnAlternativeLocator()
        {
            //WeaponControllerBaseImpl.MountWeaponOnAlternativeLocator();
        }

        public void RemountWeaponOnRightHand()
        {
            //WeaponControllerBaseImpl.RemountWeaponOnRightHand();
        }

        public void MountP3WeaponOnAlternativeLocator()
        {
            //WeaponControllerBaseImpl.MountP3WeaponOnAlternativeLocator();
        }

        public void RemountP3WeaponOnRightHand()
        {
            //WeaponControllerBaseImpl.RemountP3WeaponOnRightHand();
        }

        public void StartReload()
        {
            //WeaponControllerBaseImpl.StartReload();
        }

        public void DropMagazine()
        {
            //WeaponControllerBaseImpl.DropMagazine();
        }

        public void AddMagazine()
        {
            //WeaponControllerBaseImpl.AddMagazine();
        }

        public void EndReload()
        {
            //WeaponControllerBaseImpl.EndReload();
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
        
    }
}
