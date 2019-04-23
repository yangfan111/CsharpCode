using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Appearance
{
    /// <summary>
    /// 该接口具有数组的功能,获取数组的长度，获取数组this[int index]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAdaptiveContainer<T>
    {
        T GetAvailableItem();
        T GetAvailableItem(Func<T, bool> getItemCondition);
        T this[int index] { get; set; }
        int Length { get; }
    }
}
