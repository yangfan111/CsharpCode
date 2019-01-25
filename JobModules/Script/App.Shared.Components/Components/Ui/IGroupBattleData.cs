using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Components.Ui
{
    public interface IGroupBattleData
    {
        /// <summary>
        /// 游戏内头衔，多状态（使用(2^GameTitleType)异或|处理,如同时满足KD王和ACE，那么该值为2|4=6）
        /// </summary>
        int GameTitle { get; set; }
        /// <summary>
        /// 玩家名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 战队名称
        /// </summary>
        string CorpsName { get; set; }
        /// <summary>
        /// 玩家的击杀数量
        /// </summary>
        int KillCount { get; set; }
        /// <summary>
        /// 玩家的死亡次数
        /// </summary>
        int DeadCount { get; set; }
        /// <summary>
        /// 玩家的助攻次数
        /// </summary>
        int AssistCount { get; set; }
        /// <summary>
        /// 玩家本场的有效伤害量
        /// </summary>
        int Damage { get; set; }
        /// <summary>
        /// 网络延迟
        /// </summary>
        int Ping { get; set; }
        /// <summary>
        /// 玩家是否处于死亡状态
        /// </summary>
        bool IsDead { get; set; }
        /// <summary>
        /// 是否是当前玩家的统计信息
        /// </summary>
        bool IsMySelf { get; set; }
    }
}
