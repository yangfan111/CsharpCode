using UserInputManager.Utility;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.UserInput
{
    [UserInput, Unique]
    public class UserInputManagerComponent : IComponent
    {
        public UserInputManager.Lib.UserInputManager Instance;
        public UserInputHelper Helper;
    }
}