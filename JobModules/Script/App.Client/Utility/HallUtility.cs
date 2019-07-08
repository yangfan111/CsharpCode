using System;
using System.Collections;
using System.Collections.Generic;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.Utils;
using UnityEngine;

namespace App.Client.Utility
{
    public class HallUtility
    {
        private LoggerAdapter _logger = new LoggerAdapter(typeof(HallUtility));

        public static string Method_StartGame = "StartGame";
        public static string Method_DisposeClient = "DisposeClient";
        public static string Method_SendChatMessageToGamne = "SendChatMessageToGame";
        public static string Method_SendCheckPersonalOnlineStatusToGame = "SendCheckPersonalOnlineStatusToGame";
        public static string Method_ShowAlert = "Method_ShowAlert";
        public static string Method_ShowCrossHair = "Method_ShowCrossHair";


        private static GameObject _hall;
        private static Dictionary<string,List<Action<object>>> _actions = new Dictionary<string, List<Action<object>>>();

        
        private static GameObject Hall
        {
            get
            {
                if (_hall == null) _hall = GameObject.Find("HallGameController");
                return _hall;
            }
        }

        public static void MessageReceived(ArrayList args)
        {
            if (args.Count > 0)
            {
                string methodName = args[0] as String;
                object value = args[1];
                if (_actions.ContainsKey(methodName))
                {
                    foreach (var action in _actions[methodName])
                    {
                        action.Invoke(value);
                    }
                }
            }
        }

        private static void SendMessage(string method, object value = null)
        {
            if (Hall == null) return;
            ArrayList args = new ArrayList(2){ method , value };
            Hall.SendMessage("CallByClient", args);
        }

        public static void RegisterCallback(string method , Action<object> action)
        {
            if (_actions.ContainsKey(method) == false)
            {
                _actions.Add(method, new List<Action<object>>());
            }

            _actions[method].Add(action);
        }
        public static void UnRegisterCallback(string method, Action<object> action)
        {
            if (_actions.ContainsKey(method))
            {
                foreach (var ac in _actions[method])
                {
                    if (ac.Equals(action))
                    {
                        _actions[method].Remove(action);
                        return;
                    }
                }
            }

        }

        public static void ClearAciton()
        {
            _actions.Clear();
        }


        public static void GetGameClientInfo()
        {
            SendMessage("GetGameClientInfo");
        }

        public static void GameOver()
        {
            SendMessage("OnGameOver");
        }

        public static void DisposeClientComplete()
        {
            SendMessage("DisposeClientComplete");
        }

        public static void ShowSettingWindow(Action<bool> action)
        {
            SendMessage("ShowSettingWindow", action);
        }
        public static void HideSettingWindow()
        {
            SendMessage("HideSettingWindow");
        }
        public static void SetSettingWindowNovisCallBack(Action<bool> action)
        {
            SendMessage("SetSettingWindowNovisCallBack", action);
        }

        public static void SendChatMessage(BroadcastMessageData data)
        {
            if (string.IsNullOrEmpty(data.ChatMessage.Message))
            {
                return;
            }
            SendMessage("SendChatMessage", data);
        }

        public static void SendNearChatMessage(NearbyChatData data)
        {
            SendMessage("SendNearChatMessage", data);
        }

        public static void SendCheckPersonalOnlineStatus(string name)
        {
            SendMessage("SendCheckPersonalOnlineStatus", name);
        }

        public static void SendCampChatMessage(CampChatData data)
        {
            SendMessage("SendCampChatMessage", data);
        }

     
    }
}