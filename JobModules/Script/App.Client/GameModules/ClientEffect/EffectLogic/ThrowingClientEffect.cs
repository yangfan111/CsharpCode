using System;
using App.Shared.Components.Ui;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Audio;
using App.Shared.Configuration;
using App.Shared.Player;
using Core.Configuration;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Appearance.Bone;
using Utils.Singleton;
using Utils.Utils;

namespace App.Client.GameModules.ClientEffect.EffectLogic
{
    internal class GrenadeExplosionEffect : AbstractClientEffect
    {
    }

    internal class FlashBombEffect : AbstractClientEffect
    {
        private int _layerMask;

        public FlashBombEffect()
        {
            _layerMask = UnityLayers.AllCollidableLayerMask;
        }

        public override void Initialize(ClientEffectEntity entity)
        {
            base.Initialize(entity);
            CheckFlash(entity);
        }

        private void CheckFlash(ClientEffectEntity entity)
        {
            PlayerEntity player = AllContexts.player.flagSelfEntity;

            //闪光弹位置
            Vector3 pos = entity.position.Value;

            //水里1米
            if (SingletonManager.Get<MapConfigManager>().InWater(pos))
            {
                float wy = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(pos);
                if (!float.IsNaN(wy) && wy - pos.y > 1)
                {
                    return;
                }
            }

            bool isShow = false;
            float alpha = 0, keepTime = 0, decayTime = 0;
            Vector3 playerPos = player.position.Value;

            //位置稍上抬
            pos.y += 0.1f;

            //玩家头部位置
            Transform tran;
            if (player.appearanceInterface.Appearance.IsFirstPerson)
            {
                var root = player.RootGo();
                tran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
            }
            else
            {
                var root = player.RootGo();
                tran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName);
            }
            if (null != tran)
            {
                playerPos = tran.position;
            }

            //距离
            float dis = Vector3.Distance(playerPos, pos);
            if (dis >= 40)
            {
                return;
            }

            float diffAngle = 0;

            //判断是否手上爆炸
            bool isHandBomb = entity.hasEffectRotation && entity.effectRotation.Yaw > 0;
            if (!(entity.ownerId.Value == player.entityKey.Value && isHandBomb))
            {
                //遮挡
                Vector3 hitPoint;
                bool isHit = CommonMathUtil.Raycast(playerPos, pos, dis, _layerMask, out hitPoint);
                if (isHit)
                {
                    return;
                }

                //角度
                float yaw = CommonMathUtil.TransComAngle(player.orientation.Yaw);
                float angle = CommonMathUtil.GetAngle(new Vector2(pos.x, pos.z), new Vector2(playerPos.x, playerPos.z));
                diffAngle = Mathf.Abs(angle - yaw);
                diffAngle = Mathf.Min(360 - diffAngle, diffAngle);
            }

            if (diffAngle <= 60)
            {
                if (dis < 20)
                {
                    isShow = true;
                    alpha = 1;
                    keepTime = Mathf.Max(1.2f, 3 * Mathf.Pow(0.4f, dis / 30));
                    decayTime = alpha / 0.4f;
                }
                else if (dis < 40)
                {
                    isShow = true;
                    alpha = 1 - dis * 0.00025f;
                    keepTime = 0;
                    decayTime = alpha / 0.4f;
                }
            }
            else if (diffAngle <= 90)
            {
                if (dis <= 20)
                {
                    isShow = true;
                    alpha = Mathf.Max(0, 1 - dis * 0.00025f);
                    keepTime = 0;
                    decayTime = alpha / 0.4f;
                }
            }
            else if (diffAngle <= 180)
            {
                if (dis <= 10)
                {
                    isShow = true;
                    alpha = Mathf.Max(0, 0.8f - dis * 0.00025f);
                    keepTime = 0;
                    decayTime = alpha / 0.5f;
                }
            }

            if (isShow)
            {
                ScreenFlashInfo screenFlashInfo = new ScreenFlashInfo();
                screenFlashInfo.IsShow = true;
                screenFlashInfo.Alpha = alpha;
                screenFlashInfo.KeepTime = keepTime;
                screenFlashInfo.DecayTime = decayTime;
                AllContexts.ui.uI.ScreenFlashInfo = screenFlashInfo;
                GameAudioMedia.PlayFlashDizzyAudio(pos,Math.Min(0,40-dis));
            }
        }
    }

    internal class FogBombEffect : AbstractClientEffect
    {
        private int _layerMask;

        public FogBombEffect()
        {
            _layerMask = UnityLayers.AllCollidableLayerMask;//UnityLayerManager.GetLayerMask(EUnityLayerName.Default) | UnityLayers.VehicleLayerMask;
        }

        public override void Initialize(ClientEffectEntity entity)
        {
            base.Initialize(entity);
            var go = (GameObject)entity.assets.LoadedAssets[Asset];
            try
            {
//                go.SendMessage("ApplyTriggerMask", _layerMask);
            }
            catch (Exception e)
            {
                Debug.LogFormat("ApplyTrigger error {0}", e.Message);
            }
        }
    }

    internal class BurnBombEffect : AbstractClientEffect
    {
    }

    internal class PullBoltEffect : AbstractClientEffect
    {
        public override void Initialize(ClientEffectEntity entity)
        {
            var pullBolt = (GameObject)entity.assets.LoadedAssets[Asset];
            var position = entity.position.Value;
            var yaw = entity.effectRotation.Yaw;
            pullBolt.transform.position = position;
            pullBolt.transform.rotation = Quaternion.Euler(new Vector3(0, yaw, 0));
            pullBolt.transform.localScale = Vector3.one;
            pullBolt.SetActive(true);
        }
    }
}
