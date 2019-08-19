using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class WeaponAttachment : ParamBase
    {
        private readonly AttachmentParam[] _attachments = new AttachmentParam[(int) WeaponPartLocation.EndOfTheWorld];
        private readonly WeaponInPackage _weaponType;

        public WeaponAttachment(bool isFirstPerson, WeaponInPackage weaponType)
        {
            _weaponType = weaponType;
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i] = new AttachmentParam(isFirstPerson, (WeaponPartLocation) i, _weaponType);
            }
        }
        
        public override void SetAbstractLoadRequest(Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> loadRequest)
        {
            foreach (var attachmentParam in _attachments)
            {
                attachmentParam.SetAbstractLoadRequest(loadRequest);
            }
        }

        public override void SetChangeCallBack(Action<GameObject> action)
        {
            foreach (var attachmentParam in _attachments)
            {
                attachmentParam.SetChangeCallBack(action);
            }
        }

        public override void SetAfterDelCallBack(Action<GameObject> action)
        {
            foreach (var attachmentParam in _attachments)
            {
                attachmentParam.SetAfterDelCallBack(action);
            }
        }

        public override void SetAfterAddCallBack(Action<GameObject> action)
        {
            foreach (var attachmentParam in _attachments)
            {
                attachmentParam.SetAfterAddCallBack(action);
            }
        }

        public void SetWeaponData(WeaponGameObjectData weapon)
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].SetWeapon(weapon);
            }
        }
        
        public void SetP1WeaponTopLayerShader()
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].ChangeAttachmentShader();
            }
        }
        
        public void ResetP1WeaponTopShader()
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].ResetAttachmentShader();
            }
        }
        
        public void SetRootGo(GameObject rootGo)
        {
        }
        
        public void SetFirstPerson()
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].SetFirstPerson();
            }
        }

        public void SetThirdPerson()
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].SetThirdPerson();
            }
        }

        public void RemoveAllAttachment()
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].RemoveAttachment();
            }
        }
        
        public GameObject GetAttachmentObjByType(WeaponPartLocation type)
        {
            return _attachments[(int) type].GetAttachmentObj();
        }

        public int GetAttachmentIdByType(WeaponPartLocation type)
        {
            return _attachments[(int) type].GetAttachmentId();
        }

        public void UpdateHandWeaponType(WeaponInPackage type)
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].UpdateHandWeaponType(type);
            }
        }
        
        public void UpdateAttachmentIds(int[,] attachmentIds)
        {
            for (var i = 0; i < (int) WeaponPartLocation.EndOfTheWorld; ++i)
            {
                _attachments[i].UpdateAttachmentId(attachmentIds[(int) _weaponType, i]);
            }
        }
        
        public override IEnumerable<UnityObject> GetRecycleRequests()
        {
            foreach (var attachmentParam in _attachments)
            {
                RecycleRequestBatch.AddRange(attachmentParam.GetRecycleRequests());
            }
            
            return RecycleRequestBatch;
        }
        
        public override IEnumerable<AbstractLoadRequest> GetLoadRequests()
        {
            foreach (var attachmentParam in _attachments)
            {
                LoadRequestBatch.AddRange(attachmentParam.GetLoadRequests());
            }
            
            return LoadRequestBatch;
        }

        public override void ClearRequests()
        {
            foreach (var attachmentParam in _attachments)
            {
                attachmentParam.ClearRequests();
            }

            base.ClearRequests();
        }
    }
}
