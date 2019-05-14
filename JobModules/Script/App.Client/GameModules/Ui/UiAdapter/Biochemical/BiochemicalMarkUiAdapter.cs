using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Client.GameModules.Ui.UiAdapter.Interface;
using App.Shared.GameModules.Player;
using UnityEngine;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class BiochemicalMarkUiAdapter : UIAdapter, IBiochemicalMarkUiAdapter
    {
        private Contexts _contexts;
        private Shared.Components.Ui.UIComponent _ui;
        public BiochemicalMarkUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _ui = _contexts.ui.uI;
        }


        public List<long> MotherIdList
        {
            get
            {
                return _ui.MotherIdList;
            }
        }

        public List<long> HeroIdList
        {
            get
            {
                return _ui.HeroIdList;
            }
        }

        public List<long> HumanIdList
        {
            get
            {
                return _ui.HumanIdList;
            }
        }

        public Vector3? GetTopPos(long id)
        {
            
            foreach (PlayerEntity pe in _contexts.player.GetEntities())
            {
                if (pe.playerInfo.PlayerId == id)
                {
                    return PlayerEntityUtility.GetPlayerTopPosition(pe);
                }
            }
            return null;
        }
    }
}
