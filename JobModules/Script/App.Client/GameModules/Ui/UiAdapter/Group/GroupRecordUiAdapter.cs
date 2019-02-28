using System.Collections.Generic;
using System.Linq;
using App.Shared.Components.Ui;
using Core.Enums;
using Utils.Configuration;

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
    }

    public class GroupRecordViewData
    {
        public int Rank { get; set; }
        public IGroupBattleData BattleData { get; set; }
        public bool IsTitle { get; set; }
        public bool NeedShow { get; set; }
        public EUICampType CampType { get; set; }
        public bool CanShowC4 { get; set; }
    }

    public class GroupRecordUiAdapter : UIAdapter, IGroupRecordUiAdapter
    {

        public override bool Enable
        {
            get { return _enable && !IsConflict; }

            set { _enable = value; }
        }

        private Contexts _contexts;

        public GroupRecordUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
        }


        public List<IGroupBattleData> GetBattleDataListByCampType(EUICampType type)
        {
            var list = _contexts.ui.uI.GroupBattleDataDict[type];
            return SortList(list);
        }


        private List<IGroupBattleData> SortList(IList<IGroupBattleData> list) //排序规则:击杀、阵亡、助攻、到达时间
        {
            return list.OrderByDescending(it => it.KillCount).ThenBy(it => it.DeadCount)
                .ThenByDescending(it => it.AssistCount).ToList();
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
    }
}
