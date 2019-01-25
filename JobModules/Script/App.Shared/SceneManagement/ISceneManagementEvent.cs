using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Shared.SceneManagement
{
    public interface ISceneManagementEvent
    {
        event SceneLoadedEventHandler SceneLoadedEvent;
        event SceneUnloadEventedHandler SceneUnloadedEvent;
    }

    public delegate void SceneLoadedEventHandler(Scene scene, Vector3 center, float size);
    public delegate void SceneUnloadEventedHandler(Scene scene);
}