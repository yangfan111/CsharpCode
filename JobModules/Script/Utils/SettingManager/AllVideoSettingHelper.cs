using Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;
using ArtPlugins;

namespace Utils.SettingManager
{

    public class AllVideoSettingHelper :  Singleton<AllVideoSettingHelper>
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(AllVideoSettingHelper));

        public static AllVideoSettingHelper GetInstance()
        {
            return SingletonManager.Get<AllVideoSettingHelper>();
        }

        private Dictionary<int,float> qualityValueList;

        public void UpdateVideoSettingData(Dictionary<int, float> sendValList)
        {
            qualityValueList = sendValList;
            UpdateEffect();
            //updateEffect();
        }

        private void UpdateEffect()
        {
            foreach (var it in qualityValueList)
            {
                var id = (EVideoSettingId) it.Key;
                Action<EVideoSettingId> action;
                if(actionDict.TryGetValue(id, out action) && action != null)
                actionDict[id].Invoke(id);
            }
        }

        public AllVideoSettingHelper()
        {
            InitActionDict();
        }

        private void InitActionDict()
        {
            actionDict.Add(EVideoSettingId.ShadowDistance, UpdateShadowDistance);
            actionDict.Add(EVideoSettingId.ShadowLevel, UpdateShadowLevel);
            actionDict.Add(EVideoSettingId.ShadowQuality, UpdateShadowQuality);
            actionDict.Add(EVideoSettingId.MaxLodLevel, UpdateMaxLodLevel);
            actionDict.Add(EVideoSettingId.LodDistanceParameter, UpdateLodDistanceParameter);
            actionDict.Add(EVideoSettingId.SkeletalSkin, UpdateSkeletalSkin);
            actionDict.Add(EVideoSettingId.TextureFiltering, UpdateTextureFiltering);
            actionDict.Add(EVideoSettingId.TextureSize, UpdateTextureSize);
            actionDict.Add(EVideoSettingId.Opposite, UpdateOpposite);
            actionDict.Add(EVideoSettingId.SoftParticle, UpdateSoftParticle);
            actionDict.Add(EVideoSettingId.ParticleCollisionAccuracy, UpdateParticleCollisionAccuracy);
            actionDict.Add(EVideoSettingId.PixelLightSource, UpdatePixelLightSource);
            actionDict.Add(EVideoSettingId.ReflectiveProbe, UpdateReflectiveProbe);
            actionDict.Add(EVideoSettingId.VerticalSync, UpdateVerticalSynchronization);
            actionDict.Add(EVideoSettingId.AntiAliasing, UpdateAntiAliasing);
            actionDict.Add(EVideoSettingId.RealTimeReflection, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.AmbientLightShielding, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.Glare, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.FogEffect, UpdateFog);
            actionDict.Add(EVideoSettingId.AutomaticExposure, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.VolumetricLight, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.Halo, UpdateShaderDetail);
            actionDict.Add(EVideoSettingId.ColorCorrection, UpdateShaderDetail);
        }

        private void UpdateFog(EVideoSettingId id)
        {
            //雾效 
            RenderSettings.fog = GetBoolValueByEnum(id);
            _logger.InfoFormat("RenderSettings.fog = " + RenderSettings.fog);
        }

        private void UpdateShaderDetail(EVideoSettingId id)
        {
            var callback = VideoSettingRegister.GetCallbackByid(id);
            var val = GetFloatValueByEnum(id);
            if (callback != null)
            {
                callback.Invoke(val);
                _logger.InfoFormat("UpdateGQS_LightDetail_Camera :"+id+" =" + (int)val);
            }
        }

        private Dictionary<EVideoSettingId, Action<EVideoSettingId>> actionDict = new Dictionary<EVideoSettingId, Action<EVideoSettingId>>();


        private void UpdateShadowDistance(EVideoSettingId id)
        {
            //阴影距离 
            QualitySettings.shadowDistance = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.shadowDistance = " + QualitySettings.shadowDistance);
        }

        private void UpdateShadowLevel(EVideoSettingId id)
        {
            //阴影分级
            QualitySettings.shadowCascades = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.shadowCascades = " + QualitySettings.shadowCascades);
        }

        private void UpdateShadowQuality(EVideoSettingId id)
        {
            // 阴影质量
            switch (GetIntValueByEnum(id))
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
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.shadowResolution = ShadowResolution.Medium;
                    }
                    break;
                case 3:   //high
                    {
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.shadowResolution = ShadowResolution.High;
                    }
                    break;
                case 4:   //very high
                    {
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                    }
                    break;
            }
            _logger.InfoFormat("QualitySettings.shadows = " + QualitySettings.shadows);
            _logger.InfoFormat("QualitySettings.shadowResolution = " + QualitySettings.shadowResolution);
        }

        private void UpdateMaxLodLevel(EVideoSettingId id)
        {
            //LOD最大等级   
            QualitySettings.maximumLODLevel = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.maximumLODLevel = " + QualitySettings.maximumLODLevel);
        }

        private void UpdateLodDistanceParameter(EVideoSettingId id)
        {
            //LOD距离参数 
            QualitySettings.lodBias = GetFloatValueByEnum(id);
            _logger.InfoFormat("QualitySettings.lodBias = " + QualitySettings.lodBias);
        }

        private void UpdateSkeletalSkin(EVideoSettingId id)
        {
            //骨骼蒙皮 
            int blendWeights = GetIntValueByEnum(id);
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

        private void UpdateTextureFiltering(EVideoSettingId id)
        {
            // 纹理过滤
            switch (GetIntValueByEnum(id))
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

        private void UpdateTextureSize(EVideoSettingId id)
        {
            // 贴图纹理大小
            int textureLimit = GetIntValueByEnum(id);

            if (textureLimit == 0)  //一半
            {
                QualitySettings.masterTextureLimit = 1;
            }
            else if (textureLimit == 1)  //完整
            {
                QualitySettings.masterTextureLimit = 0;
            }
            _logger.InfoFormat("QualitySettings.masterTextureLimit = " + QualitySettings.masterTextureLimit);

        }


        private void UpdateOpposite(EVideoSettingId id)
        {
            QualitySettings.billboardsFaceCameraPosition = GetBoolValueByEnum(id);
            _logger.InfoFormat("QualitySettings.billboardsFaceCameraPosition = " + QualitySettings.billboardsFaceCameraPosition);

        }

        private void UpdateSoftParticle(EVideoSettingId id)
        {
            QualitySettings.softParticles = GetBoolValueByEnum(id);
            _logger.InfoFormat("QualitySettings.softParticles = " + QualitySettings.softParticles);

        }

        private void UpdateParticleCollisionAccuracy(EVideoSettingId id)
        {
            QualitySettings.particleRaycastBudget = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.particleRaycastBudget = " + QualitySettings.particleRaycastBudget);

        }

        private void UpdatePixelLightSource(EVideoSettingId id)
        {
            QualitySettings.pixelLightCount = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.pixelLightCount = " + QualitySettings.pixelLightCount);

        }

        private void UpdateReflectiveProbe(EVideoSettingId id)
        {
            QualitySettings.realtimeReflectionProbes = GetBoolValueByEnum(id);
            _logger.InfoFormat("QualitySettings.realtimeReflectionProbes = " + QualitySettings.realtimeReflectionProbes);

        }

        private void UpdateVerticalSynchronization(EVideoSettingId id)
        {
            QualitySettings.vSyncCount = GetIntValueByEnum(id);
            _logger.InfoFormat("QualitySettings.vSyncCount = " + QualitySettings.vSyncCount);

        }

        private void UpdateAntiAliasing(EVideoSettingId id)
        {
            var callback = VideoSettingRegister.GetCallbackByid(id);
            var val = GetFloatValueByEnum(id);
            if (callback != null)
            {
                callback.Invoke(val);

                _logger.InfoFormat("UpdateAntiAliasing :" + id + " =" + (int)val);
            }
        }

        private int GetIntValueByEnum(EVideoSettingId id)
        {
            return (int)GetFloatValueByEnum(id);
        }

        private float GetFloatValueByEnum(EVideoSettingId id)
        {
            return qualityValueList[(int)id];
        }

        private bool GetBoolValueByEnum(EVideoSettingId id)
        {
            return GetIntValueByEnum(id) != 0;
        }
    }
}
