using App.Client.CastObjectUtil;
using App.Client.GameModules.SceneObject;
using Assets.XmlConfig;
using Core;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Appearance;
using Utils.Appearance.Bone;
using Utils.Appearance.Weapon;
using Utils.AssetManager;
using XmlConfig;

namespace App.Client.GameModules
{
    /// <summary>
    /// Defines the <see cref="SimpleLoadRespondHander" />
    /// </summary>
    public class SimpleLoadRespondHander : ResponseHandler
    {
        public void OnLoadUnityObjectSucess(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (!Verify(entity, unityObj))
                return;

            entity.AddUnityObject(unityObj);

            AppearanceUtils.EnableRender(unityObj.AsGameObject);
            ProcessSceneObjectSetting(entity, unityObj.AsGameObject);
        }


        public SimpleLoadRespondHander(Contexts contexts) : base(contexts)
        {
        }
    }

    /// <summary>
    /// Defines the <see cref="LoadWeaponRespondHandler" />
    /// </summary>
    public class LoadWeaponRespondHandler : ResponseHandler
    {
        private BoneMount boneMountUtil  = new BoneMount();


        protected override bool Verify(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (!base.Verify(entity, unityObj))
                return false;
            if (!entity.hasMultiUnityObject)
            {
                SceneObjectLoadSystem.Logger.Error("MultiUnityObject component is needed !");
                return false;
            }

            return true;
        }
        public void OnLoadEffectObjetSucess(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (!Verify(entity, unityObj))
            {
                CheckLoadingState(entity);
                return;
            }
            entity.multiUnityObject.CacheAssetObject(unityObj);
            
            if (unityObj.AsGameObject != null)
            {
                entity.weaponAttachment.EffectAssetList.Add(unityObj);
                AppearanceUtils.EnableRender(unityObj.AsGameObject);

            }
            CheckLoadingState(entity);
        }

        public void OnLoadPartObjectSucess(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (!Vertify(entity, unityObj))
            {
                CheckLoadingState(entity);
                return;

            }
            entity.multiUnityObject.CacheAssetObject(unityObj);
            if (unityObj.AsGameObject != null)
            {
                var partAssetData = entity.weaponAttachment.PartAssetDict[unityObj.Address];
                partAssetData.PartLocation = WeaponTypeConvertUtil.GetLocationByPartType((EWeaponPartType) partAssetData.PartSlotType);
                partAssetData.UnityObject  = unityObj;
                entity.weaponAttachment.PartAssetDict[unityObj.Address] = partAssetData;
                AppearanceUtils.EnableRender(unityObj.AsGameObject);

            }
            CheckLoadingState(entity);
        }
        [System.Obsolete]
        public void OnLoadTextAssetSucess(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (unityObj.AsObject == null || entity == null)
            {
                entity.multiUnityObject.loadingAssetInfos.Remove(unityObj.Address);
                CheckLoadingState(entity);
                return;
            }
            if (entity.multiUnityObject.loadingAssetInfos.Remove(unityObj.Address))
            {
                entity.multiUnityObject.LoadedAssets[unityObj.Address] = unityObj;
                if (unityObj.AsObject)
                {
                    entity.multiUnityObject.LoadedAssets[unityObj.Address] = unityObj;
                }
                else 
                     SceneObjectLoadSystem.Logger.ErrorFormat("asset {0}  as AsObject missing ",
                         unityObj.Address);
            }
            else
            {
                SceneObjectLoadSystem.Logger.ErrorFormat("asset {0} should not be treated as multi asset ",
                    unityObj.Address);
            }

            CheckLoadingState(entity);
        }

        private bool Vertify(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (unityObj == null)
            {
                SceneObjectLoadSystem.Logger.Error("loaded unityObj is null");
                return false;
            }

            if (entity == null)
            {
                SceneObjectLoadSystem.Logger.Error("Scene entity is null");
                return false;

            }
            if(!entity.hasMultiUnityObject )
            {
                SceneObjectLoadSystem.Logger.Error("MultiUnityObject component is needed !");
                return false;
            }

            return true;
        }
        public void OnLoadNecessaryObjectSucess(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (!Vertify(entity, unityObj))
            {
                CheckLoadingState(entity);
                return;

            }
            entity.multiUnityObject.CacheAssetObject(unityObj);
            if(unityObj.AsGameObject)
                AppearanceUtils.EnableRender(unityObj.AsGameObject);
            else
                SceneObjectLoadSystem.Logger.ErrorFormat("asset {0}'s gameobject is null",
                    unityObj.Address);

            CheckLoadingState(entity);
        }

        private void CheckLoadingState(SceneObjectEntity entity)
        {
            if (entity.multiUnityObject.loadingAssetInfos.Count == 0)
            {
                AssembleWeapon(entity);
                // 完成
                ProcessSceneObjectSetting(entity, entity.multiUnityObject.FirstAsset);
            }
        }

        private void AssembleWeapon(SceneObjectEntity entity)
        {
            var weaponGo      = entity.multiUnityObject.FirstAsset;
            var partAssetDict = entity.weaponAttachment.PartAssetDict;
            //var attachResList = entity.weaponAttachment.AttachResList;
            // if (attachResList.Count > 0 && attachResList[0] is Material)
            // {
            //     Material newMat = GameObject.Instantiate(attachResList[0] as Material);
            //     var renderAssy = weaponGo.GetComponentsInChildren<MeshRenderer>();
            //     foreach (var render in renderAssy)
            //     {
            //         render.sharedMaterial = newMat;
            //         render.enabled = true;
            //     }
            // }
            foreach (var keyPartAssetData in partAssetDict)
            {
                if (keyPartAssetData.Value.UnityObject && keyPartAssetData.Value.PartLocation != WeaponPartLocation.EndOfTheWorld)
                {
                    boneMountUtil.MountWeaponAttachment(keyPartAssetData.Value.UnityObject,
                        weaponGo, keyPartAssetData.Value.PartLocation);
                }
                else
                {
                    SceneObjectLoadSystem.Logger.ErrorFormat("Location is illegal with item type {0}", keyPartAssetData.Value);
                }
                // entity.weaponAttachment.EffectAssetList.ForEach(
                //     effectGo =>
                //     {
                //         if (effectGo != null && effectGo.AsGameObject)
                //             effectGo.AsGameObject.transform.position = entity.position.Value;
                //     });
            }

        }

        public LoadWeaponRespondHandler(Contexts contexts) : base(contexts)
        {
        }
    }

    /// <summary>
    /// Defines the <see cref="ResponseHandler" />
    /// </summary>
    public abstract class ResponseHandler
    {
        private IGlobalEffectManager _effectManager;
        public ResponseHandler(Contexts contexts)
        {
            _effectManager = contexts.session.clientSessionObjects.GlobalEffectManager;
        }
        protected virtual bool Verify(SceneObjectEntity entity, UnityObject unityObj)
        {
            if (null == entity)
            {
                SceneObjectLoadSystem.Logger.Error("Entity type mismatched !!");
                return false;
            }

            var model = unityObj.AsGameObject;
            if (null == model)
            {
                SceneObjectLoadSystem.Logger.ErrorFormat("Resource load object mismatched {0}",unityObj.Address);
                entity.multiUnityObject.loadingAssetInfos.Remove(unityObj.Address);
                return false;
            }
            return true;
        }

        /*private void EnableAutoPickup(int entityId, GameObject normalCollider)
        {
            var listener = normalCollider.GetComponent<SceneObjectTriggerEnterListener>();
            if (listener == null)
            {
                listener = normalCollider.AddComponent<SceneObjectTriggerEnterListener>();
            }
            listener.SetEntityId(entityId);
        }*/
        
        protected void ProcessSceneObjectSetting(SceneObjectEntity entity, GameObject model)
        {
            var target = SceneObjectGoAssemble.Assemble(model, entity);

            if (entity.hasEffects)
            {
                foreach (var effect in entity.effects.GlobalEffects)
                    _effectManager.AddGameObject(effect, model);
            }

            if (entity.hasWeaponObject)
            {
                ProcessWeaonCastSetting(entity, target);
            }
            else if (entity.hasSimpleItem)
            {
                ProcessSimpleEquipment(entity, target);
            }
        }

        private void ProcessSimpleEquipment(SceneObjectEntity entity, RayCastTarget target)
        {
            var equip    = entity.simpleItem;
            var category = entity.simpleItem.Category;
            var entityId = entity.entityKey.Value.EntityId;
            SceneObjCastData.Make(target, entityId, equip.Id, equip.Count, category);
            /*switch ((ECategory) category)
            {
                case ECategory.Weapon:
                    var weaponCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(equip.Id);
                    if (null == weaponCfg)
                    {
                        break;
                    }
                    var weaponType = (EWeaponType_Config) weaponCfg.Type;
                    if (weaponType.CanAutoPick())
                    {
                        EnableAutoPickup(entityId, target.gameObject);
                    }
                    break;
                default:
                    break;
            }*/
        }

        private void ProcessWeaonCastSetting(SceneObjectEntity entity, RayCastTarget target)
        {
            var weapon   = entity.weaponObject;
            var entityId = entity.entityKey.Value.EntityId;
            SceneObjCastData.Make(target, entityId, weapon.ConfigId, entity.hasSimpleItem ? entity.simpleItem.Count : 1, (int) ECategory.Weapon);
            /*var weaponType = (EWeaponType_Config) SingletonManager
                                                  .Get<WeaponResourceConfigManager>().GetConfigById(weapon.ConfigId)
                                                  .Type;
            if (weaponType.CanAutoPick())
            {
                EnableAutoPickup(entityId, target.gameObject);
            }*/
        }
    }
}
