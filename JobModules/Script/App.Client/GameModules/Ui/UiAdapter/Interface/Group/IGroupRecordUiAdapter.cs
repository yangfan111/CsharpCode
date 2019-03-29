using System.Collections.Generic;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter
{

    public interface IGroupRecordUiAdapter : IAbstractUiAdapter
    {

        /// <summary>
        /// 返回对应阵营的数据列表，泰坦阵营位于视图的上方，风暴阵营位于视图的下方
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<IGroupBattleData> GetBattleDataListByCampType(EUICampType type);

        /// <summary>
        /// 当前频道名称
        /// </summary>
        string ChannelName { get;}

        /// <summary>
        /// 当前房间名称
        /// </summary>
        string RoomName { get;}

        /// <summary>
        /// 当前房间Id
        /// </summary>
        int RoomId { get;}

        /// <summary>
        /// 当前参与游戏的玩家人数
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// 当前游戏支持的最大玩家人数
        /// </summary>
       int PlayerCapacity { get; }

        bool NeedUpdate { get; set; }

        EUICampType MyCamp { get;}

    }
}
