using App.Protobuf;
using Free.framework;

namespace Assets.Sources.Free.UI
{
    public class FreeUIEvent
    {
        public const int EVENT_CLICK = 1;
        public const int EVENT_MOVE = 3;
        public const int EVENT_MOUSE_ENTER = 4;
        public const int EVENT_MOUSE_OUT = 5;
        public const int EVENT_KEYBOARD = 2;

        public int Event;
        public SimpleProto Key;

        public FreeUIEvent()
        {
            Key = FreePool.Allocate();
        }

        public void ResetEvent()
        {
            Event = 0;
            Key = FreePool.Allocate();
        }

        public SimpleProto GetData()
        {
//            var sp = new SimpleProto();
            //            var ba:ByteArray = new ByteArray();
            //            key.writeTo(ba);
            //            ba.position = 0;
            //            sp.mergeFrom(ba);

//            return sp;
            return Key;
        }

        public bool HasEvent()
        {
            return Event > 0;

        }
    }
}
