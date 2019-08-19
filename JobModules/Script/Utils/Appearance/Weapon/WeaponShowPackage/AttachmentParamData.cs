using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public abstract class AttachmentParamData : ParamBase
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AttachmentParam));
        protected WeaponGameObjectData Weapon = new WeaponGameObjectData();
        protected readonly WeaponPartLocation AttachmentType;
        protected readonly BoneMount Mount = new BoneMount();
        protected WeaponInPackage HandWeaponType;
        protected UnityObject Attachment;
        protected int DestAttachmentId;
        protected int AttachmentId;
        protected bool Valid;
        
        private const int AttachmentInvalidId = UniversalConsts.InvalidIntId;
        private CharacterView _view = CharacterView.ThirdPerson;
        private readonly bool _isFirstPersonClass;
        private readonly WeaponInPackage _weaponType;
        
        protected AttachmentParamData(bool isFirstPersonClass, WeaponPartLocation attachmentType, WeaponInPackage weaponType)
        {
            _isFirstPersonClass = isFirstPersonClass;
            AttachmentType = attachmentType;
            _weaponType = weaponType;
            Init();
        }
        
        private void Init()
        {
            DestAttachmentId = AttachmentInvalidId;
            AttachmentId = AttachmentInvalidId;
            Valid = false;
        }
        
        public void SetFirstPerson()
        {
            _view = CharacterView.FirstPerson;
            
            if(_isFirstPersonClass)
                Show();
            else
                Hide();
        }

        public void SetThirdPerson()
        {
            _view = CharacterView.ThirdPerson;
            
            if(!_isFirstPersonClass)
                Show();
            else
                Hide();
        }
        
        public GameObject GetAttachmentObj()
        {
            return Valid ? Attachment : null;
        }
        
        public int GetAttachmentId()
        {
            return Valid ? AttachmentId : -1;
        }
        
        protected void CheckShowHide()
        {
            if (_isFirstPersonClass && IsFirstPerson ||
                !_isFirstPersonClass && !IsFirstPerson)
            {
                Show();
                return;
            }
            
            Hide();
        }
        
        private void Hide()
        {
            AppearanceUtils.DisableRender(Attachment);
        }

        private void Show()
        {
            AppearanceUtils.EnableRender(Attachment);
        }
        
        protected GameObject GetWeaponObj()
        {
            if (!Valid || null == Weapon) return null;
            return Weapon.PrimaryAsGameObject;
        }
        
        public void ChangeAttachmentShader()
        {
            if (!_isFirstPersonClass) return;
            ReplaceMaterialShaderBase.ChangeShader(GetAttachmentObj());
        }
        
        public void ResetAttachmentShader()
        {
            if (!_isFirstPersonClass) return;
            ReplaceMaterialShaderBase.ResetShader(GetAttachmentObj());
        }
        
        private bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }

        #region override

        protected override void CallChangeFunc(GameObject obj)
        {
            if(_weaponType != HandWeaponType) return;
            base.CallChangeFunc(obj);
        }
        
        protected override void AddRecycleObject(UnityObject obj)
        {
            ReplaceMaterialShaderBase.ResetShader(obj.AsGameObject);
            base.AddRecycleObject(obj);
        }

        public override IEnumerable<UnityObject> GetRecycleRequests()
        {
            RecycleRequestBatch.AddRange(WeaponLoadAssetHandler.GetRecycleRequests());
            return base.GetRecycleRequests();
        }
        
        public override void ClearRequests()
        {
            WeaponLoadAssetHandler.ClearRequests();
            base.ClearRequests();
        }

        #endregion
    }
}
