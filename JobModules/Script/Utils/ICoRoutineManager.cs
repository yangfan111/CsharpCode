using System.Collections;
using UnityEngine;

namespace Core
{
    public interface ICoRoutineManager
    {
        Coroutine StartCoRoutine(IEnumerator enumerator);
        void StopCoRoutine(IEnumerator enumerator);
    }
}