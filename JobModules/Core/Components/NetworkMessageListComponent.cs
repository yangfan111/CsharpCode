using System.Collections.Generic;
using Entitas;

namespace Core.Components
{
    
    public class NetworkMessageListComponent<TMessage> : IComponent where TMessage : class 
    {
        public int MaxCapacity;

        public Queue<TMessage> MessageList;

        public TMessage Pop()
        {
            if (MessageList.Count > 0)
                return MessageList.Dequeue();
            return null;
        }

        public void Push(TMessage message)
        {
            MessageList.Enqueue(message);
            if (MessageList.Count > MaxCapacity)
            {
                Pop();
            }
        }

    }
}
