using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance
{
    public class BoneMount
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BoneMount));

        private Dictionary<SpecialLocation, List<string>> _specialLocationName = new Dictionary<SpecialLocation, List<string>>(CommonIntEnumEqualityComparer<SpecialLocation>.Instance)
        {
            { SpecialLocation.EjectionLocation,     new List<string>{BoneName.EjectionSocket} },
            { SpecialLocation.FirstPersonCamera,    new List<string>{BoneName.FirstPersonCameraLocator} },
            { SpecialLocation.MagazinePosition,     new List<string>{BoneName.MagazineLocator } },
            { SpecialLocation.MuzzleEffectPosition, new List<string>{BoneName.MuzzleSocket1, BoneName.MuzzleSocket} },
            { SpecialLocation.SightsLocatorPosition, new List<string>{BoneName.AttachmentSight, BoneName.WeaponSight} },
            { SpecialLocation.EffectLocation,       new List<string>{BoneName.WeaponEffectLocator} }
        };

        private Dictionary<WeaponPartLocation, string> _attachmentLocator = new Dictionary<WeaponPartLocation, string>(CommonIntEnumEqualityComparer<WeaponPartLocation>.Instance)
        {
            { WeaponPartLocation.Muzzle,      BoneName.MuzzleLocator },
            { WeaponPartLocation.LowRail,     BoneName.LowRailLocator },
            { WeaponPartLocation.Magazine,    BoneName.MagazineLocator },
            { WeaponPartLocation.Buttstock,   BoneName.ButtstockLocator },
            { WeaponPartLocation.Scope,       BoneName.ScopeLocator }
        };

        private Dictionary<SpecialLocation,string> _searchStartLocator = new Dictionary<SpecialLocation, string>(CommonIntEnumEqualityComparer<SpecialLocation>.Instance)
        {
            { SpecialLocation.EjectionLocation, BoneName.CharRightHand },
            { SpecialLocation.MagazinePosition, BoneName.CharRightHand },
            { SpecialLocation.MuzzleEffectPosition,BoneName.CharRightHand },
            { SpecialLocation.SightsLocatorPosition,BoneName.CharRightHand },
        };

        public GameObject SearchingStart(GameObject obj, SpecialLocation location)
        {
            if (null == obj) return null;
            if (_searchStartLocator.ContainsKey(location))
            {
                var transform = FindChildBoneFromCache(obj, _searchStartLocator[location]);
                if (null == transform) return null;
                return transform.gameObject;
            }
            else return obj;
        }

        public Transform GetLocation(GameObject obj, SpecialLocation location, bool containsUnActive = false)
        {
            if (null == obj) return null;
            if (_specialLocationName.ContainsKey(location))
            {
                foreach (string boneName in _specialLocationName[location])
                {
                    var transform = FindChildBoneFromCache(obj, boneName, false, containsUnActive);
                    if (transform != null)
                    {
                        return transform;
                    }
                }
            }
            return null;
        }

        public void MountWeaponOnAlternativeLocator(GameObject weapon, GameObject character)
        {
            if (weapon != null && character != null)
            {
                UnmountWeaponOnAlternativeLocator(character);
                var anchor = FindChildBoneFromCache(weapon, BoneName.WeaponRightHand, false);
                var target = FindChildBoneFromCache(character, BoneName.AlternativeWeaponLocator, false);
                FixedObj2Bones(weapon, anchor, target);
            }
        }

        public void RemountWeaponOnRightHand(GameObject weapon, GameObject character)
        {
            if (weapon != null && character != null)
            {
                UnmountRightHandWeapon(character);
                var anchor = FindChildBoneFromCache(weapon, BoneName.WeaponRightHand, false);
                var target = FindChildBoneFromCache(character, BoneName.CharRightHand, false);
                FixedObj2Bones(weapon, anchor, target);
            }
        }

        public void MountRightHandWeapon(GameObject weapon, GameObject character)
        {
            if (weapon != null && character != null)
            {
                UnmountRightHandWeapon(character);
                var anchor = FindChildBoneFromCache(weapon, BoneName.WeaponRightHand, false);
                var target = FindChildBoneFromCache(character, BoneName.CharRightHand, false);
                FixedObj2Bones(weapon, anchor, target);
            }
        }

        public void MountLeftHandWeapon(GameObject weapon, GameObject character)
        {
            if(null == weapon || null == character) return;
            UnMountLeftHandWeapon(character);
            var anchor = FindChildBoneFromCache(weapon, BoneName.WeaponLeftHand, false);
            var target = FindChildBoneFromCache(character, BoneName.CharLeftHand, false);
            FixedObj2Bones(weapon, anchor, target);
        }

        public void MountCharacterToVehicleSeat(GameObject character, Transform seat)
        {
            if(null == character || null == seat) return;
            var anchor = FindChildBoneFromCache(character, BoneName.CharacterBipPelvisBoneName, false);
            FixedObj2Bones(character, character.transform, seat);
        }

        public void MountWardrobe(GameObject wardrobe, GameObject character)
        {
            if (wardrobe != null && character != null)
            {
                var anchor = FindChildBoneFromCache(wardrobe, BoneName.AttachmentLocator, false);
                var target = FindChildBoneFromCache(character, BoneName.CharacterHeadBoneName, false);
                FixedObj2Bones(wardrobe, anchor, target);
            }
        }

        public void MountWeaponAttachment(GameObject attachment, GameObject weapon, WeaponPartLocation location)
        {
            if (attachment != null && weapon != null)
            {
                if (_attachmentLocator.ContainsKey(location))
                {
                    var anchor = FindChildBoneFromCache(attachment, BoneName.AttachmentLocator, false);
                    var target = FindChildBoneFromCache(weapon, _attachmentLocator[location], false);
                    FixedObj2Bones(attachment, anchor, target);
                }
                else
                {
                    _logger.WarnFormat("Wrong Attachment Location: {0}", location);
                }
            }
        }

        public static void FixedObj2Bones(GameObject sourceObj, Transform anchor, Transform target)
        {
            if (target != null && anchor != null)
            {
                sourceObj.transform.SetParent(target, false);

                sourceObj.transform.localPosition = Vector3.zero;
                sourceObj.transform.localRotation = Quaternion.identity;
                sourceObj.transform.localScale = Vector3.one;
                
                var m = (sourceObj.transform.worldToLocalMatrix * anchor.localToWorldMatrix).inverse;
                sourceObj.transform.localPosition = m.ExtractPosition();
                sourceObj.transform.localRotation = m.ExtractRotation();
                sourceObj.transform.localScale = m.ExtractScale();
            }
        }

        public void UnmountRightHandWeapon(GameObject character)
        {
            UnmountTransform(FindChildBoneFromCache(character, BoneName.CharRightHand, false));
        }

        public void UnMountLeftHandWeapon(GameObject character)
        {
            UnmountTransform(FindChildBoneFromCache(character, BoneName.CharLeftHand, false));
        }

        public void UnmountWeaponOnAlternativeLocator(GameObject character)
        {
            UnmountTransform(
                FindChildBoneFromCache(character, BoneName.AlternativeWeaponLocator, false));
        }

        public void MountWeaponInPackage(GameObject weapon, GameObject character, WeaponInPackage pos, bool unMount = true)
        {
            if (weapon != null && character != null)
            {
                Transform target = null;
                switch (pos)
                {
                    case WeaponInPackage.PrimaryWeaponOne:
                        target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponOneOnBag);
                        if (target == null)
                        {
                            target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponOneOnCharacter, false);
                        }
                        break;
                    case WeaponInPackage.PrimaryWeaponTwo:
                        target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponTwoOnBag);
                        if (target == null)
                        {
                            target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponTwoOnCharacter, false);
                        }
                        break;
                    case WeaponInPackage.SideArm:
                        target = FindChildBoneFromCache(character, BoneName.SideArmOnCharacter, false);
                        break;
                    case WeaponInPackage.MeleeWeapon:
                        target = FindChildBoneFromCache(character, BoneName.MeleeWeaponOnCharacter, false);
                        break;
                    case WeaponInPackage.ThrownWeapon:
                        target = FindChildBoneFromCache(character, BoneName.ThrownWeaponOnCharacter, false);
                        break;
                    case WeaponInPackage.TacticWeapon:
                        target = FindChildBoneFromCache(character, BoneName.TacticWeaponOnCharacter, false);
                        break;
                }

                if(unMount)
                    UnmountTransform(target);
                
                if (target != null)
                {
                    weapon.transform.SetParent(target, false);
                    weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = Quaternion.identity;
                    weapon.transform.localScale = Vector3.one;
                }
            }
        }

        public void UnmountWeaponInPackage(GameObject character, WeaponInPackage pos)
        {
            if (character != null)
            {
                Transform target = null;
                switch (pos)
                {
                    case WeaponInPackage.PrimaryWeaponOne:
                        target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponOneOnCharacter, false);
                        break;
                    case WeaponInPackage.PrimaryWeaponTwo:
                        target = FindChildBoneFromCache(character, BoneName.PrimaryWeaponTwoOnCharacter, false);
                        break;
                }
                UnmountTransform(target);
            }
        }

        private void UnmountTransform(Transform target)
        {
            if (target != null)
            {
                foreach (Transform o in target)
                {
                    o.gameObject.transform.SetParent(null, false);
                }
            }
        }

        public static List<TransformCache> CacheResults = new List<TransformCache>(64);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        /// <param name="containsAttach">是否包含子节点缓存</param>
        /// <param name="containsUnActive">是否包含UnActive的节点</param>
        /// <returns></returns>
        public static Transform FindChildBoneFromCache(GameObject obj, string target, bool containsAttach = true, bool containsUnActive = false)
        {
            Transform r = null;
            if (obj != null && target != null)
            {
                if (containsAttach)
                {
                    FindTarget(obj, target, ref r, containsUnActive);
                }
                else if (obj.GetComponent<TransformCache>() == null)
                {
                    FindTarget(obj, target, ref r, containsUnActive);
                }
                else
                {
                    r = BoneTool.FindTransformFromCache(obj, target, containsUnActive);
                }
                return r;
            }
            _logger.DebugFormat("null object or null target in FindChildBone");

            return null;
        }

        private static void FindTarget(GameObject obj, string target, ref Transform r, bool containsUnActive = false)
        {
            CacheResults.Clear();
            obj.GetComponentsInChildren<TransformCache>(CacheResults);
            if (CacheResults.Count == 0)
            {
                _logger.DebugFormat("obj:{0} has no cache", obj);
            }
            foreach (TransformCache transformCache in CacheResults)
            {
                r = BoneTool.FindTransformFromCache(transformCache, target, containsUnActive);
                if (r != null)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// 查找角色，武器，配件的关节请使用缓存版本的函数BoneMount.FindChildBoneFromCache
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        /// <param name="ignoreMissing"></param>
        /// <returns></returns>
        /// <seealso cref="BoneMount.FindChildBoneFromCache"/>
        public static Transform FindChildBone(GameObject obj, string target, bool ignoreMissing = false)
        {
         
            if (obj != null && target != null)
            {
                
                var r = BoneTool.FindTransformFromCache(obj, target);

                if (r == null)
                {
                    _logger.InfoFormat("target:{0} is not cached!!!!", target);
                    r = obj.transform.FindTransformRecursive(target);
                }
                
                if (r == null &&!ignoreMissing)
                    _logger.DebugFormat("missing {0} in {1}", target, obj.name);
                return r;
            }
            else
            {
                _logger.DebugFormat("null object or null target in FindChildBone");
            }

            return null;
        }
        public static List<Transform> results = new List<Transform>(512);
        public static Transform FindChildBone2(GameObject obj, string target, bool ignoreMissing = false)
        {
         
            if (obj != null && target != null)
            {
                // Transform.Find didn't recursive find
                results.Clear();
                obj.GetComponentsInChildren(results);
                foreach (var child in results)
                {
                    
                    if (child.name == target)
                    {
                        return child;
                    }
                }
                if (!ignoreMissing)
                    _logger.DebugFormat("missing {0} in {1}", target, obj.name);
            }
            else
            {
                _logger.WarnFormat("null object or null target in FindChildBone");
            }

            return null;
        }
    }
}
