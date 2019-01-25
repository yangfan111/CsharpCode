using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace App.Shared.SceneManagement
{
    public abstract class SceneAbstractController<T> : DisposableSingleton<T> where T: DisposableSingleton<T>, new()
    {
        public delegate void OnSceneLoaded(int index1, int index2, Scene scene);

        public delegate void OnSceneUnloaded(int index1, int index2);


        protected OnSceneLoaded onSceneLoaded;
        protected OnSceneUnloaded onSceneUnloaded;

        public void SetSceneListener(OnSceneLoaded onSceneLoaded, OnSceneUnloaded onSceneUnloaded)
        {
            this.onSceneLoaded = onSceneLoaded;
            this.onSceneUnloaded = onSceneUnloaded;
        }
    }
}
