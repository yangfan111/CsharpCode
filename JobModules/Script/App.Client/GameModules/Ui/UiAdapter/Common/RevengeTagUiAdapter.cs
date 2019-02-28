using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Shared.GameModules.Player;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{
    public class RevengeTagUiAdapter : UIAdapter, IRevengeTagUiAdapter
    {
        private Contexts _contexts;

        public RevengeTagUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public Vector3 KillerTopPos
        {
            get
            {
                foreach (PlayerEntity pe in _contexts.player.GetEntities())
                {
                    if (pe.playerInfo.PlayerId == _contexts.ui.uI.KillerId)
                    {
                        return PlayerEntityUtility.GetPlayerTopPosition(pe);
                    }
                }
                return Vector3.zero;
            }
        }

        public bool KillerChanged
        {
            get { return _contexts.ui.uI.KillerChanged; }
            set { _contexts.ui.uI.KillerChanged = false; }
        }
    }
}
