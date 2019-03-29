
using App.Shared;
using App.Shared.Audio;
public class AudioTriggerBase : AudioComponent
{
    public Trigger triggerDelegate = null;
    public void Register(Trigger t)
    {
        if (triggerDelegate == null) triggerDelegate = t;
        triggerDelegate += t;
    }
    public void UnRegister(Trigger t)
    {
        if (triggerDelegate != null)
        {
            triggerDelegate -= t;
        }
    }
    public delegate void Trigger(AudioTriggerArgs args);

    public static System.Collections.Generic.Dictionary<uint, string> GetAllDerivedTypes()
    {
        var derivedTypes = new System.Collections.Generic.Dictionary<uint, string>();

        var baseType = typeof(AudioTriggerBase);

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
                var typeSplits = typeName.Split('_');
                string computedSplit = typeSplits.Length > 1 ? typeSplits[1] : typeSplits[0];
                derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(computedSplit), typeName);
            }
        }
#endif

        //Add the Awake, Start and Destroy triggers and build the displayed list.
        //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
        //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
        //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");

        return derivedTypes;
    }
}




