using Core.GameModule.Module;

namespace App.Client.GameModules.Sound
{
    internal class ClientSoundModule : GameModule
    {
        public ClientSoundModule(Contexts contexts)
        {
            /*var soundParentController = new SoundParentController(contexts);
            AddSystem(new SoundResourcesLoadSystem(contexts, soundParentController));

            AddSystem(new SoundCleanUpSystem(contexts));

            AddSystem(new SoundPlaySystem(contexts));
            AddSystem(new ClientSoundAutoStopSystem(contexts.sound, contexts.session.clientSessionObjects.SoundPlayer, soundParentController));

            AddSystem(new SoundLimitSystem(contexts.sound));*/
            AddSystem(new ClientBombSoundSystem(contexts));

        }
    }
}
