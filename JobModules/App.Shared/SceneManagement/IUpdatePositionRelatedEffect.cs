using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.SceneManagement
{
    public interface IUpdatePositionRelatedEffect
    {
        void UpdatePlayerPosition(Vector3 pos);
    }
}