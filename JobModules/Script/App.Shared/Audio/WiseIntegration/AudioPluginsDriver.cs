using UnityEngine;

//[RequireComponent(typeof(AkInitializer))]
using App.Shared.Audio;


public class AudioPluginsDriver: MonoBehaviour
{
	[HideInInspector]
	public AkInitializer akInitComponent;
	[HideInInspector]
	public AkAudioListener defaultSpatialListener;
	[HideInInspector]
	public Transform defaultListenerTrans;
	private bool CanInspect;

	void Awake ()
	{
		AkSoundEngineController.OnAudioPluginInitialized += LaunchAppAudio;
		akInitComponent = gameObject.AddComponent<AkInitializer> ();
	}

	 void LaunchAppAudio ()
	{
		CanInspect = true;
        InitWiseDefaultListener();
        AKAudioEntry.LaunchAppAudio (this);
	}

	void InitWiseDefaultListener ()
	{
		if (!defaultSpatialListener) {
			GameObject listenerObj = new GameObject ("DefaultAudioListenerObj");
			defaultSpatialListener = listenerObj.AddComponent<AkAudioListener> ();
			defaultSpatialListener.SetIsDefaultListener (true);
			defaultListenerTrans = defaultSpatialListener.transform;
			if (Camera.main) {
				defaultListenerTrans.SetParent (Camera.main.transform);
				defaultListenerTrans.localPosition = Vector3.zero;
			} else {
				defaultListenerTrans.position = Vector3.zero;
			}
		}
	}

	void Update ()
	{
		if (!CanInspect)
			return;
		if (!defaultListenerTrans) {
			InitWiseDefaultListener ();
		}
		if (defaultListenerTrans.parent)
			return;
		if (Camera.main) {
			defaultListenerTrans.SetParent (Camera.main.transform);
			defaultListenerTrans.localPosition = Vector3.zero;
		}
	}
	//void OnWiseEngineStartupReady(System.Object obj)
	//{

	//    App.Shared.Audio.AKAudioEntry.LaunchAppAudio(obj);
	//}
	//public override void HandleEvent(UnityEngine.GameObject in_gameObject)
	//{
	//    App.Shared.Audio.AKAudioEntry.LaunchAppAudio(gameObject);
	//}
	////void AddDefaultListener()
	//{

	//}

}
