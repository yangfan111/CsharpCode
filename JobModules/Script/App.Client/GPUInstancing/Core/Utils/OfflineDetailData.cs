using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Utils
{
    public class OfflineDetailData
    {
        public TextAsset WholeData;
        public List<int> StartIndices;
        public List<int> Lengths;
    }
}