using System;
using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    public class KeyProvider : AbstractProvider<KeyHandler, KeyData>
    {

        public KeyData MakeKeyData(UserInputKey key, float axis)
        {
            var data = dataPool.GetData();
            data.Key = key;
            data.Axis = axis;
            return data;
        }

        protected override void DoCollect()
        {
            //没有按键按下的时候不执行收集操作
            if(!Input.anyKey && !Input.anyKeyDown)
            {
                //return;//没有anyKeyUp，会导致问题，暂时不优化，有性能问题再考虑分离keyup事件
            }
            foreach (var data in collection)
            {
                List<UserInputKey> userInputKeys = GetKeyList(data);
                foreach (UserInputKey inputKey in userInputKeys)
                {
                    // 一个行为可以绑定到多个按键，比如按回车和小键盘的回车做同一件事
                    List<KeyConvertItem> keys = keyConverter.Convert(inputKey);
                    foreach (var keyConvertItem in keys)
                    {
                        var state = keyConvertItem.State;
                        //按键消息需要检测是否响应
                        Func<KeyCode, bool> keyAction = null;
                        Func<string, bool> buttonAction = null;
                        Func<string, Luminosity.IO.PlayerID, float> axisAction = null;
                        switch (state)
                        {
                            case UserInputState.KeyDown:
                                keyAction = Luminosity.IO.InputManager.GetKeyDown;
                                break;
                            case UserInputState.KeyHold:
                                keyAction = Luminosity.IO.InputManager.GetKey;
                                break;
                            case UserInputState.KeyUp:
                                keyAction = Luminosity.IO.InputManager.GetKeyUp;
                                break;
                            case UserInputState.ButtonDown:
                                buttonAction = Luminosity.IO.InputManager.GetButtonDown;
                                break;
                            case UserInputState.ButtonUp:
                                buttonAction = Luminosity.IO.InputManager.GetButtonUp;
                                break;
                            case UserInputState.ButtonHold:
                                buttonAction = Luminosity.IO.InputManager.GetButton;
                                break;
                            case UserInputState.Axis:
                                axisAction = Luminosity.IO.InputManager.GetAxis;
                                break;
                            case UserInputState.AxisRow:
                                axisAction = Luminosity.IO.InputManager.GetAxisRaw;
                                break;
                        }

                        if (null != keyAction)
                        {
                            if (!keyAction(keyConvertItem.Key))
                            {
                                continue;
                            }
                        }

                        if (null != buttonAction)
                        {
                            if (!buttonAction(keyConvertItem.InputKey))
                            {
                                continue;
                            }
                        }

                        float axis = 0;
                        if (null != axisAction)
                        {
                            axis = axisAction(keyConvertItem.InputKey, Luminosity.IO.PlayerID.One);
                        }

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
