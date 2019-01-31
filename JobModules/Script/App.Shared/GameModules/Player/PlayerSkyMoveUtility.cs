using Utils.AssetManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Player;
using com.wd.free.item;
using com.wd.free.skill;
using Core.HitBox;
using Core.Utils;
using Utils.Appearance;

namespace App.Shared.GameModules.Player
{
    public static class PlayerSkyMoveUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyMoveUtility));
        public static void DelayLoadParachute(PlayerEntity player, Contexts contexts)
        {
            _logger.Info("Delay Load Parachute...");
            var parachuteAssetInfo = new AssetInfo("equipment/parachute", "I002");
            var assetManager = contexts.session.commonSession.AssetManager;
            assetManager.LoadAssetAsync(player, parachuteAssetInfo,
                new PlayerResourceLoadSystem.ParachuteLoadResponseHandler().OnLoadSucc);
        }


        public static void DetachParachute(Contexts contexts, PlayerEntity player)
        {
            _logger.InfoFormat("SKyDive : Detach Patachute");
            var transform = player.RootGo().transform;
            var playerSkyMove = player.playerSkyMove;
            var parachute = playerSkyMove.Parachute;
            if (parachute != null && player.playerSkyMove.IsParachuteAttached)
            {
                player.position.Value = transform.position;

                transform.parent = parachute.parent;
                parachute.gameObject.SetActive(false);
                //                playerSkyMove.Parachute = null;
                //                playerSkyMove.ParachuteAnchor = null;
                //                UnityEngine.Object.Destroy(parachute.gameObject);
            }

            if (SharedConfig.IsServer)
            {
                FreeData fd = (FreeData)player.freeData.FreeData;
                ItemInventory bag = fd.freeInventory.GetInventoryManager().GetInventory("bag");
                if (bag.posList.Count > 0)
                {
                    ISkillArgs args = (ISkillArgs) contexts.session.commonSession.FreeArgs;
                    args.TempUse("current",fd);
                    bag.RemoveItem(args, bag.posList[0]);
                    args.Resume("current");
                }
            }

            player.playerSkyMove.IsParachuteAttached = false;
        }

        public static void AttachParachute(Contexts contexts, PlayerEntity player, bool isSyncRemote)
        {
            //服务器更新动画不更新骨骼数据，要用到骨骼数据，需要更新下骨骼数据
            PlayerEntityUtility.UpdateAnimatorTransform(player);

            var transform = player.RootGo().transform;
            var parachute = player.playerSkyMove.Parachute;
            var anchor = player.playerSkyMove.ParachuteAnchor;

            if (parachute != null && !player.playerSkyMove.IsParachuteAttached)
            {
                _logger.InfoFormat("SKyDive : Attach Patachute To Parachuting State");

                if (!isSyncRemote)
                {
                    var position = transform.position;
                    parachute.SetPositionAndRotation(transform.position, transform.rotation);
                    var target = player.thirdPersonModel.Value.transform.FindChildRecursively(BoneName.CharacterRightHandName);
                    BoneMount.FixedObj2Bones(transform.gameObject, target, anchor);
                    parachute.position += position - transform.position;

                    player.playerSkyMove.Position = parachute.transform.position;
                    player.playerSkyMove.Rotation = parachute.transform.rotation;
                    player.playerSkyMove.MoveStage = (int)SkyMoveStage.Parachuting;
                    player.playerSkyMove.LocalPlayerPosition = transform.localPosition;
                    player.playerSkyMove.LocalPlayerRotation = transform.localRotation;
                }
                else
                {
                    transform.parent = anchor;
                    player.playerSkyMove.Parachute.position = player.playerSkyMove.Position;
                    player.playerSkyMove.Parachute.rotation = player.playerSkyMove.Rotation;
                    transform.localPosition = player.playerSkyMove.LocalPlayerPosition;
                    transform.localRotation = player.playerSkyMove.LocalPlayerRotation;
                }

                player.playerSkyMove.IsParachuteAttached = true;
                player.playerSkyMove.IsWaitForAttach = false;
                parachute.gameObject.SetActive(true);
            }
        }
    }
}
