using UnityEngine;

//[RequireComponent(typeof(AkInitializer))]

public class AudioBattleListener
{
	public readonly Vector3 ThrdEmitterDistanceDelta = new Vector3(0, 0, 5);
	public readonly Vector3 FstEmitterDistanceDelta  = new Vector3(0, 0, 3);
	public readonly uint Different_player_effect = 678245580;
	public AkSpatialAudioListener SpatialListener { get; private set; }
	
	public AkAudioListener DefaultListener { get; private set; }

	public AkGameObj DefaultListenerObj { get; private set; }
	
	
	public AkGameObj ThdViewEmitter { get; private set; }

	public AkGameObj FstViewEmitter { get; private set; }

	private Transform parentTrans;
    public bool IsInitialized { get; private set; }

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

    public AudioBattleListener(AkAudioListener listener, AkSpatialAudioListener spatialListener)
    {
	    Reset(listener, spatialListener);
    }

    public void Reset(AkAudioListener listener, AkSpatialAudioListener spatialListener)
    {
	    DefaultListener    = listener;
	    SpatialListener    = spatialListener;
	    DefaultListenerObj = listener.GetComponent<AkGameObj>();
    }
    public AkGameObj GetEmitterObject(bool isThrd)
    {
        if (IsInitialized)
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
        DefaultListenerObj.transform.localEulerAngles = Vector3.zero;
        var go = new GameObject("thdViewEmitter");
		ThdViewEmitter = go.AddComponent<AkGameObj>();
		ThdViewEmitter.IsMainObject = true;
		ThdViewEmitter.transform.SetParent(parentTrans);
		ThdViewEmitter.transform.localPosition = ThrdEmitterDistanceDelta;
		ThdViewEmitter.transform.localEulerAngles = Vector3.zero;
		//ThdViewEmitter.transform.LookAt(DefaultListenerObj.transform);
		AkSoundEngine.SetRTPCValue(Different_player_effect, 0f,ThdViewEmitter.gameObject);
		//ThdViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.transform.position - ThdViewEmitter.transform.position);
	
		 go = new GameObject("fstViewEmitter");
		FstViewEmitter = go.AddComponent<AkGameObj>();
		FstViewEmitter.IsMainObject = true;
		FstViewEmitter.transform.SetParent(parentTrans);
		FstViewEmitter.transform.localPosition = FstEmitterDistanceDelta;
		FstViewEmitter.transform.localEulerAngles = Vector3.zero;
		//FstViewEmitter.transform.LookAt(DefaultListenerObj.transform);
      
        AkSoundEngine.SetRTPCValue(Different_player_effect, 0f,FstViewEmitter.gameObject);
        IsInitialized = true;
        //FstViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.transform.position - FstViewEmitter.transform.position);
    }

}
