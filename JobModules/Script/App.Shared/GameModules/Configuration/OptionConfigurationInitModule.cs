using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.SessionState;
using Entitas;
using Shared.Scripts.SceneManagement;
using System.Collections.Generic;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Configuration
{
    public class OptionConfigurationInitModule : Systems
    {
        private readonly IUnityAssetManager _assetManager;


        private ISessionCondition _sessionState;

        public OptionConfigurationInitModule(ISessionCondition sessionState, IUnityAssetManager assetManager)
        {
            _assetManager = assetManager;
            var allMaps = SingletonManager.Get<MapsDescription>();
            if(allMaps.CurrentLevelType != LevelType.BigMap)
            {
                return;
            }
            SceneConfig config = allMaps.BigMapParameters;
            string preStr = config.PreMapName;
            int gridNum = config.TerrainDimension;
            StreamingLevelStructure streamingLevel = SingletonManager.Get<StreamingLevelStructure>();
            ScenesLightmapStructure lightmap = SingletonManager.Get<ScenesLightmapStructure>();
            ScenesIndoorCullStructure indoor = SingletonManager.Get<ScenesIndoorCullStructure>();
            streamingLevel.Clear();
            lightmap.Clear();
            indoor.Clear();
            for (int i = 0; i < gridNum; i++)
            {
                for(int j = 0; j < gridNum; j++)
                {
                    string preName = preStr + "_" + preStr + " " + i + "x" + j;
                    AddConfigSystem<StreamingLevelStructure>(sessionState, preName + StreamingConfig.DataXMLName, StreamingConfig.StreamingABPath + preStr + StreamingConfig.StreamingDataABName, streamingLevel);
                    AddConfigSystem<ScenesLightmapStructure>(sessionState, preName + StreamingConfig.LightXMLName, StreamingConfig.StreamingABPath + preStr + StreamingConfig.StreamingLightDataABName, lightmap);
                    AddConfigSystem<ScenesIndoorCullStructure>(sessionState, preName + StreamingConfig.InDoorXMLName, StreamingConfig.StreamingABPath + preStr + StreamingConfig.StreamingInDoorABName, indoor);
                }
            }
           
            _sessionState = sessionState;

        }





        private T AddConfigSystem<T>(ISessionCondition sessionState, string asset,
            string bundleName, T inst)
            where T : AbstractConfigManager<T>, IConfigParser, new()
        {
            this.Add(new DefaultConfigInitSystem<T>(_assetManager, sessionState, new AssetInfo(bundleName, asset),
                inst, true, true));
            return inst;
        }
        List<IExecuteSystem> _systems = new List<IExecuteSystem>();

    }
}