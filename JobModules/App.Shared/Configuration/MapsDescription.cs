using Utils.Configuration;
using XmlConfig;

namespace App.Shared.Configuration
{
    public class MapsDescription : AbstractConfigManager<MapsDescription>
    {
        public MapConfig Data { get; private set; }
        public LevelType CurrentLevelType { get; private set; }
        public AbstractMapConfig SceneParameters { get; private set; }
        public SceneConfig BigMapParameters { get; private set; }
        public LevelConfig SmallMapParameters { get; private set; }

        public override void ParseConfig(string xml)
        {
            Data = XmlConfigParser<MapConfig>.Load(xml);
        }

        public void SetMapId(int mapId)
        {
            CurrentLevelType = LevelType.Exception;
            BigMapParameters = null;
            SmallMapParameters = null;

            foreach (var levelInfo in Data.MapInfos)
            {
                if (levelInfo.Id == mapId)
                {
                    SceneParameters = levelInfo;

                    if (levelInfo is SceneConfig)
                    {
                        CurrentLevelType = LevelType.BigMap;
                        BigMapParameters = levelInfo as SceneConfig;
                    }
                    else
                    {
                        CurrentLevelType = LevelType.SmallMap;
                        SmallMapParameters = levelInfo as LevelConfig;
                    }

                    break;
                }
            }
        }
    }

    public enum LevelType
    {
        Exception,
        BigMap,
        SmallMap
    }
}