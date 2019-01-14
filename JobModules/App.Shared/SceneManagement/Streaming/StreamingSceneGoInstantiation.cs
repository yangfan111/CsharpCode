using System;
using UnityEngine;

namespace App.Shared.SceneManagement.Streaming
{
    public interface StreamingSceneGoInstantiation
    {
        event Action<GameObject> GoInstantiated;
    }
}