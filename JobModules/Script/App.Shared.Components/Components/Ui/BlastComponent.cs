using System;
using Core.Ui.Map;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Utils.Configuration;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class BlastComponent : IComponent
    {
        //爆破点坐标
        [DontInitilize] public MapFixedVector3 BlastAPosition;
        [DontInitilize] public MapFixedVector3 BlastBPosition;

        //C4掉落点
        [DontInitilize] public MapFixedVector3 C4DropPosition;

        [DontInitilize] public bool IsC4Droped;

        [DontInitilize] public int C4SetStatus;

    }
}