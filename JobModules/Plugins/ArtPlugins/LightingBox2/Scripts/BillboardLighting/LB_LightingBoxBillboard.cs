
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_LightingBoxBillboard : MonoBehaviour {

	[Header("Light Settings")]

	// Drag a spot light as light source (I seen this technique in UE4 videos)
	public Light lightSource;

	// use randomized intensity as light intensity
	public bool randomizeIntensity;

	// multiply texture brightess to light intensity (more realistic, need more computation)   
	public bool useTextureBrightness;

	// Min and max intensity range to randomize light intensity
	public float minIntensity = 1f,maxIntensity = 1.7f;

	// Speed of light randomization
	public float lightSpeed = 10f;

	// Spot light range
	public float lightRange = 30f;

	[Header("Video Player")]
	public Texture2D[] frames ;
	public float framesPerSecond = 10f;

	// Prevate variables
	private float rand ;
	private Color targetColor;
	private bool started;
	MeshRenderer meshRenderer;
	Texture2D current;

	IEnumerator Start () {

		lightSource.range = lightRange;

		yield return new WaitForEndOfFrame();

		meshRenderer = GetComponent<MeshRenderer> ();

		started = true;

		// Used to randomize light Intensity based on min and max randomizer values
		while(true)
		{
			yield return new WaitForSeconds(0.1f);
			rand = Random.Range(minIntensity,maxIntensity);
		}
	}

	void Update () 
	{
		if(!started)
			return;

		//  Video player
		int index  = (int)Mathf.Floor( Time.time * framesPerSecond); 

		index = index % frames.Length;

		meshRenderer.material.mainTexture = frames[index]; 
		current = frames[index]; 

		// get current texture color
		targetColor = CalculateAverageColorFromTexture(current);

		if(useTextureBrightness)
			lightSource.color =   targetColor * (Brightness(targetColor)/100) ;
		else
			lightSource.color =   targetColor ;

		if(randomizeIntensity)
			RandomizeIntensity();

	}

	void RandomizeIntensity()
	{

		lightSource.intensity = Mathf.Lerp(lightSource.intensity,rand,Time.deltaTime*lightSpeed);

	}

	// Calculate texture average color
	Color32 CalculateAverageColorFromTexture(Texture2D tex)
	{
		Color32[] texColors = tex.GetPixels32();

		int total = texColors.Length;

		float r = 0;
		float g = 0;
		float b = 0;

		for(int i = 0; i < total; i++)
		{
			r += texColors[i].r;
			g += texColors[i].g;
			b += texColors[i].b;
		}
		return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0);

	}

	// Calculate color brightness ( samething's used in eye adaptation image effect  ) 
	private int Brightness(Color32 c)
	{
		return (int)Mathf.Sqrt
			(
				c.r * c.r * .241f + 
				c.g * c.g * .691f + 
				c.b * c.b * .068f
			);
	}
}
