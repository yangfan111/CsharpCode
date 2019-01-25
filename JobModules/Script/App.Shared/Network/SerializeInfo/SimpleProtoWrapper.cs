namespace Free.framework
{

    public class SimpleProtoWraper
    {
        public const int IntNanN = int.MaxValue;
        private SimpleProto content;
        public SimpleProtoWraper(SimpleProto bin)
        {
            content = bin;
        }
        public int In_Val(int index) { return (content.Ins.Count > index) ? content.Ins[index] :int.MaxValue; }
        public float Fl_Val(int index) { return (content.Fs.Count > index) ? content.Fs[index] : float.NaN; }
        public string St_Val(int index) { return (content.Ss.Count > index) ? content.Ss[index] : string.Empty; }
    }
  



}