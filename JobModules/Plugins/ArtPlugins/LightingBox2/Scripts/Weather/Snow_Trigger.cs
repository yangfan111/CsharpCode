using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Snow_Trigger : MonoBehaviour {

	float radius;
	public float snowIntensity = 3f;

	void Start ()
	{
		if (Application.isPlaying)
			this.enabled = false;
	}

	void Update()
	{
		MeshRenderer[] mR = GameObject.FindObjectsOfType<MeshRenderer> ();
		foreach (MeshRenderer m in mR) 
		{
			if (Vector3.Distance (transform.position, m.transform.position) <= radius) {
				for (int a = 0; a < m.sharedMaterials.Length; a++)
					m.sharedMaterials [a].SetFloat ("_SnowIntensity", snowIntensity);
			} else {
				for (int a = 0; a < m.sharedMaterials.Length; a++)
					m.sharedMaterials [a].SetFloat ("_SnowIntensity", 0);
			}
		}
	}

	void OnDrawGizmos()
	{
		radius = transform.localScale.x + transform.localScale.y + transform.localScale.z;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, radius  );
	}

}

