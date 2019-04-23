using UnityEngine;

public class InspectorBuildConst
{
    public static readonly GUIStyle DefaultStyle = new UnityEngine.GUIStyle(UnityEngine.GUI.skin.button);

    public static GUILayoutOption DefaultLayoutOption
    {
        get { return GUILayout.MaxWidth(InspectorWidth); }
    }

    public static float InspectorWidth
    {
        get
        {
            return UnityEngine.Screen.width - UnityEngine.GUI.skin.box.margin.left -
                   UnityEngine.GUI.skin.box.margin.right;
        }
    }

//    public static int GetBuildMaskByArrIndex(int index)
//    {
//        if (index < 0)
//            return 0;//return nothing
//        return 1 << index;//return 2~
//    }
//    public static int GetArrIndexByBuildMask(int mask,int length)
//    {
//        if (mask <= 0)
//            return -1;
//        for (int i = 1; i <= length; i++)
//        {
//            if (mask >> i == 1)
//                return i;
//        }
//        return -1;
//
//    }
}