using Core.Configuration;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal class ClipDropClientEffect : AbstractClientEffect
    {
        public override void Initialize(ClientEffectEntity entity)
        {
            var clip = (GameObject)entity.assets.LoadedAssets[Asset];
            var position = entity.position.Value;
            var yaw = entity.effectRotation.Yaw;
            clip.transform.position = position;
            clip.transform.rotation = Quaternion.Euler(new Vector3(0, yaw + 90, 0));
            clip.transform.localScale = Vector3.one;
            clip.SetActive(true);
        }
    }
}
