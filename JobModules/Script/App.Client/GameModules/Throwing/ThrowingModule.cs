using Core.GameModule.Module;
using App.Client.ClientGameModules.Throwing;

namespace App.Client.GameModules.Throwing
{
    public class ThrowingModule : GameModule
    {
        public ThrowingModule(Contexts contexts) 
        {
            AddSystem(new ThrowingEntityInitSytem(contexts.throwing));
            AddSystem(new ThrowingRenderSystem(contexts));
            AddSystem(new ThrowingLineSystem(contexts));
            AddSystem(new ThrowingCountDownSystem(contexts));
        }
    }
}