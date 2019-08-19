using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.App.Client.GameModules
{
    public enum EObjectTag
    {
        TestPosition = 1 << 0,
    }
    public class ObjectTag : MonoBehaviour
    {
        public long tag = 0;
    }
}
