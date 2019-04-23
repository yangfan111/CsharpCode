namespace App.Shared.Components.GenericActions
{
    public interface IGenericAction
    {
        void Update(PlayerEntity player);
        void ActionInput(PlayerEntity player);
        void PlayerReborn(PlayerEntity player);
        void PlayerDead(PlayerEntity player);
    }
}
