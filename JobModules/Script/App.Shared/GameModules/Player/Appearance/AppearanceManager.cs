using System;
using System.Collections.Generic;
using Core.Appearance;
using Core.CharacterState;
using Core.Compare;
using Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using XmlConfig;
using Utils.Appearance;
using Utils.CharacterState;
using Utils.Configuration;
using Core.Fsm;
using Assets.Utils.Configuration;
using Core.CameraControl;
using Core.CharacterController;
using Sharpen;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.Appearance
{
    public class AppearanceManager : ICharacterAppearance
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(AppearanceManager));

        private WeaponController _weaponController;
        private WardrobeController _wardrobeController;
        private PropController _propController;

        private GameObject _characterP3;
        private GameObject _characterP1;

        private CharacterView _view = CharacterView.ThirdPerson;

        private List<UnityObjectWrapper<GameObject>> _recycleRequestBatch = new List<UnityObjectWrapper<GameObject>>();
        private List<LoadRequest> _loadRequestBatch = new List<LoadRequest>();

        public AppearanceManager()
        {
            _weaponController = new WeaponController();
            _wardrobeController = new WardrobeController(_weaponController.RemountWeaponInPackage);
            _propController = new PropController();
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
            _weaponController.SetThirdPersonCharacter(obj);
            _wardrobeController.SetThirdPersonCharacter(obj);
            _propController.SetThirdPersonCharacter(obj);

            _characterP3 = obj;
        }

        public void SetFirstPersonCharacter(GameObject obj)
        {
            _weaponController.SetFirstPersonCharacter(obj);
            _wardrobeController.SetFirstPersonCharacter(obj);
            _propController.SetFirstPersonCharacter(obj);

            _characterP1 = obj;
            AppearanceUtils.DisableRender(_characterP1);
        }
        
        public void SetAnimatorP1(Animator animator)
        {
            _weaponController.SetAnimatorP1(animator);
            _wardrobeController.SetAnimatorP1(animator);
        }

        public void SetAnimatorP3(Animator animator)
        {
            _weaponController.SetAnimatorP3(animator);
            _wardrobeController.SetAnimatorP3(animator);
        }

        public void SetRoleModelIdAndInitAvatar(int roleModelId, List<int> avatarIds)
        {
            var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModelId).Sex;
            List<int> initAvatars = new List<int>();
            // 根据modelId获取默认avatarIds
            var arr = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleModelId).Res;
            // 大厅带入的装扮
            if(null != arr)
            {
                for (var i = 0; i < arr.Count; ++i)
                {
                    initAvatars.Add(arr[i]);
                }
            }
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
            SetSex((Sex)sex);
            SetInitAvatar(initAvatars);
        }
		
        private void SetSex(Sex sex)
        {
            _weaponController.SetSex(sex);
            _wardrobeController.SetSex(sex);
        }

        public void SetFirstPerson()
        {
            _weaponController.SetFirstPerson();
            _wardrobeController.SetFirstPerson();
            _propController.SetFirstPerson();

            _view = CharacterView.FirstPerson;
        }

        public void SetThridPerson()
        {
            _weaponController.SetThirdPerson();
            _wardrobeController.SetThirdPerson();
            _propController.SetThirdPerson();

            _view = CharacterView.ThirdPerson;
        }

        public bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        public void PlayerDead()
        {
            ControlRagdoll(true);
            _logger.DebugFormat("Player Dead");
        }

        public void PlayerReborn()
        {
            ControlRagdoll(false);
            _logger.DebugFormat("Player Reborn");
        }

        public void Execute()
        {
            _weaponController.Execute();
            _wardrobeController.Execute();
            if (!SharedConfig.IsServer)
                _wardrobeController.TryRewind();
            _propController.Execute();
            if (!SharedConfig.IsServer)
                _propController.TryRewind();
        }

        #region changeAvatar

        private void SetInitAvatar(List<int> avatars)
        {
            _wardrobeController.InitAvatars = avatars;
        }

        public void UpdateAvatar()
        {
            if(!SharedConfig.IsServer)
                _wardrobeController.Update();
        }

        public void ChangeAvatar(int id)
        {
            _wardrobeController.Dress(id);
        }

        public void ClearAvatar(Wardrobe pos)
        {
            _wardrobeController.Undress(pos);
        }
        #endregion

        #region AddProps
        public void AddProp(int id)
        {
            _propController.AddProps(id);
        }

        public void RemoveProp()
        {
            _propController.RemoveProps();
        }

        #endregion

        public GameObject GetWeaponP3InHand()
        {
            return _weaponController.GetWeaponP3ObjInHand();
        }

        public GameObject GetWeaponP1InHand()
        {
            return _weaponController.GetWeaponP1ObjInHand();
        }

        public GameObject GetP3CurrentAttachmentByType(int type)
        {
            return _weaponController.GetP3CurrentAttachmentByType(type);
        }

        public int GetScopeIdInCurrentWeapon()
        {
            return _weaponController.GetCurrentScopeId();
        }

        public int GetWeaponIdInHand()
        {
            return _weaponController.GetWeaponIdInHand();
        }

        public bool IsEmptyHand()
        {
            return _weaponController.IsEmptyHand();
        }

        public void MountWeaponInPackage(WeaponInPackage pos, int id)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                _weaponController.MountWeaponToPackage(pos, id);
            }
            else
            {
                _logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void UnmountWeaponInPackage(WeaponInPackage pos)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                _weaponController.UnmountWeaponInPackage(pos);
            }
            else
            {
                _logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void MountWeaponToHand(WeaponInPackage pos)
        {
            if ((int) pos < (int) WeaponInPackage.EndOfTheWorld)
            {
                _weaponController.MountWeaponToHand(pos);
            }
            else
            {
                _logger.ErrorFormat("error slot :  slot id {0}", pos);
            }
        }

        public void UnmountWeaponFromHand()
        {
            _weaponController.UnmountWeaponFromHand();
        }

        public void UnmountWeaponFromHandAtOnce()        //仅人物死亡时使用
        {
            _weaponController.UnmountWeaponFromHandAtOnce();
        }

        public void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                _weaponController.MountAttachment(pos, location, id);
            }
        }

        public void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            if ((int) pos <= (int) WeaponInPackage.EndOfTheWorld &&
                (int) location <= (int) WeaponPartLocation.EndOfTheWorld)
            {
                _weaponController.UnmountAttachment(pos, location);
            }
        }

        public void MountWeaponOnAlternativeLocator()
        {
            _weaponController.MountWeaponOnAlternativeLocator();
        }

        public void RemountWeaponOnRightHand()
        {
            _weaponController.RemountWeaponOnRightHand();
        }

        public void MountP3WeaponOnAlternativeLocator()
        {
            _weaponController.MountP3WeaponOnAlternativeLocator();
        }

        public void RemountP3WeaponOnRightHand()
        {
            _weaponController.RemountP3WeaponOnRightHand();
        }

        public void StartReload()
        {
            _weaponController.StartReload();
        }

        public void DropMagazine()
        {
            _weaponController.DropMagazine();
        }

        public void AddMagazine()
        {
            _weaponController.AddMagazine();
        }

        public void EndReload()
        {
            _weaponController.EndReload();
        }

        #region Sync

        public void SyncFrom(ILatestAppearanceState state)
        {
            _weaponController.SyncFromLatestWeaponState(state);
            _wardrobeController.SyncFromLatestWardrobeState(state);
            _propController.SyncFromLatestPropState(state);
        }

        public void SyncTo(ILatestAppearanceState state)
        {
            _weaponController.SyncToLatestWeaponState(state);
            _wardrobeController.SyncToLatestWardrobeState(state);
            _propController.SyncToLatestPropState(state);
        }

        public void SyncFrom(IPredictedPlaybackAppearanceState state)
        {
            _weaponController.SyncFromPredictedWeaponState(state);
        }

        public void SyncTo(IPredictedPlaybackAppearanceState state)
        {
            _weaponController.SyncToPredictedWeaponState(state);
        }

        public void TryRewind()
        {
            _weaponController.TryRewind();
            if(!SharedConfig.IsServer) //服务器不需穿戴装扮
            {
                _wardrobeController.TryRewind();
                _propController.TryRewind();
            }
        }

        #endregion

        #endregion

        #region ICharacterLoadResource

        public List<LoadRequest> GetLoadRequests()
        {
            _loadRequestBatch.AddRange(_weaponController.GetLoadRequests());
            _loadRequestBatch.AddRange(_wardrobeController.GetLoadRequests());
            _loadRequestBatch.AddRange(_propController.GetLoadRequests());
            return _loadRequestBatch;
        }

        public List<UnityObjectWrapper<GameObject>> GetRecycleRequests()
        {
            var weaponRecycle = _weaponController.GetRecycleRequests();
            foreach (var v in weaponRecycle)
            {
                if (v == null)
                {
                    _logger.WarnFormat("weapon recycle null");
                }
            }
            _recycleRequestBatch.AddRange(weaponRecycle);
            var wardrobeRecycle = _wardrobeController.GetRecycleRequests();
            foreach (var v in wardrobeRecycle)
            {
                if (v == null)
                {
                    _logger.WarnFormat("wardrobe recycle null");
                }
            }
            _recycleRequestBatch.AddRange(wardrobeRecycle);
            var propRecycle = _propController.GetRecycleRequests();
            _recycleRequestBatch.AddRange(propRecycle);
            foreach (var v in propRecycle)
            {
                if (v == null)
                {
                    _logger.WarnFormat("prop recycle null");
                }
            }
            return _recycleRequestBatch;
        }

        public void ClearRequests()
        {
            _loadRequestBatch.Clear();
            _recycleRequestBatch.Clear();
            _weaponController.ClearRequests();
            _wardrobeController.ClearRequests();
            _propController.ClearRequests();
        }

        #endregion

        
        private static List<Rigidbody> RagdollList = new List<Rigidbody>(128);
        private static List<BoxCollider> RagdollBoxColliderList = new List<BoxCollider>(256);
        private static List<CapsuleCollider> RagdollCapsuleColliderList = new List<CapsuleCollider>(256);
        private static List<SphereCollider> RagdollSphereColliderList = new List<SphereCollider>(256);
        
        private void ControlRagdoll(bool enable)
        {
            RagdollList.Clear();
            _characterP3.GetComponentsInChildren<Rigidbody>(RagdollList);
            foreach (var v in RagdollList)
            {
                v.detectCollisions = enable;
                v.isKinematic = !enable;
            }
            RagdollBoxColliderList.Clear();
            _characterP3.GetComponentsInChildren<BoxCollider>(RagdollBoxColliderList);
            foreach (var v in RagdollBoxColliderList)
            {
                v.enabled = enable;
            }
            RagdollCapsuleColliderList.Clear();
            _characterP3.GetComponentsInChildren<CapsuleCollider>(RagdollCapsuleColliderList);
            foreach (var v in RagdollCapsuleColliderList)
            {
                v.enabled = enable;
            }
            RagdollSphereColliderList.Clear();
            _characterP3.GetComponentsInChildren<SphereCollider>(RagdollSphereColliderList);
            foreach (var v in RagdollSphereColliderList)
            {
                v.enabled = enable;
            }
        }

        private List<KeyValuePair<int, string>> _needUpdateActionField = new List<KeyValuePair<int, string>>();
        public void SetInputSchemeActionField(int schemeIndex, string actionName)
        {
            _needUpdateActionField.Add(new KeyValuePair<int, string>(schemeIndex, actionName));
        }

        public List<KeyValuePair<int, string>> GetInputSchemeActionFieldToUpdate()
        {
            return _needUpdateActionField;
        }

        public WardrobeController GetWardrobeController()
        {
            return _wardrobeController;
        }

        public WeaponController GetController<PlayerWeaponController>()
        {
            return _weaponController;
        }
    }
}
