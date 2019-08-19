using System;
using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Utils;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public abstract class WeaponParamData : OverrideAnimator
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponParam));
        protected readonly BoneMount Mount = new BoneMount();
        protected readonly WeaponAttachment Attachment;
        protected readonly WeaponInPackage Type;
        protected readonly bool IsFirstPersonClass;
        
        protected WeaponInPackage HandWeaponType = WeaponInPackage.EmptyHanded;
        protected OverrideControllerState OverrideControllerType = OverrideControllerState.Null;
        protected WeaponGameObjectData Weapon = new WeaponGameObjectData();
        protected GameObject Character;
        protected int DestWeaponId;
        protected int WeaponId;
        protected bool Valid;
        protected bool InHand;
        
        private const int WeaponInvalidId = UniversalConsts.InvalidIntId;
        private CharacterView _view = CharacterView.ThirdPerson;
        
        protected WeaponParamData(bool isFirstPersonClass, WeaponInPackage type) : base(isFirstPersonClass)
        {
            Attachment = new WeaponAttachment(isFirstPersonClass, type);
            IsFirstPersonClass = isFirstPersonClass;
            Type = type;
            Init();
        }
        
        private void Init()
        {
            DestWeaponId = WeaponInvalidId;
            WeaponId = WeaponInvalidId;
            Valid = false;
            InHand = false;
        }
        
        public void SetCharacter(GameObject character)
        {
            Character = character;
        }

        public void SetFirstPerson()
        {
            _view = CharacterView.FirstPerson;
            
            if(IsFirstPersonClass)
                Show();
            else
                Hide();
            
            Attachment.SetFirstPerson();
        }

        public void SetThirdPerson()
        {
            _view = CharacterView.ThirdPerson;
            
            if(!IsFirstPersonClass)
                Show();
            else
                Hide();
            
            Attachment.SetThirdPerson();
        }
        
        public GameObject GetWeaponObj()
        {
            return Valid ? Weapon.PrimaryAsGameObject : null;
        }
        
        public int GetWeaponId()
        {
            return Valid ? WeaponId : -1;
        }
        
        private bool IsFirstPerson
        {
            get { return _view == CharacterView.FirstPerson; }
        }
        
        public void ChangeWeaponShader()
        {
            if (null == Weapon || !IsFirstPersonClass) return;
            ReplaceMaterialShaderBase.ChangeShader(Weapon.PrimaryAsGameObject);
            ReplaceMaterialShaderBase.ChangeShader(Weapon.DeputyAsGameObject);
            Attachment.SetP1WeaponTopLayerShader();
        }

        public void ResetWeaponShader()
        {
            if (null == Weapon || !IsFirstPersonClass) return;
            ReplaceMaterialShaderBase.ResetShader(Weapon.PrimaryAsGameObject);
            ReplaceMaterialShaderBase.ResetShader(Weapon.DeputyAsGameObject);
            Attachment.ResetP1WeaponTopShader();
        }
        
        protected void CheckShowHide()
        {
            if (IsFirstPersonClass && IsFirstPerson ||
                !IsFirstPersonClass && !IsFirstPerson)
            {
                Show();
                return;
            }
            
            Hide();
        }
        
        private void Hide()
        {
            AppearanceUtils.DisableRender(Weapon.PrimaryAsGameObject);
            AppearanceUtils.DisableRender(Weapon.DeputyAsGameObject);
        }

        private void Show()
        {
            AppearanceUtils.EnableRender(Weapon.PrimaryAsGameObject);
            AppearanceUtils.EnableRender(Weapon.DeputyAsGameObject);
        }
        
        #region override

        public override void SetAbstractLoadRequest(Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> loadRequest)
        {
            base.SetAbstractLoadRequest(loadRequest);
            Attachment.SetAbstractLoadRequest(loadRequest);
        }

        public override void SetChangeCallBack(Action<GameObject> action)
        {
            base.SetChangeCallBack(action);
            Attachment.SetChangeCallBack(action);
        }

        public override void SetAfterDelCallBack(Action<GameObject> action)
        {
            base.SetAfterDelCallBack(action);
            Attachment.SetAfterDelCallBack(action);
        }

        public override void SetAfterAddCallBack(Action<GameObject> action)
        {
            base.SetAfterAddCallBack(action);
            Attachment.SetAfterAddCallBack(action);
        }

        protected override void CallChangeFunc(GameObject obj)
        {
            if(Type != HandWeaponType) return;
            base.CallChangeFunc(obj);
        }

        protected override void AddRecycleObject(UnityObject obj)
        {
            Attachment.RemoveAllAttachment();
            WeaponAnimationBase.FinishedWeaponAnimation(obj.AsGameObject);
            ReplaceMaterialShaderBase.ResetShader(obj.AsGameObject);
            base.AddRecycleObject(obj);
        }

        public override IEnumerable<UnityObject> GetRecycleRequests()
        {
            RecycleRequestBatch.AddRange(WeaponLoadAssetHandler.GetRecycleRequests());
            RecycleRequestBatch.AddRange(Attachment.GetRecycleRequests());
            return base.GetRecycleRequests();
        }
        
        public override IEnumerable<AbstractLoadRequest> GetLoadRequests()
        {
            LoadRequestBatch.AddRange(Attachment.GetLoadRequests());
            return LoadRequestBatch;
        }

        public override void ClearRequests()
        {
            WeaponLoadAssetHandler.ClearRequests();
            Attachment.ClearRequests();
            base.ClearRequests();
        }

        #endregion
    }
}
