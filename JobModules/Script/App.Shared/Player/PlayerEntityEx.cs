using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.Player
{
    public static class PlayerEntityEx
    {
        static LoggerAdapter logger = new LoggerAdapter("FrameTest");
        
        public static Sex GetSex(this PlayerEntity player)
        {
            if(!player.hasPlayerInfo)
            {
                return Sex.EndOfTheWorld;
            }
            var sex = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(player.playerInfo.RoleModelId).Sex;
            return (Sex)sex;
        }

        public static void AddAsset(this PlayerEntity player, UnityObject asset)
        {
            if (asset != null)
            {
                if (player.hasRecycleableAsset)
                    player.recycleableAsset.Add(asset);
                else
                    logger.InfoFormat("{0} doesn't have recycleableasset", player.entityKey);
            }
            else
                logger.InfoFormat("null asset in {0}", player.entityKey);
        }

        public static void RemoveAsset(this PlayerEntity player, UnityObject asset)
        {
            player.recycleableAsset.Remove(asset);
        }

        public static GameObject RootGo(this PlayerEntity player)
        {
            return player.recycleableAsset.PlayerRoot;
        }
    }
}
