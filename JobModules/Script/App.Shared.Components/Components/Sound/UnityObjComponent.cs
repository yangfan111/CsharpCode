using Core.Components;
using UnityEngine;

namespace App.Shared.Components.Sound
{
    [Sound]
    public class UnityObjComponent : SingleAssetComponent 
    {
        public AudioSource Source;

        public override int GetComponentId()
        {
            return (int)EComponentIds.SoundUnityObj;
        }
    }
}