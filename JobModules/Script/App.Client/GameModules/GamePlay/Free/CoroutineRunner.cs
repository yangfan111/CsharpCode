using System.Collections;
using System.Collections.Generic;
using Assets.Sources.Utils.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils.Coroutine
{
    

    public class CoroutineRunner : MonoBehaviour
    {

        public class CoroutineItem
        {
            public IEnumerator Original;
            public IEnumerator Run;
            
            public CoroutineItem(IEnumerator original, IEnumerator run)
            {
                Original = original;
                Run = run;
            }
        }

        public IList<CoroutineItem> Routines = new List<CoroutineItem>();

        public static void StartCoroutine(GameObject go, IEnumerator coroutine)
        {
            var comp = go.GetComponent<CoroutineRunner>();
            if (comp == null)
                comp = go.AddComponent<CoroutineRunner>();
            var item = new CoroutineItem(coroutine, comp.Run(coroutine));

            comp.Routines.Add(item);
            AsyncManager.Instance.StartCoRoutine(item.Run);
        }

        public static void StopCoroutine(GameObject go, IEnumerator coroutine)
        {
            var comp = go.GetComponent<CoroutineRunner>();
            if (comp == null)
                return;
           comp.StopMyCoroutine(coroutine);
        }

        private void StopMyCoroutine( IEnumerator coroutine)
        {
            foreach (var cPair in Routines)
            {
                if (cPair.Original == coroutine)
                {
                    AsyncManager.Instance.StopCoRoutine(cPair.Run);
                    Routines.Remove(cPair);
                    break;
                }
            }
        }

        private void RemoveCoroutine(IEnumerator coroutine)
        {
            foreach (var cPair in Routines)
            {
                if (cPair.Original == coroutine)
                {
                    Routines.Remove(cPair);
                    break;
                }
            }
        }

        IEnumerator Run(IEnumerator coroutine)
        {
            yield return coroutine;
            RemoveCoroutine(coroutine);
        }


        void OnDestroy()
        {
            foreach (var cPair in Routines)
            {
                AsyncManager.Instance.StopCoRoutine(cPair.Run);
            }
        }
    }
}
