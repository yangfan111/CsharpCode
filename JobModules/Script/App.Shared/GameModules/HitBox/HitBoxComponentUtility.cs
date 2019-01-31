using Core.EntityComponent;
using Core.HitBox;
using Core.Utils;
using UnityEngine;
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
            playerEntity.AddHitBox(new BoundingSphere(sc.center, sc.radius), hitboxGo);
            hitboxGo.SetActive(false);

            return hitboxGo;
        }
    }
}