using System;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Utils.Configuration;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class ChatComponent : IComponent
    {
        [DontInitilize] public EUIChatListState ChatListState;

        public void CloseChatView()
        {
            ChatListState = EUIChatListState.None;
        }

        [DontInitilize] public Action<object> AddChatMessageDataAction;
        [DontInitilize] public Action<object> GetPersonalOnlineStatusCallback;
    }
}