namespace Core.Ui
{
    public enum UiGroup
    {
        Fix = 0,//始终固定显示，不受影响的
        Base = 1,//基础UI
        Pop = 2,//弹出UI，隐藏Base组
        Alert = 3,//提示UI，

        MapHide = 4,
        SurvivalBagHide = 5,
        TimeCountDownHide = 6,  

        Singleton = 99,//次组UI只能同时显示一个，
    }
}