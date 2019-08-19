using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Client.GameModules.Ui.Models.Common.Tip
{
    public class TipPartUiData
    {
        public int Type{ get; set; }
        public int Quality { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        public bool HaveWeaponPart()
        {
            return Id > 0; 
        }
    }
}
