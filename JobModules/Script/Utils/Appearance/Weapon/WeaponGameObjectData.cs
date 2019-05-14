using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.Weapon
{
    public class WeaponGameObjectData
    {
        public bool Valid;
        
        private UnityObject _obj;
        private GameObject _primaryGameObject;
        private GameObject _deputyGameObject;
        private bool _needAssemble;
        
        public WeaponGameObjectData()
        {
            Valid = false;
            _obj = null;
            _primaryGameObject = null;
            _deputyGameObject = null;
            _needAssemble = false;
        }

        public UnityObject Obj
        {
            set
            {
                _obj = value;
                if (null != value)
                    Valid = true;
                SetPrimaryAndDeputyGameObject(_obj);
            }
        }

        public UnityObject GetRecycleUnityObject()
        {
            AssemblePrimaryAndDeputyGameObject();
            return _obj;
        }

        public GameObject PrimaryAsGameObject
        {
            get
            {
                return _primaryGameObject;
            }
        }

        public GameObject DeputyAsGameObject
        {
            get
            {
                return _deputyGameObject;
            }
        }

        private void SetPrimaryAndDeputyGameObject(UnityObject obj)
        {
            if (null == obj || null == obj.AsGameObject ||
                !SingletonManager.Get<WeaponAvatarConfigManager>().HaveLeftWeapon(obj.AsGameObject.name))
            {
                _deputyGameObject = null;
                _primaryGameObject = null != obj ? obj.AsGameObject : null;
                _needAssemble = false;
                return;
            }

            _primaryGameObject = SingletonManager.Get<WeaponAvatarConfigManager>().GetRightWeaponGameObject(obj);
            _deputyGameObject = SingletonManager.Get<WeaponAvatarConfigManager>().GetLeftWeaponGameObject(obj);

            SplitPrimaryAndDeputyGameObject();
        }

        private void SplitPrimaryAndDeputyGameObject()
        {
            _needAssemble = true;

            if (null != _primaryGameObject)
            {
                _primaryGameObject.transform.SetParent(null, false);
                BoneTool.CacheTransform(_primaryGameObject);
            }

            if (null != _deputyGameObject)
            {
                _deputyGameObject.transform.SetParent(null, false);
                BoneTool.CacheTransform(_deputyGameObject);
            }
        }

        private void AssemblePrimaryAndDeputyGameObject()
        {
            if(!_needAssemble || null == _obj || null == _obj.AsGameObject) return;
            
            if(null != _primaryGameObject)
                _primaryGameObject.transform.SetParent(_obj.AsGameObject.transform, false);
            
            if(null != _deputyGameObject)
                _deputyGameObject.transform.SetParent(_obj.AsGameObject.transform, false);

            _primaryGameObject = null;
            _deputyGameObject = null;
            _needAssemble = false;
        }
    }
}
