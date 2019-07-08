using System.Collections.Generic;
using System.Text.RegularExpressions;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.Utility;
using App.Client.GameModules.Ui.UiAdapter;
using Assets.UiFramework.Libs;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.GameModule.Interface;
using Core.Utils;
using DG.Tweening;
using Google.Protobuf;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.Configuration;
using System.Collections;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Models.Common
{


    public enum ChatType
    {
        System = 1,
        Hall = 2,
        Room = 4,
        Team = 8,
        PrivateChat = 16,
        Corps = 32,
        Near = 64,
        GameTeam = 128,
        Camp = 256,
        GameAll = 512,
        Default = 0
    }

    public partial class CommonChatModel : ClientAbstractModel, IUiSystem
    {
        private int MaxMessageCount
        {
            get { return 50; }
        }

        private EUIChatListState ChatListState
        {
            get { return _chatState.ChatListState; }
            set { _chatState.ChatListState = value; }
        }
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CommonChatModel));
        private CommonChatViewModel _viewModel = new CommonChatViewModel();

        private IChatUiAdapter _chatState;
        private float _lastMessageTime = 0;
        private InputField inputField;
        private Tween _closeViewAnim;
        private ChatChannel _curChannel = ChatChannel.None;
        private Transform parent;
        private Transform item;
        private KeyReceiver switchKeyReceiver;

        private Dictionary<Transform, BroadcastMessageData> messageDict = new Dictionary<Transform, BroadcastMessageData>();

        private List<int> channelList = new List<int>();
      
        protected override IUiViewModel ViewModel
        {
            get { return _viewModel; }
        }

        public float RemainTime
        {
            get { return 10; }
        }

        public CommonChatModel(IChatUiAdapter chatState):base(chatState)
        {
            _chatState = chatState;
            chatState.AddMessageAction = (data) => { AddMessage(data as BroadcastMessageData); };
            chatState.GetPersonalOnlineStatusCallback = (data)=>
            {
                var list = data as ArrayList;
                bool isOnline = (bool)list[0];
                string name = (string)list[1];
                long id = (long)list[2];
                GetPersonalOnlineStatus(isOnline,name,id);
            };
        }

        protected override void OnGameobjectInitialized()
        {
            base.OnGameobjectInitialized();
            InitKeyReveiver();
            InitVariable();
            InitAnim();
            InitChannel();
        }

        private void InitChannel()
        {
            var origList = _chatState.ChannelList;
            foreach (var it in origList)
            {
                if (it == (int)ChatChannel.Corps && !_chatState.HaveCorps)
                {
                    continue;
                }
                channelList.Add(it);
            }
            _curChannel = (ChatChannel)_chatState.DefaultChannel;

            if (_curChannel == ChatChannel.GameTeam && _chatState.IsSinglePlayer)
            {
                _curChannel = (ChatChannel) channelList[0];
            }
            
            UpdateChannel();
        }

        private void InitVariable()
        {
            parent = FindChildGo("Content");
            item = FindChildGo("ChatMessage");
            item.gameObject.SetActive(false);
            inputField = FindChildGo("InputField").GetComponent<InputField>();
            _viewModel.InputValueChanged = InputValueOnChanged;
            //new UIInputValid(inputField, new SenesitiveWord(), replaceStr: "*");
        }

        private void InitAnim()
        {
            _closeViewAnim = DOTween.To(() => _viewModel.Alpha, (x) => _viewModel.Alpha = x, 0, 1f);
            _closeViewAnim.SetAutoKill(false);
            _closeViewAnim.Pause();
            _closeViewAnim.OnComplete(() =>
            {
                ChatListState = EUIChatListState.None;
                ResetTime();
            });
        }

        public void CloseSendView()
        {
            ChatListState = EUIChatListState.None;
        }

        private void InputValueOnChanged(string str)
        {
            inputField.text = DeleteEnterInString(str);
        }

        private string DeleteEnterInString(string str)
        {
            string input = str;
            string pattern = "\\r+|\\n+";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            return rgx.Replace(input, replacement);
        }

        public override void Update(float interval)
        {
            if (!isVisible || !_viewInitialized) return;

            UpdateChatListState();
            UpdateInputKey();
            UpdateView();
        }

        bool haveRegister = false;
        private void UpdateInputKey()
        {
            if (switchKeyReceiver == null)
            {
                return;
            }
            if (ChatListState == EUIChatListState.Send)
            {
                if (!haveRegister)
                {
                    _chatState.RegisterKeyReceive(switchKeyReceiver);
                    haveRegister = true;
                }
            }
            else
            {
                if (haveRegister)
                {
                    _chatState.UnRegisterKeyReceive(switchKeyReceiver);
                    haveRegister = false;
                }
            }
        }

        private void InitKeyReveiver()
        {
            var keyReceiver = new KeyReceiver(UiConstant.chatWindowLayer, BlockType.None);
            keyReceiver.AddAction(UserInputKey.SendChatMessage, (data) =>
            {
                if (ChatListState != EUIChatListState.Send)
                {
                    SwitchToSendState();
                }
            });
            switchKeyReceiver = new KeyReceiver(UiConstant.chatWindowKeyBlockLayer, BlockType.All);
            switchKeyReceiver.AddAction(UserInputKey.SendChatMessage, (data) => { SendMessage();});
            switchKeyReceiver.AddAction(UserInputKey.SwitchChatChannel, (data) =>
            {
                SwitchChannel();
                UpdateChannel();
            });

            //_chatState.RegisterKeyReceive(keyReveiver);
            _chatState.RegisterOpenKey(keyReceiver);
        }

        private void SwitchChannel()
        {
            if (_curChannel == ChatChannel.PrivateChat)
            {
                if (SwitchPrivateTarget())
                {
                    return;
                }
            }
            int index = channelList.FindIndex(it=>it == (int)_curChannel) + 1;

            if (index == channelList.Count)
            {
                index = 0;
            }
            if ((ChatChannel)channelList[index] != ChatChannel.PrivateChat)
            {
                ClearPrivateTarget();
            }
            _curChannel = (ChatChannel)channelList[index];

            //if (_curChannel == ChatChannel.PrivateChat && _recentUsePrivateChatPlayerList.Count == 0)
            if (_curChannel == ChatChannel.PrivateChat)
            {
                SwitchChannel();
            }
        }

        private void ClearPrivateTarget()
        {
            string name = string.Empty;
            string text = string.Empty;
            ParseInputForPrivateChat(out name, out text);
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            bool isInQueue = false;
            foreach (var it in _recentUsePrivateChatPlayerList)
            {
                if (TargetIdDict[it] == name)
                {
                    isInQueue = true;
                }
            }

            if (isInQueue)
            {
                inputField.text = text;
            }
            
        }

        private void SwitchToSendState()
        {
            ChatListState = EUIChatListState.Send;
            _chatState.SetCrossVisible(true);
            ResetCloseViewAnim();
        }

        private void UpdateChannel()
        {
            _viewModel.ChannelTipText = GetChannelChannelTip(_curChannel);
            _viewModel.ChannelTipColor = UiCommonColor.GetChatChannelColor(_curChannel);
        }

        private string GetChannelChannelTip(ChatChannel channel)
        {
            return SingletonManager.Get<ChatConfigManager>().GetChannelName(channel);
        }

        private void UpdateView()
        {
            if (ChatListState == EUIChatListState.Send)
            {
                inputField.ActivateInputField();
            }
            _viewModel.Show = ChatListState != EUIChatListState.None;
            _viewModel.ChatListBgShow = ChatListState == EUIChatListState.Send;
            _viewModel.SendMessageGroupShow = ChatListState == EUIChatListState.Send;
        }

        private void UpdateChatListState()
        {
            if (_closeViewAnim.IsPlaying())
            {
                return;
            }
            if ((Time.time - _lastMessageTime) > RemainTime && ChatListState != EUIChatListState.Send)
            {
                PlayCloseViewAnim();
            }
            else
            {
                ResetCloseViewAnim();
            }
        }

        private void ResetCloseViewAnim()
        {
            if (_closeViewAnim == null)
            {
                return;
            }

            _closeViewAnim.Rewind();
        }

        private void PlayCloseViewAnim()
        {
            if (_closeViewAnim == null)
            {
                return;
            }

            _closeViewAnim.Restart();
        }

        public void AddMessage(BroadcastMessageData data)
        {
            var tf = GetNewMessageItem();
            if (tf != null && data != null && data.ChatMessage != null)
            {
                if (TargetIdDict != null)
                {
                    if (!TargetIdDict.ContainsKey(data.SendRoleId))
                    {
                        TargetIdDict[data.SendRoleId] = string.Empty;
                    }
                    TargetIdDict[data.SendRoleId] = data.SendRoleName;
                }

                if (data.SendRoleId == 0)
                {
                    tf.GetComponent<Text>().text = data.ChatMessage.Message;
                }
                else
                {
                    tf.GetComponent<Text>().text = data.SendRoleName + ":" + data.ChatMessage.Message;
                }
                if (data.ChatType == (int)ChatType.PrivateChat)
                {
                    long privateChatId = 0;
                    if (data.SendRoleId == _chatState.MyselfId)
                    {
                        privateChatId = data.TargetId;
                        tf.GetComponent<Text>().text = string.Format(ScriptLocalization.hall_chat.word231,
                                                           TargetIdDict[privateChatId]) + data.ChatMessage.Message;
                        PrivateChatRecentUseQueueUpdate(privateChatId);
                    }
                    else
                    {
                        privateChatId = data.SendRoleId;
                        PrivateChatRecentUseQueueUpdate(privateChatId);
                    }
                    if (ChatListState != EUIChatListState.Send)
                    {
                        _curChannel = ChatChannel.PrivateChat;
                        SetPrivateTarget(privateChatId);
                        UpdateChannel();
                    }
                }

                if (data.SendRoleId > 0)
                {
                    tf.GetComponent<Text>().color = UiCommonColor.GetChatChannelColor(ChatTypeToChannel((ChatType)data.ChatType));
                }
                else
                {
                    tf.GetComponent<Text>().color = Color.red;
                }
            }
            else
            {
                Logger.Error("Null object,can't addMessage");
            }
            ResetTime();
            _closeViewAnim.Rewind();
            if (messageDict != null)
            {
                if (ChatListState == EUIChatListState.None)
                {
                    ChatListState = EUIChatListState.Receive;
                }
                messageDict.Add(tf, data);
            }

            
        }

        private Transform GetNewMessageItem()
        {
            if(parent.childCount < MaxMessageCount)
            {
                var tf = GameObject.Instantiate(item, parent);
                if (tf == null)
                {
                    return null;
                }
                tf.gameObject.SetActive(true);
                return tf;
            }
            else
            {
                var firstChild = parent.GetChild(0);
                if (firstChild == null)
                {
                    return null;
                }
                firstChild.SetAsLastSibling();
                return firstChild;
            }
        }

        private void ResetTime()
        {
            _lastMessageTime = Time.time;
        }

        private void SendMessage()
        {
            //Debug.Log("SendMessage" + inputField.text);
            //if (string.IsNullOrEmpty(inputField.text)) return;
            //UiModule.contexts.ui.uI.OperationTipData = new BaseTipData { Title = inputField.text, DurationTime = 10 * 1000 };
            //UiModule.contexts.ui.uISession.UiState["CommonOperationTipModel"] = true;
            //UiModule.contexts.ui.uI.SystemTipDataQueue.Enqueue(new BaseTipData { Title = inputField.text, DurationTime = 10 * 1000 });
            //UiModule.contexts.ui.uISession.UiState["CommonSystemTipModel"] = true;
            //UiModule.contexts.ui.uI.GameResult = Core.Enums.EUIGameResultType.Tie;
            //UiModule.contexts.ui.uISession.UiState["CommonGameOverModel"] = true;
            SendMessageData();
            ResetInputMessage();
        }

        BroadcastMessageData GetNewBroadcastMessageData(string str)
        {
            var data = BroadcastMessageData.Allocate();
            data.SendRoleName = string.Empty;
            data.ChatMessage = ChatMessageData.Allocate();
            data.ChatMessage.Message = str;
            data.ChatMessage.Type = 1;
            return data;
        }

        private void SendMessageData()
        {
            string str = inputField.text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            var targetStrWithOutBlack = str.Trim();
            if (string.IsNullOrEmpty(targetStrWithOutBlack))
            {
                inputField.text = string.Empty;
                return;
            }
            if (CheckPersonalOnlineStatusAndSendPrivateChat())//私聊状态
            {
                return;
            }
            var data = GetNewBroadcastMessageData(str);
            data.ChatType = (int)ChannelToChatType(_curChannel);
            if (_curChannel == ChatChannel.Near)
            {
                HallUtility.SendNearChatMessage(GetNearData(data,_chatState.NearPlayerInfo));
            }
            else if(_curChannel == ChatChannel.Camp)
            {
                HallUtility.SendCampChatMessage(GetCampData(data, _chatState.CampPlayerInfo));
            }
            else if (_curChannel == ChatChannel.PrivateChat) //不合法的私聊状态
            {
                AddMessageForUnablePrivateChat();
            }
            else
            {
                SendChatMessage(data);
            }
        }

        private CampChatData GetCampData(BroadcastMessageData data, List<long> idList)
        {
            var targetData = CampChatData.Allocate();
            targetData.BroadcastMsg = data;
            List<long> nearIds = idList;
            CampRoleIdList ids = CampRoleIdList.Allocate();
            foreach (long id in nearIds)
            {
                ids.RoleId.Add(id);
            }
            targetData.CampRoleIdListData = ids.ToByteString();
            targetData.Crc = 1;
            return targetData;
        }

        private NearbyChatData GetNearData(BroadcastMessageData data, List<long> idList)
        {
            var targetData = NearbyChatData.Allocate();
            targetData.BroadcastMsg = data;
            List<long> nearIds = idList;
            NearbyRoleIdList ids = NearbyRoleIdList.Allocate();
            foreach (long id in nearIds)
            {
                ids.RoleId.Add(id);
            }
            targetData.NearbyRoleIdListData = ids.ToByteString();
            targetData.Crc = 1;
            return targetData;
        }

        private void SendChatMessage(BroadcastMessageData data)
        {
            HallUtility.SendChatMessage(data);
        }

        #region ChatType-ChatChannel
        private ChatType ChannelToChatType(ChatChannel curChannel)
        {
            switch (curChannel)
            {
                case ChatChannel.Near:
                    return ChatType.Near;
                case ChatChannel.GameTeam:
                    return ChatType.GameTeam;
                case ChatChannel.Corps:
                    return ChatType.Corps;
                case ChatChannel.PrivateChat:
                    return ChatType.PrivateChat;
                case ChatChannel.Camp:
                    return ChatType.Camp;
                case ChatChannel.All:
                    return ChatType.GameAll;
                default:
                    return ChatType.Default;
            }
        }

        private ChatChannel ChatTypeToChannel(ChatType chatType)
        {
            switch (chatType)
            {
                case ChatType.Near:
                    return ChatChannel.Near;
                case ChatType.GameTeam:
                    return ChatChannel.GameTeam;
                case ChatType.Corps:
                    return ChatChannel.Corps;
                case ChatType.PrivateChat:
                    return ChatChannel.PrivateChat;
                case ChatType.Camp:
                    return ChatChannel.Camp;
                case ChatType.GameAll:
                    return ChatChannel.All;
                default:
                    return ChatChannel.None;
            }
        }
        #endregion

        private void ResetInputMessage()
        {
            string name = string.Empty;
            string text = string.Empty;
            ParseInputForPrivateChat(out name, out text);
            if (string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(text))
            {
                inputField.text = string.Empty;
            }
            ChatListState = EUIChatListState.Receive;
        }

        public override void Destory()
        {
            base.Destory();
            if (_closeViewAnim != null)
            {
                _closeViewAnim.Kill();
            }
        }

    }
}
