using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ArtPlugins
{
    [DisallowMultipleComponent]
    public class MultiTag : MultiTagBase
    {
        /// <summary>
        /// 是否为室外物件
        /// </summary>
        public bool IsOutsideProp()
        {
            return btags[(int)TagEnum.OutProps];
        }

        public bool IsInsideProp()
        {
            return btags[(int)TagEnum.InOnlyProps] || btags[(int)TagEnum.InBothProps];
        }
    }
}