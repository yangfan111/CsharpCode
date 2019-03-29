using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Room;
using App.Shared.FreeFramework.framework.unit;
using App.Protobuf;
using Utils.Singleton;
using Utils.Configuration;
using XmlConfig;
using Utils.CharacterState;

namespace App.Shared.FreeFramework.Free.player
{
    public class PlayerPreloadAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            IPlayerInfo player = (IPlayerInfo)((ObjectUnit)args.GetUnit("playerInfo")).GetObject;
            LoginSuccMessage room = (LoginSuccMessage)((ObjectUnit)args.GetUnit("roomInfo")).GetObject;

            HashSet<string> set = new HashSet<string>();

            RoleItem item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(player.RoleModelId);

            foreach (int id in player.AvatarIds)
            {
                set.UnionWith(GetAvatarAsset(id, (Sex)item.Sex));
            }

            set.UnionWith(GetRoleAsset(item));

            room.PreLoadAssetInfo = Merge(room.PreLoadAssetInfo, set);
        }

        private string Merge(string old, HashSet<string> set)
        {
            if (!string.IsNullOrEmpty(old))
            {
                string[] uis = old.Split(',');
                foreach (string u in uis)
                {
                    if (!string.IsNullOrEmpty(u))
                    {
                        set.Add(u.Trim());
                    }
                }
            }

            return string.Join(",", set.ToArray());
        }

        private string[] GetRoleAsset(RoleItem role)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (int id in role.Res)
            {
                AvatarAssetItem item = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(id);

                set.UnionWith(new string[] { item.BundleName + "/" + item.AssetName, item.BundleName + "/" + item.SecondRes });
            }

            return set.ToArray();
        }

        private string[] GetAvatarAsset(int id, Sex sex)
        {
            int resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(id, Sex.Female);
            AvatarAssetItem item = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);

            return new string[] { item.BundleName + "/" + item.AssetName, item.BundleName + "/" + item.SecondRes };
        }
    }
}
