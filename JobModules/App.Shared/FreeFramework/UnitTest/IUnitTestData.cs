
namespace App.Shared.FreeFramework.UnitTest
{
    public interface IUnitTestData
    {
        void RecordResult(CaseKey key, TestValue[] tvs);
    }

    public class CaseKey
    {
        public string rule;
        public string suit;
        public string caseName;
        public string field;

        public override bool Equals(object obj)
        {
            if (obj is CaseKey)
            {
                CaseKey ck = (CaseKey)obj;
                return ck.rule == rule && ck.suit == suit && ck.caseName == caseName && ck.field == field;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            if (rule == null || suit == null || caseName == null || field == null)
            {
                return 1;
            }
            else
            {
                return caseName.GetHashCode() + field.GetHashCode();
            }

        }
    }
}