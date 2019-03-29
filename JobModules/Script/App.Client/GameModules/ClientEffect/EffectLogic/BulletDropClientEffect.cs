using Core.Configuration;
using UnityEngine;
using XmlConfig;
using Utils.AssetManager;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal class BulletDropClientEffect : AbstractClientEffect
    {
        public override void Initialize(ClientEffectEntity entity)
        {
            var bulletDrop = (GameObject)entity.assets.LoadedAssets[Asset];
            var position = entity.position.Value;
            var yaw = entity.effectRotation.Yaw;
            bulletDrop.transform.position = position;
            bulletDrop.transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
            bulletDrop.transform.localScale = Vector3.one;
            bulletDrop.SetActive(true);
        }
    }


}
