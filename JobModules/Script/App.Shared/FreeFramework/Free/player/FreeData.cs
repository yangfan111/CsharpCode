using App.Server.GameModules.GamePlay.Free.player;
using App.Shared;
using com.wd.free.buf;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using com.wd.free.unit;
using Core.Free;
using Entitas;

namespace App.Server.GameModules.GamePlay.free.player
{
    public class FreeData : BaseGameUnit, IFreeData, IGameUnit
    {
        public PlayerEntity Player;
        public Contexts AllContexts;

        public FreeInventory freeInventory;

        public PlayerBuf Bufs;

        public PlayerStateTimer StateTimer;

        public FreeData(Contexts context, PlayerEntity player)
        {
            AllContexts = context;
            this.Player = player;
            this.key = "player";
            this.id = player.entityKey.Value.EntityId;

            this.skill = new UnitSkill(this);

            this.freeInventory = new FreeInventory();

            this.Bufs = new PlayerBuf(player);

            this.paras = new SimpleParaList();

            this.StateTimer = new PlayerStateTimer(player);

            AddFields(new ObjectFields(player.playerInfo));
            AddFields(new ObjectFields(player.gamePlay));
            AddFields(new ObjectFields(player.orientation));
        //    AddFields(new ObjectFields(player.weaponState));
            AddFields(new PlayerFields(AllContexts, player));
            if (player.hasPlayerMask)
            {
                AddFields(new ObjectFields(player.playerMask));
            }
        }

        public string Key { get { return Player.playerInfo.PlayerName; } }

        public void AddFields(IFields fields)
        {
            ((SimpleParaList)this.paras).AddFields(fields);
        }

        public FreeInventory GetFreeInventory()
        {
            return freeInventory;
        }

    }
}
