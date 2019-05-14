using UnityEngine;
using App.Shared.Audio;
using Core;
using Core.Utils;

//[RequireComponent(typeof(AkInitializer))]

public class AudioListenerManager
{
	public AkSpatialAudioListener SpatialListener { get; private set; }
	
	public AkAudioListener DefaultListener { get; private set; }

	public AkGameObj DefaultListenerObj { get; private set; }
	
	
	public AkGameObj ThdViewEmitter { get; private set; }

	public AkGameObj FstViewEmitter { get; private set; }

	private Transform parentTrans;
    public bool Initialized { get; private set; }

	private Transform ParentTrans
	{
		get
		{
			if (parentTrans)
				return parentTrans;
			if (!DefaultListenerObj)
				return null;
			if (!DefaultListenerObj.transform.parent)
				return null;
			parentTrans = DefaultListenerObj.transform.parent;
			return parentTrans;
		}
	}

    public AudioListenerManager(AkAudioListener listener, AkSpatialAudioListener spatialListener)
    {
        DefaultListener = listener;
        SpatialListener = spatialListener;
        DefaultListenerObj = listener.GetComponent<AkGameObj>();
    }
    public AkGameObj GetEmitterObject(bool isThrd)
    {
        if (Initialized)
        {
            return isThrd ? ThdViewEmitter : FstViewEmitter;
        }
        return DefaultListenerObj;
    }
	
	public bool HasParent
	{
		get { return ParentTrans != null; }
	}

	public void SetPartent( Transform parentTrans)
	{
		DefaultListenerObj.transform.transform.SetParent(parentTrans);
		DefaultListenerObj.transform.localPosition =Vector3.zero;
		var go = new GameObject("thdViewEmitter");
		ThdViewEmitter = go.AddComponent<AkGameObj>();
		ThdViewEmitter.transform.SetParent(parentTrans);
		ThdViewEmitter.transform.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
		ThdViewEmitter.transform.LookAt(DefaultListenerObj.transform);
		//ThdViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.transform.position - ThdViewEmitter.transform.position);
	
		
		 go = new GameObject("fstViewEmitter");
		FstViewEmitter = go.AddComponent<AkGameObj>();
		FstViewEmitter.transform.SetParent(parentTrans);
		FstViewEmitter.transform.localPosition = GlobalConst.FstEmitterDistanceDelta;
		FstViewEmitter.transform.LookAt(DefaultListenerObj.transform);
        Initialized = true;
        //FstViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.transform.position - FstViewEmitter.transform.position);
    }

}
