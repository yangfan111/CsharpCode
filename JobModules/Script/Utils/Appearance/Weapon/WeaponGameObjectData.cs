using UnityEngine;
using Utils.Appearance.Bone;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace Utils.Appearance.Weapon
{
    public class WeaponGameObjectData
    {
        public bool Valid;
        
        private UnityObject _obj;
        private bool _needAssemble;
        
        public WeaponGameObjectData()
        {
            Valid = false;
            _obj = null;
            PrimaryAsGameObject = null;
            DeputyAsGameObject = null;
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

        public GameObject PrimaryAsGameObject { get; private set; }
        public GameObject DeputyAsGameObject { get; private set; }

        private void SetPrimaryAndDeputyGameObject(UnityObject obj)
        {
            if (null == obj || null == obj.AsGameObject ||
                !SingletonManager.Get<WeaponAvatarConfigManager>().HaveLeftWeapon(obj.AsGameObject.name))
            {
                DeputyAsGameObject = null;
                PrimaryAsGameObject = null != obj ? obj.AsGameObject : null;
                _needAssemble = false;
                return;
            }

            PrimaryAsGameObject = SingletonManager.Get<WeaponAvatarConfigManager>().GetRightWeaponGameObject(obj);
            DeputyAsGameObject = SingletonManager.Get<WeaponAvatarConfigManager>().GetLeftWeaponGameObject(obj);

            SplitPrimaryAndDeputyGameObject();
        }

        private void SplitPrimaryAndDeputyGameObject()
        {
            _needAssemble = true;

            if (null != PrimaryAsGameObject)
            {
                PrimaryAsGameObject.transform.SetParent(null, false);
                BoneTool.CacheTransform(PrimaryAsGameObject);
            }

            if (null != DeputyAsGameObject)
            {
                DeputyAsGameObject.transform.SetParent(null, false);
                BoneTool.CacheTransform(DeputyAsGameObject);
            }
        }

        private void AssemblePrimaryAndDeputyGameObject()
        {
            if(!_needAssemble || null == _obj || null == _obj.AsGameObject) return;
            
            if(null != PrimaryAsGameObject)
                PrimaryAsGameObject.transform.SetParent(_obj.AsGameObject.transform, false);
            
            if(null != DeputyAsGameObject)
                DeputyAsGameObject.transform.SetParent(_obj.AsGameObject.transform, false);

            PrimaryAsGameObject = null;
            DeputyAsGameObject = null;
            _needAssemble = false;
        }
    }
}
