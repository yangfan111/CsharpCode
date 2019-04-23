using System;
using Core.HitBox;
using Utils.AssetManager;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class ParachuteLoadResponseHandler
    {
        public void OnLoadSucc(PlayerEntity player, UnityObject unityObj)
        {
            if (player.isFlagDestroy)
            {
                return;
            }

            var transform = unityObj.AsGameObject.transform;
            const string anchorName = "Driver_Seat";
            var anchor = transform.FindChildRecursively(anchorName);
            if (anchor == null)
            {
                throw new Exception(String.Format("Can not find anchor with name {0} for parachute!", anchorName));
            }

            player.playerSkyMove.IsParachuteLoading = false;
            player.playerSkyMove.Parachute = transform;
            player.playerSkyMove.ParachuteAnchor = anchor;
        }
    }
}
