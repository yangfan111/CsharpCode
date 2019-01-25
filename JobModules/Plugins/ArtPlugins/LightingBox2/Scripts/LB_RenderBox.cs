// orginally from unity manual - edited by ALIyerEdon
/// use this script to capture high quality screenshots with custom RES
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectResolution
{
	_720P,_1080P,_4K,_8K,Custom
}
public class LB_RenderBox : MonoBehaviour {

	// The folder to contain our screenshots.
	// If the folder exists we will append numbers to create an empty folder.

	[Header("ScreenShot Settings")]
	[Space(3)]
	public string screenshotPath = "C:/RenderBox/ScreenShot";
	public KeyCode screenshotCaptureKey = KeyCode.F2;
	public SelectResolution screenShotResolution;
	[Header("Custom")]
	public int resWidth = 1920; 
	public int resHeight = 1080;

	[Header("Global Settings")]
	[Space(3)]
	public Camera customCamera;

	void Start()
	{

		// Create the folder
		if(!System.IO.Directory.Exists(screenshotPath))
			System.IO.Directory.CreateDirectory(screenshotPath);
		
	}

	// Capture high resolution screenshot
	// Source : http://answers.unity3d.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html

	void LateUpdate() 
	{
		if (Input.GetKeyDown(screenshotCaptureKey)) 
			TakeScreenShot ();
	}

	[ContextMenu("Take Screenshot")]
	void TakeScreenShot()
	{
		float tScale = Time.timeScale;
		Time.timeScale = 0;

		RenderTexture rt = null;

		if(screenShotResolution == SelectResolution._720P)
			rt	= new RenderTexture(1280, 720, 24);
		if(screenShotResolution == SelectResolution._1080P)
			rt	= new RenderTexture(1920, 1080, 24);
		if(screenShotResolution == SelectResolution._4K)
			rt	= new RenderTexture(3840, 2160, 24);
		if(screenShotResolution == SelectResolution._8K)
			rt	= new RenderTexture(7680, 4320, 24);
		if(screenShotResolution == SelectResolution.Custom)
			rt	= new RenderTexture(resWidth, resHeight, 24);


		if(customCamera)
			customCamera.targetTexture = rt;
		else
			Camera.main.targetTexture = rt;

		Texture2D screenShot = null;

		if(screenShotResolution == SelectResolution._720P)
			screenShot = new Texture2D(1280, 720, TextureFormat.RGB24, false);
		if(screenShotResolution == SelectResolution._1080P)
			screenShot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
		if(screenShotResolution == SelectResolution._4K)
			screenShot = new Texture2D(3840, 2160, TextureFormat.RGB24, false);
		if(screenShotResolution == SelectResolution._8K)
			screenShot = new Texture2D(7680, 4320, TextureFormat.RGB24, false);
		if(screenShotResolution == SelectResolution.Custom)
			screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

		if(customCamera)
			customCamera.Render();
		else
			Camera.main.Render();	

		RenderTexture.active = rt;

		if(screenShotResolution == SelectResolution._720P)
			screenShot.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
		if(screenShotResolution == SelectResolution._1080P)
			screenShot.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
		if(screenShotResolution == SelectResolution._4K)
			screenShot.ReadPixels(new Rect(0, 0, 3840, 2160), 0, 0);
		if(screenShotResolution == SelectResolution._8K)
			screenShot.ReadPixels(new Rect(0, 0, 7680, 4320), 0, 0);
		if(screenShotResolution == SelectResolution.Custom)
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);

		if(customCamera)				
			customCamera.targetTexture = null;
		else
			Camera.main.targetTexture = null;

		RenderTexture.active = null; // JC: added to avoid errors
		Destroy(rt);
		byte[] bytes = screenShot.EncodeToPNG();
		PlayerPrefs.SetInt ("ScreenShotNumber", PlayerPrefs.GetInt ("ScreenShotNumber") + 1);
		string filename = screenshotPath + "ScreenShot"+PlayerPrefs.GetInt ("ScreenShotNumber").ToString () + ".png";

		System.IO.File.WriteAllBytes(filename, bytes);
		Time.timeScale = tScale;
	}
}
