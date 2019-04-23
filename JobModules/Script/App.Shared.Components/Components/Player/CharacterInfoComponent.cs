using Core.CharacterState;
using Entitas;

namespace App.Shared.Components.Player
{
    [Player]
    public class CharacterInfoComponent: IComponent
    {
        public ICharacterInfoProviderContext CharacterInfoProviderContext;
    }
}