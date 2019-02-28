#if UNITY_EDITOR
using System.Collections.Generic;

#region Custom Editor
[UnityEditor.CustomEditor (typeof(AkPluginSettingsContainer))]
public class Editor : UnityEditor.Editor
{
	private const string UserSettings = "UserSettings";
	private const string AdvancedSettings = "AdvancedSettings";
	private const string CommsSettings = "CommsSettings";

	private List<AkPluginSettingsContainer.PlatformSettings> PreviousPlatformSettingsList
        = new List<AkPluginSettingsContainer.PlatformSettings> ();

	private List<GlobalSettingsGroupData> GlobalSettingsGroups
        = new List<GlobalSettingsGroupData> ();

	private Dictionary<string, HashSet<string>> GlobalGroupSettingsMap
        = new Dictionary<string, HashSet<string>> ();

	private List<PlatformSpecificSettingsData> PlatformSpecificSettingsGroups
        = new List<PlatformSpecificSettingsData> ();

	public void OnEnable ()
	{
		foreach (var settingsGroup in new[] { UserSettings, AdvancedSettings, CommsSettings }) {
			var property = serializedObject.FindProperty (settingsGroup);
			if (property == null)
				return;

			var type = System.Type.GetType (property.type);
			foreach (var field in type.GetFields()) {
				var childProperty = property.FindPropertyRelative (field.Name);
				if (childProperty == null)
					continue;

				HashSet<string> hashSet = null;
				if (!GlobalGroupSettingsMap.TryGetValue (settingsGroup, out hashSet)) {
					hashSet = new HashSet<string> ();
					GlobalGroupSettingsMap.Add (settingsGroup, hashSet);
				}

				hashSet.Add (childProperty.propertyPath);
			}
		}

		GlobalSettingsGroups.Add (new GlobalSettingsGroupData (UserSettings, serializedObject, "", GlobalGroupSettingsMap [UserSettings]));
		GlobalSettingsGroups.Add (new GlobalSettingsGroupData (AdvancedSettings, serializedObject, "", GlobalGroupSettingsMap [AdvancedSettings]));
		GlobalSettingsGroups.Add (new GlobalSettingsGroupData (CommsSettings, serializedObject, "Wwise Communication Settings", GlobalGroupSettingsMap [CommsSettings]));
	}

	public override void OnInspectorGUI ()
	{
		var settings = target as AkPluginSettingsContainer;
        if (!settings)
            return;
		if (!settings.IsValid) {
			UnityEditor.EditorGUILayout.HelpBox ("Platform names do not correspond with their associated settings data.", UnityEditor.MessageType.Error);
			return;
		}

		UpdatePlatformData (settings);
		DrawHelpBox (settings);

		UnityEditor.EditorGUILayout.Space ();

		if (PreviousPlatformSettingsList.Count == 0) {
			if (!AkUtilities.IsWwiseProjectAvailable) {
				UnityEditor.EditorGUILayout.HelpBox ("The Wwise project is not available. Please specify its location within the Wwise Settings.", UnityEditor.MessageType.Warning);
				return;
			}

			UnityEditor.EditorGUILayout.HelpBox ("No Wwise platforms have been added. Editing global settings.", UnityEditor.MessageType.Warning);
			UnityEditor.EditorGUILayout.Space ();
		}

		UnityEditor.EditorGUI.BeginChangeCheck ();

		serializedObject.Update ();

		foreach (var setting in GlobalSettingsGroups)
			setting.Draw ();

		serializedObject.ApplyModifiedProperties ();

		UnityEditor.EditorGUILayout.Space ();

		foreach (var setting in PlatformSpecificSettingsGroups)
			setting.Draw ();

		if (UnityEditor.EditorGUI.EndChangeCheck ())
			settings.ActiveSettingsHaveChanged = true;
	}

	private void UpdatePlatformData (AkPluginSettingsContainer settings)
	{
		var firstNotSecond = System.Linq.Enumerable.Except (PreviousPlatformSettingsList, settings.PlatformSettingsList);
		var secondNotFirst = System.Linq.Enumerable.Except (settings.PlatformSettingsList, PreviousPlatformSettingsList);
		var refreshRequired = System.Linq.Enumerable.Any (firstNotSecond) || System.Linq.Enumerable.Any (secondNotFirst);
		if (!refreshRequired) {
			foreach (var platformSettings in settings.PlatformSettingsList) {
				if (platformSettings == null) {
					refreshRequired = true;
					break;
				}
			}

			if (!refreshRequired)
				return;
		}

		PreviousPlatformSettingsList.Clear ();
		PlatformSpecificSettingsGroups.Clear ();

		var platformNames = new HashSet<string> ();

		foreach (var setting in GlobalSettingsGroups)
			setting.ClearPlatformData ();

		for (var i = 0; i < settings.Count; ++i) {
			var platformSettings = settings.PlatformSettingsList [i];
			if (!platformSettings)
				continue;

			var platformName = settings.PlatformSettingsNameList [i];
			if (string.IsNullOrEmpty (platformName))
				continue;

			if (!platformNames.Contains (platformName)) {
				platformNames.Add (platformName);
				PreviousPlatformSettingsList.Add (platformSettings);

				var platform = new PlatformData {
					Settings = platformSettings,
					Name = platformName,
					SerializedObject = new UnityEditor.SerializedObject (platformSettings)
				};

				foreach (var setting in GlobalSettingsGroups)
					setting.SetupPlatform (platform);

				foreach (var settingsGroup in new[] { UserSettings, AdvancedSettings, CommsSettings })
					PlatformSpecificSettingsGroups.Add (new PlatformSpecificSettingsData (platform, settingsGroup, GlobalGroupSettingsMap [settingsGroup]));
			}
		}
	}

	private static void DrawHelpBox (AkPluginSettingsContainer settings)
	{
		if (settings.ActiveSettingsHaveChanged) {
			var hash = AkUtilities.GetHashOfActiveSettings ();
			settings.ActiveSettingsHaveChanged = string.IsNullOrEmpty (hash) || hash != settings.ActiveSettingsHash;
		}

		var helpBoxText = "No changes have been made. Please be advised that changes will take effect once the Editor exits play mode.";
		var messageType = UnityEditor.MessageType.Info;

		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isPlaying || UnityEditor.BuildPipeline.isBuildingPlayer) {
			helpBoxText = "Changes will take effect once the Editor exits play mode.";
		} else if (settings.ActiveSettingsHaveChanged) {
			helpBoxText = "Changes have been made and will take effect once the Editor exits play mode.";
			messageType = UnityEditor.MessageType.Warning;
		}

		UnityEditor.EditorGUILayout.HelpBox (helpBoxText, messageType);
	}

	private static IEnumerable<UnityEditor.SerializedProperty> GetChildren (UnityEditor.SerializedProperty property)
	{
		property = property.Copy ();
		var nextElement = property.Copy ();
		bool hasNextElement = nextElement.Next (false);
		if (!hasNextElement)
			nextElement = null;

		if (!property.Next (true))
			yield break;

		while (!UnityEditor.SerializedProperty.EqualContents (property, nextElement)) {
			yield return property.Copy ();

			if (!property.Next (false))
				break;
		}
	}

	private static bool DrawFoldout (UnityEditor.SerializedProperty property, UnityEngine.GUIContent label, UnityEngine.FontStyle fontStyle)
	{
		var settingsFoldoutStyle = new UnityEngine.GUIStyle (UnityEditor.EditorStyles.foldout) { fontStyle = fontStyle };
		var value = UnityEditor.EditorGUILayout.Foldout (property.isExpanded, label, true, settingsFoldoutStyle);
		property.isExpanded = value;
		return value;
	}

	private class PlatformData
	{
		public AkPluginSettingsContainer.PlatformSettings Settings;
		public UnityEditor.SerializedObject SerializedObject;
		public string Name;
	}

	private class GlobalSettingsGroupData
	{
		string ToolTip;
		string DisplayName;
		UnityEditor.SerializedProperty Property;
		List<GlobalSettingData> SettingsList;

		public GlobalSettingsGroupData (string settingsGroup, UnityEditor.SerializedObject serializedObject, string displayName, HashSet<string> propertyHashSet)
		{
			Property = serializedObject.FindProperty (settingsGroup);
			if (Property == null)
				return;

			ToolTip = AkUtilities.GetTooltip (Property);
			DisplayName = string.IsNullOrEmpty (displayName) ? ("Common " + Property.displayName) : displayName;
			SettingsList = new List<GlobalSettingData> ();

			foreach (var childPropertyPath in propertyHashSet) {
				var childProperty = serializedObject.FindProperty (childPropertyPath);
				if (childProperty == null)
					continue;

				SettingsList.Add (new GlobalSettingData (childProperty, childPropertyPath));
			}
		}

		public void SetupPlatform (PlatformData platform)
		{
			if (SettingsList == null || SettingsList.Count == 0)
				return;

			foreach (var settings in SettingsList)
				settings.SetupPlatform (platform);
		}

		public void ClearPlatformData ()
		{
			if (SettingsList != null)
				foreach (var setting in SettingsList)
					setting.ClearPlatformData ();
		}

		public void Draw ()
		{
			if (SettingsList == null || SettingsList.Count == 0)
				return;

			using (var verticalScope = new UnityEditor.EditorGUILayout.VerticalScope ("box")) {
				++UnityEditor.EditorGUI.indentLevel;

				var label = new UnityEngine.GUIContent { text = DisplayName, tooltip = ToolTip };
				if (DrawFoldout (Property, label, UnityEngine.FontStyle.Bold))
					foreach (var settings in SettingsList)
						settings.Draw ();

				--UnityEditor.EditorGUI.indentLevel;
			}
		}

		public class GlobalSettingData
		{
			string ToolTip;
			readonly string DisplayName;
			readonly string PropertyPath;
			readonly bool IsStringValue;
			UnityEditor.SerializedProperty Property;
			readonly UnityEditor.SerializedPropertyType PropertyType;
			List<PlatformSettingData> SettingsList;
			List<GlobalSettingData> Children;

			bool HasChildren { get { return Children != null && Children.Count > 0; } }

			public GlobalSettingData (UnityEditor.SerializedProperty property, string propertyPath)
			{
				Property = property;
				PropertyType = property.propertyType;
				PropertyPath = propertyPath;
				DisplayName = property.displayName;
				ToolTip = AkUtilities.GetTooltip (property);

				if (property.type == "string") {
					IsStringValue = true;
				} else if (property.hasChildren) {
					Children = new List<GlobalSettingData> ();
					foreach (var child in GetChildren(property))
						Children.Add (new GlobalSettingData (child, child.propertyPath));
				}
			}

			public void SetupPlatform (PlatformData platform)
			{
				if (HasChildren) {
					foreach (var child in Children)
						child.SetupPlatform (platform);
				} else {
					if (platform.Settings.IsPropertyIgnored (PropertyPath))
						return;

					if (SettingsList == null)
						SettingsList = new List<PlatformSettingData> ();

					SettingsList.Add (new PlatformSettingData (platform, PropertyPath, ToolTip));
				}
			}

			public void ClearPlatformData ()
			{
				SettingsList = null;

				if (HasChildren)
					foreach (var child in Children)
						child.ClearPlatformData ();
			}

			private bool AnyChildUsesGlobalValue {
				get {
					if (Children != null && Children.Count > 0) {
						foreach (var child in Children)
							if (child.AnyChildUsesGlobalValue)
								return true;

						return false;
					}

					if (SettingsList == null || SettingsList.Count == 0)
						return true;

					foreach (var settings in SettingsList)
						if (settings.Platform.Settings.GlobalPropertyHashSet.Contains (PropertyPath))
							return true;

					return false;
				}
			}

			private bool AllChildrenAreEqual {
				get {
					if (Children != null && Children.Count > 0) {
						foreach (var child in Children)
							if (!child.AllChildrenAreEqual)
								return false;

						return true;
					}

					if (SettingsList == null)
						return true;

					switch (PropertyType) {
					case UnityEditor.SerializedPropertyType.Boolean:
						var boolValue = Property.boolValue;
						foreach (var settings in SettingsList)
							if (boolValue != settings.Property.boolValue)
								return false;
						return true;

					case UnityEditor.SerializedPropertyType.Enum:
						var enumValueIndex = Property.enumValueIndex;
						foreach (var settings in SettingsList)
							if (enumValueIndex != settings.Property.enumValueIndex)
								return false;
						return true;

					case UnityEditor.SerializedPropertyType.Float:
						var floatValue = Property.floatValue;
						foreach (var settings in SettingsList)
							if (floatValue != settings.Property.floatValue)
								return false;
						return true;

					case UnityEditor.SerializedPropertyType.Integer:
						var longValue = Property.longValue;
						foreach (var settings in SettingsList)
							if (longValue != settings.Property.longValue)
								return false;
						return true;

					case UnityEditor.SerializedPropertyType.String:
						var stringValue = Property.stringValue;
						foreach (var settings in SettingsList)
							if (stringValue != settings.Property.stringValue)
								return false;
						return true;
					}

					return true;
				}
			}

			public void Draw ()
			{
				var hasChanged = false;

				var isString = IsStringValue;
				var hasChildren = HasChildren;

				if (!hasChildren && (SettingsList == null || SettingsList.Count == 0))
					return;

				using (var verticalScope = new UnityEditor.EditorGUILayout.VerticalScope ()) {
					var indentLevel = UnityEditor.EditorGUI.indentLevel++;

					var forceExpand = !AnyChildUsesGlobalValue;
					if (forceExpand && !hasChildren) {
						UnityEditor.EditorGUILayout.LabelField (DisplayName);
					} else {
						var label = new UnityEngine.GUIContent (DisplayName, ToolTip);
						DrawFoldout (Property, label, AllChildrenAreEqual ? UnityEngine.FontStyle.Normal : UnityEngine.FontStyle.Italic);

						if (!hasChildren) {
							UnityEditor.EditorGUI.BeginChangeCheck ();
							var labelWithTooltipOnly = new UnityEngine.GUIContent { tooltip = ToolTip };
							if (isString)
								UnityEditor.EditorGUILayout.DelayedTextField (Property, labelWithTooltipOnly);
							else
								UnityEditor.EditorGUILayout.PropertyField (Property, labelWithTooltipOnly, false);
							hasChanged = UnityEditor.EditorGUI.EndChangeCheck ();
						}
					}

					if (hasChildren) {
						if (Property.isExpanded)
							foreach (var child in Children)
								child.Draw ();
					} else if (forceExpand || Property.isExpanded) {
						foreach (var settings in SettingsList)
							settings.Draw (Property, PropertyPath, forceExpand);
					} else if (hasChanged) {
						foreach (var settings in SettingsList)
							if (settings.Platform.Settings.GlobalPropertyHashSet.Contains (PropertyPath))
								settings.UpdateValue (Property);
					}

					UnityEditor.EditorGUI.indentLevel = indentLevel;
				}
			}

			public class PlatformSettingData
			{
				string ToolTip;
				public UnityEditor.SerializedProperty Property;
				public PlatformData Platform;

				public PlatformSettingData (PlatformData platform, string propertyPath, string tooltip)
				{
					Platform = platform;
					Property = Platform.SerializedObject.FindProperty (propertyPath);
					ToolTip = string.IsNullOrEmpty (tooltip) ? AkUtilities.GetTooltip (Property) : tooltip;
				}

				public void UpdateValue (UnityEditor.SerializedProperty globalProperty)
				{
					if (Property == null)
						return;

					Platform.SerializedObject.Update ();
					PropagateValue (Property, globalProperty);
					Platform.SerializedObject.ApplyModifiedProperties ();
				}

				public void Draw (UnityEditor.SerializedProperty globalProperty, string propertyPath, bool forceExpand)
				{
					if (Property == null)
						return;

					var indentLevel = UnityEditor.EditorGUI.indentLevel++;
					var position = UnityEngine.GUILayoutUtility.GetRect (UnityEngine.GUIContent.none, UnityEngine.GUIStyle.none, UnityEngine.GUILayout.Height (UnityEditor.EditorGUIUtility.singleLineHeight));

					var wasUsingGlobalValue = Platform.Settings.GlobalPropertyHashSet.Contains (propertyPath);
					var width = position.width;
					if (!wasUsingGlobalValue)
						position.width = UnityEditor.EditorGUIUtility.labelWidth;

					var isUsingGlobalValue = UnityEditor.EditorGUI.ToggleLeft (position, Platform.Name, wasUsingGlobalValue);
					position.width = width;

					if (wasUsingGlobalValue != isUsingGlobalValue)
						Platform.Settings.SetUseGlobalPropertyValue (propertyPath, isUsingGlobalValue);

					if (!isUsingGlobalValue) {
						position.x += UnityEditor.EditorGUIUtility.labelWidth;
						position.width -= UnityEditor.EditorGUIUtility.labelWidth;
						UnityEditor.EditorGUI.indentLevel = 1; // Not zero, so that a control handle is available

						Platform.SerializedObject.Update ();
						UnityEditor.EditorGUI.PropertyField (position, Property, new UnityEngine.GUIContent { tooltip = ToolTip });
						Platform.SerializedObject.ApplyModifiedProperties ();
					} else if (forceExpand)
						PropagateValue (globalProperty, Property);
					else
						UpdateValue (globalProperty);

					UnityEditor.EditorGUI.indentLevel = indentLevel;
				}

				private static void PropagateValue (UnityEditor.SerializedProperty x, UnityEditor.SerializedProperty y)
				{
					//if (x.propertyType != y.propertyType)
					//	return;

					switch (x.propertyType) {
					case UnityEditor.SerializedPropertyType.Boolean:
						x.boolValue = y.boolValue;
						break;

					case UnityEditor.SerializedPropertyType.Enum:
						x.longValue = y.longValue;
						break;

					case UnityEditor.SerializedPropertyType.Float:
						x.floatValue = y.floatValue;
						break;

					case UnityEditor.SerializedPropertyType.Integer:
						x.longValue = y.longValue;
						break;

					case UnityEditor.SerializedPropertyType.String:
						x.stringValue = y.stringValue;
						break;

					case UnityEditor.SerializedPropertyType.Generic:
						if (x.type == y.type) {
							var XProperty = x.Copy ();
							var YProperty = y.Copy ();
							var XEndProperty = x.Copy ();
							var YEndProperty = y.Copy ();
							XEndProperty = XEndProperty.Next (false) ? XEndProperty : null;
							YEndProperty = YEndProperty.Next (false) ? YEndProperty : null;

							while (XProperty.Next (true) && YProperty.Next (true) && !UnityEditor.SerializedProperty.EqualContents (XProperty, XEndProperty) && !UnityEditor.SerializedProperty.EqualContents (YProperty, YEndProperty))
								PropagateValue (XProperty, YProperty);
						}
						break;
					}
				}
			}
		}
	}

	private class PlatformSpecificSettingsData
	{
		public string ToolTip;
		public UnityEditor.SerializedProperty Property;
		public List<UnityEditor.SerializedProperty> SettingsList;
		public PlatformData Platform;

		public PlatformSpecificSettingsData (PlatformData platform, string propertyPath, HashSet<string> globalPropertyHashSet)
		{
			Platform = platform;
			Property = Platform.SerializedObject.FindProperty (propertyPath);
			if (Property == null)
				return;

			ToolTip = AkUtilities.GetTooltip (Property);

			HashSet<string> hashSet = new HashSet<string> ();
			foreach (var childProperty in GetChildren(Property))
				hashSet.Add (childProperty.propertyPath);

			var remainder = System.Linq.Enumerable.ToArray (System.Linq.Enumerable.Except (hashSet, globalPropertyHashSet));
			if (remainder.Length > 0)
				SettingsList = new List<UnityEditor.SerializedProperty> ();

			foreach (var childPropertyPath in remainder) {
				var childProperty = Platform.SerializedObject.FindProperty (childPropertyPath);
				if (childProperty != null)
					SettingsList.Add (childProperty);
			}
		}

		public void Draw ()
		{
			if (SettingsList == null || SettingsList.Count == 0)
				return;

			Platform.SerializedObject.Update ();

			using (new UnityEditor.EditorGUILayout.VerticalScope ("box")) {
				++UnityEditor.EditorGUI.indentLevel;

				var label = new UnityEngine.GUIContent (Platform.Name + " Specific " + Property.displayName, ToolTip);
				if (DrawFoldout (Property, label, UnityEngine.FontStyle.Bold)) {
					++UnityEditor.EditorGUI.indentLevel;

					foreach (var child in SettingsList)
						UnityEditor.EditorGUILayout.PropertyField (child, true);

					--UnityEditor.EditorGUI.indentLevel;
				}

				--UnityEditor.EditorGUI.indentLevel;
			}

			Platform.SerializedObject.ApplyModifiedProperties ();
		}
	}
}
#endregion
#endif