using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class HintWindowConfig
    {
        public HintWindow[] Items;
    }
    [XmlType("Item")]
    public class HintWindow
    {
        public int Id;
        public int Type;         //类型（1提示，2确认取消3 确认）//
        public string Content;
        public int ContentId;
        public string Title;
        public int TitleId;
        public string Confirm;
        public string ConfirmId;
        public string Cancle;
        public int CancleId;
        public int Close;       //关闭按钮(1:需要；0：不需要）//
        public int Sound;
    }
}
