using System.Collections.Generic;

namespace Assets.Sources.Free.Data
{
    public class AllObserverNameData : UIBaseData
    {

        private IList<string> _nameList;

        public AllObserverNameData()
        {
        }

//        public void ObserverNameChanged(ObserverNameData data)
//        {
//            this.Changed = true;
//            _nameList = data.PlayerName;
//        }

        public IList<string> NameList
        {
            get { return _nameList; }
        }
    }
}
