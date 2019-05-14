using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Components.Ui
{

    public interface IChickenBagItemUiData : IBaseChickenBagItemData
    {
        bool isBagTitle { get; set; }
        string title { get; set; }
    }

    public interface IBaseChickenBagItemData
    {
        int cat { get; set; }
        int id { get; set; }
        int count { get; set; }
        string key { get; set; }
    }

    
}
