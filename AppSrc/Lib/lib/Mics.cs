using UnityEngine;
using System.Collections;
#if UNITY_2017_1_OR_NEWER
#endif
//unity editor atrribute
[UnityEngine.AddComponentMenu("Wwise/AkEnvironmentPortal")]
[UnityEngine.RequireComponent(typeof(UnityEngine.BoxCollider))]
[UnityEngine.ExecuteInEditMode]
[UnityEngine.DisallowMultipleComponent]
public class Mics : MonoBehaviour
{
    [UnityEngine.Range(0, 1)]
    public float abc = 0f;

}
