using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class CharacterWeapon : ParamBase
    {
        private readonly WeaponParam[] _weapons = new WeaponParam[(int) WeaponInPackage.EndOfTheWorld];
        
        private GameObject _rootGo;
        private GameObject _taaObj;

        private WeaponInPackage _handWeaponType;
        
        public CharacterWeapon(bool isFirstPerson)
        {
            for (var i = 0; i < _weapons.Length; ++i)
            {
                _weapons[i] = new WeaponParam(isFirstPerson, (WeaponInPackage) i);
            }
        }

        public override void SetAbstractLoadRequest(Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> loadRequest)
        {
            foreach (var weaponParam in _weapons)
            {
                weaponParam.SetAbstractLoadRequest(loadRequest);
            }
        }

        public override void SetChangeCallBack(Action<GameObject> action)
        {
            foreach (var weaponParam in _weapons)
            {
                weaponParam.SetChangeCallBack(action);
            }
        }

        public override void SetAfterDelCallBack(Action<GameObject> action)
        {
            foreach (var weaponParam in _weapons)
            {
                weaponParam.SetAfterDelCallBack(action);
            }
        }

        public override void SetAfterAddCallBack(Action<GameObject> action)
        {
            foreach (var weaponParam in _weapons)
            {
                weaponParam.SetAfterAddCallBack(action);
            }
        }

        public void SetCharacter(GameObject character)
        {
            if(null == Camera.main) return;
            var transforms = Camera.main.GetComponentsInChildren<Transform>();
            foreach (var transform in transforms)
            {
                if (!transform.name.Equals("TAA_Helper")) continue;
                _taaObj = transform.gameObject;
                break;
            }
            
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetCharacter(character);
            }
        }

        public void SetRootGo(GameObject rootGo)
        {
            _rootGo = rootGo;
        }
        
        public void SetSex(Sex sex)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetSex(sex);
            }
        }

        public void SetUnique(bool unique)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetUnique(unique);
            }
        }
        
        public void SetAnimator(Animator animator)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetAnimator(animator);
            }
        }
        
        public void SetFirstPerson()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetFirstPerson();
            }
        }

        public void SetThirdPerson()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].SetThirdPerson();
            }
        }
        
        public void SetP1WeaponTopLayerShader()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].ChangeWeaponShader();
            }
        }
        
        public void ResetP1WeaponTopShader()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].ResetWeaponShader();
            }
        }
        
        public GameObject GetHandWeaponObj()
        {
            return _weapons[(int) _handWeaponType].GetWeaponObj();
        }

        public int GetHandWeaponId()
        {
            return _weapons[(int) _handWeaponType].GetWeaponId();
        }

        public int GetWeaponIdByType(WeaponInPackage type)
        {
            return _weapons[(int) type].GetWeaponId();
        }

        public WeaponInPackage GetHandWeaponType()
        {
            return _handWeaponType;
        }

        public int GetAttachmentId(WeaponPartLocation attachmentType, WeaponInPackage weaponType)
        {
            return _weapons[(int) weaponType].GetAttachmentId(attachmentType);
        }

        public void Update(WeaponDataController data)
        {
            UpdateHandWeaponType(data.GetHandWeaponType());
            UpdateOverrideControllerType(data.GetChangeOverrideControllerType());
            UpdateWeaponIds(data.GetWeaponIds());
            UpdateAttachmentIds(data.GetAttachmentIds());
            UpdateWeaponInHand();
        }

        private void UpdateHandWeaponType(WeaponInPackage type)
        {
            _handWeaponType = type;
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].UpdateHandWeaponType(_handWeaponType);
            }
        }
        
        private void UpdateWeaponIds(int[] weaponIds)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].UpdateWeaponId(weaponIds[i]);
            }
        }

        private void UpdateAttachmentIds(int[,] ids)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].UpdateAttachmentIds(ids);
            }
        }

        private void UpdateWeaponInHand()
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].UpdateWeaponInHand(_handWeaponType);
            }
        }

        private void UpdateOverrideControllerType(OverrideControllerState type)
        {
            for (var i = 0; i < (int) WeaponInPackage.EndOfTheWorld; ++i)
            {
                _weapons[i].UpdateOverrideControllerType(type);
            }
        }
        
        public override IEnumerable<UnityObject> GetRecycleRequests()
        {
            foreach (var weaponParam in _weapons)
            {
                RecycleRequestBatch.AddRange(weaponParam.GetRecycleRequests());
            }
            
            return RecycleRequestBatch;
        }
        
        public override IEnumerable<AbstractLoadRequest> GetLoadRequests()
        {
            foreach (var weaponParam in _weapons)
            {
                LoadRequestBatch.AddRange(weaponParam.GetLoadRequests());
            }
            
            return LoadRequestBatch;
        }

        public override void ClearRequests()
        {
            foreach (var weaponParam in _weapons)
            {
                weaponParam.ClearRequests();
            }

            base.ClearRequests();
        }
    }
}
