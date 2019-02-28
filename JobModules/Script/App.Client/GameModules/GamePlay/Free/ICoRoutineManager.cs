using System.Collections;
using UnityEngine;

namespace Assets.Sources.Components.Asset
{
    public interface ICoRoutineManager
    {
        Coroutine StartCoRoutine(IEnumerator enumerator);
        void StopCoRoutine(IEnumerator enumerator);
    }
}