using System.Collections.Generic;
using System.Text.RegularExpressions;
using App.Client.Utility;
using Utils.Configuration;

namespace App.Client.GameModules.Ui.Models.Common
{
    public partial class CommonChatModel
    {
        private List<long> _recentUsePrivateChatPlayerList = new List<long>();

        private int MaxRecentUsePrivateChatCount
        {
            get { return 5; }
        }

        private Dictionary<long, string> TargetIdDict = new Dictionary<long, string>();
        private long _curTargetPlayerId;
        private string _curTargetPlayerName;

        private bool SwitchPrivateTarget()
        {
            int index = 0;

            if (_curTargetPlayerId != 0)
            {
                index = _recentUsePrivateChatPlayerList.FindIndex(it => it == _curTargetPlayerId) + 1;
            }

            if (index < _recentUsePrivateChatPlayerList.Count)
            {
                var id = _recentUsePrivateChatPlayerList[index];
                SetPrivateTarget(id);
                return true;
            }
          
            _curTargetPlayerId = 0;
            _curTargetPlayerName = string.Empty;
            return false;

        }

        private void SetPrivateTarget(long id)
        {
            _curTargetPlayerId = id;
            _curTargetPlayerName = TargetIdDict[id];
            string name = string.Empty;
            string text = string.Empty;
            ParseInputForPrivateChat(out name, out text);
            ShowPrivateTarget();

        }

        private void PrivateChatRecentUseQueueUpdate(long id)
        {
            //if (_curChannel == ChatChannel.PrivateChat)
            {
                int pos = 0;
                while (pos < _recentUsePrivateChatPlayerList.Count)
                {
                    if (_recentUsePrivateChatPlayerList[pos] == id)
                    {
                        _recentUsePrivateChatPlayerList.RemoveAt(pos);
                        break;
                    }
                    pos++;
                }

                if (_recentUsePrivateChatPlayerList.Count < MaxRecentUsePrivateChatCount)
                {
                    _recentUsePrivateChatPlayerList.Insert(0,id);
                }
            }
        }

        private bool CheckPersonalOnlineStatusAndSendPrivateChat()
        {
            bool res = false;
            string name = string.Empty;
            string message = string.Empty;
            ParseInputForPrivateChat(out name, out message);
            if (!string.IsNullOrEmpty(name))
            {
                res = true;
            }
            else if(_curChannel == ChatChannel.PrivateChat && !string.IsNullOrEmpty(_curTargetPlayerName))
            {
                res = true;
                name = _curTargetPlayerName;
                message = inputField.text;
            }
            else
            {
                return false;
            }
            if (!toBeSendDict.ContainsKey(name))
            {
                toBeSendDict.Add(name, new Queue<string>());
            }
            toBeSendDict[name].Enqueue(message);
            //inputField.text = string.Empty;
            HallUtility.SendCheckPersonalOnlineStatus(name);
            return res;
        }

        private Dictionary<string, Queue<string>> toBeSendDict = new Dictionary<string, Queue<string>>();
        private void ParseInputForPrivateChat(out string name, out string messageText)
        {
            string input = inputField.text;
            string pattern = "^/(?<Name>[\\s\\S]*?) (?<Message>[\\s\\S]*?)$";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                name = match.Groups["Name"].Value;
                messageText = match.Groups["Message"].Value;
                return;
            }

            name = string.Empty;
            messageText = string.Empty;
        }

        public void GetPersonalOnlineStatus(bool isOnline, string name, long id)
        {
            if (!TargetIdDict.ContainsKey(id))
            {
                TargetIdDict.Add(id, string.Empty);
            }
            TargetIdDict[id] = name;
            if (!isOnline)
            {
                toBeSendDict.Remove(name);
                AddMessageForUnablePrivateChat();
                return;
            }

            SendPrivateChat(id);
        }

        private void AddMessageForUnablePrivateChat()
        {
            string text = I2.Loc.ScriptLocalization.client_common.word42;
            var data = GetNewBroadcastMessageData(text);
            AddMessage(data);
        }

        private void SendPrivateChat(long id)
        {
            var str = toBeSendDict[TargetIdDict[id]].Dequeue();
            var data = GetNewBroadcastMessageData(str);
            data.ChatType = (int)ChannelToChatType(ChatChannel.PrivateChat);
            data.TargetId = id;
            HallUtility.SendChatMessage(data);
        }

    }
}
