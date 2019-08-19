using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.AssetManager;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public class ParamBase
    {
        protected WeaponLoadAssetHandler WeaponLoadAssetHandler;
        protected Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> LoadRequest;
        private Action<GameObject> _changedCallBack;
        private Action<GameObject> _afterDelCallBack;
        private Action<GameObject> _afterAddCallBack;
        
        public virtual void SetAbstractLoadRequest(Func<AssetInfo, ILoadedHandler, AbstractLoadRequest> loadRequest)
        {
            LoadRequest = loadRequest;
        }
        
        #region ICharacterLoadResource

        protected readonly List<AbstractLoadRequest> LoadRequestBatch = new List<AbstractLoadRequest>();
        protected readonly List<UnityObject> RecycleRequestBatch = new List<UnityObject>();

        protected virtual void AddRecycleObject(UnityObject obj)
        {
            if (obj == null) return;
            RecycleRequestBatch.Add(obj);
        }
        
        public virtual IEnumerable<UnityObject> GetRecycleRequests()
        {
            return RecycleRequestBatch;
        }
        
        public virtual IEnumerable<AbstractLoadRequest> GetLoadRequests()
        {
            return LoadRequestBatch;
        }

        public virtual void ClearRequests()
        {
            LoadRequestBatch.Clear();
            RecycleRequestBatch.Clear();
        }

        #endregion
        
        protected static bool AssetInfoIsEmpty(AssetInfo info)
        {
            return (null == info.AssetName || info.AssetName.Equals(string.Empty)) &&
                   (null == info.BundleName || info.BundleName.Equals(string.Empty));
        }
        
        protected static bool HaveCommitLoadAssetRequest(WeaponLoadAssetHandler handler, AssetInfo info)
        {
            return info.Equals(handler.GetAssetInfo());
        }
        
        public virtual void SetChangeCallBack(Action<GameObject> action)
        {
            _changedCallBack = action;
        }

        public virtual void SetAfterDelCallBack(Action<GameObject> action)
        {
            _afterDelCallBack = action;
        }

        public virtual void SetAfterAddCallBack(Action<GameObject> action)
        {
            _afterAddCallBack = action;
        }

        protected virtual void CallChangeFunc(GameObject obj)
        {
            if(null == _changedCallBack) return;
            _changedCallBack.Invoke(obj);
        }

        protected virtual void CallDelFunc(GameObject obj)
        {
            if(null == _afterDelCallBack) return;
            _afterDelCallBack.Invoke(obj);
        }

        protected virtual void CallAddFunc(GameObject obj)
        {
            if (null == _afterAddCallBack) return;
            _afterAddCallBack.Invoke(obj);
        }
    }
}
