using App.Shared.Components.Player;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IPaintUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 喷漆盘上对应的喷漆列表，索引对应的喷漆不存在时，Id为0
        /// </summary>
        List<int> PaintIdList { get; }

        /// <summary>
        /// 选择的喷漆索引,12点钟方向为起点，顺时针0~7
        /// </summary>
        int SelectedPaintIndex { get; set; }

        /// <summary>
        /// 执行喷漆动作
        /// </summary>
        void Paint();

        /// <summary>
        /// 开火，开镜时无法打开
        /// </summary>
        bool CanOpen { get; }

        void SetCrossVisible(bool val);

        void Select();

        GamePlayComponent gamePlay { get; }
    }


}
