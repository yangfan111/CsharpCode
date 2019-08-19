using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Luminosity.IO;
using UnityEngine;

namespace UserInputManager.Lib
{
    public class KeyProvider : AbstractProvider<KeyReceiver, KeyData>
    {
        StringBuilder stringBuilder = new StringBuilder();

        public KeyData MakeKeyData(UserInputKey key, float axis)
        {
            var data = dataPool.GetData();
            data.Key  = key;
            data.Axis = axis;
            return data;
        }

        protected override void DoCollect()
        {
            //没有按键按下的时候不执行收集操作
            // if(!Input.anyKey && !Input.anyKeyDown)
            // {
            //     //return;//没有anyKeyUp，会导致问题，暂时不优化，有性能问题再考虑分离keyup事件
            // }

       //     int count = Time.frameCount;
            Func<KeyCode, bool>           keyAction    = null;
            Func<string, bool>            buttonAction = null;
            Func<string, PlayerID, float> axisAction   = null;
            InputInspectType inspectType;
            bool stepPass = false;
            float axis = 0;
            foreach (KeyReceiver data in collection)
                            //{
                            //    stringBuilder.Length = 0;
                            //    stringBuilder.AppendFormat("[{0}][{1}] ", count,data);
            {
                List<UserInputKey> userInputKeys = GetKeyList(data);
                foreach (UserInputKey inputKey in userInputKeys)
                {
                    // 一个行为可以绑定到多个按键，比如按回车和小键盘的回车做同一件事
                    List<KeyConvertItem> keys = keyConverter.Convert(inputKey);
                    foreach (var keyConvertItem in keys)
                    {
                        stepPass = false;
                        var state = keyConvertItem.State;
                        //按键消息需要检测是否响应
                        keyAction    = null;
                        buttonAction = null;
                        axisAction   = null;
                        inspectType = InputInspectType.None;
                        switch (state)
                        {
                            case UserInputState.KeyDown:
                                keyAction = InputManager.GetKeyDown;
                                inspectType = InputInspectType.Key;
                                break;
                            case UserInputState.KeyHold:
                                keyAction = InputManager.GetKey;
                                inspectType = InputInspectType.Key;
                                break;
                            case UserInputState.KeyUp:
                                keyAction = InputManager.GetKeyUp;
                                inspectType = InputInspectType.Key;
                                break;
                            case UserInputState.ButtonDown:
                                buttonAction = InputManager.GetButtonDown;
                                inspectType = InputInspectType.Button;
                                break;
                            case UserInputState.ButtonUp:
                                buttonAction = InputManager.GetButtonUp;
                                inspectType = InputInspectType.Button;
                                break;
                            case UserInputState.ButtonHold:
                                buttonAction = InputManager.GetButton;
                                inspectType = InputInspectType.Button;
                                break;
                            case UserInputState.Axis:
                                axisAction = InputManager.GetAxis;
                                inspectType = InputInspectType.Aixs;
                                break;
                            case UserInputState.AxisRow:
                                axisAction = InputManager.GetAxisRaw;
                                inspectType = InputInspectType.Aixs;

                                break;
                        }
        
                        switch (inspectType)
                        {
                            case InputInspectType.Aixs:
                                axis = axisAction(keyConvertItem.InputKey, PlayerID.One);
                                stepPass = true;
                                break;
                            case InputInspectType.Button:
                                stepPass = buttonAction(keyConvertItem.InputKey);
                                break;
                            case InputInspectType.Key:
                                stepPass = keyAction(keyConvertItem.Key);
                                break;
                        }

                        if (stepPass)
                        {
                            var keydata = MakeKeyData(inputKey, axis);
                            KeyDatas.Enqueue(keydata);
                            // 只要有一个条件满足就如队列，后续条件就不判断了

                            break;
                        }
                     
                    }
                }
            }
        }
    }
}