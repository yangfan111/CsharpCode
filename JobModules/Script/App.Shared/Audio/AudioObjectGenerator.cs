using System.Collections.Generic;
using UnityEngine;

namespace App.Shared
{
    public class AudioObjectGenerator
    {
        
        public static LinkedList<GameObject> audioObjectUsableList = new LinkedList<GameObject>();
        public static int accumulator = 0;
        public static int ObjectUsageMaxCount = 50;
        
        public void Dispose()
        {
            if (generatorGo)
                UnityEngine.Object.Destroy(generatorGo);
            generatorGo = null;
        }
        
        private GameObject generatorGo;
        private GameObject GenerateGo
        {
            get
            {

                if (generatorGo == null)
                {
                    generatorGo = new GameObject("AKAudioObjGenerator");
                    audioObjectUsableList.Clear();
                    accumulator = 0;
                }
                return generatorGo;
            }
        }
        public GameObject GetAudioEmitter()
        {
            GameObject newObject;

            if (audioObjectUsableList.Count >= ObjectUsageMaxCount)
            {
                newObject = audioObjectUsableList.First.Value;
                audioObjectUsableList.RemoveFirst();
            }
            else
            {
                newObject = new GameObject(string.Format("aktarget:{0}", accumulator++));
                newObject.transform.SetParent(GenerateGo.transform);
            }

            audioObjectUsableList.AddLast(newObject);
            return newObject;
        }
    }
}