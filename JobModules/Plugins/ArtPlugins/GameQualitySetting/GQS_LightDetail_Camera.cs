using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ArtPlugins
{
    public class GQS_LightDetail_Camera : GameQualitySettingBaseMB
    {
        private Flare sunFlare;

        private void Awake()
        {
            GameQualitySettingManager.maxQualityAction = (type) =>
            {
                SetMaxQialityByConsole(type);
            };

            GameQualitySettingManager.getMaxQualityStatue = ()=>
            {
                return isSet;
            };


            init(104);
        }
        [ContextMenu("test")]
        public override void updateEffect()
        {
            if (qualityValueList == null) return;
            PostProcessLayer ppl = GetComponent<PostProcessLayer>();
            var lb= FindObjectOfType<LB_LightingBoxHelper>();
            if (lb == null|| lb.mainLightingProfile==null) return;
            if (lb.mainLightingProfile.sunFlare != null) sunFlare = lb.mainLightingProfile.sunFlare;

            var ppv = FindObjectOfType<PostProcessVolume>();
          
            lb.mainLightingProfile.SSR_Enabled= (int)qualityValueList[1] != 0;
            lb.mainLightingProfile.AO_Enabled = (int)qualityValueList[2] != 0;
            lb.mainLightingProfile.sunFlare = (int)qualityValueList[3] != 0?sunFlare:null;
            
            lb.mainLightingProfile.Fog_Enabled = (int)qualityValueList[4] != 0;
            lb.mainLightingProfile.Bloom_Enabled = (int)qualityValueList[5]!=0;
            lb.mainLightingProfile.VL_Enabled = (int)qualityValueList[6]!=0;
            lb.mainLightingProfile.SunShaft_Enabled = (int)qualityValueList[7]!=0;
            BoolParameter boolParameter = new BoolParameter();
            ColorGrading cg = ppl.GetSettings<ColorGrading>();
            if (cg != null)
            {
                boolParameter.value = (int)qualityValueList[8] != 0;
                cg.enabled = boolParameter; 
            }
       var cmr = GetComponent<Camera>();
           
            //switch ((int)qualityValueList[9])
            //{
            //    case 0:

            //        lb.Update_AA(cmr, AAMode.FXAA, false);
            //        lb.Update_AA(cmr, AAMode.TAA, false);
            //        lb.Update_AA(cmr, AAMode.SMAA, false);
            //        break;

            //    case 1:

            //        lb.Update_AA(cmr, AAMode.FXAA, true);
            //        break;
            //    case 2:
            //        lb.Update_AA(cmr, AAMode.SMAA, true);
            //        break;
            //    case 3:

            //        lb.Update_AA(cmr, AAMode.TAA, true);
 
            //         break;

            //}


            lb.SendMessage("Start");
  


        }


        #region maxQuality
        bool isSet = false;
        int level = 0;
        int pixelLightCount = 0;
        int masterTextureLimit = 0;
        int anisotropicFiltering = 0;
        int antiAliasing = 0;
        bool softParticles = false;
        bool realtimeReflectionProbes = false;
        bool billboardsFaceCameraPosition = false;

        ShadowQuality shadows = 0;
        int shadowResolution = 0;
        int shadowProjection = 0;
        float shadowDistance = 0;
        float shadowNearPlaneOffset = 0;
        int shadowCascades = 0;


        bool VL_Enabled = false;
        bool SunShaft_Enabled = false;
        bool Fog_Enabled = false;
        bool Bloom_Enabled = false;
        bool AA_Enabled = false;
        bool AO_Enabled = false;
        bool Vignette_Enabled = false;
        bool SSR_Enabled = false;

        private string SetMaxQialityByConsole(string type)
        {
            var lb = FindObjectOfType<LB_LightingBoxHelper>();
            if (type == "yes")
            {
                if (isSet == false)
                {
                    //Render
                    level = QualitySettings.GetQualityLevel();
                    QualitySettings.SetQualityLevel(0, true);

                    pixelLightCount = QualitySettings.pixelLightCount;
                    QualitySettings.pixelLightCount = 4;

                    masterTextureLimit = QualitySettings.masterTextureLimit; 
                    QualitySettings.masterTextureLimit = 0;

                    anisotropicFiltering = (int)QualitySettings.anisotropicFiltering;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

                    antiAliasing = QualitySettings.antiAliasing;
                    QualitySettings.antiAliasing = 8;

                    softParticles = QualitySettings.softParticles; 
                    QualitySettings.softParticles = true;

                    realtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
                    QualitySettings.realtimeReflectionProbes = true;

                    billboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition;
                    QualitySettings.billboardsFaceCameraPosition = true;

                    //shadow
                    shadows = QualitySettings.shadows;
                    QualitySettings.shadows = ShadowQuality.All;

                    shadowResolution = (int)QualitySettings.shadowResolution;
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;

                    shadowProjection = (int)QualitySettings.shadowProjection;
                    QualitySettings.shadowProjection = ShadowProjection.StableFit;

                    shadowDistance = QualitySettings.shadowDistance;
                    QualitySettings.shadowDistance = 1000;

                    shadowNearPlaneOffset = QualitySettings.shadowNearPlaneOffset;
                    QualitySettings.shadowNearPlaneOffset = 1;

                    shadowCascades = QualitySettings.shadowCascades;
                    QualitySettings.shadowCascades = 4;

                    if (lb != null || lb.mainLightingProfile != null)
                    {
                        //…Ë÷√µ∆π‚≤Âº˛
                        VL_Enabled = lb.mainLightingProfile.VL_Enabled;
                        lb.mainLightingProfile.VL_Enabled = true;

                        SunShaft_Enabled = lb.mainLightingProfile.SunShaft_Enabled;
                        lb.mainLightingProfile.SunShaft_Enabled = true;

                        Fog_Enabled = lb.mainLightingProfile.Fog_Enabled;
                        lb.mainLightingProfile.Fog_Enabled = true;

                        Bloom_Enabled = lb.mainLightingProfile.Bloom_Enabled;
                        lb.mainLightingProfile.Bloom_Enabled = true;

                        AA_Enabled = lb.mainLightingProfile.AA_Enabled;
                        lb.mainLightingProfile.AA_Enabled = true;

                        AO_Enabled = lb.mainLightingProfile.AO_Enabled;
                        lb.mainLightingProfile.AO_Enabled = true;

                        Vignette_Enabled = lb.mainLightingProfile.Vignette_Enabled;
                        lb.mainLightingProfile.Vignette_Enabled = true;

                        SSR_Enabled = lb.mainLightingProfile.SSR_Enabled;
                        lb.mainLightingProfile.SSR_Enabled = true;
                    }

                    isSet = true;
                }
            }
            else if (type == "no")
            {
                if (isSet)
                {
                    QualitySettings.SetQualityLevel(level, true);
                    QualitySettings.pixelLightCount = pixelLightCount;
                    QualitySettings.masterTextureLimit = masterTextureLimit;
                    QualitySettings.anisotropicFiltering = (AnisotropicFiltering)anisotropicFiltering;
                    QualitySettings.antiAliasing = antiAliasing;
                    QualitySettings.softParticles = softParticles;
                    QualitySettings.realtimeReflectionProbes = realtimeReflectionProbes;
                    QualitySettings.billboardsFaceCameraPosition = billboardsFaceCameraPosition;

                    QualitySettings.shadows = shadows;
                    QualitySettings.shadowResolution = (ShadowResolution)shadowResolution;
                    QualitySettings.shadowProjection = (ShadowProjection)shadowProjection;
                    QualitySettings.shadowDistance = shadowDistance;
                    QualitySettings.shadowNearPlaneOffset = shadowNearPlaneOffset;
                    QualitySettings.shadowCascades = shadowCascades;

                    if (lb != null || lb.mainLightingProfile != null)
                    {
                        lb.mainLightingProfile.VL_Enabled = VL_Enabled;
                        lb.mainLightingProfile.SunShaft_Enabled = SunShaft_Enabled;
                        lb.mainLightingProfile.Fog_Enabled = Fog_Enabled;
                        lb.mainLightingProfile.Bloom_Enabled = Bloom_Enabled;
                        lb.mainLightingProfile.AA_Enabled = AA_Enabled;
                        lb.mainLightingProfile.AO_Enabled = AO_Enabled;
                        lb.mainLightingProfile.Vignette_Enabled = Vignette_Enabled;
                        lb.mainLightingProfile.SSR_Enabled = SSR_Enabled;
                    }

                    isSet = false;
                }
            }
            return "Success SetMaxQialityByConsole";
        }
        #endregion
    }

}
 