using System;
using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public class UserInputKeyComparer : IEqualityComparer<UserInputKey>
    {
        public bool Equals(UserInputKey x, UserInputKey y)
        {
            return x == y;
        }

        public int GetHashCode(UserInputKey obj)
        {
            return (int)obj;
        }

        public static readonly UserInputKeyComparer Instance = new UserInputKeyComparer();
    }
    /// <summary>
    /// 所有的按钮事件都要在这里定义，并在KeyConvert里面设定绑定的Unity.KeyCode
    /// </summary>
    public enum UserInputKey
    {
        None,
        Action,
        Switch1,
        Switch2,
        Switch3,
        Switch4,
        Switch5,
        Switch6,
        Switch7,
        OpenBag,
        OpenWeaponBag,
        LockCursor,
        UnlockCursor,
        //Tab,
        Fire,
        Yaw,
        Pitch,
        MoveHorizontal,
        MoveVertical,
        MoveUpDown,
        Jump,
        EnterVehicle,
        Run,
        Crouch,
        Prone,
        SlightWalk,
        //Swim,
        //Injured,
        Reload,
        PeekLeft,
        PeekRight,
        SwitchWeapon,
        CameraFocus,
        FirstPerson,
        FreeCamera,
        PickUp,
        PickUpTip,
        DropWeapon,
        SwitchFireMode,
        VehicleSpeedUp,
        VehicleBrake,
        VehicleHorn,
        VehicleLeftShift,
        VehicleRightShift,
        VehicleStunt,
        //VehicleReset,
        IsPDown,
        IsYDown,
        IsCDown,
        IsSpaceDown,
        IsTabDown,
        RightAttack,
        DrawWeapon,
        ChangeMapRate,      //改变大地图的缩放比例
        ShowMaxMap,         //显示大地图
        AddMark,            //添加标记
        MouseAddMark,       //鼠标添加标记
        LocationCurPlay,    //定位当前玩家
        OpenMenu,            //打开战斗菜单
        BreathHold,
        Throwing,
        HideWindow,         //关闭当前界面
        SwitchAutoRun,
        SplitProp,         //拆封道具
        SendChatMessage,  //发聊天消息
        SwitchChatChannel, //切换聊天频道
        CheckRanging,       //测试距离
        F1, 
        F2, 
        F3, 
        F4, 
        F5, 
        HoldF,
        ShowRecord,
        HideRecord,
        ShowDebug,
        SprayPaint, /*喷漆*/
        ScopeIn,
        ScopeOut,
        Count,
    }

    public enum InputInspectType
    {
        None,
        Button,
        Aixs,
        Key,
        
    }
    /// <summary>
    /// 键盘的按键状态,button和key的区别在于，button是可指定的（在setting->input)，key是固定的
    /// 基本和Input类一一对应，具体请参照文档 https://docs.unity3d.com/ScriptReference/Input.html
    /// 没有发现KeyDown和MouseButtonDown的区别，不重复定义了
    /// touch 和 joystick 暂不使用
    /// </summary>
    public enum  UserInputState
    {
        
        PointerMove,//鼠标直接移动
        PointerRaycast,//鼠标指向
        KeyDown,
        KeyUp,//Mouse0,Mouse0,Tab
        KeyHold,
        [Obsolete]ButtonHold,
        ButtonDown, //InputKey: Jump,CameraFocus
        [Obsolete]ButtonUp,
        Axis,//摇杆 InputKey : Horizontal,Vertical,UpDown,Mouse ScrollWheel
        [Obsolete]AxisRow,
    }
}
