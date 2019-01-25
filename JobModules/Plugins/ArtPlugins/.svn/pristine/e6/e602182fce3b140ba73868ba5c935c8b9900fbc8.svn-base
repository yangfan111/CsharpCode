using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class LB_SettingsMenu : MonoBehaviour {

	#region Variables
	// Access unity post effects
	PostProcessLayer[] pb;
	PostProcessVolume gVolume;

	[Header("Effects")]
	// Settings ui elements
	public Dropdown antiAliasing;
	public Dropdown ambientOcclusion;
	public Dropdown screenSpaceReflections;
	public Dropdown stSSR;
	public Dropdown depthOfField;
	public Dropdown motionBlur;
	public Dropdown bloom;
	public Dropdown chromaticAberration;
	public Dropdown vignette;

	[Header("Quality")]
	// Quality ui elements
	public Dropdown grass;
	public Dropdown terrain;
	public Dropdown textureResolution;
	public Dropdown textureAnistropic;
	public Dropdown shadows;
	public Dropdown realtimeReflections;
	public Dropdown softParticles;

	[Header("Display")]
	// Display ui elements
	public Dropdown targetFPS;
	public Dropdown vSync;
	public Dropdown fullScreen;

	// Device info
	public Text deviceInfo;


	// private Variables;
	Bloom vBloom;
	Vignette vVignette;
	ChromaticAberration vCA;
	MotionBlur vMB;
	AmbientOcclusion vAO;
	ScreenSpaceReflections ssr;

	Terrain[] t;
	#endregion

	void Start ()
	{

		#region Init
		t = GameObject.FindObjectsOfType<Terrain> ();

		if (!GameObject.Find ("Global Volume")) 
		{
			Debug.Log("Settings menu : Couldn't find global volume game object");
			Destroy (this);
		}

		// Load default game settings on first ruuning of game   
		if (PlayerPrefs.GetInt ("FirstRun") != 3) 
		{ // 3=>true , 0=>false
			// Quality
			PlayerPrefs.SetString ("Grass","Medium");
			PlayerPrefs.SetString ("Terrain","Medium");
			PlayerPrefs.SetString ("TextureResolution","High");
			PlayerPrefs.SetString ("TextureAnistropic","Enable");
			PlayerPrefs.SetString ("Shadows","Medium");
			PlayerPrefs.SetString ("Reflections","On");
			PlayerPrefs.SetString ("SoftParticles","On");

			// Display 
			PlayerPrefs.SetString ("FPS","60");
			PlayerPrefs.SetString ("VSync","On");
			PlayerPrefs.SetString ("FullScreen","On");

			// Effects
			PlayerPrefs.SetString ("AA","TAA");
			PlayerPrefs.SetString ("AO","Classic High");
			PlayerPrefs.SetString ("SSR","Off");
			PlayerPrefs.SetString ("Bloom","On");
			PlayerPrefs.SetString ("Vignette","On");
			PlayerPrefs.SetString ("ChromaticAberration","On");
			PlayerPrefs.SetString ("MotionBlur","On");
			PlayerPrefs.SetString ("DOF","On");

			PlayerPrefs.SetInt ("FirstRun",3);
		}

		deviceInfo.text = SystemInfo.graphicsDeviceName.ToString ();

		pb = GameObject.FindObjectsOfType<PostProcessLayer> ();
		gVolume = GameObject.Find ("Global Volume").GetComponent<PostProcessVolume> ();

		gVolume.profile.TryGetSettings(out vBloom);
		gVolume.profile.TryGetSettings(out vVignette);
		gVolume.profile.TryGetSettings(out vCA);
		gVolume.profile.TryGetSettings(out vMB);
		gVolume.profile.TryGetSettings(out vAO);
		gVolume.profile.TryGetSettings(out ssr);
		#endregion

		#region AA
		//  Effects  Start//////////////////////////////////////////////////////////////////////////////////////////
		// Load default settings into UI component elements
		for (int a = 0; a < pb.Length; a++) 
		{

			// AA settings
			if (PlayerPrefs.GetString ("AA") == "Off") {
				antiAliasing.value = 0;
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.None;
				pb [a].Init (null);
			}
			if (PlayerPrefs.GetString ("AA") == "FXAA") {
				antiAliasing.value = 1;
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				pb [a].Init (null);
			}
			if (PlayerPrefs.GetString ("AA") == "Sub") {
				antiAliasing.value = 2;
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				pb [a].Init (null);
			}
			if (PlayerPrefs.GetString ("AA") == "TAA") {
				antiAliasing.value = 3;
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				pb [a].Init (null);
			}
		}
		#endregion

		#region AO
		// AO settings
		if (PlayerPrefs.GetString ("AO") == "Off") {
			ambientOcclusion.value = 0;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("AO") == "Classic Low") {
			ambientOcclusion.value = 1;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Low;
		}
		if (PlayerPrefs.GetString ("AO") == "Classic Medium") {
			ambientOcclusion.value = 2;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Medium;
		}
		if (PlayerPrefs.GetString ("AO") == "Classic High") {
			ambientOcclusion.value = 3;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.High;
		}
		if (PlayerPrefs.GetString ("AO") == "Classic Ultra") {
			ambientOcclusion.value = 4;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Ultra;
		}
		if (PlayerPrefs.GetString ("AO") == "Modern") {
			ambientOcclusion.value = 5;
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance;
		}
		#endregion	

		#region SSR
		if (PlayerPrefs.GetString ("SSR") == "Off") 
		{
			screenSpaceReflections.value = 0;
			ssr.enabled.overrideState = true;
			ssr.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("SSR") == "Lower")
		{
			screenSpaceReflections.value = 1;
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Lower;
			ssr.enabled.value = true;
		}
		if (PlayerPrefs.GetString ("SSR") == "Low")
		{
			screenSpaceReflections.value = 2;
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Low;
			ssr.enabled.value = true;
		}
		if (PlayerPrefs.GetString ("SSR") == "Medium")
		{
			screenSpaceReflections.value = 3;
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Medium;
			ssr.enabled.value = true;
		}
		if (PlayerPrefs.GetString ("SSR") == "High")
		{
			screenSpaceReflections.value = 4;
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.High;
			ssr.enabled.value = true;
		}
		if (PlayerPrefs.GetString ("SSR") == "Ultra")
		{
			screenSpaceReflections.value = 5;
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Ultra;
			ssr.enabled.value = true;
		}
		#endregion

		#region DOF
		// DOF settings
		if (PlayerPrefs.GetString ("DOF") == "Off")
		{
			depthOfField.value = 0;
			if(Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ())
				Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ().enabled = false;
		}
		if (PlayerPrefs.GetString ("DOF") == "On")
		{
			depthOfField.value = 1;
			if(Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ())
				Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ().enabled = true;
		}
		#endregion


		#region Stochastic SSR
		// DOF settings
		if (PlayerPrefs.GetString ("stSSR") == "Off")
		{
			stSSR.value = 0;
			if(Camera.main.GetComponent<cCharkes.StochasticSSR> ())
				Camera.main.GetComponent<cCharkes.StochasticSSR> ().enabled = false;
		}
		if (PlayerPrefs.GetString ("stSSR") == "On")
		{
			stSSR.value = 1;
			if(Camera.main.GetComponent<cCharkes.StochasticSSR> ())
				Camera.main.GetComponent<cCharkes.StochasticSSR> ().enabled = true;
		}
		#endregion

		#region Motion Blur
		// Motion Blur settings
		if (PlayerPrefs.GetString ("MotionBlur") == "Off")
		{
			motionBlur.value = 0;
			vMB.enabled.overrideState = true;
			vMB.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("MotionBlur") == "On")
		{
			motionBlur.value = 1;
			vMB.enabled.overrideState = true;
			vMB.enabled.value = true;
		}
		#endregion

		#region Bloom
		// Bloom settings
		if (PlayerPrefs.GetString ("Bloom") == "Off") {
			bloom.value = 0;
			vBloom.enabled.overrideState = true;
			vBloom.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("Bloom") == "On") {
			bloom.value = 1;
			vBloom.enabled.overrideState = true;
			vBloom.enabled.value = true;
		}
		#endregion

		#region ChromaticAberration
			// Chromattic Abberation settings
		if (PlayerPrefs.GetString ("ChromaticAberration") == "Off") {
			chromaticAberration.value = 0;
			vCA.enabled.overrideState = true;
			vCA.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("ChromaticAberration") == "On") {
			chromaticAberration.value = 1;
			vCA.enabled.overrideState = true;
			vCA.enabled.value = true;
		}
		#endregion

		#region Vignette
		// Vignette settings
		if (PlayerPrefs.GetString ("Vignette") == "Off") {
			vignette.value = 0;
			vVignette.enabled.overrideState = true;
			vVignette.enabled.value = false;
		}
		if (PlayerPrefs.GetString ("Vignette") == "On") {
			vignette.value = 1;
			vVignette.enabled.overrideState = true;
			vVignette.enabled.value = true;
		}
		#endregion

		// Quality
		for (int a = 0; a < t.Length; a++) 
		{
			#region Grass
			// Grass
			if (PlayerPrefs.GetString ("Grass") == "Low") 
			{
				t [a].detailObjectDensity = 0.3f;
				t [a].detailObjectDistance = 90f;
				grass.value = 0;
			}
					
			if (PlayerPrefs.GetString ("Grass") == "Medium") 
			{
				t [a].detailObjectDensity = 0.5f;
				t [a].detailObjectDistance = 140f;
				grass.value = 1;
			}
					
			if (PlayerPrefs.GetString ("Grass") == "High") 
			{
				t [a].detailObjectDensity = 1f;
				t [a].detailObjectDistance = 243f;
				grass.value = 2;
			}
			#endregion

			#region Terrain
			// Terrain
			if (PlayerPrefs.GetString ("Terrain") == "Low") 
			{
				t [a].heightmapPixelError = 170f;
				terrain.value = 0;
			}
			if (PlayerPrefs.GetString ("Terrain") == "Medium") 
			{
				t [a].heightmapPixelError = 73f;
				terrain.value = 1;
			}
			if (PlayerPrefs.GetString ("Terrain") == "High") 
			{
				t [a].heightmapPixelError = 5f;
				terrain.value = 2;
			}
			#endregion

		}

		#region Texture Resolution
		// Texture Resolution   
		if (PlayerPrefs.GetString ("TextureResolution") == "Low")
		{
			QualitySettings.masterTextureLimit = 2;
			textureResolution.value = 0;
		}
		if (PlayerPrefs.GetString ("TextureResolution") == "Medium")
		{
			QualitySettings.masterTextureLimit = 1;
			textureResolution.value = 1;
		}
		if (PlayerPrefs.GetString ("TextureResolution") == "High") 
		{
			QualitySettings.masterTextureLimit = 0;
			textureResolution.value = 2;
		}
		#endregion

		#region Texture Anistropic
		// Texture Anistropic   
		if (PlayerPrefs.GetString ("TextureAnistropic") == "Disable") 
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			textureAnistropic.value = 0;
		}
		if (PlayerPrefs.GetString ("TextureAnistropic") == "Enable") 
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
			textureAnistropic.value = 1;
		}
		if (PlayerPrefs.GetString ("TextureAnistropic") == "ForceEnable") 
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;

			textureAnistropic.value = 2;
		}
		#endregion

		#region Shadows
		// Shadows
		if (PlayerPrefs.GetString ("Shadows") == "Low") {
			QualitySettings.shadowResolution = ShadowResolution.Low;
			shadows.value = 0;
		}
		if (PlayerPrefs.GetString ("Shadows") == "Medium") {
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			shadows.value = 1;
		}
		if (PlayerPrefs.GetString ("Shadows") == "High") {
			QualitySettings.shadowResolution = ShadowResolution.High;
			shadows.value = 2;
		}
		if (PlayerPrefs.GetString ("Shadows") == "VeryHigh") {
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			shadows.value = 3;
		}
		#endregion

		#region Reflections
		// Reflections
		if (PlayerPrefs.GetString ("Reflections") == "Off") {
			QualitySettings.realtimeReflectionProbes = false;
			realtimeReflections.value = 0;
		}
		if (PlayerPrefs.GetString ("Reflections") == "On") {
			QualitySettings.realtimeReflectionProbes = true;
			realtimeReflections.value = 1;
		}
		#endregion

		#region Particles
		// Soft Parricles
		if (PlayerPrefs.GetString ("SoftParticles") == "Off") {
			QualitySettings.softParticles = false;
			softParticles.value = 0;
		}
		if (PlayerPrefs.GetString ("SoftParticles") == "On") {
			QualitySettings.softParticles = true;
			softParticles.value = 1;
		}
		#endregion


		//  Display 

		#region Target FPS
		// Traget fps
		if (PlayerPrefs.GetString ("FPS") == "30") {
			Application.targetFrameRate = 30;
			targetFPS.value = 0;
		}
		if (PlayerPrefs.GetString ("FPS") == "60") {
			Application.targetFrameRate = 60;
			targetFPS.value = 1;
		}
		#endregion

		#region VSync
		// Vsync
		if (PlayerPrefs.GetString ("VSync") == "On") {
			QualitySettings.vSyncCount = 1;
			vSync.value = 0;
		}
		if (PlayerPrefs.GetString ("VSync") == "Off") {
			QualitySettings.vSyncCount = 0;
			vSync.value = 1;
		}
		#endregion

		#region FullScreen
		//  Fullscreen
		if (PlayerPrefs.GetString ("FullScreen") == "On") {
			Screen.fullScreen = true;
			fullScreen.value = 0;
		}
		if (PlayerPrefs.GetString ("FullScreen") == "Off") {
			Screen.fullScreen = false;
			fullScreen.value = 1;
		}

		#endregion

		/////////////////////////////////////////

	}

	#region AA
	/////////////////////////////////////////////////////////////////////////////////1
	// AntiAliasing
	public void Change_AntiAliasing()
	{
		StartCoroutine ("save_AntiAliasing");
	}

	IEnumerator save_AntiAliasing()
	{
		
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < pb.Length; a++)
		{
			if (antiAliasing.value == 0) 
			{
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.None;
				pb [a].Init (null);
				PlayerPrefs.SetString ("AA", "Off");
			}		
			if (antiAliasing.value == 1) 
			{				
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				pb [a].Init (null);
				PlayerPrefs.SetString ("AA", "FXAA");
			}
			if (antiAliasing.value == 2) 
			{
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				pb [a].Init (null);
				PlayerPrefs.SetString ("AA", "Sub");
			}
			if (antiAliasing.value == 3) 
			{
				pb [a].antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				pb [a].Init (null);
				PlayerPrefs.SetString ("AA", "TAA");
			}
		}
			
	}
	#endregion

	#region AO
	/////////////////////////////////////////////////////////////////////////////////2
	/// 
	/// // AmbientOcclusion
	public void Change_AmbientOcclusion()
	{
		StartCoroutine ("save_AmbientOcclusion");
	}

	IEnumerator save_AmbientOcclusion()
	{
		yield return new WaitForEndOfFrame ();

		if (ambientOcclusion.value == 0) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = false;
			PlayerPrefs.SetString ("AO", "Off");
		}
		if (ambientOcclusion.value == 1) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Low;
			PlayerPrefs.SetString ("AO", "Classic Low");
		}
		if (ambientOcclusion.value == 2) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Medium;
			PlayerPrefs.SetString ("AO", "Classic Medium");
		}
		if (ambientOcclusion.value == 3) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.High;
			PlayerPrefs.SetString ("AO", "Classic High");
		}
		if (ambientOcclusion.value == 4) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
			vAO.quality.overrideState = true;
			vAO.quality.value = AmbientOcclusionQuality.Ultra;
			PlayerPrefs.SetString ("AO", "Classic Ultra");
		}
		if (ambientOcclusion.value == 5) {
			vAO.enabled.overrideState = true;
			vAO.enabled.value = true;
			vAO.mode.overrideState = true;
			vAO.mode.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance;
			PlayerPrefs.SetString ("AO", "Modern");
		}
	}
	#endregion

	#region SSR
	/////////////////////////////////////////////////////////////////////////////////3
	/// /// // Screen Space Reflections
	public void Change_ScreenSpaceReflections()
	{
		StartCoroutine ("save_ScreenSpaceReflections");
	}

	IEnumerator save_ScreenSpaceReflections()
	{
		yield return new WaitForEndOfFrame ();

		if (screenSpaceReflections.value == 0) 
		{
			ssr.enabled.overrideState = true;
			ssr.enabled.value = false;
			PlayerPrefs.SetString ("SSR", "Off");
		}
		if (screenSpaceReflections.value == 1) 
		{
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Lower;
			ssr.enabled.value = true;
			PlayerPrefs.SetString ("SSR", "Lower");
		}
		if (screenSpaceReflections.value == 2) 
		{
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Low;
			ssr.enabled.value = true;
			PlayerPrefs.SetString ("SSR", "Low");
		}
		if (screenSpaceReflections.value == 3) 
		{
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Medium;
			ssr.enabled.value = true;
			PlayerPrefs.SetString ("SSR", "Medium");
		}
		if (screenSpaceReflections.value == 4) 
		{
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.High;
			ssr.enabled.value = true;
			PlayerPrefs.SetString ("SSR", "High");
		}
		if (screenSpaceReflections.value == 5) 
		{
			ssr.enabled.overrideState = true;
			ssr.preset.overrideState = true;   
			ssr.preset.value = ScreenSpaceReflectionPreset.Ultra;
			ssr.enabled.value = true;
			PlayerPrefs.SetString ("SSR", "Ultra");
		}
		
	}
	#endregion

	#region DOF
	/////////////////////////////////////////////////////////////////////////////////4
	/// /// // Depth Of Field
	public void Change_DepthOfField()
	{
		StartCoroutine ("save_DepthOfField");
	}

	IEnumerator save_DepthOfField()
	{
		yield return new WaitForEndOfFrame ();

		if (depthOfField.value == 0) {
			if(Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ())
				Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ().enabled = false;
			PlayerPrefs.SetString ("DOF", "Off");
		}		
		if (depthOfField.value == 1) {			
			if(Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ())	
				Camera.main.GetComponent<LightingBox.Effects.DepthOfField> ().enabled = true;
			PlayerPrefs.SetString ("DOF", "On");
		}
	}
	#endregion

	#region Stochastic SSR
	/////////////////////////////////////////////////////////////////////////////////4
	/// /// // Stochastic SSR
	public void Change_StochasticSSR()
	{
		StartCoroutine ("save_StochasticSSR");
	}

	IEnumerator save_StochasticSSR()
	{
		yield return new WaitForEndOfFrame ();

		if (stSSR.value == 0) {
			if(Camera.main.GetComponent<cCharkes.StochasticSSR> ())
				Camera.main.GetComponent<cCharkes.StochasticSSR> ().enabled = false;
			PlayerPrefs.SetString ("stSSR", "Off");
		}		
		if (stSSR.value == 1) {				
			if(Camera.main.GetComponent<cCharkes.StochasticSSR> ())
				Camera.main.GetComponent<cCharkes.StochasticSSR> ().enabled = true;
			PlayerPrefs.SetString ("stSSR", "On");
		}
	}
	#endregion

	#region Motion Blur
	/////////////////////////////////////////////////////////////////////////////////5
	/// /// // Motion Blur
	public void Change_MotionBlur()
	{
		StartCoroutine ("save_MotionBlur");
	}

	IEnumerator save_MotionBlur()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < pb.Length; a++)
		{
			if (motionBlur.value == 0) 
			{
				vMB.enabled.overrideState = true;
				vMB.enabled.value = false;
				PlayerPrefs.SetString ("MotionBlur", "Off");
			}		
			if (motionBlur.value == 1) 
			{				
				vMB.enabled.overrideState = true;
				vMB.enabled.value = true;
				PlayerPrefs.SetString ("MotionBlur", "On");
			}
		}
	}
	#endregion

	#region Bloom
	/////////////////////////////////////////////////////////////////////////////////7
	/// /// // Bloom
	public void Change_Bloom()
	{
		StartCoroutine ("save_Bloom");
	}

	IEnumerator save_Bloom()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < pb.Length; a++)
		{
			if (bloom.value == 0) 
			{
				vBloom.enabled.overrideState = true;
				vBloom.enabled.value = false;
				PlayerPrefs.SetString ("Bloom", "Off");
			}		
			if (bloom.value == 1) 
			{				
				vBloom.enabled.overrideState = true;
				vBloom.enabled.value = true;
				PlayerPrefs.SetString ("Bloom", "On");
			}
		}
	}
	#endregion

	#region CA
	/////////////////////////////////////////////////////////////////////////////////8
	/// /// // Chromatic Aberration
	public void Change_ChromaticAberration()
	{
		StartCoroutine ("save_ChromaticAberration");
	}

	IEnumerator save_ChromaticAberration()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < pb.Length; a++)
		{
			if (chromaticAberration.value == 0) 
			{
				vCA.enabled.overrideState = true;
				vCA.enabled.value = false;
				PlayerPrefs.SetString ("ChromaticAberration", "Off");
			}		
			if (chromaticAberration.value == 1) 
			{				
				vCA.enabled.overrideState = true;
				vCA.enabled.value = true;
				PlayerPrefs.SetString ("ChromaticAberration", "On");
			}
		}
	}
	#endregion

	#region Vignette
	/////////////////////////////////////////////////////////////////////////////////
	// Vignette
	public void Change_Vignette()
	{
		StartCoroutine ("save_Vignette");
	}

	IEnumerator save_Vignette()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < pb.Length; a++)
		{
			if (vignette.value == 0) 
			{
				vVignette.enabled.overrideState = true;
				vVignette.enabled.value = false;
				PlayerPrefs.SetString ("Vignette", "Off");
			}		
			if (vignette.value == 1)  
			{
				vVignette.enabled.overrideState = true;
				vVignette.enabled.value = true;
				PlayerPrefs.SetString ("Vignette", "On");
			}
		}
	}
	#endregion

	#region Grass
	/////////////////////////////////////////////////////////////////////////////////
	// Grass
	public void Change_Grass()
	{
		StartCoroutine ("save_Grass");
	}

	IEnumerator save_Grass()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < t.Length; a++) {
			if (grass.value == 0) {
				t [a].detailObjectDensity = 0.3f;
				t [a].detailObjectDistance = 90f;
				PlayerPrefs.SetString ("Grass", "Low");
			}
			if (grass.value == 1) {
				t [a].detailObjectDensity = 0.5f;
				t [a].detailObjectDistance = 140f;
				PlayerPrefs.SetString ("Grass", "Medium");
			}
			if (grass.value == 2) {
				t [a].detailObjectDensity = 1f;
				t [a].detailObjectDistance = 243f;
				PlayerPrefs.SetString ("Grass", "High");
			}
		}
	}
	#endregion

	#region Terrain
	/////////////////////////////////////////////////////////////////////////////////
	// Terrain
	public void Change_Terrain()
	{
		StartCoroutine ("save_Terrain");
	}

	IEnumerator save_Terrain()
	{
		yield return new WaitForEndOfFrame ();

		for (int a = 0; a < t.Length; a++) {
			if (terrain.value == 0) {
				t [a].heightmapPixelError = 170f;
				PlayerPrefs.SetString ("Terrain", "Low");
			}
			if (terrain.value == 1) {
				t [a].heightmapPixelError = 73f;
				PlayerPrefs.SetString ("Terrain", "Medium");
			}
			if (terrain.value == 2) {
				t [a].heightmapPixelError = 5f;
				PlayerPrefs.SetString ("Terrain", "High");
			}
		}
	}
	#endregion

	#region Texture Resolution
	/////////////////////////////////////////////////////////////////////////////////
	// Texture Resolution
	public void Change_TextureResolution()
	{
		StartCoroutine ("save_TextureResolution");
	}

	IEnumerator save_TextureResolution()
	{
		yield return new WaitForEndOfFrame ();

		if (textureResolution.value == 0) {
			QualitySettings.masterTextureLimit = 2;
			PlayerPrefs.SetString ("TextureResolution", "Low");
		}
		if (textureResolution.value == 1) {
			QualitySettings.masterTextureLimit = 1;
			PlayerPrefs.SetString ("TextureResolution", "Medium");
		}
		if (textureResolution.value == 2) {
			QualitySettings.masterTextureLimit = 0;
			PlayerPrefs.SetString ("TextureResolution", "High");
		}

	}
	#endregion

	#region Anistropic
	/////////////////////////////////////////////////////////////////////////////////
	// Texture Anistropic
	public void Change_TextureAnistropic()
	{
		StartCoroutine ("save_TextureAnistropic");
	}

	IEnumerator save_TextureAnistropic()
	{
		yield return new WaitForEndOfFrame ();

		if (textureAnistropic.value == 0) 
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable ;
			PlayerPrefs.SetString ("TextureAnistropic", "Disable");
		}
		if (textureAnistropic.value == 1) 
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable ;
			PlayerPrefs.SetString ("TextureAnistropic", "Enable");
		}
		if (textureAnistropic.value == 2)
		{
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable ;
			PlayerPrefs.SetString ("TextureAnistropic", "ForceEnable");
		}

	}
	#endregion

	#region Shadows
	/////////////////////////////////////////////////////////////////////////////////
	// Shadows
	public void Change_Shadows()
	{
		StartCoroutine ("save_Shadows");
	}

	IEnumerator save_Shadows()
	{
		yield return new WaitForEndOfFrame ();

		if (shadows.value == 0) 
		{
			QualitySettings.shadowResolution = ShadowResolution.Low;
			PlayerPrefs.SetString ("Shadows", "Low");
		}
		if (shadows.value == 1) 
		{
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			PlayerPrefs.SetString ("Shadows", "Medium");
		}
		if (shadows.value == 2)
		{
			QualitySettings.shadowResolution = ShadowResolution.High;
			PlayerPrefs.SetString ("Shadows", "High");
		}

		if (shadows.value == 3)
		{
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			PlayerPrefs.SetString ("Shadows", "VeryHigh");
		}
	}
	#endregion

	#region Reflections
	//////////////////////////////////////////////////////////////////////////////
	// Realtime Reflections
	public void Change_Reflections()
	{
		StartCoroutine ("save_Reflections");
	}

	IEnumerator save_Reflections()
	{
		yield return new WaitForEndOfFrame ();

		if (realtimeReflections.value == 0) 
		{
			QualitySettings.realtimeReflectionProbes = false;
			PlayerPrefs.SetString ("Reflections", "Off");
		}
		if (realtimeReflections.value == 1) 
		{
			QualitySettings.realtimeReflectionProbes = true;
			PlayerPrefs.SetString ("Reflections", "On");
		}
	}
	#endregion

	#region Soft Particles
	/////////////////////////////////////////////////////////////////////////////////
	// Soft Particles
	public void Change_SoftParticles()
	{
		StartCoroutine ("save_SoftParticles");
	}

	IEnumerator save_SoftParticles()
	{
		yield return new WaitForEndOfFrame ();

		if (softParticles.value == 0) 
		{
			QualitySettings.softParticles = false;
			PlayerPrefs.SetString ("SoftParticles", "Off");
		}
		if (softParticles.value == 1) 
		{
			QualitySettings.softParticles = true;
			PlayerPrefs.SetString ("SoftParticles", "On");
		}
	}
	#endregion

	#region Global Functions
	//////////////////////////////////////////////////////////////////////////////
	/// 
	/// 
	public void SetTrue(GameObject target)
	{
		target.SetActive (true);
	}
	public void SetFalse(GameObject target)
	{
		target.SetActive (false);
	}
	public void ToggleObject(GameObject target)
	{
		target.SetActive (!target.activeSelf);
	}
	#endregion

	#region Traget FPS
	// target fps 30 or 60   
	public void Change_targetFPS()
	{
		StartCoroutine ("save_targetFPS");
	}

	IEnumerator save_targetFPS()
	{
		yield return new WaitForEndOfFrame ();

		if (targetFPS.value == 0) 
		{
			Application.targetFrameRate = 30;
			PlayerPrefs.SetString ("FPS", "30");
		}
		if (targetFPS.value == 1) 
		{
			Application.targetFrameRate = 60;
			PlayerPrefs.SetString ("FPS", "60");
		}
	}
	#endregion

	#region VSync
	//////////////////////////////////////////////////////////////////////////////
	/// // VSync  
	public void Change_VSync()
	{
		StartCoroutine ("save_VSync");
	}

	IEnumerator save_VSync()
	{
		yield return new WaitForEndOfFrame ();

		if (vSync.value == 0) 
		{
			QualitySettings.vSyncCount = 1;
			PlayerPrefs.SetString ("VSync", "On");
		}
		if (vSync.value == 1) 
		{
			QualitySettings.vSyncCount = 0;
			PlayerPrefs.SetString ("VSync", "Off");
		}
	}
	#endregion

	#region FullScreen
	//////////////////////////////////////////////////////////////////////////////
	/// // full screen  
	public void Change_FullScreen()
	{
		StartCoroutine ("save_FullScreen");
	}

	IEnumerator save_FullScreen()
	{
		yield return new WaitForEndOfFrame ();

		if (fullScreen.value == 0) 
		{
			Screen.fullScreen = true;
			PlayerPrefs.SetString ("FullScreen", "On");
		}
		if (fullScreen.value == 1) 
		{
			Screen.fullScreen = false;
			PlayerPrefs.SetString ("FullScreen", "Off");
		}
	}
	//////////////////////////////////////////////////////////////////////////////
	#endregion
}