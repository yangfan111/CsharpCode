using System.Collections.Generic;
using System.Linq;
using Core.EntityComponent;
using Core.HitBox;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig.HitBox;

namespace App.Shared.GameModules.HitBox
{
    public class HitBoxComponentUtility
    {
        public static GameObject InitHitBoxComponent(EntityKey entityKey, IHitBox playerEntity, GameObject hitboxGo)
        {
            hitboxGo.name = "hitbox_" + entityKey;
            GameObject bsGo = HitBoxConstants.FindBoundingSphereModel(hitboxGo);

            SphereCollider sc = bsGo.GetComponent<SphereCollider>();
            sc.enabled = false;
            
            hitboxGo.transform.Recursively(t =>
            {
                var go = t.gameObject;
                HitBoxOwnerComponent pc = go.GetComponent< HitBoxOwnerComponent>();
                if (pc == null)
                {
                    pc = go.AddComponent<HitBoxOwnerComponent>();
                }
                pc.OwnerEntityKey = entityKey;
                pc.gameObject.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.Hitbox);
            });
            playerEntity.AddHitBox(new BoundingSphere(sc.center, sc.radius), hitboxGo, false);
            hitboxGo.SetActive(false);

            return hitboxGo;
        }

        public static void InitHitBoxComponent(EntityKey entityKey, IHitBox playerEntity,
            HitBoxTransformProvider provider)
        {
            provider.SetActive(false);
            playerEntity.ReplaceHitBox(new BoundingSphere(provider.BoundSpherePosition(), provider.BoundSphereRadius()),
                null, false);
            var trans = provider.GetHitBoxTransforms();
            foreach (var item in trans)
            {
                var comp = item.Value.gameObject.GetComponent<HitBoxOwnerComponent>();
                if (comp == null)
                    comp = item.Value.gameObject.AddComponent<HitBoxOwnerComponent>();
                comp.OwnerEntityKey = entityKey;
            }
            provider.FlushLayerOfHitBox();
        }

        public static List<Transform> GetHitBoxTransforms(GameObject model)
        {
            var provider = SingletonManager.Get<HitBoxTransformProviderCache>().GetProvider(model);
            return provider.GetHitBoxTransforms().Values.ToList();
        }
        
    }
}