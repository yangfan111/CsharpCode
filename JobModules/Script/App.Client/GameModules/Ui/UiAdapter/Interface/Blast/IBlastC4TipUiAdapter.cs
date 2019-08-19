using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Core.Enums;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Blast
{
    public interface IBlastC4TipUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// C4安装的状态，Installed表示安装完成，显示图标；Explode表示爆炸；安装中断或者游戏重新开始设为None
        /// </summary>
        EUIBombInstallState C4InstallState { get;}
        /// <summary>
        /// C4初始进度
        /// </summary>
        float C4InitialProgress { get; }
    }
}
