using System.Collections.Generic;
using com.wd.free.para;
using App.Shared.Components.Player;
using App.Server.GameModules.GamePlay.Free.weapon;
using App.Shared.GameModules.Player;
using App.Shared;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.player
{
    public class PlayerFields : IFields
    {
        private static string[] fields;
        private static HashSet<string> fieldSet;

        private PlayerEntity player;
        private Contexts _contexts;

        static PlayerFields()
        {
            fields = new string[] { "x", "y", "z", "id", "team", "pitch", "yaw", "isDead", "currentWeaponKey", "currentWeaponId", "inCar", "StrAvatarIds" };

            fieldSet = new HashSet<string>();
            fieldSet.UnionWith(fields);
        }

        public PlayerFields(Contexts contexts, PlayerEntity player)
        {
            this.player = player;
            _contexts = contexts;
        }

        public IPara Get(string field)
        {
            if ("id" == field)
            {
                return new IntPara(field, player.entityKey.Value.EntityId);
            }
            else if ("x" == field)
            {
                return new FloatPara(field, player.position.Value.x);
            }
            else if ("y" == field)
            {
                return new FloatPara(field, player.position.Value.y);
            }
            else if ("z" == field)
            {
                return new FloatPara(field, player.position.Value.z);
            }
            else if ("pitch" == field)
            {
                return new FloatPara(field, player.orientation.Pitch);
            }
            else if ("yaw" == field)
            {
                return new FloatPara(field, player.orientation.Yaw);
            }
            else if ("currentWeaponKey" == field)
            {
                return new IntPara(field, FreeWeaponUtil.GetWeaponKey(player.WeaponController().HeldSlotType));
            }
            else if ("currentWeaponId" == field)
            {
                return new IntPara(field, player.WeaponController().HeldWeaponAgent.ConfigId);
            }
            else if ("inCar" == field)
            {
                return new BoolPara(field, player.IsOnVehicle());
            }
            else if ("team" == field)
            {
                return new IntPara(field, (int)player.playerInfo.Camp);
            }
            else if ("isDead" == field)
            {
                return new BoolPara(field, player.gamePlay.LifeState == (int)EPlayerLifeState.Dead);
            }else if("StrAvatarIds" == field)
            {
                string[] ids = new string[player.playerInfo.AvatarIds.Count];
                for(int i = 0; i < ids.Length; i++)
                {
                    ids[i] = player.playerInfo.AvatarIds[i].ToString();
                }
                return new StringPara(field, string.Join(",", ids));
            }

            return null;
        }

        public string[] GetFields()
        {
            return fields;
        }

        public bool HasField(string field)
        {
            return fieldSet.Contains(field);
        }
    }
}
