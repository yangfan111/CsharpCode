namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class StartThrowingThirdPerson : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if (!player.appearanceInterface.Appearance.IsFirstPerson 
                && player.hasThrowingAction
                && player.hasThrowingUpdate
                && player.throwingAction.ActionInfo.IsReady)
            {
                player.throwingUpdate.ReadyFly = true;
            }
        }
    }
}
