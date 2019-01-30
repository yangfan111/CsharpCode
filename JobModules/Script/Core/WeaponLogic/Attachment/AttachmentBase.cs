using Core.Utils;
using System.Collections.Generic;
using XmlConfig;

namespace Core.WeaponLogic.Attachment
{
    public struct BulletModifierArg
    {
        public float Velocity;

        public static BulletModifierArg Default()
        {
            return new BulletModifierArg
            {
                Velocity = 1
            };
        }
    }
    public struct EffectArg
    {
        public int Spark;
    }
    public struct DefaultFireModifierArg
    {
        public float Fov;
        public float FocusSpeed;
        public float ReloadSpeed;
        public float BreathFactor;
    }
    public struct RifleKickbackModifierArg
    {
        public float BasicWidth;
        public float ContinusWidth;
        public float MaxWidth;
        public float BasicHeight;
        public float ContinusHeight;
        public float MaxHeight;
        public float Turnback;

        public void CopyTo(ref RifleKickbackModifierArg arg)
        {
            arg.BasicWidth = BasicWidth;
            arg.ContinusWidth = ContinusWidth;
            arg.MaxWidth = MaxWidth;
            arg.BasicHeight = BasicHeight;
            arg.ContinusHeight = ContinusHeight;
            arg.MaxHeight = MaxHeight;
            arg.Turnback = Turnback;
        }

        public static RifleKickbackModifierArg Default()
        {
            return new RifleKickbackModifierArg {
                BasicHeight = 1,
                ContinusWidth = 1,
                MaxHeight = 1,
                MaxWidth = 1,
                BasicWidth = 1,
                ContinusHeight = 1,
                Turnback = 1,
            };
        }
    }
    
    public struct DefaultSoundArg
    {
        public int Fire;
    }
    
    public struct ThrowingModifierArg
    {
        public float Velocity;

        public static ThrowingModifierArg Default()
        {
            return new ThrowingModifierArg
            {
                Velocity = 1
            };
        }
    }



    public interface IAttachmentManager
    {
        void Prepare(WeaponPartsStruct attachments);

        void ApplyAttachment(ISpreadLogic logic);
        void ApplyAttachment(IKickbackLogic logic);
        void ApplyAttachment(IAccuracyLogic logic);
        void ApplyAttachment(IBulletContainer logic);
        void ApplyAttachment(IWeaponSoundLogic logic);
        void ApplyAttachment(IWeaponEffectLogic logic);
        void ApplyAttachment(IWeaponLogic logic);
        void ApplyAttachment(IBulletFactory logic);
        void ApplyAttachment(IFireLogic logic);
    }

    public struct WeaponPartsStruct
    {
        public int UpperRail;
        public int LowerRail;
        public int Magazine;
        public int Stock;
        public int Muzzle;

        public static WeaponPartsStruct Default()
        {
            return new WeaponPartsStruct
            {
                UpperRail = 0,
                LowerRail = 0,
                Magazine = 0,
                Stock = 0,
                Muzzle = 0,
            };
        }

        public WeaponPartsStruct Clone()
        {
            var result = new WeaponPartsStruct();
            result.UpperRail = UpperRail;
            result.LowerRail = LowerRail;
            result.Magazine = Magazine;
            result.Stock = Stock;
            result.Muzzle = Muzzle;
            return result;
        }

        public override string ToString()
        {
            return string.Format("Upper {0}, Lower {1}, Magazine {2}, Stock {3}, Muzzle {4}",
                UpperRail,
                LowerRail,
                Magazine,
                Stock,
                Muzzle);
        }
    }

    
}
