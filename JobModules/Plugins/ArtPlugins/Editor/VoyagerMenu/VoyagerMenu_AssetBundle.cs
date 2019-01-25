using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor.Build;
using Random = System.Random;
using System.Collections;


public class VoyagerMenu_AssetBundle : MonoBehaviour {

	//=========================Debug Build Asset Bundle===============================================//
	private const string TargetDir = "../../../release/";

	private static void BuildPartlyAssetBunles_compress(int n, int m){
		#if CUSTOM_BUILD_ASSETBUNDLES
		AssetDatabase.RemoveUnusedAssetBundleNames();
		// Choose the output path according to the build target.
		string outputPath = Path.GetFullPath(Path.Combine(TargetDir + "AssetBundles_partly", VoyagerMenu.GetPlatformName()));
		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);
		string[] allBundles = GetBundleNameList();
		int size = allBundles.Length;
		int start = size * (n-1)/m;
		int end = size * n/m ;
		List<string> bundleNameList = new List<string>();
		for(int index=start; index< end ; index++){
			Debug.LogFormat("AssetBundle index=[{0}], name=[{1}] is added to list", index, allBundles[index]);
			bundleNameList.Add(allBundles[index]);
		}
		var manifest = BuildPipeline.BuildSpecifiedAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64, bundleNameList.ToArray());
		if (manifest == null)
			throw new Exception("Build Asset Bundle fail, for manifest is null");
		EditorUtility.DisplayDialog("Info", "Build Assets at " + outputPath + ", result" + manifest.GetAllAssetBundles().Length, "OK");
		#endif
	}

	private static void BuildPartlyAssetBunles_include(string nameStartWith, string partId){
		#if CUSTOM_BUILD_ASSETBUNDLES
		AssetDatabase.RemoveUnusedAssetBundleNames();
		// Choose the output path according to the build target.
		string outputPath = Path.GetFullPath(Path.Combine(TargetDir + "AssetBundles_partly", VoyagerMenu.GetPlatformName()+partId));
		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);
		string[] allBundles = GetBundleNameList();
		int size = allBundles.Length;
		List<string> bundleNameList = new List<string>();
		for(int index=0; index< size ; index++){
			if(allBundles[index].StartsWith(nameStartWith)){
				Debug.LogFormat("AssetBundle index=[{0}], name=[{1}] is added to list", index, allBundles[index]);
				bundleNameList.Add(allBundles[index]);
			}
		}
		var manifest = BuildPipeline.BuildSpecifiedAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64, bundleNameList.ToArray());
		if (manifest == null)
			throw new Exception("Build Asset Bundle fail, for manifest is null");
		EditorUtility.DisplayDialog("Info", "Build Assets at " + outputPath + ", result" + manifest.GetAllAssetBundles().Length, "OK");
		#endif
	}

	private static void BuildPartlyAssetBunles_exclude(List<string> nameStartWith, string partId){
		#if CUSTOM_BUILD_ASSETBUNDLES
		AssetDatabase.RemoveUnusedAssetBundleNames();
		// Choose the output path according to the build target.
		string outputPath = Path.GetFullPath(Path.Combine(TargetDir + "AssetBundles_partly", VoyagerMenu.GetPlatformName()+partId));
		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);
		string[] allBundles = GetBundleNameListExclude(nameStartWith);
		int size = allBundles.Length;
		List<string> bundleNameList = new List<string>();
		for(int index=0; index< size ; index++){
			Debug.LogFormat("AssetBundle index=[{0}], name=[{1}] is added to list", index, allBundles[index]);
			bundleNameList.Add(allBundles[index]);
		}
		var manifest = BuildPipeline.BuildSpecifiedAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64, bundleNameList.ToArray());
		if (manifest == null)
			throw new Exception("Build Asset Bundle fail, for manifest is null");
		EditorUtility.DisplayDialog("Info", "Build Assets at " + outputPath + ", result" + manifest.GetAllAssetBundles().Length, "OK");
		#endif
	}

	//if number=0, build all except dynamicscene
	private static void BuildRandomAssetBundles_compress(int number)
	{
		#if CUSTOM_BUILD_ASSETBUNDLES
		List<string> excludeList = new List<string>() {"dynamicscene" };

		AssetDatabase.RemoveUnusedAssetBundleNames();
		// Choose the output path according to the build target.
		string outputPath = Path.GetFullPath(Path.Combine(TargetDir + "AssetBundles_random", VoyagerMenu.GetPlatformName()));
		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);
		string[] allBundles = GetBundleNameList();
		int size = allBundles.Length;
		List<string> bundleNameList = new List<string>();
		if (number == 0)
		{
			Debug.LogFormat("number=0, build all except dynamicscene");
			foreach (string name in allBundles)
			{
				if (!excludeList.Contains(name))
				{
					bundleNameList.Add(name);
				}
			}
		}else{
			Debug.LogFormat("number={0}, ramdom build bundles", number);
			for (int index = 0; index < number; index++)
			{
				Random random = new Random();
				bundleNameList.Add(allBundles[random.Next(size - 1)]);
			}
		}
		var manifest = BuildPipeline.BuildSpecifiedAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64, bundleNameList.ToArray());
		if (manifest == null)
			throw new Exception("Build Asset Bundle fail, for manifest is null");
		EditorUtility.DisplayDialog("Info", "Build Assets at " + outputPath + ", result" + manifest.GetAllAssetBundles().Length, "OK");
		#endif
	}


	public static List<string> packages = new List<string>{"dynamicscene", "maps_maps", "level", "maps_textures"};
	/**
	 * arg1:
	 * if = dynamicscene, build dynamicscene
	 * if = maps_nonprefab/, build maps_nonprefab/*
	 * if = maps_textures/, build maps_textures/*
	 * if = others, build all exclude packages list
	 * if = n/m, build from (n-1)/m to n/m
	 * if = n, random build n bundles
	 * */
	public static void RandomBuildAssetBundles()
	{
		Debug.LogFormat("[RandomBuildAssetBundles]Begin RandomBuildAssetBundles at: {0}", DateTime.Now);
		Debug.LogFormat("Begin FinalizeGameRoadNetwork at: {0}", DateTime.Now);
		VoyagerMenu.FinalizeGameRoadNetwork();
        Debug.LogFormat("End FinalizeGameRoadNetwork at: {0}", DateTime.Now);
        Debug.LogFormat("Begin BuildOC at: {0}", DateTime.Now);
		VoyagerMenu.BuildOC();
        Debug.LogFormat("End BuildOC at: {0}", DateTime.Now);
        string arg1 = System.Environment.GetCommandLineArgs()[1];
		if (arg1.Equals ("P1")) {
			Debug.LogFormat ("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build dynamicscene", arg1);
			BuildPartlyAssetBunles_include (packages [0], "");
		}
		else if (arg1.Equals ("P2")) {
			Debug.LogFormat ("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build maps_0001_*/*", arg1);
			BuildPartlyAssetBunles_include (packages [1], "");
		}
		else if (arg1.Equals ("P3")) {
			Debug.LogFormat ("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build maps_M00*/*", arg1);
			BuildPartlyAssetBunles_include (packages [2], "");
		}
		else if (arg1.Equals("P4")) {
			Debug.LogFormat("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build maps_textures/*", arg1);
			BuildPartlyAssetBunles_include(packages[3], "");
		}
		else if (arg1.Equals ("P5")) {
			Debug.LogFormat ("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build others", arg1);
			BuildPartlyAssetBunles_exclude (packages, "");
		}else if (arg1.Contains ("-")) {
			Debug.LogFormat ("[RandomBuildAssetBundles]System.Environment.GetCommandLineArgs is {0}, build n/m", arg1);
			string[] args = arg1.Split ('-');
			BuildPartlyAssetBunles_compress (int.Parse (args[0]), int.Parse(args[1]));
		} else {
			int number = int.Parse (arg1);
			Debug.LogFormat ("[RandomBuildAssetBundles]RandomBuildAssetBundles({0})", number);
			BuildRandomAssetBundles_compress (number);
		}
		Debug.LogFormat ("[RandomBuildAssetBundles]End RandomBuildAssetBundles at: {0}", DateTime.Now);
	}

	public static long GetDirectoryLength(string dirPath)
	{
		if (!Directory.Exists(dirPath))
			return 0;
		long len = 0;

		DirectoryInfo dir = new DirectoryInfo(dirPath);

		foreach (FileInfo fi in dir.GetFiles())
		{
			len += fi.Length/1024;
		}

		DirectoryInfo[] dis = dir.GetDirectories();
		if (dis.Length > 0)
		{
			for (int i = 0; i < dis.Length; i++)
			{
				len += GetDirectoryLength(dis[i].FullName);
			}
		}
		return len;
	}

	private static string[] GetBundleNameList()
	{
		AssetDatabase.RemoveUnusedAssetBundleNames();
		return AssetDatabase.GetAllAssetBundleNames();
	}

	private static string[] GetBundleNameListExclude(List<string> nameStartWith){
		string[] all = GetBundleNameList ();
		List<string> retList = new List<string> ();
		foreach(string name in all){
			bool isAdd = true;
			foreach(string startWith in nameStartWith){
				if(name.StartsWith(startWith)){
					isAdd = false;
					break;
				}
			}
			if (isAdd) {
				retList.Add (name);
			}
		}
		Debug.LogFormat ("[GetBundleNameListExclude]AssetBundels Count = {0} are added to list from count={1}", retList.Count, all.Length);
		return retList.ToArray();
	}


	static string localOutput = "../../../release";
	static int threshold = 999;
	//[MenuItem("Voyager/DebugBuildAB/degbug")]
	public static void BuildAssetBundle_debug()
	{
		//BuildHalfAssetBundles_compress(true);
		GetBundleNameListExclude(packages);
	}

	private static Hashtable FetchAssetBundleTable()
	{
		AssetDatabase.RemoveUnusedAssetBundleNames();
		string[] abNameList = AssetDatabase.GetAllAssetBundleNames();

		Hashtable ht = new Hashtable();
		foreach (string abName in abNameList)
		{
			int assetCount = 0;
			string[] assetPathList = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
			foreach (string assetPath in assetPathList)
			{
				//Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
				string[] assets = AssetDatabase.GetDependencies(assetPath);
				assetCount += assets.Length;
			}
			ht.Add(abName, assetCount);
		}
		return ht;
	}

	private static string[] SortAssetBundle(Hashtable ht)
	{
		string[] keys = new string[ht.Count];
		double[] values = new double[ht.Count];
		ht.Keys.CopyTo(keys, 0);
		ht.Values.CopyTo(values, 0);
		Array.Sort(values, keys);
		return keys;
	}

	static StreamWriter sw;
	static FileInfo myFile;
	public static void WritetLog(string line)
	{
		sw = myFile.AppendText();
		sw.WriteLine(line);
		sw.Close();
	}

	public static void AppendLog(string line)
	{
		sw = myFile.AppendText();
		sw.Write(line);
		sw.Close();
	}

	public static void WritetLogFormat(string format, params object[] args)
	{
		WritetLog(String.Format(format, args));
	}

	public static void AppendLogFormat(string format, params object[] args)
	{
		AppendLog(String.Format(format, args));
	}

	public static void StartLog()
	{
		DateTime t = DateTime.Now;
		string logPath = Path.GetFullPath(localOutput);
		if (!Directory.Exists(logPath))
			Directory.CreateDirectory(logPath);
		myFile = new FileInfo(string.Format(logPath + "/unity-log-{0}-{1}-{2}.txt", t.Hour, t.Minute, t.Millisecond));
		sw = myFile.CreateText();
		sw.WriteLine("Begin Log at: {0}", DateTime.Now);
		sw.Close();
	}

	public static void EndLog()
	{
		sw = myFile.AppendText();
		sw.WriteLine("End Log at: {0}", DateTime.Now);
		sw.Close();
	}
}
