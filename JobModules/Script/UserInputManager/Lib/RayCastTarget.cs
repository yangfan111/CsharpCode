using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    /// <summary>
    /// 用于获取射线判定信息, 没有使用泛型，因为GetComponent时不太好操作
    /// 如果有好的办法请修改成泛型
    /// </summary>
    //[DisallowMultipleComponent]
    public class RayCastTarget : MonoBehaviour
    {
        /// <summary>
        /// Id 信息，用于标记，如果对象是值类型，防止装箱拆箱
        /// </summary>
        public List<int> IdList = new List<int>();
        /// <summary>
        /// 用于存储信息，如果是引用类型，可以直接把引用存下来
        /// </summary>
        public object Data;
        /// <summary>
        /// 用于标记是用作哪些操作
        /// </summary>
        public List<UserInputKey> KeyList = new List<UserInputKey>();

        public override string ToString()
        {
            var content = "Id : ";
            foreach (var id in IdList)
            {
                content += id + ",";
            }
            content += string.Format("DataType is {0}, KeyList: \n", null == Data ? "null" : Data.GetType().ToString());
            foreach (var userInputKey in KeyList)
            {
                content += userInputKey + ",";
            }
            return content;
        }
    }
}
