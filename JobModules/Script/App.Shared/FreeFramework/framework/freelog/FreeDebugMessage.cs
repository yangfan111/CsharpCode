using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.wd.free.debug
{
    public  class FreeDebugMessage
    {
        public int index;
        public string type;
        public string info;
        public string time;

        private static FreeDebugMessageList list;

        public override string ToString()
        {
            return string.Format("{0}  {1}  {2}", time, type, info);
        }

        public static void AddMessage(string type, string info)
        {
            InitialList();

            list.AddDebugInfo(type, info);
        }

        public static string ToMessage(string type)
        {
            InitialList();

            return list.ToMessage();
        }

        private static void InitialList()
        {
            if (list == null)
            {
                list = new FreeDebugMessageList(10000);
            }
        }
    }

    class FreeDebugMessageList
    {
        private Queue<FreeDebugMessage> cache;

        public Queue<FreeDebugMessage> infos;

        private int capacity;
        private int index;

        public FreeDebugMessageList(int capacity = 1000)
        {
            infos = new Queue<FreeDebugMessage>();
            cache = new Queue<FreeDebugMessage>();

            this.capacity = capacity;
            for (int i = 0; i <= capacity + 2; i++)
            {
                cache.Enqueue(new FreeDebugMessage());
            }
        }

        public void Reset()
        {
            for (int i = 0; i < infos.Count; i++)
            {
                cache.Enqueue(infos.Dequeue());
            }

            index = 0;
        }

        public string ToMessage()
        {
            StringBuilder sb = new StringBuilder();
            FreeDebugMessage[] infoArray = infos.ToArray();
            for (int i = 0; i < infoArray.Length; i++)
            {
                sb.AppendLine(infoArray[i].ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void AddDebugInfo(string type, string info)
        {
            index++;

            FreeDebugMessage debugInfo = cache.Dequeue();
            debugInfo.index = index;
            debugInfo.info = info;
            debugInfo.type = type;
            debugInfo.time = DateTime.Now.ToString();

            if (infos.Count >= capacity)
            {
                for (int i = 0; i < capacity / 10; i++)
                {
                    cache.Enqueue(infos.Dequeue());
                }
            }

            infos.Enqueue(debugInfo);
        }
    }
}
