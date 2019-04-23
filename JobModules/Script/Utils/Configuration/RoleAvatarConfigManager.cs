using System.Collections.Generic;
using Shared.Scripts;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Configuration
{
    public class RoleAvatarConfigManager : AbstractConfigManager<RoleAvatarConfigManager>
    {
        private Dictionary<int, RoleAvatarConfigItem> _configs = new Dictionary<int, RoleAvatarConfigItem>();
        private Dictionary<int, RoleAvatarConfigItem> _resDic = new Dictionary<int, RoleAvatarConfigItem>();
        private RoleAvatarConfig _config;
        public const int C4 = 210;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<RoleAvatarConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach(var item in _config.Items)
            {
                _configs[item.Id] = item;
                _resDic[item.FRes] = item;
                _resDic[item.Res] = item;
            }
        }

        public int GetResId(int id, Sex sex)
        {
            var cfg = GetConfigById(id);
            if(null != cfg)
            {
                switch(sex)
                {
                    case Sex.Female:
                        return cfg.FRes;
                    case Sex.Male:
                        return cfg.Res;
                }
            }
            Logger.ErrorFormat("illegal sex value {0}", sex);
            return cfg.FRes;
        }

        public RoleAvatarConfigItem GetConfigById(int id)
        {
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("GetConfigById {0}", id);
            }
            if(!_configs.ContainsKey(id))
            {
                Logger.WarnFormat("{0} doesn't exist in role_avatar config ", id);
                return null; 
            }
            return _configs[id];
        }

        public RoleAvatarConfigItem GetConfigByResId(int id)
        {
            if (!_resDic.ContainsKey(id))
            {
                Logger.WarnFormat("FRes {0} doesn't exist in role_avatar config ", id);
                return null;
            }
            return _resDic[id];
        }


        public string GetIcon(int id, Sex sex)
        {
            var cfg = GetConfigById(id);
            if(null == cfg)
            {
                return string.Empty;
            }
            switch (sex)
            {
                case Sex.Female:
                    return cfg.Icon;
                case Sex.Male:
                    return cfg.Icon;
            }
            return string.Empty;
        }

        public RoleAvatarConfig GetConfig()
        {
            return _config;
        }

        public string GetTypeNameById(int id)
        {
            string result = "";
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                switch ((Wardrobe)cfg.Type)
                {
                    case Wardrobe.CharacterHead:   //人物头
                        {
                            result = "人物头";                                
                        }
                        break;
                    case Wardrobe.CharacterHairContainer:   //人物发型
                        {
                            result = "人物发型";
                        }
                        break;
                    case Wardrobe.CharacterHair:   //人物头发
                        {
                            result = "人物头发";
                        }
                        break;
                    case Wardrobe.Cap:   //帽子(头部装扮)
                        {
                            result = "帽子";
                        }
                        break;
                    case Wardrobe.PendantFace:   //面部挂件
                        {
                            result = "面部挂件";
                        }
                        break;
                    case Wardrobe.Inner:   //上身
                        {
                            result = "上身";
                        }
                        break;
                    case Wardrobe.Armor:   //防弹衣
                        {
                            result = "防弹衣";
                        }
                        break;
                    case Wardrobe.Outer:   //外套
                        {
                            result = "外套";
                        }
                        break;
                    case Wardrobe.Glove:   //手套
                        {
                            result = "手套";
                        }
                        break;
                    case Wardrobe.Waist:   //腰部
                        {
                            result = "腰部";
                        }
                        break;
                    case Wardrobe.Trouser:   //腿部
                        {
                            result = "腿部";
                        }
                        break;
                    case Wardrobe.Foot:   //脚部
                        {
                            result = "脚部";
                        }
                        break;
                    case Wardrobe.Bag:   //背包
                        {
                            result = "背包";
                        }
                        break;
                    case Wardrobe.Entirety:   //全身
                        {
                            result = "全身";
                        }
                        break;
                    case Wardrobe.Parachute:   //降落伞包
                        {
                            result = "降落伞包";
                        }
                        break;
                    case Wardrobe.CharacterGlove:   //人物手
                        {
                            result = "人物手";
                        }
                        break;
                    case Wardrobe.CharacterInner:   //人物上身
                        {
                            result = "人物上身";
                        }
                        break;
                    case Wardrobe.CharacterTrouser:   //人物下身
                        {
                            result = "人物下身";
                        }
                        break;
                    case Wardrobe.CharacterFoot:   //人物脚
                        {
                            result = "人物脚";
                        }
                        break;
                }
            }
            return result;
        }


    }
}
