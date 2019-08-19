using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Components.Ui;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Utils.Configuration;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IChatUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 是否单人模式
        /// </summary>
        bool IsSinglePlayer { get; }   
        /// <summary>
        /// 附近玩家Id列表(不包括自己，200米)
        /// </summary>
        List<long> NearPlayerInfo { get;}
        /// <summary>
        /// 当前模式对应的聊天频道Id列表,见GameMode表
        /// </summary>
        List<int> ChannelList { get; }
        /// <summary>
        /// 当前模式对应的默认频道
        /// </summary>
        int DefaultChannel { get; }
        /// <summary>
        /// 玩家是否有战队
        /// </summary>
        bool HaveCorps { get; }
        /// <summary>
        /// 同阵营玩家Id列表
        /// </summary>
        List<long> CampPlayerInfo { get; }
        /// <summary>
        /// 玩家自己的Id，用于处理私聊消息
        /// </summary>
        long MyselfId { get; }
        EUIChatListState ChatListState { get; set; }

        void SetCrossVisible(bool isVisible);

        Action<object> AddMessageAction { get; set; }
        Action<object> GetPersonalOnlineStatusCallback { get; set; }
        GamePlayComponent gamePlay { get; }
    }
}
