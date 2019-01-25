using XmlConfig;

namespace Core.Configuration.Equipment
{
    public class SceneConfigManager
    {
        public static SceneConfigManager Instance = new SceneConfigManager();
        public AbstractMapConfig SceneParameters { get; set; }
    }
}