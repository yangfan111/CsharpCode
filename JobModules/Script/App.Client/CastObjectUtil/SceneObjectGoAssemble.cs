using App.Client.GameModules.SceneObject;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using UserInputManager.Lib;
using UserInputManager.Utility;
using Utils.Appearance.Weapon;
using Utils.Singleton;

namespace App.Client.CastObjectUtil
{
    public static class SceneObjectGoAssemble
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectGoAssemble));

        public static RayCastTarget Assemble(GameObject model, SceneObjectEntity entity)
        {
            model.transform.localRotation = Quaternion.Euler(0, 0, 0);
            model.transform.position = Vector3.zero;

            var child = model.transform.Find(SceneObjectConstant.NormalColliderName);
            BoxCollider boxCollider;
            if (null == child)
            {
                child = new GameObject(SceneObjectConstant.NormalColliderName).transform;
                child.parent = model.transform;
                boxCollider = child.gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
            }
            else
            {
                boxCollider = child.gameObject.GetComponent<BoxCollider>();
            }
            boxCollider.center = Vector3.zero;
            boxCollider.enabled = true;
            child.gameObject.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.UserInputRaycast);

            if(entity.hasWeaponObject && SingletonManager.Get<WeaponResourceConfigManager>().IsC4(entity.weaponObject.ConfigId))
                WeaponAnimationBase.FinishedWeaponAnimation(model);

            Bounds bounds = CalculateBounds(model);
            boxCollider.size = bounds.size; // * model.transform.worldToLocalMatrix;
            /*if (entity.hasWeaponObject)
            {
                var config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(entity.weaponObject.ConfigId);
                if (config != null && ((EWeaponType_Config) config.Type).CanAutoPick())
                {
                    if (SingletonManager.Get<WeaponResourceConfigManager>().IsC4(entity.weaponObject.ConfigId))
                    {
                        boxCollider.size = new Vector3(bounds.size.x, 0.4f, bounds.size.z);
                    }
                    else
                    {
                        boxCollider.size = new Vector3(0.4f, bounds.size.y, bounds.size.z);
                    }
                }
            }*/
            entity.position.Bounds = bounds;

            child.localEulerAngles = Vector3.zero;
            child.localPosition = bounds.center - model.transform.position;
            child.localScale = Vector3.one;

            if (entity.hasSize) model.transform.localScale  = Vector3.one * Mathf.Max(1f, entity.size.Value);
            else model.transform.localScale = Vector3.one;

            if ((entity.hasWeaponObject && !SingletonManager.Get<WeaponResourceConfigManager>().IsC4(entity.weaponObject.ConfigId) &&
                 !SingletonManager.Get<WeaponResourceConfigManager>().IsArmors(entity.weaponObject.ConfigId))
                || (entity.hasSimpleItem && entity.simpleItem.Category == (int) ECategory.WeaponPart))
            {
                model.transform.localRotation = Quaternion.Euler(0, 0, -90);
                entity.position.ModelRotate = true;
            }
            else
            {
                entity.position.ModelRotate = false;
            }

            
            var target = RayCastTargetUtil.AddRayCastTarget(child.gameObject);
            return target;
        }

        private static Bounds CalculateBounds(GameObject model)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();
            model.GetComponentsInChildren(false, renderers);
            MeshRenderer standard = renderers[0];
            Vector3 totalMin = standard.bounds.min;
            Vector3 totalMax = standard.bounds.max;
            if (renderers.Count > 1)
            {
                foreach (MeshRenderer meshRenderer in renderers)
                {
                    if (meshRenderer == standard) continue;
                    var min = meshRenderer.bounds.min;
                    var max = meshRenderer.bounds.max;
                    if (min.x < totalMin.x) totalMin.x = min.x;
                    if (min.y < totalMin.y) totalMin.y = min.y;
                    if (min.z < totalMin.z) totalMin.z = min.z;
                    if (max.x > totalMax.x) totalMax.x = max.x;
                    if (max.y > totalMax.y) totalMax.y = max.y;
                    if (max.z > totalMax.z) totalMax.z = max.z;
                }
            }

            List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();
            model.GetComponentsInChildren(false, skinnedRenderers);
            if (skinnedRenderers.Count > 0)
            {
                foreach (SkinnedMeshRenderer skinnedRenderer in skinnedRenderers)
                {
                    var min = skinnedRenderer.bounds.min;
                    var max = skinnedRenderer.bounds.max;
                    if (min.x < totalMin.x) totalMin.x = min.x;
                    if (min.y < totalMin.y) totalMin.y = min.y;
                    if (min.z < totalMin.z) totalMin.z = min.z;
                    if (max.x > totalMax.x) totalMax.x = max.x;
                    if (max.y > totalMax.y) totalMax.y = max.y;
                    if (max.z > totalMax.z) totalMax.z = max.z;
                }
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(totalMin, totalMax);
            return bounds;
        }

    }
}
