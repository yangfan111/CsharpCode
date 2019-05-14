using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter.Interface
{
    public interface IBiochemicalMarkUiAdapter
    {
        /// <summary>
        /// 需要显示头顶标志的母体id列表
        /// </summary>
        List<long> MotherIdList { get; }
        /// <summary>
        /// 需要显示头顶标志的英雄id列表
        /// </summary>
        List<long> HeroIdList { get; }
        /// <summary>
        /// 需要显示头顶标志的人类id列表
        /// </summary>
        List<long> HumanIdList { get; }

        /// <summary>
        /// 获取头顶坐标
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Vector3? GetTopPos(long id);
    }
}
