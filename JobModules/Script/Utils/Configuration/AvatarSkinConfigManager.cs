using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using XmlConfig;

namespace Utils.Configuration
{
    public class AvatarSkinConfigManager:  AbstractConfigManager<AvatarSkinConfigManager>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AvatarSkinConfigManager));
        
        private Dictionary<int, Dictionary<int, SkinParam>> _configs = new Dictionary<int, Dictionary<int, SkinParam>>();

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("AvatarSkinConfig config xml is empty !");
                return;
            }
            _configs.Clear();
            var cfg = XmlConfigParser<AvatarSkinConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("AvatarSkinConfig config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                if (!_configs.ContainsKey(item.TintId))
                {
                    _configs.Add(item.TintId, new Dictionary<int, SkinParam>());
                }

                var dict = _configs[item.TintId];

                if (!dict.ContainsKey(item.SourceId))
                {
                    var skinParam = new SkinParam()
                    {
                        Type = item.Type,
                        MainColor = ArrayToColor32(item.MainColor),
                        SpecularColor = ArrayToColor32(item.SpecularColor),
                        SecondarySpecularColor = ArrayToColor32(item.SecondarySpecularColor)
                    };
                    dict.Add(item.SourceId, skinParam);
                }
                else
                {
                    Logger.ErrorFormat("SourceId:{0} is already exist", item.SourceId);
                }
            }
        }

        private static Color32 ArrayToColor32(List<int> item)
        {
            var ret = new Color32();
            if (item.Count != 4)
            {
                Logger.ErrorFormat("can not convert AvatarSkinConfig color, please check,{0}", string.Join(",", item.Select(i => i.ToString()).ToArray()));
                return ret;
            }

            ret.r = (byte)item[0];
            ret.g = (byte)item[1];
            ret.b = (byte)item[2];
            ret.a = (byte)item[3];
            return ret;
        }

        public SkinParam GetSkinConfigById(int skindId, int sourceId)
        {
            SkinParam ret = null;
            Dictionary<int, SkinParam> data;
            _configs.TryGetValue(skindId, out data);
            if (data == null)
            {
                Logger.WarnFormat("skindId:{0} is not exist in AvatarSkinConfig", skindId);
            }
            else
            {
                data.TryGetValue(sourceId, out ret);
                if (ret == null)
                {
                    Logger.WarnFormat("skindId:{0}, sourceId:{1} is not exist in AvatarSkinConfig", skindId, sourceId);
                }
            }

            return ret;
        }
    }
}