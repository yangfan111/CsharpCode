using System;
using System.IO;
using System.Xml.Serialization;

namespace Utils.Replay
{
    public class ReplayInfo
    {
        public int ChannedId;
        public int MapId;
        public string RuleName;
        public String DateTime;
        public int Time;
        public int FrameCount;
        public string InBinName;
        public string OutBinName;
        public string LocalVersion = "localTest";
        public string LocalAsset = "localTest";
        public string RemoteVersion = "remoteTest";
        public string RemoteAsset = "remoteTest";

        public static void WriteToFile(string path, ReplayInfo info)
        {
            var writer = new XmlSerializer(typeof(ReplayInfo));
            using (StringWriter textWriter = new StringWriter())
            {
                writer.Serialize(textWriter, info);
                File.WriteAllText(path, textWriter.ToString());
                ;
            }
        }

        public static ReplayInfo ReadFromFile(string path)
        {
          
            var writer = new XmlSerializer(typeof(ReplayInfo));

            return writer.Deserialize(new StringReader(File.ReadAllText(path))) as ReplayInfo;
        }
    }


    public enum EReplayMessageType
    {
        IN,
        OUT,
        GM,
    }
}