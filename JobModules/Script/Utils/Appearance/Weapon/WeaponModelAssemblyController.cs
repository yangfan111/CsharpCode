//#define UnitTest
using Core.Utils;
using UnityEngine;
using Utils.Appearance.Bone;
using XmlConfig;

namespace Utils.Appearance.Weapon
{
    public class WeaponModelAssemblyController : IWeaponModelAssemblyController
    {
#if !UnitTest
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponModelAssemblyController));
#endif
        protected GameObject _rootGo;
        private BoneMount _boneMount = new BoneMount();

        public WeaponModelAssemblyController(object root)
        {
            _rootGo = root as GameObject;            
        }

        public virtual void ShowWeapon(object weaponObj)
        {
            var weaponGo = weaponObj as GameObject;
            if(null != _rootGo)
            {
                var srcRotation = _rootGo.transform.rotation;
                _rootGo.transform.rotation = Quaternion.identity;
//                weaponGo.transform.parent = _rootGo.transform;
                weaponGo.transform.SetParent(_rootGo.transform);
                weaponGo.transform.localScale = Vector3.one;
                weaponGo.transform.localRotation = Quaternion.identity;
                weaponGo.transform.localPosition = GetCenterOffset(weaponGo);
                _rootGo.transform.rotation = srcRotation;
            }
            else
            {
#if !UnitTest
                Logger.Error("root go is null ");
#endif
            }

        }

        public void Attach(WeaponPartLocation partLocation, object partObj, object weaponObj)
        {

            var partGo = partObj as GameObject;
            var weaponGo = weaponObj as GameObject;
#if !UnitTest
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("attach {0} to {1} in {2}", partGo.name, weaponGo.name, partLocation);
            }
#endif
            
            if(partLocation != WeaponPartLocation.EndOfTheWorld)
            {
                _boneMount.MountWeaponAttachment(partGo, weaponGo, partLocation);
            }
            else
            {
#if !UnitTest
                Logger.ErrorFormat("Location is illegal with item location {0}", partLocation);
#endif
            }

            ShowWeapon(weaponGo);
        }

        protected Vector3 GetCenterOffset(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if(renderers.Length < 1)
            {
                Logger.ErrorFormat("no renderer in gameobject {0}", go.name);
                return Vector3.zero;
            }
            var min = Vector3.one * float.MaxValue;
            var max = Vector3.one * float.MinValue;
            for(var i = 0; i < renderers.Length; i++)
            {
                min = Vector3.Min(renderers[i].bounds.min, min);
                max = Vector3.Max(renderers[i].bounds.max, max);
            }
            var renderCenter = -((max + min) / 2f - go.transform.position);
            return renderCenter;
        }
    }
}
