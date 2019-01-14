using Sharpen;
using com.wd.free.para;
using com.wd.free.skill;

namespace com.wd.free.unit
{
    public interface IGameUnit : IParable
    {
        long GetID();

        string GetKey();

        UnitSkill GetUnitSkill();

        XYZPara GetXYZ();
    }
}
