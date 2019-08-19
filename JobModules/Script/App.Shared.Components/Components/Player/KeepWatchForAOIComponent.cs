using Entitas;
using System;
using System.Collections.Generic;
using Core.EntityComponent;
using Core.EntityComponent;

namespace App.Shared.Components.Player
{
    [Player]
    public class KeepWatchForAOIComponent: IComponent
    {
        /// <summary>
        ///  �����ڲ��������key �Լ� ������ֵ����Ϣ��Ŀǰ��Ϣ��ʱ��(��λms)���Ժ�Ҳ�����Ǳ����Ϣ��
        /// </summary>
        public IWatchDict watchMap;
    }

}