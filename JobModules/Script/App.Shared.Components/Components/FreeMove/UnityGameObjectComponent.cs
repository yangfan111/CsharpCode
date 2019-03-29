using Core.Components;

namespace App.Shared.Components.FreeMove
{
    [FreeMove]
    public class UnityGameObjectComponent : SingleAssetComponent
    {
        public override int GetComponentId()
        {
            return (int)EComponentIds.FreeMoveUnityObj; 
        }
    }
}
