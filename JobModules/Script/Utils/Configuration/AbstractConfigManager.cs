using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;
using Utils.Singleton;

namespace Utils.Configuration
{
    public interface IConfigParser
    {
        void ParseConfig(string xml);
        bool IsInitialized { get; set; }
    }

    public abstract class AbstractConfigManager<T> : Singleton<T>, IConfigParser where T : Singleton<T>, new()
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(T));
        public abstract void ParseConfig(string xml);
     
        public bool IsInitialized { get; set; }
        public void DeSerializeXML(string xPath)
        {
           
        }
    }
    
}
