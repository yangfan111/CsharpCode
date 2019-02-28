using System;

namespace App.Client.SceneManagement.DistanceCulling
{
    interface ICacheableElement : ICloneable
    {
        void Reset();
        void Free();
    }
}