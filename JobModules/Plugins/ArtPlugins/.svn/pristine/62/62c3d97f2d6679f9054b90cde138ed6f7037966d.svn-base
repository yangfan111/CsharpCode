using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;

public class BigMapAssetPreload : EditorWindow
{
    private static string[] bigMaps = new string[] { "level", "number" };
    private static string resPath = "http://192.168.0.41:8030/Windows/";
    private static long currentByteLoaded = 0;
    private static long currentByteAll = 0;
    static int percent;
    static int currentFileIndex=-2;
    private static string currentFile;
    private static string loaedInfo;
    [MenuItem("Tools/大地图资源预加载")]
    private static void loadAllMaps() {
        EditorWindow.GetWindow<BigMapAssetPreload>().Show();

        Debug.LogError(4);
        PlayerPrefs.DeleteKey("alwaysAssetsModeRes");
    }
    private void OnGUI()
    {
  
        if (GUILayout.Button("开始加载")) {
            currentFileIndex = -1;
            currentFile = null;
            loaedInfo = "";
          //  loadOne(bigMaps[currentFileIndex]);
            return;
        }
        if(currentFile!=null)
            GUILayout.Label("loading "+ currentFile+":" + percent + "%");
        if (currentFile == null&& currentFileIndex>=-1) {
            currentFileIndex++;
            if (currentFileIndex < bigMaps.Length)
                loadOne(bigMaps[currentFileIndex]);
        }
        if (loaedInfo != null&&loaedInfo!="") {
            GUILayout.Label(loaedInfo);
        }
     
    }
    private static void loadOne(string file)
    {
        string tempPath = Application.persistentDataPath + "\\tempBigMap";
        Debug.Log(tempPath);
        currentFile = file;
        System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
        WebClient wc=new WebClient();
        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
        wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
        
        wc.DownloadFileAsync(new Uri( resPath + file), tempPath+"\\"+ file);
        PlayerPrefs.SetString("alwaysAssetsModeRes", PlayerPrefs.GetString("alwaysAssetsModeRes","")+ file + "|");
        
    }

    private static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
     
        currentByteLoaded = e.BytesReceived;
        percent= e.ProgressPercentage;
        currentByteAll = e.TotalBytesToReceive;
        
      
    }

    private static void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        WebClient wc = sender as WebClient;
        loaedInfo += "已加载完成 " + currentFile + " !!\n";
       currentFile = null;
        wc.DownloadFileCompleted -= Wc_DownloadFileCompleted;
        wc.DownloadProgressChanged -= Wc_DownloadProgressChanged;

    }
}
