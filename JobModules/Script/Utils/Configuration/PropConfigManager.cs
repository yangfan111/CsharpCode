using System.Collections.Generic;
using Utils.CharacterState;
using XmlConfig;

namespace Utils.Configuration
{
    public class PropConfigManager : AbstractConfigManager<PropConfigManager>
    {    
        public enum PropType
        {
            BUFF = 1,                           //BUFF品
            Consume = 2,                        //消耗品
            Other = 3,                          //其他
            NewPlayerReward = 4,                //新手礼包
            NSelectOneReward = 5,               //N选一礼包
            AllReward = 6,                      //全获得礼包
            NChouONEReward = 7,                 //N抽1礼包
            NumberReward = 8,                   //数量区间礼包         
            Helmet = 10,                        //头盔
            Bag = 11,                           //背包
            Armor = 12,                         //防弹衣           
        }


        private Dictionary<int, PropConfigItem> _configs = new Dictionary<int, PropConfigItem>();
        private PropConfig _config;

        public override void ParseConfig(string xml)
        {
            _config = XmlConfigParser<PropConfig>.Load(xml);
            if (null == _config)
            {
                Logger.Error("invalid xml content");
                return;
            }
            foreach(var item in _config.Items)
            {
                _configs[item.Id] = item;
            }
        }
       
        public PropConfigItem GetConfigById(int id)
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

        public string GetTypeNameById(int id)
        {
            string result = "";
            var cfg = GetConfigById(id);
            if (null != cfg)
            {
                switch ((PropType)cfg.Type)
                {
                    case PropType.BUFF:   
                        {
                            result = "Buff品";                                
                        }
                        break;
                    case PropType.Consume:  
                        {
                            result = "消耗品";
                        }
                        break;
                    case PropType.Other:  
                        {
                            result = "其他";
                        }
                        break;
                    case PropType.NewPlayerReward:   
                        {
                            result = "新手礼包";
                        }
                        break;
                    case PropType.NSelectOneReward:   
                        {
                            result = "N选一礼包";
                        }
                        break;
                    case PropType.AllReward:   
                        {
                            result = "全获得礼包";
                        }
                        break;
                    case PropType.NChouONEReward:   
                        {
                            result = "N抽1礼包";
                        }
                        break;
                    case PropType.NumberReward:   
                        {
                            result = "数量区间礼包";
                        }
                        break;
                    case PropType.Helmet:   
                        {
                            result = "头盔";
                        }
                        break;
                    case PropType.Bag:  
                        {
                            result = "背包";
                        }
                        break;
                    case PropType.Armor:   
                        {
                            result = "防弹衣";
                        }
                        break;                                       
                }
            }
            return result;
        }

    }
}
