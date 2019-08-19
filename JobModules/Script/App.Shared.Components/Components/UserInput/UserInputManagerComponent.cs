using UserInputManager.Utility;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UserInputManager.Lib;

namespace App.Shared.Components.UserInput
{
    [UserInput, Unique]
    public class UserInputManagerComponent : IComponent
    {
        public GameInputManager Mgr;
        public GameInputHelper Helper;
    }
}