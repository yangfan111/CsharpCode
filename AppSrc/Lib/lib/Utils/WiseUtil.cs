using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using UnityEngine;

namespace YF.Utils
{
    public partial class CommonUtil
    {
	    public static T GetOrCreateAssetEditor<T>(string relativePath,string fileName) where T : ScriptableObject
	    {
		 //   var fullPath = Path.Combine(Application.dataPath, relativePath);
			var path = Path.Combine(relativePath, fileName);
		    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
		    if (!asset)
		    {
			    var fullPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, relativePath);
			    if (!System.IO.Directory.Exists(fullPath))
				    System.IO.Directory.CreateDirectory(fullPath);

			    asset = ScriptableObject.CreateInstance(fileName) as T;
			    UnityEditor.AssetDatabase.CreateAsset(asset, path);
		    }

		    return asset;
	    }
    	public static Dictionary<uint, string> GetAllDerivedTypes(System.Type baseType)
    	{
    		var derivedTypes = new System.Collections.Generic.Dictionary<uint, string>();
    
    
    #if UNITY_WSA && !UNITY_EDITOR
    		var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
    		var typeInfos = baseTypeInfo.Assembly.DefinedTypes;
    
    		foreach (var typeInfo in typeInfos)
    		{
    			if (typeInfo.IsClass && (typeInfo.IsSubclassOf(baseType) || baseTypeInfo.IsAssignableFrom(typeInfo) && baseType != typeInfo.AsType()))
    			{
    				var typeName = typeInfo.Name;
    				derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(typeName), typeName);
    			}
    		}
    #else
    		var types = baseType.Assembly.GetTypes();
    
    		for (var i = 0; i < types.Length; i++)
    		{
    			if (types[i].IsClass &&
    			    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i]) && baseType != types[i]))
    			{
    				var typeName = types[i].Name;
    				derivedTypes.Add(ShortIDGenerator.Compute(typeName), typeName);
    			}
    		}
    #endif
    
    		//Add the Awake, Start and Destroy triggers and build the displayed list.
//    		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
//    		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
//    		derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");
    
    		return derivedTypes;
    	}
    }
}