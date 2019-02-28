using System.Collections;
using Assets.Sources.Components.Asset;
using UnityEngine;

namespace Assets.Sources.Utils.Threading
{
    public class AsyncManager : MonoBehaviour
    {
        public static AsyncManager Instance;

        void Awake()
        {
            Instance = this;
        }

        public Coroutine StartCoRoutine(IEnumerator enumerator)
        {
            return StartCoroutine(enumerator);
        }

        public void StopCoRoutine(IEnumerator enumerator)
        {
            StopCoroutine(enumerator);
        }
    }
}