using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class AttachmentParam : AttachmentParamData
    {
        public AttachmentParam(bool isFirstPersonClass, WeaponPartLocation attachmentType, WeaponInPackage weaponType) :
            base(isFirstPersonClass, attachmentType, weaponType)
        {
            WeaponLoadAssetHandler = new WeaponLoadAssetHandler(AttachmentAssetLoadSuccess);
        }

        public void UpdateHandWeaponType(WeaponInPackage type)
        {
            HandWeaponType = type;
        }
        
        public void UpdateAttachmentId(int destId)
        {
            DestAttachmentId = destId;
            if(DestAttachmentId == AttachmentId) return;

            if (DestAttachmentId <= 0)
            {
                AttachmentId = DestAttachmentId;
                RemoveAttachment();
            }
            else
                LoadResource();
        }
        
        private void LoadResource()
        {
            var assetInfo = SingletonManager.Get<WeaponPartsConfigManager>().GetAsset(DestAttachmentId);
            
            if (AssetInfoIsEmpty(assetInfo))
            {
                Logger.ErrorFormat("ErrorAttachmentId Try To Mount  id:  {0}", DestAttachmentId);
                return;
            }
            
            if(HaveCommitLoadAssetRequest(WeaponLoadAssetHandler, assetInfo)) return;
            
            WeaponLoadAssetHandler.SetInfo(assetInfo, DestAttachmentId);
            LoadRequestBatch.Add(LoadRequest(assetInfo, WeaponLoadAssetHandler));
        }
        
        private void AttachmentAssetLoadSuccess(UnityObject attachment, int correctId)
        {
            AttachmentId = correctId;
            RemoveAttachment();
            Attachment = attachment;
            MountAttachment();
            ChangeAttachmentShader();
        }
        
        public void RemoveAttachment()
        {
            if(!Valid) return; //has be removed
            Valid = false;

            if(null == Attachment || null == Attachment.AsGameObject) return;
            Attachment.AsGameObject.SetActive(false);
            Attachment.AsGameObject.transform.SetParent(null, false);

            AddRecycleObject(Attachment);
            CallDelFunc(Attachment.AsGameObject);
            CallChangeFunc(GetWeaponObj());
        }
        
        private void MountAttachment()
        {
            Valid = true;
            CheckShowHide();
            Mount.MountWeaponAttachment(Attachment, GetWeaponObj(), AttachmentType);
            CallAddFunc(GetAttachmentObj());
            
            CallChangeFunc(GetWeaponObj());
        }

        public void SetWeapon(WeaponGameObjectData weapon)
        {
            Weapon = weapon;
            
            if (AttachmentId <= 0)
                RemoveAttachment();
            else
                LoadResource();
        }
    }
}
