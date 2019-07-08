// 
// Created 8/28/2015 01:15:50
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using StatsMonitor.Core;
using StatsMonitor.Util;
using UnityEditor;
using UnityEngine;


namespace StatsMonitor
{
	[CustomEditor(typeof(StatsMonitor))]
	public class StatsMonitorEditor : Editor
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		private StatsMonitor _self;

		private SerializedProperty _mode;
		private SerializedProperty _style;
		private SerializedProperty _alignment;
		private SerializedProperty _keepAlive;
		private SerializedProperty _statsUpdateInterval;
		private SerializedProperty _graphUpdateInterval;
		private SerializedProperty _objectsCountInterval;

		private SerializedProperty _inputEnabled;
		private SerializedProperty _hotKeyGroupToggle;
		private SerializedProperty _modKeyToggle;
		private SerializedProperty _hotKeyToggle;

		private SerializedProperty _touchControlGroupToggle;
		private SerializedProperty _toggleTouchCount;
		private SerializedProperty _switchAlignmentTapCount;
		private SerializedProperty _switchStyleTapCount;

		private SerializedProperty _modKeyAlignment;
		private SerializedProperty _hotKeyAlignment;
		private SerializedProperty _modKeyStyle;
		private SerializedProperty _hotKeyStyle;

		private SerializedProperty _layoutAndStylingToggle;
		private SerializedProperty _fontFace;
		private SerializedProperty _fontSizeLarge;
		private SerializedProperty _fontSizeSmall;
		private SerializedProperty _padding;
		private SerializedProperty _spacing;
		private SerializedProperty _graphHeight;
		private SerializedProperty _autoScale;
		private SerializedProperty _scale;
		private SerializedProperty _colorBGUpper;
		private SerializedProperty _colorBGLower;
		private SerializedProperty _colorGraphBG;
		private SerializedProperty _colorFPS;
		private SerializedProperty _colorFPSWarning;
		private SerializedProperty _colorFPSCritical;
		private SerializedProperty _colorFPSMin;
		private SerializedProperty _colorFPSMax;
		private SerializedProperty _colorFPSAvg;
		private SerializedProperty _colorFXD;
		private SerializedProperty _colorMS;
		private SerializedProperty _colorGCBlip;
		private SerializedProperty _colorObjCount;
		private SerializedProperty _colorMemTotal;
		private SerializedProperty _colorMemAlloc;
		private SerializedProperty _colorMemMono;
		private SerializedProperty _colorSysInfoOdd;
		private SerializedProperty _colorSysInfoEven;
		private SerializedProperty _colorOutline;

		private SerializedProperty _fpsGroupToggle;
		private SerializedProperty _warningThreshold;
		private SerializedProperty _criticalThreshold;
		private SerializedProperty _throttleFrameRate;
		private SerializedProperty _throttledFrameRate;
		private SerializedProperty _avgSamples;


		// ----------------------------------------------------------------------------
		// Unity Editor Callbacks
		// ----------------------------------------------------------------------------

		public void OnEnable()
		{
			_self = (target as StatsMonitor);

			/* General parameters section */
			_mode = serializedObject.FindProperty("_mode");
			_style = serializedObject.FindProperty("_style");
			_alignment = serializedObject.FindProperty("_alignment");
			_keepAlive = serializedObject.FindProperty("_keepAlive");
			_statsUpdateInterval = serializedObject.FindProperty("_statsUpdateInterval");
			_graphUpdateInterval = serializedObject.FindProperty("_graphUpdateInterval");
			_objectsCountInterval = serializedObject.FindProperty("_objectsCountInterval");

			/* Hot keys section */
			_inputEnabled = serializedObject.FindProperty("inputEnabled");
			_hotKeyGroupToggle = serializedObject.FindProperty("hotKeyGroupToggle");
			_modKeyToggle = serializedObject.FindProperty("modKeyToggle");
			_hotKeyToggle = serializedObject.FindProperty("hotKeyToggle");
			_modKeyAlignment = serializedObject.FindProperty("modKeyAlignment");
			_hotKeyAlignment = serializedObject.FindProperty("hotKeyAlignment");
			_modKeyStyle = serializedObject.FindProperty("modKeyStyle");
			_hotKeyStyle = serializedObject.FindProperty("hotKeyStyle");

			/* Hot keys section */
			_touchControlGroupToggle = serializedObject.FindProperty("touchControlGroupToggle");
			_toggleTouchCount = serializedObject.FindProperty("toggleTouchCount");
			_switchAlignmentTapCount = serializedObject.FindProperty("switchAlignmentTapCount");
			_switchStyleTapCount = serializedObject.FindProperty("switchStyleTapCount");

			/* Look and feel section */
			_layoutAndStylingToggle = serializedObject.FindProperty("layoutAndStylingToggle");
			_fontFace = serializedObject.FindProperty("fontFace");
			_fontSizeLarge = serializedObject.FindProperty("fontSizeLarge");
			_fontSizeSmall = serializedObject.FindProperty("fontSizeSmall");
			_padding = serializedObject.FindProperty("padding");
			_spacing = serializedObject.FindProperty("spacing");
			_graphHeight = serializedObject.FindProperty("graphHeight");
			_autoScale = serializedObject.FindProperty("autoScale");
			_scale = serializedObject.FindProperty("scale");
			_colorBGUpper = serializedObject.FindProperty("colorBGUpper");
			_colorBGLower = serializedObject.FindProperty("colorBGLower");
			_colorGraphBG = serializedObject.FindProperty("colorGraphBG");
			_colorFPS = serializedObject.FindProperty("colorFPS");
			_colorFPSWarning = serializedObject.FindProperty("colorFPSWarning");
			_colorFPSCritical = serializedObject.FindProperty("colorFPSCritical");
			_colorFPSMin = serializedObject.FindProperty("colorFPSMin");
			_colorFPSMax = serializedObject.FindProperty("colorFPSMax");
			_colorFPSAvg = serializedObject.FindProperty("colorFPSAvg");
			_colorFXD = serializedObject.FindProperty("colorFXD");
			_colorMS = serializedObject.FindProperty("colorMS");
			_colorGCBlip = serializedObject.FindProperty("colorGCBlip");
			_colorObjCount = serializedObject.FindProperty("colorObjCount");
			_colorMemTotal = serializedObject.FindProperty("colorMemTotal");
			_colorMemAlloc = serializedObject.FindProperty("colorMemAlloc");
			_colorMemMono = serializedObject.FindProperty("colorMemMono");
			_colorSysInfoOdd = serializedObject.FindProperty("colorSysInfoOdd");
			_colorSysInfoEven = serializedObject.FindProperty("colorSysInfoEven");
			_colorOutline = serializedObject.FindProperty("colorOutline");

			/* FPS-Specific section */
			_fpsGroupToggle = serializedObject.FindProperty("fpsGroupToggle");
			_warningThreshold = serializedObject.FindProperty("_warningThreshold");
			_criticalThreshold = serializedObject.FindProperty("_criticalThreshold");
			_throttleFrameRate = serializedObject.FindProperty("_throttleFrameRate");
			_throttledFrameRate = serializedObject.FindProperty("_throttledFrameRate");
			_avgSamples = serializedObject.FindProperty("_avgSamples");
		}


		public override void OnInspectorGUI()
		{
			if (_self == null) return;

			serializedObject.Update();
			int indent = EditorGUI.indentLevel;

			EditorGUIUtility.labelWidth = 160;
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("*** " + StatsMonitor.NAME + " v" + StatsMonitor.VERSION + " by Hexagon Star Softworks ***", MessageType.None);
			EditorGUILayout.HelpBox("HOW TO CHANGE AND STORE PREFERENCES:" +
				"\n1. Enter play mode" +
				"\n2. Adjust settings in this component pane" +
				"\n3. Right-click on script name, choose 'Copy Component'" +
				"\n4. Exit play mode" +
				"\n5. Right-click on script name, choose 'Paste Component Values'", MessageType.Info);

			/* General parameters section */
			if (EditorUtil.PropertyChanged(_mode, new GUIContent("Mode",
				"Active: (Default) The stats monitor is drawn and operates in the foreground.\n\nInactive: Disables the stats monitor completely except for hotkeys checking.\n\nPassive: Doesn't draw the stats monitor but still polls stats in the background. Useful for hidden performance monitoring.")))
				_self.Mode = (Mode)_mode.enumValueIndex;
			if (EditorUtil.PropertyChanged(_style, new GUIContent("Style",
				"Minimal: Displays only the FPS counter.\n\nStatsOnly: Displays only the textual stats section.\n\nStandard: (Default) Displays the textual stats section and the graph section.\n\nFull: Displays the textual stats section, the graph section, and the sysinfo section.")))
				_self.Style = (Style)_style.enumValueIndex;
			if (EditorUtil.PropertyChanged(_alignment, new GUIContent("Alignment",
				"Determines the position of the stats monitor.")))
				_self.Alignment = (Alignment)_alignment.enumValueIndex;
			if (EditorUtil.PropertyChanged(_keepAlive, new GUIContent("Keep Alive", "If checked prevents the stats monitor object from being destroyed on level (scene) load.")))
				_self.KeepAlive = _keepAlive.boolValue;
			if (EditorUtil.PropertyChanged(_statsUpdateInterval, new GUIContent("Stats Update Interval", "The time, in seconds at which the text displays are updated.")))
				_self.StatsUpdateInterval = _statsUpdateInterval.floatValue;
			if (EditorUtil.PropertyChanged(_graphUpdateInterval, new GUIContent("Graph Update Interval", "The time, in seconds at which the graph is updated.")))
				_self.GraphUpdateInterval = _graphUpdateInterval.floatValue;
			if (EditorUtil.PropertyChanged(_objectsCountInterval, new GUIContent("Object Count Interval", "The time, in seconds at which objects are counted.")))
				_self.ObjectsCountInterval = _objectsCountInterval.floatValue;

			EditorGUILayout.Space();

			if (EditorUtil.PropertyChanged(_inputEnabled, new GUIContent("Input Enabled", "You can choose to disable Stats Monitor's own hotkey- and touch input polling and instead control the API externally, for example with another input manager.")))
				_self.inputEnabled = _inputEnabled.boolValue;

			/* Hot keys section */
			if (EditorUtil.Foldout(_hotKeyGroupToggle, "Hot Keys"))
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField(_modKeyToggle, new GUIContent("Toggle Modifier Key", "Optional modifier key used in combination with the toggle hot key. Set this to None if you want to use the hot key without modifier key."));
				EditorGUILayout.PropertyField(_hotKeyToggle, new GUIContent("Toggle Hot Key", "The hot key used for toggling the stats monitor visibility."));
				EditorGUILayout.PropertyField(_modKeyAlignment, new GUIContent("Alignment Modifier Key", "Optional modifier key used in combination with the alignment hot key. Set this to None if you want to use the hot key without modifier key."));
				EditorGUILayout.PropertyField(_hotKeyAlignment, new GUIContent("Alignment Hot Key", "The hot key used for switching between the alignment modes (see Alignment parameter)."));
				EditorGUILayout.PropertyField(_modKeyStyle, new GUIContent("Style Modifier Key", "Optional modifier key used in combination with the style hot key. Set this to None if you want to use the hot key without modifier key."));
				EditorGUILayout.PropertyField(_hotKeyStyle, new GUIContent("Style Hot Key", "The hot key used for switching between the different styles (see Style parameter)."));
			}

			EditorGUI.indentLevel = indent;
			EditorGUILayout.Space();

			/* Touch control section */
			if (EditorUtil.Foldout(_touchControlGroupToggle, "Touch Control"))
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField(_toggleTouchCount, new GUIContent("Toggle Touch Count", "How many fingers have to touch the display to toggle between active/inactive mode. Set to 0 to disable."));
				EditorGUILayout.PropertyField(_switchAlignmentTapCount, new GUIContent("Switch Align Tap Count", "How many taps are required to switch between alignment positions. Set to 0 to disable."));
				EditorGUILayout.PropertyField(_switchStyleTapCount, new GUIContent("Switch Style Tap Count", "How many taps are required to switch between the different layout styles. Set to 0 to disable."));
			}

			EditorGUI.indentLevel = indent;
			EditorGUILayout.Space();

			/* Look and feel section */
			if (EditorUtil.Foldout(_layoutAndStylingToggle, "Layout & Styling"))
			{
				EditorGUI.indentLevel = 1;
				if (EditorUtil.PropertyChanged(_fontFace, new GUIContent("Font", "The font used for all text fields. Leave blank to use the included font.")))
					_self.FontFace = (Font)_fontFace.objectReferenceValue;
				if (EditorUtil.PropertyChanged(_fontSizeLarge, new GUIContent("Large font size", "Font size for the FPS counter.")))
					_self.FontSizeLarge = _fontSizeLarge.intValue;
				if (EditorUtil.PropertyChanged(_fontSizeSmall, new GUIContent("Small font size", "Font size for all small text.")))
					_self.FontSizeSmall = _fontSizeSmall.intValue;
				if (EditorUtil.PropertyChanged(_padding, new GUIContent("Padding", "The padding between the outer edges and text fields.")))
					_self.Padding = _padding.intValue;
				if (EditorUtil.PropertyChanged(_spacing, new GUIContent("Spacing", "The spacing between text fields.")))
					_self.Spacing = _spacing.intValue;
				if (EditorUtil.PropertyChanged(_graphHeight, new GUIContent("Graph height", "Determines the height of the graph.")))
					_self.GraphHeight = _graphHeight.intValue;
				if (EditorUtil.PropertyChanged(_autoScale,
					new GUIContent("Autoscale", "If turned on, scale will be determined according to current screen DPI. Recommended to leave on for testing on various supported devices that have large differences in screen resolution.")))
					_self.AutoScale = _autoScale.boolValue;
				GUI.enabled = !_autoScale.boolValue;
				if (EditorUtil.PropertyChanged(_scale, new GUIContent("Scale", "Allows to scale up the size of the stats monitor view by a multiplier.")))
					_self.Scale = _scale.intValue;
				GUI.enabled = true;
				if (EditorUtil.PropertyChanged(_colorBGUpper, new GUIContent("BG Color Top", "The color and transparency of the background panel top.")))
					_self.ColorBgUpper = _colorBGUpper.colorValue;
				if (EditorUtil.PropertyChanged(_colorBGLower, new GUIContent("BG Color Bottom", "The color and transparency of the background panel bottom.")))
					_self.ColorBgLower = _colorBGLower.colorValue;
				if (EditorUtil.PropertyChanged(_colorGraphBG, new GUIContent("Graph Background Color", "The background color and transparency of the graph area.")))
					_self.ColorGraphBG = _colorGraphBG.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPS, new GUIContent("FPS Color", "Color used for the FPS value.")))
					_self.ColorFPS = _colorFPS.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPSWarning, new GUIContent("FPS Warning Color", "The FPS counter will use this color if FPS falls below the FPS warning threshold.")))
					_self.ColorFPSWarning = _colorFPSWarning.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPSCritical, new GUIContent("FPS Critical Color", "The FPS counter will use this color if FPS falls below the FPS critical threshold.")))
					_self.ColorFPSCritical = _colorFPSCritical.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPSMin, new GUIContent("FPS Min Color", "Color used for the FPS minimum value.")))
					_self.ColorFPSMin = _colorFPSMin.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPSMax, new GUIContent("FPS Max Color", "Color used for the FPS maximum value.")))
					_self.ColorFPSMax = _colorFPSMax.colorValue;
				if (EditorUtil.PropertyChanged(_colorFPSAvg, new GUIContent("FPS Average Color", "Color used for FPS average value.")))
					_self.ColorFPSAvg = _colorFPSAvg.colorValue;
				if (EditorUtil.PropertyChanged(_colorMS, new GUIContent("Milliseconds Color", "Color used for the milliseconds value.")))
					_self.ColorMS = _colorMS.colorValue;
				if (EditorUtil.PropertyChanged(_colorGCBlip, new GUIContent("GC Blip Color", "Color used for garbage collection graph blip.")))
					_self.ColorGCBlip = _colorGCBlip.colorValue;
				if (EditorUtil.PropertyChanged(_colorFXD, new GUIContent("Fixed Update Color", "Color used for Fixed Update 'frame'-rate.")))
					_self.ColorFxd = _colorFXD.colorValue;
				if (EditorUtil.PropertyChanged(_colorObjCount, new GUIContent("Objects Count Color", "Color used for the 'currently rendered objects' / 'total render objects' / 'total game objects' values.")))
					_self.ColorObjectCount = _colorObjCount.colorValue;
				if (EditorUtil.PropertyChanged(_colorMemTotal, new GUIContent("Total Memory Color", "Color used for the total memory value.")))
					_self.ColorMemTotal = _colorMemTotal.colorValue;
				if (EditorUtil.PropertyChanged(_colorMemAlloc, new GUIContent("Allocated Memory Color", "Color used for the allocated memory value.")))
					_self.ColorMemAlloc = _colorMemAlloc.colorValue;
				if (EditorUtil.PropertyChanged(_colorMemMono, new GUIContent("Mono Memory Color", "Color used for the mono memory value.")))
					_self.ColorMemMono = _colorMemMono.colorValue;
				if (EditorUtil.PropertyChanged(_colorSysInfoOdd, new GUIContent("System Info Odd Color", "Color used for odd sysinfo rows.")))
					_self.ColorSysInfoOdd = _colorSysInfoOdd.colorValue;
				if (EditorUtil.PropertyChanged(_colorSysInfoEven, new GUIContent("System Info Even Color", "Color used for even sysinfo rows.")))
					_self.ColorSysInfoEven = _colorSysInfoEven.colorValue;
				if (EditorUtil.PropertyChanged(_colorOutline, new GUIContent("Outline Color", "Color used for text and graph outline. Setting alpha to 0 will remove the effect components.")))
					_self.ColorOutline = _colorOutline.colorValue;
			}

			EditorGUI.indentLevel = indent;
			EditorGUILayout.Space();

			/* FPS-Specific section */
			if (EditorUtil.Foldout(_fpsGroupToggle, "FPS Specific"))
			{
				EditorGUI.indentLevel = 1;
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
				if (EditorUtil.PropertyChanged(_throttleFrameRate, new GUIContent("Throttle FPS", "Enables or disables FPS throttling. This allows to see how the game performs under a specific frame rate. Does not guarantee selected frame rate. Set this to -1 to render as fast as possible in current conditions.\n\nIMPORTANT: this option disables VSync while enabled!")))
					_self.ThrottleFrameRate = _throttleFrameRate.boolValue;
				if (EditorUtil.PropertyChanged(_throttledFrameRate, new GUIContent()))
					_self.ThrottledFrameRate = _throttledFrameRate.intValue;
				EditorGUILayout.EndHorizontal();
				if (EditorUtil.PropertyChanged(_avgSamples, new GUIContent("Average FPS Samples", "The amount of samples collected to calculate the average FPS value from. Setting this to 0 will result in an average FPS value calculated from all samples since startup or level load.")))
					_self.AverageSamples = _avgSamples.intValue;
				if (EditorUtil.PropertyChanged(_warningThreshold, new GUIContent("FPS Warning Threshold", "The threshold below which the FPS will be marked with warning color.")))
					_self.WarningThreshold = _warningThreshold.intValue;
				if (EditorUtil.PropertyChanged(_criticalThreshold, new GUIContent("FPS Critical Threshold", "The threshold below which the FPS will be marked with critical color.")))
					_self.CriticalThreshold = _criticalThreshold.intValue;
			}

			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}
	}
}
