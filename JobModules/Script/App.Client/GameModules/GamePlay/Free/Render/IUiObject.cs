using UnityEngine;

namespace Assets.Sources.Free.Render
{
    public interface IUiObject
    {
        int width { get; set; }
        int height { get; set; }
        bool visible { get; set; }
        float x { get; set; }
        float y { get; set; }
        float alpha { get; set; }
        float scaleX { get; set; }
        float scaleY { get; set; }
        float rotation { get; set; }
        GameObject gameObject { get; }

        void AddChild(IUiObject child);

        T FindComponentInChildren<T>() where T:MonoBehaviour;

        T GetComponent<T>() where T : MonoBehaviour;
    }
}
