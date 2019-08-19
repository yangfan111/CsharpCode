using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public abstract class NewWeaponControllerBase : ParamBase
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NewWeaponControllerBase));
        
        public bool P3HaveInit;

        protected readonly ChangeWeaponAnimationController WeaponAnimation = new ChangeWeaponAnimationController();
        protected Action CacheChangeAction;

        private readonly WeaponDataController _weaponDataController = new WeaponDataController();
        private readonly CharacterWeapon _characterWeaponP1;
        private readonly CharacterWeapon _characterWeaponP3;
        
        private const int ValidP1Index = 0;
        private const int ValidP3Index = 1;
        
        private int _p1Index = UniversalConsts.InvalidIntId;
        private int _p3Index = UniversalConsts.InvalidIntId;
        
        protected NewWeaponControllerBase()
        {
            _characterWeaponP1 = new CharacterWeapon(true);
            _characterWeaponP1.SetAbstractLoadRequest(CreateLoadRequest);
            
            _characterWeaponP3 = new CharacterWeapon(false);
            _characterWeaponP3.SetAbstractLoadRequest(CreateLoadRequest);

            Init();
        }
        
        public void SetWeaponChangedCallBack(Action<GameObject> callBackP1, Action<GameObject> callBackP3)
        {
            _characterWeaponP1.SetChangeCallBack(callBackP1);
            _characterWeaponP3.SetChangeCallBack(callBackP3);
        }

        //枪械和配件卸载后，在缓存过他们的脚本中注销
        public void SetWeaponOrAttachmentDeleteCallBack(Action<GameObject> deleteAction)
        {
           _characterWeaponP1.SetAfterDelCallBack(deleteAction);
           _characterWeaponP3.SetAfterDelCallBack(deleteAction);
        }

        public void SetWeaponOrAttachementAddCallBack(Action<GameObject> addAction)
        {
            _characterWeaponP1.SetAfterAddCallBack(addAction);
            _characterWeaponP3.SetAfterAddCallBack(addAction);
        }
        
        
        public void SetRootGo(GameObject obj)
        {
            _characterWeaponP1.SetRootGo(obj);
            _characterWeaponP3.SetRootGo(obj);
        }
        
        public void SetAnimatorP1(Animator animator)
        {
            _characterWeaponP1.SetAnimator(animator);
        }

        public void SetAnimatorP3(Animator animator)
        {
            _characterWeaponP3.SetAnimator(animator);
        }
        
        public void SetSex(Sex sex)
        {
            _characterWeaponP3.SetSex(sex);
            _characterWeaponP1.SetSex(sex);
        }

        public void SetUnique(bool unique)
        {
            _characterWeaponP3.SetUnique(unique);
            _characterWeaponP1.SetUnique(unique);
        }
        
        public void SetFirstPerson()
        {
            if (ThirdPersonIncluded) _characterWeaponP3.SetFirstPerson();
            if (FirstPersonIncluded) _characterWeaponP1.SetFirstPerson();
        }

        public void SetThirdPerson()
        {
            if (ThirdPersonIncluded) _characterWeaponP3.SetThirdPerson();
            if (FirstPersonIncluded) _characterWeaponP1.SetThirdPerson();
        }
        
        public void SetThirdPersonCharacter(GameObject obj)
        {
            _characterWeaponP3.SetCharacter(obj);
            _p3Index = ValidP3Index;
            P3HaveInit = true;
            if (null != CacheChangeAction)
                CacheChangeAction.Invoke();
        }
        
        public void SetFirstPersonCharacter(GameObject obj)
        {
            _characterWeaponP1.SetCharacter(obj);
            _p1Index = ValidP1Index;
            if (null != CacheChangeAction)
                CacheChangeAction.Invoke();
        }
        
        public GameObject GetWeaponP3ObjInHand()
        {
            return !ThirdPersonIncluded ? null : _characterWeaponP3.GetHandWeaponObj();
        }

        public GameObject GetWeaponP1ObjInHand()
        {
            return !FirstPersonIncluded ? null : _characterWeaponP1.GetHandWeaponObj();
        }

        public int GetWeaponIdInHand()
        {
            return !ThirdPersonIncluded ? -1 : _characterWeaponP3.GetHandWeaponId();
        }
        
        public bool IsPrimaryWeaponOrSideArm()
        {
            if (!ThirdPersonIncluded) return false;
            var type = _characterWeaponP3.GetHandWeaponType();
            return WeaponInPackage.PrimaryWeaponOne == type ||
                   WeaponInPackage.PrimaryWeaponTwo == type ||
                   WeaponInPackage.SideArm == type;
        }

        public bool IsEmptyHand()
        {
            return !ThirdPersonIncluded || WeaponInPackage.EmptyHanded == _characterWeaponP3.GetHandWeaponType();
        }
        
        public int GetCurrentLowRailId()
        {
            return !ThirdPersonIncluded
                ? UniversalConsts.InvalidIntId
                : _characterWeaponP3.GetAttachmentId(WeaponPartLocation.LowRail,
                    _characterWeaponP3.GetHandWeaponType());
        }
        
        public int GetCurrentScopeId()
        {
            return !ThirdPersonIncluded
                ? UniversalConsts.InvalidIntId
                : _characterWeaponP3.GetAttachmentId(WeaponPartLocation.Scope,
                    _characterWeaponP3.GetHandWeaponType());
        }
        
        public void ClearThirdPersonCharacter()
        {
            GetRecycleRequests();

            _p3Index = -1;
            //_characterP3 = null;
        }
        
        public void RemountWeaponDueToBag()
        {
        }
        
        public void SetP1WeaponTopLayerShader()
        {
            if (FirstPersonIncluded) _characterWeaponP1.SetP1WeaponTopLayerShader();
            if(ThirdPersonIncluded) _characterWeaponP3.SetP1WeaponTopLayerShader();
        }

        public void ResetP1WeaponShader()
        {
            if (FirstPersonIncluded) _characterWeaponP1.ResetP1WeaponTopShader();
            if(ThirdPersonIncluded) _characterWeaponP3.ResetP1WeaponTopShader();
        }

        #region sync

        public void SetWeaponIdByType(WeaponInPackage type, int weaponId)
        {
            _weaponDataController.SetWeaponIdByType(type, weaponId);
        }

        protected int GetWeaponIdByType(WeaponInPackage type)
        {
            return !ThirdPersonIncluded ? -1 : _characterWeaponP3.GetWeaponIdByType(type);
        }
        
        public void SetAttachmentIdByType(WeaponInPackage weaponType, WeaponPartLocation attachmentType, int attachmentId)
        {
            _weaponDataController.SetAttachmentIdByType(weaponType, attachmentType, attachmentId);
        }

        public void SetHandWeaponType(WeaponInPackage type)
        {
            _weaponDataController.SetHandWeaponType(type);
        }

        protected WeaponInPackage GetHandWeaponType()
        {
            return !ThirdPersonIncluded ? WeaponInPackage.EmptyHanded : _characterWeaponP3.GetHandWeaponType();
        }
        
        protected void SetChangeOverrideControllerType(OverrideControllerState type)
        {
            _weaponDataController.SetChangeOverrideControllerType(type);
        }

        #endregion

        public void Update()
        {
            if(FirstPersonIncluded) _characterWeaponP1.Update(_weaponDataController);
            if(ThirdPersonIncluded) _characterWeaponP3.Update(_weaponDataController);
        }

        public void Init()
        {
            _weaponDataController.Init();
        }
        
        public void HandleAllWeapon(Action<UnityObject> act)
        {
//            foreach (var data in _weapons)
//            {
//                var unityObj = data.GetRecycleUnityObject();
//                if (unityObj != null)
//                    act(unityObj);
//            }
        }

        public void HandleAllAttachments(Action<UnityObject> act)
        {
//            foreach (var data in _attachments)
//            {
//                var unityObj = data.GetRecycleUnityObject();
//                if (unityObj != null)
//                    act(unityObj);
//            }
        }
        
        #region ICharacterLoadResource

        public override IEnumerable<AbstractLoadRequest> GetLoadRequests()
        {
            if(ThirdPersonIncluded)
                LoadRequestBatch.AddRange(_characterWeaponP3.GetLoadRequests());
            if(FirstPersonIncluded)
                LoadRequestBatch.AddRange(_characterWeaponP1.GetLoadRequests());
            
            return LoadRequestBatch;
        }

        public override IEnumerable<UnityObject> GetRecycleRequests()
        {
            if(ThirdPersonIncluded)
                RecycleRequestBatch.AddRange(_characterWeaponP3.GetRecycleRequests());
            if(FirstPersonIncluded)
                RecycleRequestBatch.AddRange(_characterWeaponP1.GetRecycleRequests());
            
            return RecycleRequestBatch;
        }

        public override void ClearRequests()
        {
            if(ThirdPersonIncluded)
                _characterWeaponP3.ClearRequests();
            if(FirstPersonIncluded)
                _characterWeaponP1.ClearRequests();
            
            base.ClearRequests();
        }

        #endregion

        #region HelperFunc

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHandler);

        private bool FirstPersonIncluded
        {
            get { return _p1Index == ValidP1Index; }
        }

        private bool ThirdPersonIncluded
        {
            get { return _p3Index == ValidP3Index; }
        } 

        #endregion
    }
}
