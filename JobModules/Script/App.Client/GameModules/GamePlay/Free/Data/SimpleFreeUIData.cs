using System.Collections.Generic;
using App.Protobuf;
using Assets.Sources.Utils;
using Core.Utils;
using Core.Utils.System46;
using Free.framework;

namespace Assets.Sources.Free.Data
{
    public class SimpleFreeUIData
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(SimpleFreeUIData));

        public int Key;
        public SimpleProto SimpleData;

        private readonly Dictionary<string, List<SimpleProto>> _dic;
        private readonly Dictionary<string, bool> _changeDic;
        private readonly Dictionary<int, List<string>> _id2SourceDic;

        public  SimpleFreeUIData(bool sub = false)
        {
            if (!sub)
            {
                _dic = new Dictionary<string, List<SimpleProto>>();
                _changeDic = new Dictionary<string, bool>();
                _id2SourceDic = new Dictionary<int, List<string>>();
            }
        }

        public List<string> GetSourceList(int id)
        {
            return _id2SourceDic[id];
        }

        public bool IsChanged(int method, string source){
            return _changeDic.GetOrDefault(GetKey(method, source));
        }

        private static string GetKey(int method, string source){
            return method + "_" + source;
        }

        public IList<SimpleProto> GetData(int method, string source)
        {
            return _dic[GetKey(method, source)];
        }

        public void ResetChange(int method, string source)
        {
            _changeDic[GetKey(method, source)] = false;
            _dic[GetKey(method, source)].Clear();
            _id2SourceDic[method].Clear();
        }
		
        public void SimpleDataChanged(SimpleProto sp)
        {
            var method = sp.Key;
            var source = sp.Ss[0];

            var key = GetKey(method, source);

            if (!_dic.ContainsKey(key)) _dic[key] = new List<SimpleProto>();

            if (!_id2SourceDic.ContainsKey(method)) _id2SourceDic[method] = new List<string>();
            _id2SourceDic[method].Add(source);

            var vec = _dic[key];
            if (vec.Count < 10000) vec.Add(sp);

            _changeDic[key] = true;
        }
    }
}
