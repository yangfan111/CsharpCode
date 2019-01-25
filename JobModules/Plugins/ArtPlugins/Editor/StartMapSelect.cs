﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StartMapSelect : Editor {

    [MenuItem("Tools/启动地图选择/读配置模式")]
    private static void selectmap99()
    {
        PlayerPrefs.DeleteKey("Test_InitMapId");
    }
    [MenuItem("Tools/启动地图选择/test地图")]
    private static void selectmap0() {
        PlayerPrefs.SetInt("Test_InitMapId", 0);
    }
    [MenuItem("Tools/启动地图选择/射击测试")]
    private static void selectmap3()
    {
        PlayerPrefs.SetInt("Test_InitMapId", 3);
    }
    [MenuItem("Tools/启动地图选择/冰雪堡垒")]
    private static void selectmap1()
    {
        PlayerPrefs.SetInt("Test_InitMapId", 1);
    }
    [MenuItem("Tools/启动地图选择/海岛地图")]
    private static void selectmap2()
    {
        PlayerPrefs.SetInt("Test_InitMapId", 2);
    }

    [MenuItem("Tools/启动地图选择/S003")]
    private static void selectS003()
    {
        PlayerPrefs.SetInt("Test_InitMapId", 1003);
    }  
     
}
