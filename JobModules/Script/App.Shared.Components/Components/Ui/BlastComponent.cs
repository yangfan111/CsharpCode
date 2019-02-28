using System;
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
        [DontInitilize] public Vector3 BlastAPosition;
        [DontInitilize] public Vector3 BlastBPosition;

        //C4掉落点
        [DontInitilize] public Vector3 C4DropPosition;

        [DontInitilize] public bool IsC4Droped;

        [DontInitilize] public int C4SetStatus;

    }
}