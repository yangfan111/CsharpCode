using System.Collections.Generic;
using System.Linq;
using App.Shared.Components.Ui;
using Core.Enums;
using Utils.Configuration;
using NotImplementedException = System.NotImplementedException;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public class GroupBattleData : IGroupBattleData
    {
        public int GameTitle { get; set; }
        public string Name { get; set; }
        public string CorpsName { get; set; }
        public int KillCount { get; set; }
        public int DeadCount { get; set; }
        public int AssistCount { get; set; }
        public int Damage { get; set; }
        public int Ping { get; set; }
        public bool IsDead { get; set; }
        public bool IsMySelf { get; set; }
        public int C4PlantCount { get; set; }
        public int C4DefuseCount { get; set; }
        public bool HaveC4 { get; set; }
        public int BadgeId { get; set; }
        public bool IsHurt { get; set; }
        public int HitDownCount { get; set; }
        public int ResqueCount { get; set; }
    }

    public class GroupRecordViewData
    {
        public int Rank { get; set; }
        public IGroupBattleData BattleData { get; set; }
        public bool IsTitle { get; set; }
        public bool NeedShow { get; set; }
        public EUICampType CampType { get; set; }
        public bool CanShowC4 { get; set; }
        public bool CanResque { get; set; }
    }

    public class GroupRecordUiAdapter : UIAdapter, IGroupRecordUiAdapter
    {

        public override bool Enable
        {
            get
            {
                if (IsConflict) _enable = false;
                return _enable;
            }

            set { _enable = value; }
        }

        private Contexts _contexts;

        public GroupRecordUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }


        public int GetScoreByCampType(EUICampType type)
        {
            return _contexts.ui.uI.ScoreByCampTypeDict[(int)type];
        }

        public List<IGroupBattleData> GetBattleDataListByCampType(EUICampType type)
        {
            var list = _contexts.ui.uI.GroupBattleDataDict[(int)type];
            return SortList(list);
        }

        //排序规则:击杀、阵亡倒序、助攻、伤害、到达时间(非救援)
        //击倒、阵亡倒序、救助、伤害、到达顺序(救援)
        private List<IGroupBattleData> SortList(IList<IGroupBattleData> list) 
        {
            if (CanRescue)
            {
                return list.OrderByDescending(it => it.HitDownCount).ThenBy(it => it.DeadCount)
                    .ThenByDescending(it => it.ResqueCount)
                    .ThenByDescending(it => it.Damage).ToList();
            }
            return list.OrderByDescending(it => it.KillCount).ThenBy(it => it.DeadCount)
                .ThenByDescending(it => it.AssistCount)
                .ThenByDescending(it => it.Damage).ToList();
        }

        public string ChannelName
        {
            get { return _contexts.ui.uI.ChannelName; }
        }

        public string RoomName
        {
            get { return _contexts.ui.uI.RoomName; }
        }

        public int PlayerCount
        {
            get { return _contexts.ui.uI.PlayerCount; }
        }

        public int PlayerCapacity
        {
            get { return _contexts.ui.uI.PlayerCapacity; }
        }

        public int RoomId
        {
            get { return _contexts.ui.uI.RoomId; }
        }

        public bool IsConflict
        {
            get { return _contexts.ui.chat.ChatListState == EUIChatListState.Send; }
        }

        public bool NeedUpdate
        {
            get { return _contexts.ui.uI.GroupBattleDataChanged; }
            set { _contexts.ui.uI.GroupBattleDataChanged = value;}
        }

        public EUICampType MyCamp
        {
            get
            {
                PlayerEntity myEntity = _contexts.player.flagSelfEntity;
                if (null != myEntity)
                {
                    return (EUICampType)myEntity.playerInfo.Camp;
                }

                return EUICampType.None;
            }
        }

        public bool CanRescue
        {
            get
            {
                return _contexts.session.commonSession.RoomInfo.TeamCapacity > 1;
            }
        }
    }
}
