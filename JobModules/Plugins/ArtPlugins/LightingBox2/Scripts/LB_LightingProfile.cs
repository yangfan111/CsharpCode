
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using cCharkes;

[CreateAssetMenu(fileName = "Lighting Data", menuName = "Lighting Profile", order = 1)]
public class LB_LightingProfile : ScriptableObject {

	[Header("Camera")]
	public string mainCameraName = "Main Camera";

	public string objectName = "LB_LightingProfile";
	[Header("Profiles")]
	public PostProcessProfile postProcessingProfile;
	public bool webGL_Mobile = false;

	[Header("Global")]
	public Render_Path renderPath = Render_Path.Default;
	public  LightingMode lightingMode = LightingMode.RealtimeGI;
	public  float bakedResolution = 10f;
	public  LightSettings lightSettings = LightSettings.Mixed;
	public MyColorSpace colorSpace = MyColorSpace.Linear;

	[Header("Environment")]
	public Material skyBox;
	public  AmbientLight ambientLight = AmbientLight.Color;
	public  Color ambientColor = new Color32(96,104,116,255);
	public Color skyColor = Color.white;
	public Color equatorColor = Color.white,groundColor = Color.white;

	[Header("Sun")]
	public  Color sunColor = Color.white;
	public float sunIntensity = 2.1f;
	public Flare sunFlare;
	public float indirectIntensity = 1.43f;

	[Header("Fog")]
	public CustomFog fogMode = CustomFog.Global;
	public float fogDistance = 0;
	public float fogHeight = 10f;
	public float fogHeightIntensity = 0.5f;
	public Color fogColor = Color.white;
	public float fogIntensity;

	[Header("Bloom")]
	public float bIntensity = 0.73f;
	public float bThreshould = 0.5f;
	public Color bColor = Color.white;
	public Texture2D dirtTexture;
	public float dirtIntensity;
	public bool mobileOptimizedBloom = false;
	public float bRotation;

	[Header("AO")]
	public AOType aoType = AOType.Modern;
	public float aoRadius = 0.3f;
	public float aoIntensity = 1f;
	public bool ambientOnly = false;
	public Color aoColor = Color.black;
	public AmbientOcclusionQuality aoQuality = AmbientOcclusionQuality.Medium;

	[Header("Other")]
	public VolumetricLightType vLight = VolumetricLightType.OnlyDirectional;
	public VLightLevel vLightLevel = VLightLevel.Level3;
	public LightsShadow lightsShadow = LightsShadow.OnlyDirectionalSoft;
	public LightProbeMode lightProbesMode;
	public bool automaticLightmap = false;

	[Header("Depth of Field Legacy")]
	public float dofRange;
	public float dofBlur;   
	public float falloff = 30f;
	public DOFQuality dofQuality;
	public bool visualize;

	// Auto Focus
	public float afRange = 100f;
	public float afBlur = 30f;
	public float afSpeed = 100f;
	public float afUpdate = 0.001f;
	public float afRayLength = 10f;
	public LayerMask afLayer = 1;

	[Header("Color settings")]
	public float exposureIntensity = 1.43f;
	public float contrastValue = 30f;
	public float temp = 0;
	public ColorMode colorMode = ColorMode.ACES;
	public float saturation = 0;
	public float gamma = 0;
	public Color colorGamma = Color.black;
	public Color colorLift = Color.black;
	public Texture lut;

	[Header("Effects")]
	public float vignetteIntensity = 0.1f;
	public float CA_Intensity = 0.1f;
	public bool mobileOptimizedChromattic = false;

	[Header("Unity SSR")]
	public ScreenSpaceReflectionPreset ssrQuality = ScreenSpaceReflectionPreset.Lower;
	public float ssrAtten = 0;
	public float ssrFade = 0;

	[Header("Stochastic SSR")]
	public ResolutionMode resolutionMode = ResolutionMode.halfRes;
	public SSRDebugPass debugPass = SSRDebugPass.Combine;
	public int rayDistance = 70;
	public float screenFadeSize = 0;
	public float smoothnessRange = 1f;

	[Header("Sun Shaft")]
	public UnityStandardAssets.ImageEffects.SunShafts.SunShaftsResolution shaftQuality = UnityStandardAssets.ImageEffects.SunShafts.SunShaftsResolution.High;
	public float shaftDistance = 0.5f;
	public float shaftBlur = 4f;
	public Color shaftColor = new Color32 (255,189,146,255);

	[Range(0,1f)]
	public float eyeKeyValue  =   0.17f;   

	[Header("AA")]
	public AAMode aaMode;

	[Header("Foliage")]
	public float translucency;
	public float ambient;
	public float shadows;
	public Color tranColor;
	public float windSpeed;
	public float windScale;
	public string CustomShader = "Legacy Shaders/Transparent/Diffuse";

	[Header("Snow")]
	public Texture2D snowAlbedo;
	public Texture2D snowNormal;
	public float snowIntensity = 0;
	public string customShaderSnow = "Legacy Shaders/Diffuse";

	[Header("Material Converter")]
	public MatType matType;

	[Header("Enabled Options")]
	public bool Ambient_Enabled = true;
	public bool Scene_Enabled = true;
	public bool Sun_Enabled = true;
	public bool VL_Enabled = false;
	public bool SunShaft_Enabled = false;
	public bool Fog_Enabled = false;
	public bool AutoFocus_Enabled = false;
	public bool DOF_Enabled = true;
	public bool Bloom_Enabled = false;
	public bool AA_Enabled = true;
	public bool AO_Enabled = false;
	public bool MotionBlur_Enabled = true;
	public bool Vignette_Enabled = true;
	public bool Chromattic_Enabled = true;
	public bool SSR_Enabled = false;
	public bool ST_SSR_Enabled = false;

	public bool ambientState = false;
	public bool sunState = false;
	public bool lightSettingsState = false;
	public bool cameraState = false;
	public bool profileState = false;
	public bool buildState = false;
	public bool vLightState = false;
	public bool sunShaftState = false;
	public bool fogState = false;
	public bool dofState = false;
	public bool autoFocusState =  false;
	public bool colorState = false;
	public bool bloomState = false;
	public bool aaState = false;
	public bool aoState = false;
	public bool motionBlurState = false;
	public bool vignetteState = false;
	public bool chromatticState = false;
	public bool ssrState;
	public bool st_ssrState;
	public bool foliageState = false,snowState = false;
	public bool OptionsState = true;
	public bool LightingBoxState = true;
}