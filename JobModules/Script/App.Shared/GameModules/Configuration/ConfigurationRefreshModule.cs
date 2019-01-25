using Core.GameModule.Module;

namespace App.Shared.GameModules.Configuration
{
    public class ConfigurationRefreshModule : GameModule
    {
        public ConfigurationRefreshModule(Contexts contexts)
        {
            AddSystem(new ConfigReloadSystem(contexts));
        }
    }
}
