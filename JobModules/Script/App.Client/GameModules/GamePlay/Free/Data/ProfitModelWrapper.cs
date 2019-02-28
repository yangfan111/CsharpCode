using System.Collections.Generic;

namespace Assets.Sources.Free.Data
{
    public class ProfitModelWrapper
    {
        private ProfitModel profitModel;


        public ProfitModel GetProfitModel()
        {
            return profitModel;
        }

        private bool _changed;
        public ProfitModelWrapper()
        {
            profitModel = new ProfitModel();
        }
//        public void profitDataChanged(ExpData expData)
//        {
//            _changed = true;
//
//
//            profitModel.totalExp = expData.BaseExp;
//            profitModel.totalGp = expData.BaseGp;
//            profitModel.totalCrystal = expData.WhiteCrystalNum;
//
//            profitModel.expPlusList.Clear();
//            profitModel.expPlusList.Add(expData.BattleAddExp);
//            profitModel.expPlusList.Add(expData.HeroWeaponAddExp);
//            profitModel.expPlusList.Add(expData.AdvancedExpCardAddExp);
//            profitModel.expPlusList.Add(expData.PrimaryExpCardAddExp);
//            profitModel.expPlusList.Add(expData.TeamAndFriendAddExp);
//            profitModel.expPlusList.Add(expData.DecorationAddExp);
//
//            profitModel.gpPlusList.Clear();
//            profitModel.gpPlusList.Add(expData.BattleAddGp);
//            profitModel.gpPlusList.Add(expData.HeroWeaponAddGp);
//            profitModel.gpPlusList.Add(expData.AdvancedGpCardAddGp);
//            profitModel.gpPlusList.Add(expData.PrimaryGpCardAddGp);
//
//            profitModel.totalExpPer = 100;
//            profitModel.totalGpPer = 100;
//            var i = 0;
//            for (i = 0; i < profitModel.expPlusList.Count; ++i)
//            {
//                profitModel.totalExpPer += profitModel.expPlusList[i];
//            }
//
//            for (i = 0; i < profitModel.gpPlusList.Count; ++i)
//            {
//                profitModel.totalGpPer += profitModel.gpPlusList[i];
//            }
//
//            profitModel.totalAllExp = (int)(profitModel.totalExp * profitModel.totalExpPer * 0.01);
//            profitModel.totalAllGp = (int)(profitModel.totalGp * profitModel.totalGpPer * 0.01);
//        }

        public bool changed
        {
            get { return _changed; }
            set { _changed = value; }
        }

        public int totalExp
        {
            get { return profitModel.totalExp; }
        }

        public int totalExpPer
        {
            get { return profitModel.totalExpPer; }
        }

        public int totalGp
        {
            get { return profitModel.totalGp; }
        }

        public int totalGpPer
        {
            get { return profitModel.totalGpPer; }
        }

        public int totalCrystal
        {
            get { return profitModel.totalCrystal; }
        }

        public IList<int> expPlusList
        {
            get { return profitModel.expPlusList; }
        }

        public IList<int> gpPlusList
        {
            get { return profitModel.gpPlusList; }
        }

        public int totalAllExp
        {
            get { return profitModel.totalAllExp; }
        }

        public int totalAllGp
        {
            get { return profitModel.totalAllGp; }
        }
    }
}
