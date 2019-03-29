using System;
using App.Client.GameModules.Ui.ViewModels.Group;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using Assets.UiFramework.Libs;
using Core.Enums;
using Core.GameModule.Interface;
using Core.Utils;
using UIComponent.UI;
using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Models.Group
{

    public class GroupRecordModel : ClientAbstractModel, IUiSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GroupRecordModel));
        private GroupRecordViewModel _viewModel = new GroupRecordViewModel();

        private IGroupRecordUiAdapter _adapter;

        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected int MaxInfoCount
        {
            get { return 8; }
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitVariable();
            InitInfoGroup();
            InitInfoTitle();
            InitKey();
        }

        private void InitKey()
        {
            KeyReceiver = new KeyReceiver(Layer.Env, BlockType.None);
            KeyReceiver.AddAction(UserInputKey.ShowRecord, (data) => { _adapter.Enable = true; });
            KeyReceiver.AddAction(UserInputKey.HideRecord, (data) => { _adapter.Enable = false; });
            _adapter.RegisterKeyReceive(KeyReceiver);
        }

        private void InitInfoTitle()
        {
            var titleList1 = FindComponent<UIList>("TitleGroup1");
            var titleList2 = FindComponent<UIList>("TitleGroup2");
            InitTitleList(titleList1, EUICampType.T);
            InitTitleList(titleList2, EUICampType.CT);
        }

        private void InitTitleList(UIList titleList, EUICampType campType)
        {
            if (titleList == null) return;
            titleList.Clear();
            InitTitleInfo(titleList, campType);
            titleList.CreatItemObj();
        }

        protected virtual void InitTitleInfo(UIList titleList, EUICampType campType)
        {
            titleList.Add<GroupRecordInfo>(new GroupRecordViewData() { IsTitle = true, NeedShow = true});
        }

        private void InitVariable()
        {
            _campInfoList1 = FindComponent<UIList>("InfoGroup1");
            _campInfoList2 = FindComponent<UIList>("InfoGroup2");
        }

        private UIList _campInfoList1, _campInfoList2;
        private KeyReceiver KeyReceiver;

        private void InitInfoGroup()
        {
            InitInfoGroup(_campInfoList1, EUICampType.T);
            InitInfoGroup(_campInfoList2, EUICampType.CT);
        }

        private void InitInfoGroup(UIList campInfoList, EUICampType campType)
        {
            if (campInfoList == null) return;
            campInfoList.Clear();
            InitAllRecordInfo(campInfoList, campType);
            campInfoList.CreatItemObj();
        }

        protected virtual void InitAllRecordInfo(UIList campInfoList, EUICampType campType)
        {
            for (int i = 0; i < MaxInfoCount; i++)
            {
                campInfoList.Add<GroupRecordInfo>(new GroupRecordViewData() { IsTitle = false, NeedShow = false, Rank = i + 1 });
            }
        }

        public GroupRecordModel(IGroupRecordUiAdapter groupTechStatState):base(groupTechStatState)
        {
            _adapter = groupTechStatState;
            _adapter.Enable = false;
        }

        public override void Update(float interval)
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (!_adapter.NeedUpdate) return;
            UpdateInfoView();
            _adapter.NeedUpdate = false;
        }

        private void UpdateInfoView()
        {
            UpdateRoomInfo();
            UpdatePlayerCount();
            UpdateInfoView(EUICampType.T);
            UpdateInfoView(EUICampType.CT);
        }

        private void UpdatePlayerCount()
        {
            var curPlayer = _adapter.PlayerCount;
            var maxPlayer = _adapter.PlayerCapacity;
            _viewModel.PlayerCountText = string.Format(I2.Loc.ScriptLocalization.client_common.word43, curPlayer, maxPlayer);
        }

        private void UpdateRoomInfo()
        {
            var channel = _adapter.ChannelName;
            var roomName = _adapter.RoomName;
            var roomId = _adapter.RoomId;
            _viewModel.RoomInfoText = string.Format("{0}/{1}/{2}", channel, roomName, roomId);
        }

        private void UpdateInfoView(EUICampType campType)
        {
            var list = _adapter.GetBattleDataListByCampType(campType);
            var trans = campType == EUICampType.T ? _campInfoList1 : campType == EUICampType.CT ? _campInfoList2:null;
            int index = 0;
            if (trans == null) return;
            for (; index < list.Count && index < MaxInfoCount; index++)
            {
                UpdateInfoViewData(list[index], trans[index]);
                UpdateInfoShow(trans[index], true);
            }

            for (; index < MaxInfoCount; index++)
            {
                UpdateInfoShow(trans[index], false);
            }
        }

        private void UpdateInfoViewData(IGroupBattleData groupBattleData, IUIItem iUIItem)
        {
            (iUIItem.Data as GroupRecordViewData).BattleData = groupBattleData;
        }

        protected virtual void UpdateInfoShow(IUIItem iUIItem, bool show)
        {
            (iUIItem.Data as GroupRecordViewData).NeedShow = show;
            (iUIItem as GroupRecordInfo).UpdateInfoView();
        }
    }
}
