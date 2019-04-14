using System.Collections.Generic;
using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.GameModule.Module;
using Core.SessionState;
using Entitas;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.SettingManager;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public class MapConfigInitModule : Systems
    {
        public MapConfigInitModule(Contexts contexts, ISessionCondition sessionState)
        {
            AddConfigSystem<MapPositionConfigManager>(contexts, sessionState, contexts.session.commonSession.RoomInfo.MapId.ToString(), "gamedata/map");
        }

        private void AddConfigSystem<T>(Contexts contexts, ISessionCondition sessionState, string asset, string bundleName) where T : AbstractConfigManager<T>, IConfigParser, new()
        {
            this.Add(new DefaultConfigInitSystem<T>(contexts.session.commonSession.AssetManager, sessionState, new AssetInfo(bundleName, asset), SingletonManager.Get<T>(), true));
        }
        
    }
}