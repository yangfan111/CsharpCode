using System;
using System.Collections;
using UnityEngine;
namespace ArtPligins
{
	public class DelayEvent : MonoBehaviour 
	{
        [Header("延迟 几秒")]
        public float delayTime;
        [Header("延迟 隐藏")]
        public bool disableMode = true;
        [Header("延迟 销毁")]
        public bool destroyMode;
        private Action onDelayCmp;
        public void OnEnable (){

            StartCoroutine(delay());

	    }

        public void regDelayEventCallBack(Action onDelayCmp)
        {

           this.onDelayCmp = onDelayCmp;

        }

        private IEnumerator delay()
        {
            yield return new WaitForSeconds( delayTime);
            if (onDelayCmp != null) onDelayCmp();
            if (destroyMode)
            {
                Destroy(gameObject);

            }
            else if (disableMode) {
                gameObject.SetActive(false);
            }
        }

	    public void Clone(DelayEvent source)
	    {
	        delayTime = source.delayTime;
	        disableMode = source.disableMode;
	        destroyMode = source.destroyMode;
	    }
    }
}

