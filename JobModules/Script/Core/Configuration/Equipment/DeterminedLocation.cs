using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Configuration.Equipment
{
    public class DeterminedLocation
    {
        public int Id;
        public Dictionary<int, DeterminedResourceItem> ResourceItems;
    }


    public class DeterminedResourceItem
    {
        public int Id;
        public int ResourceItemId;
        public Vector3 Position;

        private int _number;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value >= 0 ? value : 0;
                if (NumberChanged != null)
                {
                    NumberChanged(_number);
                }
            }
        }
        public Action<int> NumberChanged;

        public bool Updated;
    }
}
