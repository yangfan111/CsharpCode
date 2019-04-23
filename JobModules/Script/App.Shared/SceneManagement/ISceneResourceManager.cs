using Core.SceneManagement;
using UnityEngine;

namespace App.Shared.SceneManagement
{
    public interface ISceneResourceManager
    {
        void UpdateOrigin(Vector3 value);
        void SetAsapMode(bool value);
    }
}