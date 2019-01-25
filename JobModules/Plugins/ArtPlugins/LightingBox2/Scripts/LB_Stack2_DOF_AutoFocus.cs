// An adaptaion of Frank Otto's AutoFocus script for Unity's new PostProcessing Stack
// Original at: http://wiki.unity3d.com/index.php?title=DoFAutoFocus
// Adapted by Michael Hazani 
// For more info see: http://www.michaelhazani.com/autofocus-on-whats-important
// Edited and upgraded to post processing 2 by ALIyerEdon for Lightign Box 2.6    


using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera))]

public class LB_Stack2_DOF_AutoFocus : MonoBehaviour
{

    private GameObject doFFocusTarget;
    private Vector3 lastDoFPoint;

	private PostProcessProfile m_Profile;

	DepthOfField dof;

	public DoFAFocusQuality focusQuality = LB_Stack2_DOF_AutoFocus.DoFAFocusQuality.NORMAL;
    public LayerMask hitLayer = 1;
    public float maxDistance = 100.0f;
    public bool interpolateFocus = false;
    public float interpolationTime = 0.7f;

	Camera cam;

    public enum DoFAFocusQuality
    {
        NORMAL,
        HIGH
    }

    void Start()
    {
		cam = GetComponent<Camera> ();

		m_Profile = GameObject.Find ("Global Volume").GetComponent<PostProcessVolume> ().profile;
		m_Profile.TryGetSettings (out dof);

        doFFocusTarget = new GameObject("DoFFocusTarget");
      //  var behaviour = GetComponent<PostProcessingBehaviour>();
       // m_Profile = behaviour.profile;
    }

    void Update()
    {

        // switch between Modes Test Focus every Frame
		if (focusQuality == LB_Stack2_DOF_AutoFocus.DoFAFocusQuality.HIGH)
        {
            Focus();
        }

    }

    void FixedUpdate()
    {
        // switch between modes Test Focus like the Physicsupdate
		if (focusQuality == LB_Stack2_DOF_AutoFocus.DoFAFocusQuality.NORMAL)
        {
            Focus();
        }
    }

    IEnumerator InterpolateFocus(Vector3 targetPosition)
    {

        Vector3 start = this.doFFocusTarget.transform.position;
        Vector3 end = targetPosition;
        float dTime = 0;

        // Debug.DrawLine(start, end, Color.green);
       /// var depthOfField = m_Profile.depthOfField.settings;
        while (dTime < 1)
        {
            yield return new WaitForEndOfFrame();
            dTime += Time.deltaTime / this.interpolationTime;
            this.doFFocusTarget.transform.position = Vector3.Lerp(start, end, dTime);
			dof.focusDistance.value = Vector3.Distance(doFFocusTarget.transform.position, transform.position);
        }
        this.doFFocusTarget.transform.position = end;
    }

    void Focus()
    {
        // our ray
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, this.maxDistance, this.hitLayer))
        {
            Debug.DrawLine(ray.origin, hit.point);

            // do we have a new point?					
            if (this.lastDoFPoint == hit.point)
            {
                return;
                // No, do nothing
            }
            else if (this.interpolateFocus)
            { // Do we interpolate from last point to the new Focus Point ?
              // stop the Coroutine
                StopCoroutine("InterpolateFocus");
                // start new Coroutine
                StartCoroutine(InterpolateFocus(hit.point));
            }
            else
            {
                this.doFFocusTarget.transform.position = hit.point;
              ///  var depthOfField = m_Profile.depthOfField.settings;
				dof.focusDistance.value = Vector3.Distance(doFFocusTarget.transform.position, transform.position);
                // print(depthOfField.focusDistance);
               ///// m_Profile.depthOfField.settings = depthOfField;
            }
            // asign the last hit
            this.lastDoFPoint = hit.point;
        }
    }
}