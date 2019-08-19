using System;
using System.Collections.Generic;
using Luminosity.IO;
using UnityEngine;

namespace UserInputManager.Lib
{
    /// <summary>
    /// 一个条目也可能用作组合键必要条件存在如SlightWalk
    /// axis,buttonDown => InputKey
    /// PointerMove，PointerRaycast => None
    /// Other =>Key
    /// </summary>
    [Serializable]
    public class KeyConvertItem
    {
        public KeyCode Key;
        
        public UserInputState State;
        /// <summary>
        /// 参考Setting->Input
        /// </summary>
        public string InputKey;
        /// <summary>
        /// 通用传值字段 
        /// </summary>
        public float FloatParam;

        // public KeyConvertItem(KeyCode key, UserInputState state)
        // {
        //     Key = key;
        //     State = state;
        //     InputKey = string.Empty;
        //     FloatParam = 0;
        // }
        //
        // public KeyConvertItem(KeyCode key, UserInputState state, int intparam)
        // {
        //     Key = key;
        //     State = state;
        //     InputKey = string.Empty;
        //     FloatParam = intparam;
        // }
        //
        // public KeyConvertItem(KeyCode key)
        // {
        //     Key = key;
        //     State = UserInputState.KeyDown;
        //     InputKey = string.Empty;
        //     FloatParam = 0;
        // }
        //
        // public KeyConvertItem(string key, UserInputState state)
        // {
        //     Key = KeyCode.None;
        //     State = state;
        //     InputKey = key;
        //     FloatParam = 0;
        // }


    }

    public class KeyConverter 
    {
        private Dictionary<UserInputKey, List<KeyConvertItem>> inputConvertDict = new Dictionary<UserInputKey, List<KeyConvertItem>>(UserInputKeyComparer.Instance);

        public void SetConfig(InputConfig cfg)
        {
            var items = cfg.Items;
            inputConvertDict.Clear();
            for (int i = 0; i < items.Length; i++)
            {
                inputConvertDict[items[i].Key] = items[i].Items;
            }

        }
        private readonly List<KeyConvertItem> emptyList = new List<KeyConvertItem>(0) { };

        public List<KeyConvertItem> Convert(UserInputKey code)
        {
            List<KeyConvertItem> list;
            if (inputConvertDict.TryGetValue(code, out list))
                return list;
            return emptyList;
        }
    }
}
