using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArtPlugins
{
    partial class GameQualitySettingManager
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(GameQualitySettingManager));


        public static void Init()
        {
            RegSettingChangeCallback(EVideoSettingId.FogEffect, UpdateFog,()=> { return RenderSettings.fog.ToString(); });
            //RegSettingChangeCallback(EVideoSettingId.ShadowDistance, UpdateShadowDistance, () => { return QualitySettings.shadowDistance.ToString(); });
            //RegSettingChangeCallback(EVideoSettingId.ShadowLevel, UpdateShadowLevel, () => { return QualitySettings.shadowCascades.ToString(); });
            //RegSettingChangeCallback(EVideoSettingId.ShadowQuality, UpdateShadowQuality, () => { return "shadows " + QualitySettings.shadows + " shadowResolution " + QualitySettings.shadowCascades; });
            RegSettingChangeCallback(EVideoSettingId.MaxLodLevel, UpdateMaxLodLevel, () => { return QualitySettings.maximumLODLevel.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.LodDistanceParameter, UpdateLodDistanceParameter, () => { return QualitySettings.lodBias.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.SkeletalSkin, UpdateSkeletalSkin, () => { return QualitySettings.blendWeights.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.TextureFiltering, UpdateTextureFiltering, () => { return QualitySettings.anisotropicFiltering.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.TextureSize, UpdateTextureSize, () => { return QualitySettings.masterTextureLimit.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.Opposite, UpdateOpposite, () => { return QualitySettings.billboardsFaceCameraPosition.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.SoftParticle, UpdateSoftParticle, () => { return QualitySettings.softParticles.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.ParticleCollisionAccuracy, UpdateParticleCollisionAccuracy, () => { return QualitySettings.particleRaycastBudget.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.PixelLightSource, UpdatePixelLightSource, () => { return QualitySettings.pixelLightCount.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.ReflectiveProbe, UpdateReflectiveProbe, () => { return QualitySettings.realtimeReflectionProbes.ToString(); });
            RegSettingChangeCallback(EVideoSettingId.VerticalSync, UpdateVerticalSynchronization, () => { return QualitySettings.vSyncCount.ToString(); });
        }



        private static void UpdateFog(float value)
        {
            //雾效 
            RenderSettings.fog = GetBoolValue(value);
            _logger.InfoFormat("RenderSettings.fog = " + RenderSettings.fog);
        }

        private static void UpdateShadowDistance( float value)
        {
            //阴影距离 
            QualitySettings.shadowDistance = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.shadowDistance = " + QualitySettings.shadowDistance);
        }

        private static void UpdateShadowLevel(float value)
        {
            //阴影分级
            QualitySettings.shadowCascades = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.shadowCascades = " + QualitySettings.shadowCascades);
        }

        private static void UpdateShadowQuality( float value)
        {
            // 阴影质量
            switch (GetIntlValue(value))
            {
                case 0:   //关闭阴影
                    {
                        QualitySettings.shadows = ShadowQuality.Disable;
                    }
                    break;
                case 1:   //low
                    {
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                    }
                    break;
                case 2:   //medium
                    {
                        QualitySettings.shadows = ShadowQuality.All;
                        QualitySettings.shadowResolution = ShadowResolution.Medium;
                    }
                    break;
                case 3:   //high
                    {
                        QualitySettings.shadows = ShadowQuality.All;
                        QualitySettings.shadowResolution = ShadowResolution.High;
                    }
                    break;
                case 4:   //very high
                    {
                        QualitySettings.shadows = ShadowQuality.All;
                        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    }
                    break;
            }
            _logger.InfoFormat("QualitySettings.shadows = " + QualitySettings.shadows);
            _logger.InfoFormat("QualitySettings.shadowResolution = " + QualitySettings.shadowResolution);
        }

        private static void UpdateMaxLodLevel(float value)
        {
            //LOD最大等级   
            QualitySettings.maximumLODLevel = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.maximumLODLevel = " + QualitySettings.maximumLODLevel);
        }

        private static void UpdateLodDistanceParameter(float value)
        {
            //LOD距离参数 
            QualitySettings.lodBias = value;
            _logger.InfoFormat("QualitySettings.lodBias = " + QualitySettings.lodBias);
        }

        private static void UpdateSkeletalSkin( float value)
        {
            //骨骼蒙皮 
            int blendWeights = GetIntlValue(value);
            if (blendWeights == 2)
            {
                QualitySettings.blendWeights = BlendWeights.TwoBones;
            }
            else if (blendWeights == 4)
            {
                QualitySettings.blendWeights = BlendWeights.FourBones;
            }
            _logger.InfoFormat("QualitySettings.blendWeights = " + QualitySettings.blendWeights);

        }

        private static void UpdateTextureFiltering( float value)
        {
            // 纹理过滤
            switch (GetIntlValue(value))
            {
                case 0:    //关
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    }
                    break;
                case 1:    //PerTex
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    }
                    break;
                case 2:    //Force
                    {
                        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    }
                    break;
            }
            _logger.InfoFormat("QualitySettings.anisotropicFiltering = " + QualitySettings.anisotropicFiltering);

        }

        private static void UpdateTextureSize( float value)
        {
            float textureLimit = value;
            QualitySettings.masterTextureLimit = (int)Mathf.Log((1f / textureLimit), 2f);
            _logger.InfoFormat("QualitySettings.masterTextureLimit = " + QualitySettings.masterTextureLimit);

        }


        private static void UpdateOpposite( float value)
        {
            QualitySettings.billboardsFaceCameraPosition = GetBoolValue(value);
            _logger.InfoFormat("QualitySettings.billboardsFaceCameraPosition = " + QualitySettings.billboardsFaceCameraPosition);

        }

        private static void UpdateSoftParticle( float value)
        {
            QualitySettings.softParticles = GetBoolValue(value);
            _logger.InfoFormat("QualitySettings.softParticles = " + QualitySettings.softParticles);

        }

        private static void UpdateParticleCollisionAccuracy( float value)
        {
            QualitySettings.particleRaycastBudget = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.particleRaycastBudget = " + QualitySettings.particleRaycastBudget);

        }

        private static void UpdatePixelLightSource( float value)
        {
            QualitySettings.pixelLightCount = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.pixelLightCount = " + QualitySettings.pixelLightCount);

        }

        private static void UpdateReflectiveProbe( float value)
        {
            QualitySettings.realtimeReflectionProbes = GetBoolValue(value);
            _logger.InfoFormat("QualitySettings.realtimeReflectionProbes = " + QualitySettings.realtimeReflectionProbes);

        }

        private static void UpdateVerticalSynchronization( float value)
        {
            QualitySettings.vSyncCount = GetIntlValue(value);
            _logger.InfoFormat("QualitySettings.vSyncCount = " + QualitySettings.vSyncCount);

        }

        private static int GetIntlValue(float id)
        {
            return (int)id;
        }

        private static bool GetBoolValue(float id)
        {
            return (int)id != 0;
        }
    }
}
