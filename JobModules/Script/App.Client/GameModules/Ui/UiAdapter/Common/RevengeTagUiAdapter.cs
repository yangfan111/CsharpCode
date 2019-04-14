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
                if (KillerPlayerEntity == null)
                    return Vector3.zero;
                return PlayerEntityUtility.GetPlayerTopPosition(KillerPlayerEntity);
            }
        }

        public bool KillerChanged
        {
            get
            {
                var changed = _contexts.ui.uI.KillerChanged;
                if (changed)
                {
                    KillerPlayerEntity = null;
                    foreach (PlayerEntity pe in _contexts.player.GetEntities())
                    {
                        if (pe.playerInfo.PlayerId == _contexts.ui.uI.KillerId)
                        {
                            KillerPlayerEntity = pe;
                            break;
                        }
                    }
                }
                return changed;
            }
            set { _contexts.ui.uI.KillerChanged = false; }
        }

        public long KillerId
        {
            get { return _contexts.ui.uI.KillerId; }
        }

        
        private PlayerEntity KillerPlayerEntity
        {
            set;get;
        }


        public bool IsKillerDead
        {
            get
            {
                if (KillerPlayerEntity == null) return true;
                if (KillerPlayerEntity.gamePlay.IsDead())//杀手第一次死亡就清空
                {
                    KillerPlayerEntity = null;
                    return true;
                }
                return false;
            }
        }
    }
}
