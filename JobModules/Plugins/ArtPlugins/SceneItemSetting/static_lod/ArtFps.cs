using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtFps : MonoBehaviour {


    int framePerS = 0;
    int frameRendered = 0;
    int calculateOnce = 500;
    float startTime;

    private void OnEnable()
    {
        startTime = Time.unscaledTime;
        framePerS = 0;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(0, 100, 100, 40), "artFps:" + framePerS);
    }
    private void OnPostRender()
    {
  
        frameRendered++;
        if (frameRendered >=calculateOnce) {
            framePerS =(int)( frameRendered / (Time.unscaledTime-startTime));
            startTime = Time.unscaledTime;
            frameRendered = 0;
        }

    }
}
