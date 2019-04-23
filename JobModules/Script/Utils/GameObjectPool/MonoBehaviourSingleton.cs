using UnityEngine;

namespace Assets.Sources.Utils
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<T>();
                }
                return _instance;
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        public static bool HasInstance
        {
            get { return _instance != null; }
        }
    }
}
