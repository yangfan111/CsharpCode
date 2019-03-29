using Core.GameModule.Module;

namespace App.Server.GameModules.Player
{
    public class ServerPostPredictionModule : GameModule
    {
        public ServerPostPredictionModule(Contexts contexts)
        {
            AddSystem(new ServerTimeWriterSystem(contexts));
        }
    }
}