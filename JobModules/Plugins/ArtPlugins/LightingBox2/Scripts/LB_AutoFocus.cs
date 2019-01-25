// Attach this to a camera
// Works using ray casts
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_AutoFocus : MonoBehaviour {

	LightingBox.Effects.DepthOfField dof;

	// Hitted collider's layer mask
	public LayerMask layerMask = 1;

	[Header("Depth Of Field")]
	// Depth of field range (focus range)
	public float maxRange = 100f; 
	// Max blur amount (far blur)
	public float maxBlur = 30f;
	// Changes speed (*Time.deltaTime)   
	public float speed = 100f;

	[Header("Raycast Settings")]
	// Update time for raycasting (lower has more performance, higher has more currect result)
	public float updateInterval = 0.001f;
	// Raycast length to detect colliders
	public float rayLength = 10f;

	[Header("Debug Mode")]
	// Display hitted target (collider) name and ray line s
	public bool debugMode = false;

	// temps
	float rangeRef,radiusRef;

	IEnumerator Start () 
	{
		dof = GetComponent<LightingBox.Effects.DepthOfField> ();

		while (true) 
		{
			yield return new WaitForSeconds (updateInterval);
			DOF_Raycast_System ();
		}
	}

	// Autofocus mode baswd on raycast   
	void DOF_Raycast_System()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit,rayLength,layerMask)) 
		{
			if (debugMode) {
				Debug.DrawRay (transform.position,
					ray.direction, Color.red, rayLength);
			}

			// Blur Range-------------------------------------------------------------
			if (radiusRef < maxBlur)
				radiusRef += 1 * Time.deltaTime * speed;
			
			dof.focus.farBlurRadius = radiusRef;
			//-------------------------------------------------------------
			// Focus Range-------------------------------------------------------------
			if (rangeRef > 0)
				rangeRef -= 1 * Time.deltaTime * speed;

			dof.focus.range = rangeRef;
			//-------------------------------------------------------------

			dof.focus.transform = hit.transform;

			if(debugMode)
				Debug.Log (hit.transform.name);

		} else {
			if (hit.transform == null) {
				// Blur Range-------------------------------------------------------------

				if (radiusRef > 0)
					radiusRef -= 1 * Time.deltaTime *speed;
				
				dof.focus.farBlurRadius = radiusRef;
				//-------------------------------------------------------------
				// Focus Range-------------------------------------------------------------
				if (rangeRef < maxRange)
					rangeRef += 1 * Time.deltaTime *speed;

				dof.focus.range = rangeRef;
				//-------------------------------------------------------------

				dof.focus.transform = null;
				if (debugMode) {
					Debug.Log ("Null");
				}
			}
		}
	}

}
