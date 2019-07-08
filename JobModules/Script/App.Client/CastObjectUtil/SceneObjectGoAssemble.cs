using App.Client.GameModules.SceneObject;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Utils;
using Entitas.VisualDebugging.Unity;
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
            
            var child = model.transform.Find(SceneObjectConstant.NormalColliderName);
            if(null != child) child.gameObject.DestroyGameObject();
            //var needRotate = false;
            int cat = 0;
            if (entity.hasWeaponObject)
            {
                cat = (int) ECategory.Weapon;
            }
            else if (entity.hasSimpleItem)
            {
                cat = entity.simpleItem.Category;
            }

            if (cat == 0)
            {
                Logger.Error("illegal sceneobject has no weapon or equip component ");
            }

            switch (cat)
            {
                case (int) ECategory.Weapon:
                    if (SingletonManager.Get<WeaponResourceConfigManager>().IsC4(entity.weaponObject.ConfigId) ||
                        SingletonManager.Get<WeaponResourceConfigManager>().IsArmors(entity.weaponObject.ConfigId))
                    {
                        WeaponAnimationBase.FinishedWeaponAnimation(model);
                        model.transform.localRotation = Quaternion.identity;
                        var c4Collider = model.GetComponent<SphereCollider>();
                        if (c4Collider == null) c4Collider = model.AddComponent<SphereCollider>();
                        c4Collider.radius = 0.3f;
                        c4Collider.isTrigger = true;
                    }
                    else
                    {
                        model.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        var weaponCollider = model.GetComponent<CapsuleCollider>();
                        if (weaponCollider == null) weaponCollider = model.AddComponent<CapsuleCollider>();
                        weaponCollider.direction = 2;
                        weaponCollider.radius = 0.3f;
                        weaponCollider.height = 0.8f;
                        weaponCollider.isTrigger = true;
                    }
                    model.transform.localScale = Vector3.one;
                    model.transform.position = entity.position.Value + BaseGoAssemble.GetGroundAnchorOffset(model);
                    break;
                case (int) ECategory.Avatar:
                    model.transform.localRotation = Quaternion.identity;
                    var avatarCollider = model.GetComponent<SphereCollider>();
                    if (avatarCollider == null) avatarCollider = model.AddComponent<SphereCollider>();
                    avatarCollider.radius = 0.3f;
                    avatarCollider.isTrigger = true;
                    model.transform.localScale = Vector3.one;
                    break;
                case (int) ECategory.GameItem:
                    model.transform.localRotation = Quaternion.identity;
                    var itemCollider = model.GetComponent<SphereCollider>();
                    if (itemCollider == null) itemCollider = model.AddComponent<SphereCollider>();
                    if (entity.hasSize)
                    {
                        var size = Mathf.Max(1f, entity.size.Value);
                        model.transform.localScale  = Vector3.one * size;
                        itemCollider.radius = 0.3f / size;
                    }
                    else
                    {
                        model.transform.localScale = Vector3.one;
                        itemCollider.radius = 0.3f;
                    }
                    itemCollider.isTrigger = true;
                    break;
                case (int) ECategory.WeaponPart:
                    model.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    var partCollider = model.GetComponent<SphereCollider>();
                    if (partCollider == null) partCollider = model.AddComponent<SphereCollider>();
                    if (entity.hasSize)
                    {
                        var size = Mathf.Max(1f, entity.size.Value);
                        model.transform.localScale  = Vector3.one * size;
                        partCollider.radius = 0.3f / size;
                    }
                    else
                    {
                        model.transform.localScale = Vector3.one;
                        partCollider.radius = 0.3f;
                    }
                    partCollider.isTrigger = true;
                    break;
            }

            var targer = RayCastTargetUtil.AddRayCastTarget(model.gameObject);
            return targer;
        }
    }
}
