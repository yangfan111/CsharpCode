#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

//namespace Assets.Editor.packageUtils
//{ 
public class CheckGameObjectLogs
{
    private List<CheckGameObjectLog> _logList = new List<CheckGameObjectLog>();

    public CheckGameObjectLogs()
    {

    }

    [XmlArray("checks")]
    [XmlArrayItem("check")]
    public List<CheckGameObjectLog> LogList
    {
        get { return _logList; }
        set { _logList = value; }
    }

    public void AddLog(CheckGameObjectLog log)
    {
        _logList.Add(log);
    }

    public void WriteToXml(string xmlPath)
    {
        XmlLogUtil<CheckGameObjectLogs>.Write(this, xmlPath);
    }

    /**public void WriteToHtml(string xmlPath, string xsltPath, String htmlPath)
    {
        XmlLogUtil<CheckGameObjectLogs>.TransformToHtml(xmlPath, xsltPath, htmlPath);
    }**/

}

public class CheckGameObjectLog
{
    private string _methodName = null;
    private List<PathNode> data;

    [XmlAttribute("name")]
    public string MethodName
    {
        get { return _methodName; }
        set { _methodName = value; }
    }

    [XmlArray("result")]
    [XmlArrayItem("path")]
    public List<PathNode> PathNodeList
    {
        get { return data; }
        set { data = value; }
    }

    public CheckGameObjectLog()
    {
    }

    public CheckGameObjectLog(string methodName, List<PathNode> list) : this(methodName)
    {
        data = list;
    }

    public CheckGameObjectLog(string methodName)
    {
        _methodName = methodName;
    }

    public void AddAssetPath(PathNode asset)
    {
        data.Add(asset);
    }

    public void WriteToFile(string xmlPath)
    {
        XmlLogUtil<CheckGameObjectLog>.Write(this, xmlPath);
    }

}

public class PathNode
{
    private string _scene;
    private string _path;
    private string _svnpath;
    static string svnArtRepo = @"svn://svn2.wd.com/art_ssjj2/develop/Art.Client.Unity/";
    static string svnMapRepo = @"svn://svn2.wd.com/art_ssjj2/develop/Map.Client.Unity/";
    static string[] mapRepoDirs = { "Assets/DropPoints", "Assets/Gizmos", "Assets/Maps" };

    public PathNode()
    {

    }

    public PathNode(string scene, string path)
    {
        _scene = scene;
        _path = path;
    }

    [XmlAttribute("scene")]
    public string Type
    {
        get { return _scene; }
        set { _scene = value; }
    }

    [XmlAttribute("lastUpdated")]
    public string LastUpdated
    {
        get { return _svnpath; }
        set { _svnpath = value; }
    }

    [XmlTextAttribute]
    public string Path
    {
        get { return _path; }
        set { _path = value; }
    }

    public PathNode(GameObject go)
    {
        string sceneName = "none";

        if (go == null)
        {
            _scene = null;
            _path = null;
            _svnpath = null;
        }

        string scenePath = AssetDatabase.GetAssetPath(go);

        if (string.IsNullOrEmpty(scenePath))
        {
            _svnpath = GetSVNLastUpdate(go.scene.path);
            sceneName = go.scene.name;           
        }
        else
        {
            _svnpath = GetSVNLastUpdate(scenePath);
            scenePath = System.IO.Path.GetDirectoryName(scenePath);            
        }

        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        if (!string.IsNullOrEmpty(scenePath))
        {
            path = scenePath + "=>" + path;
        }
        //Debug.Log("GetAssetNode:sceneName=" + sceneName + ",path=" + path);
        _scene = sceneName;
        _path = path;
        //_svnpath = GetSVNLastUpdate(_svnpath);
    }

    private string GetSVNLastUpdate(string path)
    {
        string svnRepo = svnArtRepo;
        foreach(string dir in mapRepoDirs)
        {
            if (path.StartsWith(dir))
            {
                svnRepo = svnMapRepo;
                break;
            }
        }
        return svnRepo + path;
    }
}
//}
#endif