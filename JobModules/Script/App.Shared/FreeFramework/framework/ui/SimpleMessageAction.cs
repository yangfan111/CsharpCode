using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace com.wooduan.free.ui
{
    [Serializable]
    public class SimpleMessageAction : SendMessageAction
    {
        public string key;
        public List<MessageField> fields;

        private const int BoolField = 1;
        private const int KeyField = 2;
        private const int IntField = 3;
        private const int FloatField = 4;
        private const int LongField = 5;
        private const int StringField = 6;
        private const int DoubleField = 7;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = args.GetInt(key);
            if(fields != null)
            {
                for(int i = 0; i < fields.Count; i++)
                {
                    switch (fields[i].type)
                    {
                        case KeyField:
                            builder.Ks.Add(args.GetInt(fields[i].value));
                            break;
                        case IntField:
                            builder.Ins.Add(args.GetInt(fields[i].value));
                            break;
                        case BoolField:
                            builder.Bs.Add(args.GetBool(fields[i].value));
                            break;
                        case StringField:
                            builder.Ss.Add(args.GetString(fields[i].value));
                            break;
                        case FloatField:
                            builder.Fs.Add(args.GetFloat(fields[i].value));
                            break;
                        case DoubleField:
                            builder.Ds.Add(args.getDouble(fields[i].value));
                            break;
                        case LongField:
                            builder.Ls.Add(args.GetLong(fields[i].value));
                            break;
                    }
                }
            }
        }
    }

    [Serializable]
    public class MessageField
    {
        public int type;
        public string desc;
        public string value;
    }
}
