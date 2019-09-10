using App.Client.GameModules.Ui.UiAdapter;
using App.Client.GameModules.Ui.Utils;
using App.Client.GameModules.Ui.ViewModels.Common;
using App.Client.Utility;
using App.Shared.GameModules.Player;
using Assets.UiFramework.Libs;
using Com.Wooduan.Ssjj2.Common.Net.Proto;
using Core.GameModule.Interface;
using Core.Utils;
using DG.Tweening;
using Google.Protobuf;
using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UIComponent.UI;
using UnityEngine;
using UnityEngine.UI;
using UserInputManager.Lib;
using Utils.Configuration;
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
            get { return 15; }
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
        private KeyHandler _switchKeyHandler;
        private Vector2 origInputFieldSize;
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
            //InitAnim();
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

        private UIText privateNameRtf;
        private void InitVariable()
        {
            parent = FindChildGo("Content");
            item = FindChildGo("ChatMessage");
            item.gameObject.SetActive(false);
            inputField = FindChildGo("InputField").GetComponent<InputField>();
            privateNameRtf = FindComponent<UIText>("PrivateName");
            _viewModel.InputValueChanged = InputValueOnChanged;
            origInputFieldSize = _viewModel.InputSize;
        }

        private void InitAnim()
        {
            _closeViewAnim = DOTween.To(() => _viewModel.Alpha, (x) => _viewModel.Alpha = x, 0, 1f);
            _closeViewAnim.SetAutoKill(false);
            _closeViewAnim.Pause();
            _closeViewAnim.OnComplete(CloseSendView);
        }

        private void CloseSendView()
        {
            ChatListState = EUIChatListState.None;
            ResetTime();
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
            UpdateChatListState();
            UpdateInputKey();
            UpdateView();
        }

        bool haveRegister = false;
        private void UpdateInputKey()
        {
            if (_switchKeyHandler == null)
            {
                return;
            }
            if (ChatListState == EUIChatListState.Send)
            {
                if (!haveRegister)
                {
                    _chatState.RegisterKeyReceive(_switchKeyHandler);
                    haveRegister = true;
                }
            }
            else
            {
                if (haveRegister)
                {
                    _chatState.UnRegisterKeyReceive(_switchKeyHandler);
                    haveRegister = false;
                }
            }
        }

        private void InitKeyReveiver()
        {
            var keyhandler = new KeyHandler(UiConstant.chatWindowLayer, BlockType.None);
            keyhandler.BindKeyAction(UserInputKey.SendChatMessage, (data) =>
            {
                if (ChatListState != EUIChatListState.Send)
                {
                    SwitchToSendState();
                }
            });
            _switchKeyHandler = new KeyHandler(UiConstant.chatWindowKeyBlockLayer, BlockType.All);
            _switchKeyHandler.BindKeyAction(UserInputKey.SendChatMessage, (data) => { SendMessage();});
            _switchKeyHandler.BindKeyAction(UserInputKey.SwitchChatChannel, (data) =>
            {
                SwitchChannel();
                UpdateChannel();
            });

            _chatState.RegisterOpenKey(keyhandler);
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

            if (_curChannel == ChatChannel.PrivateChat)
            {
                SwitchChannel();
            }
        }

        private void ShowPrivateTarget()
        {
            var name = "/" + _curTargetPlayerName + ":";
            _viewModel.PrivateNameShow = true;
            _viewModel.PrivateNameText = name;
            _viewModel.InputSize =
                new Vector2(origInputFieldSize.x - privateNameRtf.preferredWidth, origInputFieldSize.y);
        }

        private void ClearPrivateTarget()
        {
            _viewModel.PrivateNameShow = false;
            _viewModel.PrivateNameText = string.Empty;
            _viewModel.InputSize = origInputFieldSize;
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
            if(PlayerStateUtil.HasUIState(EPlayerUIState.ChatOpen, _chatState.gamePlay))
                PlayerStateUtil.RemoveUIState(EPlayerUIState.ChatOpen, _chatState.gamePlay);
            else PlayerStateUtil.AddUIState(EPlayerUIState.ChatOpen, _chatState.gamePlay);
            _chatState.SetCrossVisible(true);
            ResetCloseViewAnim();
        }

        private void UpdateChannel()
        {
            _viewModel.ChannelTipText = GetChannelChannelTip(_curChannel);
            _viewModel.ChannelTipColor = UiCommonColor.GetChatColorByChatChannel(_curChannel);
            if (inputField == null || inputField.textComponent == null)
            {
                Logger.Error("can't found text in inputfield");
                return;
            }
            inputField.textComponent.color = _curChannel == ChatChannel.PrivateChat
                ? UiCommonColor.GetChatColorByChatChannel(ChatChannel.PrivateChat)
                : Color.white;
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
            //_viewModel.ChatListBgShow = ChatListState == EUIChatListState.Send;
            _viewModel.SendMessageGroupShow = ChatListState == EUIChatListState.Send;
            _viewModel.Alpha = ChatListState == EUIChatListState.Receive ? 0.7f : 1f;

        }

        private void UpdateChatListState()
        {
            //if (_closeViewAnim.IsPlaying())
            //{
            //    return;
            //}
            if ((Time.time - _lastMessageTime) > RemainTime && ChatListState != EUIChatListState.Send)
            {
                CloseSendView();
                //PlayCloseViewAnim();
            }
            //else
            //{
            //    ResetCloseViewAnim();
            //}
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

        private const string commonMessageWithColorFormat = "<color=#{0}>{1}</color>";

        public void AddMessage(BroadcastMessageData data)
        {
            var tf = GetNewMessageItem();
            var channelColor = GetChannelColor(data);
            var channelName = GetChannelNamePrefix(data, channelColor);
            var messageText = string.Empty;
            var textComponent = tf.GetComponent<Text>();
            var channelRoot = textComponent.transform.GetChild(0);
            var channelTextComponent = channelRoot != null ? channelRoot.GetComponent<Text>() : null;
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

                if (IsFromSystem(data))
                {
                    var color = ColorUtility.ToHtmlStringRGB(UiCommonColor.SystemMessageColor);
                    messageText = string.Format(commonMessageWithColorFormat, color, data.ChatMessage.Message);
                }
                else if (data.ChatType == (int) ChatType.PrivateChat)
                {
                    var color = ColorUtility.ToHtmlStringRGB(
                        UiCommonColor.GetChatColorByChatChannel(ChatChannel.PrivateChat));
                    messageText = string.Format(commonMessageWithColorFormat, color, messageText);
                    long privateChatId = 0;
                    if (data.SendRoleId == _chatState.MyselfId)
                    {
                        privateChatId = data.TargetId;
                        messageText = string.Format(ScriptLocalization.hall_chat.word231,
                                                           TargetIdDict[privateChatId]) + data.ChatMessage.Message;
                    }
                    else
                    {
                        privateChatId = data.SendRoleId;
                        var name = string.Format(ScriptLocalization.hall_chat.PlayerNameFormat, data.SendRoleName);
                        messageText = name + data.ChatMessage.Message;
                    }

                        PrivateChatRecentUseQueueUpdate(privateChatId);
                    messageText = string.Format(commonMessageWithColorFormat, color, messageText);

                    if (ChatListState != EUIChatListState.Send)
                    {
                        _curChannel = ChatChannel.PrivateChat;
                        SetPrivateTarget(privateChatId);
                        UpdateChannel();
                    }
                }
                else
                {
                    var color = ColorUtility.ToHtmlStringRGB(UiCommonColor.ChatSenderColor);
                    var name = string.Format(ScriptLocalization.hall_chat.PlayerNameFormat, data.SendRoleName);
                    messageText = string.Format(commonMessageWithColorFormat, color, name) +
                                  data.ChatMessage.Message;
                }

                if (channelTextComponent != null)
                {
                    channelTextComponent.text = channelName;
                }
                textComponent.text = messageText;
            }
            else
            {
                Logger.Error("Null object,can't addMessage");
            }

            if (ChatListState == EUIChatListState.None)
            {
                ChatListState = EUIChatListState.Receive;
            }
            ResetTime();
            //_closeViewAnim.Rewind();           
        
        }

        private string GetChannelNamePrefix(BroadcastMessageData data,Color color)
        {
            string channelName;
            var channel = ChatTypeToChannel((ChatType)data.ChatType);
            channelName = IsFromSystem(data) ? ScriptLocalization.hall_chat.word228 : GetChannelChannelTip(channel);
            var colorText = ColorUtility.ToHtmlStringRGB(color);
            if (!string.IsNullOrEmpty(channelName))
            {
                channelName = string.Format("<color=#{0}>[{1}]</color>  ", colorText, channelName);
            }

            return channelName;
        }

        private bool IsFromSystem(BroadcastMessageData data)
        {
            return data.SendRoleId == 0;
        }

        private Color GetChannelColor(BroadcastMessageData data)
        {
            if (IsFromSystem(data)) return UiCommonColor.SystemMessageColor;
            var channel = ChatTypeToChannel((ChatType) data.ChatType);
            return UiCommonColor.GetChatColorByChatChannel(channel);
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
            ResetTime();
        }

        public override void OnDestory()
        {
            base.OnDestory();
            if (_closeViewAnim != null)
            {
                _closeViewAnim.Kill();
            }
        }

    }
}
