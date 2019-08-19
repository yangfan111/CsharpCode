using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class WeaponParam : WeaponParamData
    {
        public WeaponParam(bool isFirstPersonClass, WeaponInPackage type) : base(isFirstPersonClass, type)
        {
            WeaponLoadAssetHandler = new WeaponLoadAssetHandler(WeaponAssetLoadSuccess);
        }

        public void UpdateWeaponId(int destId)
        {
            DestWeaponId = destId;
            if(DestWeaponId == WeaponId) return;

            if (DestWeaponId <= 0)
            {
                WeaponId = DestWeaponId;
                RemoveWeapon();
            }
            else
                LoadResource();
        }

        public void UpdateAttachmentIds(int[,] attachmentIds)
        {
            Attachment.UpdateAttachmentIds(attachmentIds);
        }

        public int GetAttachmentId(WeaponPartLocation type)
        {
            return Attachment.GetAttachmentIdByType(type);
        }

        private void LoadResource()
        {
            var weaponAvatarManager = SingletonManager.Get<WeaponAvatarConfigManager>();
            var assetInfo = IsFirstPersonClass
                ? weaponAvatarManager.GetFirstPersonWeaponModel(DestWeaponId)
                : weaponAvatarManager.GetThirdPersonWeaponModel(DestWeaponId);

            if (AssetInfoIsEmpty(assetInfo))
            {
                Logger.ErrorFormat("ErrorWeaponId Try To MountInPackage  id:  {0}", DestWeaponId);
                return;
            }
            
            if(HaveCommitLoadAssetRequest(WeaponLoadAssetHandler, assetInfo)) return;
            
            WeaponLoadAssetHandler.SetInfo(assetInfo, DestWeaponId);
            LoadRequestBatch.Add(LoadRequest(assetInfo, WeaponLoadAssetHandler));
        }
        
        public void UpdateHandWeaponType(WeaponInPackage type)
        {
            HandWeaponType = type;
            Attachment.UpdateHandWeaponType(type);
        }
        
        public void UpdateWeaponInHand(WeaponInPackage type)
        {
            if (WeaponInPackage.EmptyHanded == type)
            {
                if (!InHand) return;
                MountWeaponInPackage(Character);
                InHand = false;
            }
            else
            {
                var needInHand = type == Type;
                if(InHand == needInHand) return;

                if (needInHand)
                {
                    MountWeaponToHand(Character);
                    Change(WeaponId);
                }
                else
                    MountWeaponInPackage(Character);

                InHand = needInHand;
            }
        }

        public void UpdateOverrideControllerType(OverrideControllerState type)
        {
            if(type == OverrideControllerType) return;
            OverrideControllerType = type;
            switch (OverrideControllerType)
            {
                case OverrideControllerState.Transition:
                    ChangeTransition(WeaponId);
                    break;
                case OverrideControllerState.NoTransition:
                    Change(WeaponId);
                    break;
            }
        }
        
        private void WeaponAssetLoadSuccess(UnityObject weapon, int correctId)
        {
            WeaponId = correctId;
            RemoveWeapon();
            Weapon = new WeaponGameObjectData {Obj = weapon};
            Attachment.SetWeaponData(Weapon);
            MountWeapon();
            ChangeWeaponShader();
        }
        
        private void RemoveWeapon()
        {
            if(!Valid) return; // has be removed
            Valid = false;
            
            if (null != Weapon.DeputyAsGameObject)
            {
                Weapon.DeputyAsGameObject.SetActive(false);
                Weapon.DeputyAsGameObject.transform.SetParent(null, false);
                CallDelFunc(Weapon.DeputyAsGameObject);
            }
            
            if(null == Weapon.PrimaryAsGameObject) return;
            Weapon.PrimaryAsGameObject.SetActive(false);
            Weapon.PrimaryAsGameObject.transform.SetParent(null, false);
            CallDelFunc(Weapon.PrimaryAsGameObject);
            
            if(InHand)
                Change(UniversalConsts.InvalidIntId);
            InHand = false;
            
            AddRecycleObject(Weapon.GetRecycleUnityObject());

            CallChangeFunc(GetWeaponObj());
        }

        private void MountWeapon()
        {
            Valid = true;
            
            CheckShowHide();
            InHand = HandWeaponType == Type;
            if (InHand)
            {
                Mount.MountRightHandWeapon(Weapon.PrimaryAsGameObject, Character);
                Mount.MountLeftHandWeapon(Weapon.DeputyAsGameObject, Character);
                Change(WeaponId);
            }
            else
            {
                Mount.MountWeaponInPackage(Weapon.PrimaryAsGameObject, Character, Type);
                Mount.MountWeaponInPackage(Weapon.DeputyAsGameObject, Character, Type);
            }

            CallAddFunc(Weapon.PrimaryAsGameObject);
            CallAddFunc(Weapon.DeputyAsGameObject);
            
            CallChangeFunc(GetWeaponObj());
        }

        private void MountWeaponToHand(GameObject character)
        {
            Mount.MountRightHandWeapon(Weapon.PrimaryAsGameObject, character);
            Mount.MountLeftHandWeapon(Weapon.DeputyAsGameObject, character);
            CallChangeFunc(GetWeaponObj());
        }

        private void MountWeaponInPackage(GameObject character)
        {
            if (null != Weapon.DeputyAsGameObject)
            {
                Weapon.DeputyAsGameObject.transform.SetParent(null, false);
            }
            
            if (null != Weapon.PrimaryAsGameObject)
            {
                Weapon.PrimaryAsGameObject.transform.SetParent(null, false);
            }
            
            Mount.MountWeaponInPackage(Weapon.PrimaryAsGameObject, character, Type);
            Mount.MountWeaponInPackage(Weapon.DeputyAsGameObject, character, Type);
            CallChangeFunc(GetWeaponObj());
        }
    }
}
