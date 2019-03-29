using System.Collections.Generic;

namespace Assets.Sources.Free.Data
{
    public class ProfitModel
    {
        private int _totalExp = 0;
        private int _totalExpPer = 0;
        private int _totalGp = 0;
        private int _totalGpPer = 0;
        private int _totalCrystal = 0;
        private IList<int> _expPlusList = new List<int>();
        private IList<int> _gpPlusList = new List<int>();
        private bool _changed = false;
        private int _totalAllExp = 0;
        private int _totalAllGp = 0;



        public bool changed
        {
            get { return _changed; }
            set { _changed = value; }
        }

        public int totalExp
        {
            get { return _totalExp; }
            set { _totalExp = value; }
        }

        public int totalExpPer
        {
            get { return _totalExpPer; }
            set { _totalExpPer = value; }
        }

        public int totalGp
        {
            get { return _totalGp; }
            set { _totalGp = value; }
        }

        public int totalGpPer
        {
            get { return _totalGpPer; }
            set { _totalGpPer = value; }
        }

        public int totalCrystal
        {
            get { return _totalCrystal; }
            set { _totalCrystal = value; }
        }

        public IList<int> expPlusList
        {
            get { return _expPlusList; }
        }


        public IList<int> gpPlusList
        {
            get { return _gpPlusList; }
        }


        public int totalAllExp
        {
            get { return _totalAllExp; }
            set { _totalAllExp = value; }
        }

        public int totalAllGp
        {
            get { return _totalAllGp; }
            set { _totalAllGp = value; }
        }

    }
}
