using Assets.Sources.Free.Render;
using Utils.Singleton;

namespace Assets.Sources.Free.Utility
{
    public class UnitySceneManager : Singleton<UnitySceneManager>, ISceneManage
    {
        public int frameTime
        {
            get { return (int) (UnityEngine.Time.deltaTime * 1000); }
        }

    }
}
