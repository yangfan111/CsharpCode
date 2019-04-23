using UnityEngine;
using App.Shared.Audio;
using Core;
using Core.Utils;

//[RequireComponent(typeof(AkInitializer))]

public class AudioListenerManager
{
	public AkSpatialAudioListener SpatialListener { get; private set; }
	
	public AkAudioListener DefaultListener { get; private set; }

	public Transform DefaultListenerTrans { get; private set; }
	
	
	public Transform ThdViewEmitter { get; private set; }

	public Transform FstViewEmitter { get; private set; }

	private Transform parentTrans;

	private Transform ParentTrans
	{
		get
		{
			if (parentTrans)
				return parentTrans;
			if (!DefaultListenerTrans)
				return null;
			if (!DefaultListenerTrans.parent)
				return null;
			parentTrans = DefaultListenerTrans.parent;
			return parentTrans;
		}
	}

	public AudioListenerManager(AkAudioListener listener, AkSpatialAudioListener spatialListener)
	{
		DefaultListener = listener;
		SpatialListener = spatialListener;
		DefaultListenerTrans = listener.transform;
	}
	
	public bool HasParent
	{
		get { return ParentTrans != null; }
	}

	public void SetPartent( Transform parentTrans)
	{
		DefaultListenerTrans.SetParent(parentTrans);
		DefaultListenerTrans.localPosition =Vector3.zero;
		ThdViewEmitter = new GameObject("thdViewEmitter").transform;
		ThdViewEmitter.SetParent(parentTrans);
		ThdViewEmitter.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
		ThdViewEmitter.transform.LookAt(DefaultListenerTrans);
		//ThdViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.position - ThdViewEmitter.transform.position);
	
		
		FstViewEmitter = new GameObject("fstViewEmitter").transform;
		FstViewEmitter.SetParent(parentTrans);
		FstViewEmitter.localPosition = GlobalConst.FstEmitterDistanceDelta;
		FstViewEmitter.transform.LookAt(DefaultListenerTrans);

		//FstViewEmitter.localRotation = Quaternion.LookRotation(DefaultListenerTrans.position - FstViewEmitter.transform.position);
	}

}
