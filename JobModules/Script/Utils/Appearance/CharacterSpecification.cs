using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.CharacterState
{
    public enum Sex
    {
        Male = 1,
        Female,
        EndOfTheWorld
    }

    public enum Weapon
    {
        EmptyHanded,
        AssaultRifle,
        Shotgun,
        SubmachineGun,
        SniperRifle,
        EndOfTheWorld
    }

//    // 与Shared.Scripts中的Wardrobe一一对应
//    public enum Wardrobe
//    {
//        CharacterHead = 1, //人物头
//        CharacterHairContainer, //人物发型
//        CharacterHair, //人物头发
//        Cap,//帽子(头部装扮)
//        PendantFace,//面部挂件
//        Inner,//上身
//        Armor,//防弹衣
//        Outer,//外套
//        Glove,//手套
//        Waist,//腰部
//        Trouser,//腿部
//        Foot,//脚部
//        Bag,//背包
//        Entirety, //全身
//        Parachute, //降落伞包
//        CharacterGlove,//人物手
//        CharacterInner,//人物上身
//        CharacterTrouser,//人物下身
//        CharacterFoot,//人物脚
//        EndOfTheWorld
//    }

    [Flags]
    public enum CharacterView
    {
        FirstPerson = 0x1,
        ThirdPerson = 0x2,
        EndOfTheWorld
    }

    public enum SpecialLocation
    {
        // 抛弹壳点
        EjectionLocation,
        // 第一人称相机
        FirstPersonCamera,
        // 弹夹位置
        MagazinePosition,
        // 枪口火焰
        MuzzleEffectPosition,
        // 有瞄具返回瞄具位置，没有瞄具返回机瞄位置
        SightsLocatorPosition,
        // 武器特效
        EffectLocation,
        //
        EndOfTheWorld
    }
}
