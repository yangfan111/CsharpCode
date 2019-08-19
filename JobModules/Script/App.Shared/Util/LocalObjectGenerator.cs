using UnityEngine;

namespace App.Shared.Util
{
    public class LocalObjectGenerator : MonoBehaviour
    {
        private static AudioLocalObjectGenerator audioLocalObjectGenerator;
        private static EffectLocalObjectGenerator effectLocalObjectGenerator;
        protected uint accumulator = 0;

        public static GameObject generatorGo { get; private set; }
        
        
        public static AudioLocalObjectGenerator AudioLocal
        {
            get
            {
                Initialize();
                return audioLocalObjectGenerator;
            }
        }

        public static EffectLocalObjectGenerator EffectLocal
        {
            get
            {
                Initialize();
                return effectLocalObjectGenerator;
            }
        }

        public static void Dispose(bool soft = false)
        {
            if (soft)
            {
                EffectLocal.Clear();
                AudioLocal.Clear();
            }
            else
            {
                if (generatorGo)
                    Destroy(generatorGo);
            }
           
        }
        

        private static void Initialize()
        {
            if (!generatorGo)
            {
                generatorGo                 = new GameObject("LocalizationObjGenerator");
                effectLocalObjectGenerator = generatorGo.AddComponent<EffectLocalObjectGenerator>();
                audioLocalObjectGenerator  = generatorGo.AddComponent<AudioLocalObjectGenerator>();
            }
        }

        protected virtual void Clear()
        {
        }
    }
}