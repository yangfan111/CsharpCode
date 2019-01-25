using System;
using UnityEngine;
namespace ArtPligins
{
	public class PrefabLoader:MonoBehaviour 
	{
		public GameObject asset;
	    public bool isCombinedAsset;
		public void Awake (){
		    if (asset != null)
		    {
		        var newItem = Instantiate(asset);
		        newItem.transform.parent = transform;
		        newItem.transform.localPosition = Vector3.zero;
		        newItem.transform.localRotation = Quaternion.identity;
                newItem.transform.localScale = Vector3.one;;
		        newItem.name = name;

		        var newEvt = newItem.GetComponent<DelayEvent>();
		        if (newEvt != null)
		        {
		            var evt = gameObject.AddComponent<DelayEvent>();
		            evt.Clone(newEvt);
		            Destroy(newEvt);
		        }

		        newItem.SetActive(true);
		        if (isCombinedAsset)
		        {
		            gameObject.SetActive(true);
		            transform.parent.gameObject.SetActive(false);
		        }
		        else
		        {
		            gameObject.SetActive(false);
		        }
            }
		    else
		    {
		        Debug.LogFormat("There is no asset for PrefabLoader for game object {0}", gameObject.name);
		    }
			
        }
}
}

