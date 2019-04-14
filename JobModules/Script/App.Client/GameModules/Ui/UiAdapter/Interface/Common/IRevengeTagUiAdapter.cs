using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter.Interface.Common
{
    public interface IRevengeTagUiAdapter
    {
        Vector3 KillerTopPos { get;}
        bool KillerChanged { get; set; }
        long KillerId { get; }
        bool IsKillerDead { get; }
    }
}
