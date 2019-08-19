using App.Server.GameModules.GamePlay;
using com.wd.free.para;
using Core.Free;

namespace App.Client.GameModules.GamePlay
{
    class ClientRule : IFreeRule, IParable
    {
        private SimpleParaList paras;

        public ClientRule(IFreeArgs FreeArgs)
        {
            this.paras = new SimpleParaList();
            this.paras.AddFields(new ObjectFields(this));

            ((FreeRuleEventArgs)(FreeArgs)).AddDefault(this);
        }

        public long ServerTime
        {
            get
            {
                return 0;
            }
            set { }
        }

        public string FreeType
        {
            get
            {
                return string.Empty;
            }
            set { }
        }

        public bool GameOver
        {
            get
            {
                return false;
            }
            set { }
        }

        public bool GameExit {
            get
            {
                return false;
            }
            set { }
        }

        public long StartTime { get{return 0;}}
        public long GameStartTime { get{return 0;} set { } }
        public long GameEndTime { get{return 0;} set { } }

        public ParaList GetParameters()
        {
            return paras;
        }
    }
}
