using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace Assets.Sources.Free.Scene
{
    public class FogManager : Singleton<FogManager>
    {

        private IDictionary<int, FreeFog> fogs = new Dictionary<int, FreeFog>();

        public void Frame(int frameTime)
        {
            foreach (var pair in fogs)
            {
                pair.Value.frame(frameTime);
            }
        }

        public void Update()
        {
            Frame((int)(Time.deltaTime * 1000));
            var fogOpen = fogs.Count > 0;
            if (fogOpen)
            {
               

            }

        }

        public FreeFog GetFog(int fogId)
        {
            FreeFog ret = null;
            fogs.TryGetValue(fogId, out ret);
            return ret;
        }

        public void RemoveFog(int fogId)
        {
            fogs.Remove(fogId);
        }

        public void AddFog(int fogId, FreeFog fog)
        {
            fogs.Add(fogId, fog);
        }

        private void xx()
        {

        }
    }
}
