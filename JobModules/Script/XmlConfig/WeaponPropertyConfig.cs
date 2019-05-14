using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlType("child")]
    public class WeaponPropertyConfigItem
    {
        public int Id;
        public int Category;
        public int WeaponId;
        public string Name;
        public int Bullet;  //装弹数
        public int Bulletmax; //最大备弹数
        public float Power;     //威力
        public float Limitcycle;  //射速
        public float Accurate;    //精准
        public float Weight;      //射程
        public float Stability;   //稳定
        public float Penetrate;   //穿透
        public float Scope;       //范围      

        public WeaponProperty GetWeaponProperty()
        {
            WeaponProperty wp = new WeaponProperty();
            wp.Bulletmax = Bulletmax;
            wp.Bullet = Bullet;
            wp.Power = Power;
            wp.Limitcycle = Limitcycle;
            wp.Accurate = Accurate;
            wp.Weight = Weight;
            wp.Stability = Stability;
            wp.Penetrate = Penetrate;
            wp.Scope = Scope;
            return wp;
        }
    }

    [XmlRoot("root")]
    public class WeaponPropertyConfig
    {
        public WeaponPropertyConfigItem[] Items;
    }
    
    public struct WeaponProperty
    {
        public static float UpperLimit = 100;   //上限
        public static float LowerLimit = 0;     //下限

        public int Bullet;  //装弹数
        public int Bulletmax; //最大备弹数
        public float Power;     //威力
        public float Limitcycle;  //射速
        public float Accurate;    //精准
        public float Weight;      //射程
        public float Stability;   //稳定
        public float Penetrate;   //穿透
        public float Scope;       //范围    

        public WeaponProperty Add(WeaponProperty b)
        {
            WeaponProperty c = new WeaponProperty();
            c.Bullet = Bullet + b.Bullet;
            c.Bulletmax = Bulletmax + b.Bulletmax;

            c.Power = AddValue(Power,b.Power);
            c.Limitcycle = AddValue(Limitcycle, b.Limitcycle);
            c.Accurate = AddValue(Accurate, b.Accurate);
            c.Weight = AddValue(Weight , b.Weight);
            c.Stability = AddValue(Stability , b.Stability);
            c.Penetrate = AddValue(Penetrate , b.Penetrate);
            c.Scope = AddValue(Scope , b.Scope);
            return c;
        }

        public WeaponProperty Cut(WeaponProperty b)
        {
            WeaponProperty c = new WeaponProperty();
            c.Bullet = Bullet - b.Bullet;
            c.Bulletmax = Bulletmax - b.Bulletmax;

            c.Power = CutValue(Power, b.Power);
            c.Limitcycle = CutValue(Limitcycle, b.Limitcycle);
            c.Accurate = CutValue(Accurate, b.Accurate);
            c.Weight = CutValue(Weight, b.Weight);
            c.Stability = CutValue(Stability, b.Stability);
            c.Penetrate = CutValue(Penetrate, b.Penetrate);
            c.Scope = CutValue(Scope, b.Scope);
            return c;
        }

        float AddValue(float a , float b)
        {
            float c = a + b;
            if(c > UpperLimit)
            {
                c = UpperLimit;
            }
            return c;
        }

        float CutValue(float a, float b)
        {
            float c = a - b;
            if (c < LowerLimit)
            {
                c = LowerLimit;
            }
            return c;
        }



    }
}
