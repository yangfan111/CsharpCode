// 
// Created 8/23/2015 21:29:27
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using System;
using System.Collections;
using System.Collections.Generic;
using StatsMonitor.Core;
using StatsMonitor.Util;
using StatsMonitor.View;
using UnityEngine;
using UnityEngine.UI;


namespace StatsMonitor
{
	// ----------------------------------------------------------------------------
	// Enums
	// ----------------------------------------------------------------------------

	public enum Mode : byte
	{
		Active,
		Inactive,
		Passive
	}


	public enum Style : byte
	{
		Minimal,
		StatsOnly,
		Standard,
		Full
	}


	internal enum ViewType : byte
	{
		Background,
		FPSView,
		StatsView,
		GraphView,
        CustomInfo,
		SysInfoView
	}

	internal enum InvalidationFlag : byte
	{
		Any,
		Text,
		Graph,
		Background,
		Scale
	}

	/// <summary>
	///		Provides the main functionailty for the stats monitor.
	/// </summary>
	[DisallowMultipleComponent]
	public class StatsMonitor : MonoBehaviour
	{
		// ----------------------------------------------------------------------------
		// Constants
		// ----------------------------------------------------------------------------

		public const string NAME = "Stats Monitor";
		public const string VERSION = "1.3.0";
		private const float MEMORY_DIVIDER = 1048576.0f; // 1024^2
		private const int MINMAX_SKIP_INTERVALS = 3;


		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		/* General parameters */
		[SerializeField] private Mode _mode = Mode.Active;
		[SerializeField] private Style _style = Style.Standard;
		[SerializeField] private Alignment _alignment = Alignment.UpperRight;
		[SerializeField] private bool _keepAlive = true;
		[SerializeField] [Range(0.1f, 10.0f)] private float _statsUpdateInterval = 0.5f;
		[SerializeField] [Range(0.01f, 10.0f)] private float _graphUpdateInterval = 0.05f;
		[SerializeField] [Range(0.01f, 100000000.0f)] private float _objectsCountInterval = 100000000.0f;

		/* Hot keys */
		public bool inputEnabled = true;
		public KeyCode modKeyToggle = KeyCode.LeftShift;
		public KeyCode hotKeyToggle = KeyCode.BackQuote;
		public KeyCode modKeyAlignment = KeyCode.LeftControl;
		public KeyCode hotKeyAlignment = KeyCode.BackQuote;
		public KeyCode modKeyStyle = KeyCode.LeftAlt;
		public KeyCode hotKeyStyle = KeyCode.BackQuote;

		/* Touch Control */
		[Range(0, 5)] public int toggleTouchCount = 3;
		[Range(0, 5)] public int switchAlignmentTapCount = 3;
		[Range(0, 5)] public int switchStyleTapCount = 3;

        /* Look and feel */
        [SerializeField] [Range(1, 4)] public int space = 2;
        [SerializeField] internal Font fontFace;
		[SerializeField] [Range(8, 128)] internal int fontSizeLarge = 32;
		[SerializeField] [Range(8, 128)] internal int fontSizeSmall = 16;
		[SerializeField] [Range(0, 100)] internal int padding = 4;
		[SerializeField] [Range(0, 100)] internal int spacing = 2;
		[SerializeField] [Range(10, 400)] internal int graphHeight = 40;
		[SerializeField] [Range(1, 10)] internal int scale = 1;
		[SerializeField] internal bool autoScale = true;
        
		[SerializeField] internal Color colorBGUpper = Util.Utils.HexToColor32("00314ABE");
		[SerializeField] internal Color colorBGLower = Util.Utils.HexToColor32("002525C8");
		[SerializeField] internal Color colorGraphBG = Util.Utils.HexToColor32("00800010");
		[SerializeField] internal Color colorFPS = Color.white;
		[SerializeField] internal Color colorFPSWarning = Util.Utils.HexToColor32("FFA000FF");
		[SerializeField] internal Color colorFPSCritical = Util.Utils.HexToColor32("FF0000FF");
		[SerializeField] internal Color colorFPSMin = Util.Utils.HexToColor32("999999FF");
		[SerializeField] internal Color colorFPSMax = Util.Utils.HexToColor32("CCCCCCFF");
		[SerializeField] internal Color colorFPSAvg = Util.Utils.HexToColor32("00C8DCFF");
		[SerializeField] internal Color colorFXD = Util.Utils.HexToColor32("C68D00FF");
		[SerializeField] internal Color colorMS = Util.Utils.HexToColor32("C8C820FF");
		[SerializeField] internal Color colorObjCount = Util.Utils.HexToColor32("00B270FF");
		[SerializeField] internal Color colorGCBlip = Util.Utils.HexToColor32("870000FF");
		[SerializeField] internal Color colorMemTotal = Util.Utils.HexToColor32("4080FFFF");
		[SerializeField] internal Color colorMemAlloc = Util.Utils.HexToColor32("B480FFFF");
		[SerializeField] internal Color colorMemMono = Util.Utils.HexToColor32("FF66D1FF");
		[SerializeField] internal Color colorSysInfoOdd = Util.Utils.HexToColor32("D2EBFFFF");
		[SerializeField] internal Color colorSysInfoEven = Util.Utils.HexToColor32("A5D6FFFF");
		[SerializeField] internal Color colorOutline = Util.Utils.HexToColor32("00000000");

		/* FPS-specific */
		[SerializeField] private bool _throttleFrameRate;
		[SerializeField] [Range(-1, 200)] private int _throttledFrameRate = -1;
		[SerializeField] [Range(0, 100)] private int _avgSamples = 50;
		[SerializeField] [Range(1, 200)] private int _warningThreshold = 40;
		[SerializeField] [Range(1, 200)] private int _criticalThreshold = 20;

		/* Calculated values */
		public int fps { get; private set; }
		public int fpsMin { get; private set; }
		public int fpsMax { get; private set; }
		public int fpsAvg { get; private set; }
		public float ms { get; private set; }
		public float fixedUpdateRate { get; private set; }
		public float memAlloc { get; private set; }
		public float memTotal { get; private set; }
		public float memMono { get; private set; }
		public int objectCount { get; private set; }
		public int renderObjectCount { get; private set; }
		public int renderedObjectCount { get; private set; }

		internal int fpsLevel;

		private float _fpsNew;
		private int _minMaxIntervalsSkipped;
		private int _currentAVGSamples;
		private float _currentAVGRaw;
		private float[] _accAVGSamples;
		private int _cachedVSync = -1;
		private int _cachedFrameRate = -1;
		private float _actualUpdateInterval;
		private float _intervalTimeCount;
		private float _intervalTimeCount2;

		private int _totalWidth;
		private int _totalHeight;

		private bool _isInitialized;

		private Anchor _anchor;
		private RawImage _background;
		private Texture2D _gradient;
		private FPSView _fpsView;
		private StatsView _statsView;
		private GraphView _graphView;
        private CustomInfoView _customView;
        private SysInfoView _sysInfoView;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private Rect _touchArea;
		private Vector2 _touchOrigin = -Vector2.one;
#endif

#if UNITY_EDITOR
		[HideInInspector] [SerializeField] public bool hotKeyGroupToggle = false;
		[HideInInspector] [SerializeField] public bool touchControlGroupToggle = false;
		[HideInInspector] [SerializeField] public bool layoutAndStylingToggle = true;
		[HideInInspector] [SerializeField] public bool fpsGroupToggle = true;
#endif


        public List<IProfiler> profilerList = new List<IProfiler>();


		// ----------------------------------------------------------------------------
		// Accessors
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Determines the mode that Stats Monitor is running in.
		/// 
		///		Active: (Default) The stats monitor is drawn and operates in the
		///		foreground.
		///		Inactive: Disables the stats monitor completely except for hotkeys
		///		checking.
		///		Passive: Doesn't draw the stats monitor but still polls stats in the
		///		background. Useful for hidden performance monitoring.
		/// </summary>
		public Mode Mode
		{
			get { return _mode; }
			set
			{
				if (_mode == value || !Application.isPlaying) return;
				_mode = value;
				if (!enabled) return;
				if (_mode != Mode.Inactive)
				{
					OnEnable();
					UpdateData();
					UpdateView();
				}
				else
				{
					OnDisable();
				}
			}
		}

		/// <summary>
		///		The layout style of Stats Monitor.
		/// 
		///		Minimal: Displays only the FPS counter.
		///		StatsOnly: Displays only the textual stats section.
		///		Standard: (Default) Displays the textual stats section and the graph
		///		section.
		///		Full: Displays the textual stats section, the graph section, and the
		///		sysinfo section.
		/// </summary>
		public Style Style
		{
			get { return _style; }
			set
			{
				if (_style == value || !Application.isPlaying) return;
				_style = value;
				if (!enabled) return;
				CreateChildren();
				UpdateData();
				UpdateView();
			}
		}

		/// <summary>
		///		Determines the position of Stats Monitor.
		/// </summary>
		public Alignment Alignment
		{
			get { return _alignment; }
			set
			{
				if (_alignment == value || !Application.isPlaying) return;
				_alignment = value;
				if (!enabled) return;
				Align(_alignment);
			}
		}

		/// <summary>
		///		If checked prevents Stats Monitor from being destroyed on level (scene)
		///		load.
		/// </summary>
		public bool KeepAlive
		{
			get { return _keepAlive; }
			set { _keepAlive = value; }
		}

		/// <summary>
		///		The time, in seconds at which the text displays are updated.
		/// </summary>
		public float StatsUpdateInterval
		{
			get { return _statsUpdateInterval; }
			set
			{
				if (Mathf.Abs(_statsUpdateInterval - value) < 0.001f || !Application.isPlaying) return;
				_statsUpdateInterval = value;
				if (!enabled) return;
				DetermineActualUpdateInterval();
				RestartCoroutine();
			}
		}

		/// <summary>
		///		The time, in seconds at which the graph is updated.
		/// </summary>
		public float GraphUpdateInterval
		{
			get { return _graphUpdateInterval; }
			set
			{
				if (Mathf.Abs(_graphUpdateInterval - value) < 0.001f || !Application.isPlaying) return;
				_graphUpdateInterval = value;
				if (!enabled) return;
				DetermineActualUpdateInterval();
				RestartCoroutine();
			}
		}

		/// <summary>
		///		The time, in seconds at which objects are counted.
		/// </summary>
		public float ObjectsCountInterval
		{
			get { return _objectsCountInterval; }
			set
			{
				if (Mathf.Abs(_objectsCountInterval - value) < 0.001f || !Application.isPlaying) return;
				_objectsCountInterval = value;
				if (!enabled) return;
				DetermineActualUpdateInterval();
				RestartCoroutine();
			}
		}

		/// <summary>
		///		The font used for all text fields. Set to null to use the included font.
		/// </summary>
		public Font FontFace
		{
			get { return fontFace; }
			set
			{
				if (fontFace == value || !Application.isPlaying) return;
				fontFace = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Text);
			}
		}

		/// <summary>
		///		Font size for all small text.
		/// </summary>
		public int FontSizeSmall
		{
			get { return fontSizeSmall; }
			set
			{
				if (fontSizeSmall == value || !Application.isPlaying) return;
				fontSizeSmall = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Text);
			}
		}

		/// <summary>
		///		Font size for the FPS counter.
		/// </summary>
		public int FontSizeLarge
		{
			get { return fontSizeLarge; }
			set
			{
				if (fontSizeLarge == value || !Application.isPlaying) return;
				fontSizeLarge = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Text);
			}
		}

		/// <summary>
		///		The padding between the outer edges and text fields.
		/// </summary>
		public int Padding
		{
			get { return padding; }
			set
			{
				if (padding == value || !Application.isPlaying) return;
				padding = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Text);
			}
		}

		/// <summary>
		///		The spacing between text fields.
		/// </summary>
		public int Spacing
		{
			get { return spacing; }
			set
			{
				if (spacing == value || !Application.isPlaying) return;
				spacing = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Text);
			}
		}

		/// <summary>
		///		Determines the height of the graph.
		/// </summary>
		public int GraphHeight
		{
			get { return graphHeight; }
			set
			{
				if (graphHeight == value || !Application.isPlaying) return;
				graphHeight = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Graph);
			}
		}

		/// <summary>
		///		The scaling multiplier.
		/// </summary>
		public int Scale
		{
			get { return scale; }
			set
			{
				if (scale == value || !Application.isPlaying) return;
				scale = value;
				if (!enabled || _mode != Mode.Active) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Scale);
			}
		}

		/// <summary>
		///		If true, scale will be determined according to current screen DPI.
		///		Recommended to leave on for testing on various supported devices that
		///		have large differences in screen resolution.
		/// </summary>
		public bool AutoScale
		{
			get { return autoScale; }
			set
			{
				if (autoScale == value || !Application.isPlaying) return;
				autoScale = value;
				if (!enabled || _mode != Mode.Active) return;
				Invalidate(ViewInvalidationType.All, InvalidationFlag.Scale);
			}
		}

		/// <summary>
		///		The color and transparency of the background panel top.
		/// </summary>
		public Color ColorBgUpper
		{
			get { return colorBGUpper; }
			set
			{
				if (colorBGUpper == value || !Application.isPlaying) return;
				colorBGUpper = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style, InvalidationFlag.Background);
			}
		}

		/// <summary>
		///		The color and transparency of the background panel bottom.
		/// </summary>
		public Color ColorBgLower
		{
			get { return colorBGLower; }
			set
			{
				if (colorBGLower == value || !Application.isPlaying) return;
				colorBGLower = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style, InvalidationFlag.Background);
			}
		}

		/// <summary>
		///		The background color and transparency of the graph area.
		/// </summary>
		public Color ColorGraphBG
		{
			get { return colorGraphBG; }
			set
			{
				if (colorGraphBG == value || !Application.isPlaying) return;
				colorGraphBG = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the FPS value.
		/// </summary>
		public Color ColorFPS
		{
			get { return colorFPS; }
			set
			{
				if (colorFPS == value || !Application.isPlaying) return;
				colorFPS = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		The FPS counter will use this color if FPS falls below the FPS warning
		///		threshold.
		/// </summary>
		public Color ColorFPSWarning
		{
			get { return colorFPSWarning; }
			set
			{
				if (colorFPSWarning == value || !Application.isPlaying) return;
				colorFPSWarning = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		The FPS counter will use this color if FPS falls below the FPS critical
		///		threshold.
		/// </summary>
		public Color ColorFPSCritical
		{
			get { return colorFPSCritical; }
			set
			{
				if (colorFPSCritical == value || !Application.isPlaying) return;
				colorFPSCritical = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the FPS minimum value.
		/// </summary>
		public Color ColorFPSMin
		{
			get { return colorFPSMin; }
			set
			{
				if (colorFPSMin == value || !Application.isPlaying) return;
				colorFPSMin = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the FPS maximum value.
		/// </summary>
		public Color ColorFPSMax
		{
			get { return colorFPSMax; }
			set
			{
				if (colorFPSMax == value || !Application.isPlaying) return;
				colorFPSMax = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the FPS average value.
		/// </summary>
		public Color ColorFPSAvg
		{
			get { return colorFPSAvg; }
			set
			{
				if (colorFPSAvg == value || !Application.isPlaying) return;
				colorFPSAvg = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for Fixed Update framerate value.
		/// </summary>
		public Color ColorFxd
		{
			get { return colorFXD; }
			set
			{
				if (colorFXD == value || !Application.isPlaying) return;
				colorFXD = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the milliseconds value.
		/// </summary>
		public Color ColorMS
		{
			get { return colorMS; }
			set
			{
				if (colorMS == value || !Application.isPlaying) return;
				colorMS = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the garbage collection graph blip.
		/// </summary>
		public Color ColorGCBlip
		{
			get { return colorGCBlip; }
			set
			{
				if (colorGCBlip == value || !Application.isPlaying) return;
				colorGCBlip = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for visible renderers and total objects values.
		/// </summary>
		public Color ColorObjectCount
		{
			get { return colorObjCount; }
			set
			{
				if (colorObjCount == value || !Application.isPlaying) return;
				colorObjCount = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the total memory value.
		/// </summary>
		public Color ColorMemTotal
		{
			get { return colorMemTotal; }
			set
			{
				if (colorMemTotal == value || !Application.isPlaying) return;
				colorMemTotal = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the allocated memory value.
		/// </summary>
		public Color ColorMemAlloc
		{
			get { return colorMemAlloc; }
			set
			{
				if (colorMemAlloc == value || !Application.isPlaying) return;
				colorMemAlloc = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for the Mono memory value.
		/// </summary>
		public Color ColorMemMono
		{
			get { return colorMemMono; }
			set
			{
				if (colorMemMono == value || !Application.isPlaying) return;
				colorMemMono = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for odd sysinfo rows.
		/// </summary>
		public Color ColorSysInfoOdd
		{
			get { return colorSysInfoOdd; }
			set
			{
				if (colorSysInfoOdd == value || !Application.isPlaying) return;
				colorSysInfoOdd = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for even sysinfo rows.
		/// </summary>
		public Color ColorSysInfoEven
		{
			get { return colorSysInfoEven; }
			set
			{
				if (colorSysInfoEven == value || !Application.isPlaying) return;
				colorSysInfoEven = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.Style);
			}
		}

		/// <summary>
		///		Color used for text and graph outline. Setting alpha to 0 will remove
		///		the effect components.
		/// </summary>
		public Color ColorOutline
		{
			get { return colorOutline; }
			set
			{
				if (colorOutline == value || !Application.isPlaying) return;
				colorOutline = value;
				if (!enabled) return;
				Invalidate(ViewInvalidationType.All);
			}
		}

		/// <summary>
		///		Enables or disables FPS throttling. This can be used to see how the game
		///		performs under a specific framerate. Note that this option doesn't guarantee
		///		the selected framerate. Set this value to -1 to render as fast as the system
		///		allows under current conditions. IMPORTANT: this option disables VSync while
		///		enabled!
		/// </summary>
		public bool ThrottleFrameRate
		{
			get { return _throttleFrameRate; }
			set
			{
				if (_throttleFrameRate == value || !Application.isPlaying) return;
				_throttleFrameRate = value;
				if (!enabled || _mode == Mode.Inactive) return;
				RefreshThrottledFrameRate();
			}
		}

		/// <summary>
		///		The maximum framerate at which the game may run if Framerate Throttling
		///		is enabled. When setting this value to -1 the game will run at a framerate
		///		as fast as the system's performance allows.
		/// </summary>
		public int ThrottledFrameRate
		{
			get { return _throttledFrameRate; }
			set
			{
				if (_throttledFrameRate == value || !Application.isPlaying) return;
				_throttledFrameRate = value;
				if (!enabled || _mode == Mode.Inactive) return;
				RefreshThrottledFrameRate();
			}
		}

		/// <summary>
		///		The amount of samples collected to calculate the average FPS value from.
		///		Setting this to 0 will result in an average FPS value calculated from
		///		all samples since startup or level load.
		/// </summary>
		public int AverageSamples
		{
			get { return _avgSamples; }
			set
			{
				if (_avgSamples == value || !Application.isPlaying) return;
				_avgSamples = value;
				if (!enabled) return;
				if (_avgSamples > 0)
				{
					if (_accAVGSamples == null)
						_accAVGSamples = new float[_avgSamples];
					else if (_accAVGSamples.Length != _avgSamples)
						Array.Resize(ref _accAVGSamples, _avgSamples);
				}
				else
				{
					_accAVGSamples = null;
				}
				ResetAverageFPS();
				UpdateData();
				UpdateView();
			}
		}

		/// <summary>
		///		The threshold below which the FPS will be marked with warning color.
		/// </summary>
		public int WarningThreshold
		{
			get { return _warningThreshold; }
			set { _warningThreshold = value; }
		}

		/// <summary>
		///		The threshold below which the FPS will be marked with critical color.
		/// </summary>
		public int CriticalThreshold
		{
			get { return _criticalThreshold; }
			set { _criticalThreshold = value; }
		}


		// ----------------------------------------------------------------------------
		// Constructor
		// ----------------------------------------------------------------------------

		private StatsMonitor()
		{
			// Prevent direct instantiation!
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Toggles the mode of Stats Monitor between Active and Inactive.
		/// </summary>
		public void Toggle()
		{
			if (_mode == Mode.Inactive) Mode = Mode.Active;
			else if (_mode == Mode.Active) Mode = Mode.Inactive;
		}


		/// <summary>
		///		Switches to the next layout style.
		/// </summary>
		public void NextStyle()
		{
			if (_style == Style.Minimal) Style = Style.StatsOnly;
			else if (_style == Style.StatsOnly) Style = Style.Standard;
			else if (_style == Style.Standard) Style = Style.Full;
			else if (_style == Style.Full) Style = Style.Minimal;
		}


		/// <summary>
		///		Switches to the next alignment, in a clock-wise order.
		/// </summary>
		public void NextAlignment()
		{
			if (_alignment == Alignment.UpperLeft) Align(Alignment.UpperCenter);
			else if (_alignment == Alignment.UpperCenter) Align(Alignment.UpperRight);
			else if (_alignment == Alignment.UpperRight) Align(Alignment.LowerRight);
			else if (_alignment == Alignment.LowerRight) Align(Alignment.LowerCenter);
			else if (_alignment == Alignment.LowerCenter) Align(Alignment.LowerLeft);
			else if (_alignment == Alignment.LowerLeft) Align(Alignment.UpperLeft);
		}


		/// <summary>
		/// 	Allows to set the alignment of Stats Monitor to a specific value.
		/// </summary>
		/// <param name="alignment">The aligment value.</param>
		/// <param name="newScale">Allows for specifying a new scale value. This is
		/// used by the script internally to provide a correctly updated scale for
		/// touch areas.</param>
		public void Align(Alignment alignment, int newScale = -1)
		{
			_alignment = alignment;
			switch (alignment)
			{
				case Alignment.UpperLeft:
					_anchor = StatsMonitorWrapper.anchors.upperLeft; break;
				case Alignment.UpperCenter:
					_anchor = StatsMonitorWrapper.anchors.upperCenter; break;
				case Alignment.UpperRight:
					_anchor = StatsMonitorWrapper.anchors.upperRight; break;
				case Alignment.LowerRight:
					_anchor = StatsMonitorWrapper.anchors.lowerRight; break;
				case Alignment.LowerCenter:
					_anchor = StatsMonitorWrapper.anchors.lowerCenter; break;
				case Alignment.LowerLeft:
					_anchor = StatsMonitorWrapper.anchors.lowerLeft; break;
				default:
					Debug.LogWarning("Align() Invalid value: " + alignment);
					break;
			}

			RectTransform t = gameObject.GetComponent<RectTransform>();
			t.anchoredPosition = _anchor.position;
			t.anchorMin = _anchor.min;
			t.anchorMax = _anchor.max;
			t.pivot = _anchor.pivot;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			StartCoroutine("UpdateTouchAreaLater", newScale);
#endif
		}


		/// <summary>
		///		Resets the FPS minimum and maximum values.
		/// </summary>
		public void ResetMinMaxFPS()
		{
			fpsMin = -1;
			fpsMax = -1;
			_minMaxIntervalsSkipped = 0;
			if (!Application.isPlaying) return;
			UpdateData(true);
		}


		/// <summary>
		///		Resets the average FPS value.
		/// </summary>
		public void ResetAverageFPS()
		{
			if (!Application.isPlaying) return;
			fpsAvg = 0;
			_currentAVGSamples = 0;
			_currentAVGRaw = 0;
			if (_avgSamples > 0 && _accAVGSamples != null)
			{
				Array.Clear(_accAVGSamples, 0, _accAVGSamples.Length);
			}
		}


		/// <summary>
		///		Completely disposes Stats Monitor.
		/// </summary>
		public void Dispose()
		{
			StopCoroutine("Interval");
			DisposeChildren();
			Destroy(this);
		}


		// ----------------------------------------------------------------------------
		// Internal Methods
		// ----------------------------------------------------------------------------

		internal void Invalidate(ViewInvalidationType type, InvalidationFlag flag = InvalidationFlag.Any, bool invalidateChildren = true)
		{
			UpdateFont();

			float totalWidth = 0.0f;
			float totalHeight = 0.0f;

			if (_fpsView != null)
			{
				if (invalidateChildren && (flag == InvalidationFlag.Any || flag == InvalidationFlag.Text))
				{
					_fpsView.Invalidate(type);
				}
				if (_fpsView.Width > totalWidth)
				{
					totalWidth = _fpsView.Width;
				}
				totalHeight += _fpsView.Height;
			}
			if (_statsView != null)
			{
				if (invalidateChildren && (flag == InvalidationFlag.Any || flag == InvalidationFlag.Text))
				{
					_statsView.Invalidate(type);
				}
				if (_statsView.Width > totalWidth)
				{
					totalWidth = _statsView.Width;
				}
				totalHeight += _statsView.Height;
			}
			if (_graphView != null)
			{
				_graphView.SetWidth(totalWidth);
				if (invalidateChildren && (flag == InvalidationFlag.Any || flag == InvalidationFlag.Graph || flag == InvalidationFlag.Text))
				{
					_graphView.Invalidate();
				}
				_graphView.Y = -totalHeight;
				if (_graphView.Width > totalWidth)
				{
					totalWidth = _graphView.Width;
				}
				totalHeight += _graphView.Height;
			}
            if (_customView != null)
            {
                if (invalidateChildren && (flag == InvalidationFlag.Any || flag == InvalidationFlag.Graph || flag == InvalidationFlag.Text))
                {
                    _customView.SetWidth(totalWidth);
                    _customView.Invalidate();
                    totalHeight += _customView.Height;
                }
            }
            if (_sysInfoView != null)
			{
				_sysInfoView.SetWidth(totalWidth);
				if (invalidateChildren && (flag == InvalidationFlag.Any || flag == InvalidationFlag.Text))
				{
					_sysInfoView.Invalidate(type);
				}
				_sysInfoView.Y = -totalHeight;
				if (_sysInfoView.Width > totalWidth)
				{
					totalWidth = _sysInfoView.Width;
				}
				totalHeight += _sysInfoView.Height;
			}

			if (_style != Style.Minimal && (type == ViewInvalidationType.All || (type == ViewInvalidationType.Style && flag == InvalidationFlag.Background)))
			{
				CreateBackground();
			}

			if (totalWidth > 0.0f || totalHeight > 0.0f)
			{
				_totalWidth = (int)totalWidth;
				_totalHeight = (int)totalHeight;
				gameObject.transform.localScale = Vector3.one;
                Util.Utils.RTransform(gameObject, Vector2.one, 0, 0, _totalWidth, _totalHeight);

				/* Re-apply current scale factor. */
				if (autoScale)
				{
					int scaleFactor = (int)Util.Utils.DPIScaleFactor(true);
					if (scaleFactor > -1) scale = scaleFactor;
					if (scale > 10) scale = 10; // Unlikely, but just to be safe.

					/* Prevent scaled size to be larger than fullscreen resolution! Mainly
					 * useful for small devices like iPhone to prevent the view extended
					 * outside the screen. */
					if (_totalWidth * scale > Screen.currentResolution.width) scale--;
				}

				gameObject.transform.localScale = new Vector3(scale, scale, 1.0f);
				Align(_alignment, scale);
			}
		}


		// ----------------------------------------------------------------------------
		// Protected & Private Methods
		// ----------------------------------------------------------------------------

		private void CreateChildren()
		{
			UpdateFont();

			/* Reset any applied scaling. */
			gameObject.transform.localScale = Vector3.one;

			switch (_style)
			{
				case Style.Minimal:
					DisposeChild(ViewType.Background);
					DisposeChild(ViewType.StatsView);
					DisposeChild(ViewType.GraphView);
					DisposeChild(ViewType.SysInfoView);
                    DisposeChild(ViewType.CustomInfo);
                    if (_fpsView == null) _fpsView = new FPSView(this);
					break;
				case Style.StatsOnly:
					CreateBackground();
					DisposeChild(ViewType.FPSView);
					DisposeChild(ViewType.GraphView);
					DisposeChild(ViewType.SysInfoView);
                    DisposeChild(ViewType.CustomInfo);
                    if (_statsView == null) _statsView = new StatsView(this);
					break;
				case Style.Standard:
					CreateBackground();
					DisposeChild(ViewType.FPSView);
					DisposeChild(ViewType.SysInfoView);
                    if (_statsView == null) _statsView = new StatsView(this);
					if (_graphView == null) _graphView = new GraphView(this);
                    if (_customView == null) _customView = new CustomInfoView(this);
                    break;
				case Style.Full:
					CreateBackground();
					DisposeChild(ViewType.FPSView);
					if (_statsView == null) _statsView = new StatsView(this);
					if (_graphView == null) _graphView = new GraphView(this);
					if (_sysInfoView == null) _sysInfoView = new SysInfoView(this);
                    if (_customView == null) _customView = new CustomInfoView(this);
                    break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			/* Add all child objects to UI layer. */
			foreach (Transform t1 in transform)
			{
                Util.Utils.AddToUILayer(t1.gameObject);
				foreach (Transform t2 in t1.transform)
				{
                    Util.Utils.AddToUILayer(t2.gameObject);
				}
			}

			Invalidate(ViewInvalidationType.All);
		}


		private void DisposeChildren()
		{
			DisposeChild(ViewType.Background);
			DisposeChild(ViewType.FPSView);
			DisposeChild(ViewType.StatsView);
			DisposeChild(ViewType.GraphView);
			DisposeChild(ViewType.SysInfoView);
            DisposeChild(ViewType.CustomInfo);
        }


		private void DisposeChild(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Background:
					if (_background != null)
					{
						Destroy(_gradient);
						Destroy(_background);
						_gradient = null;
						_background = null;
					}
					break;
				case ViewType.FPSView:
					if (_fpsView != null)
					{
						_fpsView.Dispose();
					}
					_fpsView = null;
					break;
				case ViewType.StatsView:
					if (_statsView != null)
					{
						_statsView.Dispose();
					}
					_statsView = null;
					break;
				case ViewType.GraphView:
					if (_graphView != null)
					{
						_graphView.Dispose();
					}
					_graphView = null;
					break;
				case ViewType.SysInfoView:
					if (_sysInfoView != null)
					{
						_sysInfoView.Dispose();
					}
					_sysInfoView = null;
					break;
                case ViewType.CustomInfo:
                    if (_customView != null)
                    {
                        _customView.Dispose();
                    }
                    _customView = null;
                    break;
                default:
					Debug.LogWarning("DisposeChild() Invalid value: " + viewType);
					break;
			}
		}


		private void UpdateData(bool forceUpdate = false, float timeElapsed = -1.0f)
		{
			/* Update FPS value. */
			int roundedFPS = (int) _fpsNew;
			ms = (1000.0f / _fpsNew);
			if (fps != roundedFPS || forceUpdate) fps = roundedFPS;

			if (fps <= _criticalThreshold) fpsLevel = 2;
			else if (fps <= _warningThreshold) fpsLevel = 1;
			else
			{
				if (fps > 999) fps = 999;
				fpsLevel = 0;
			}

			if (_style == Style.Minimal) return;

			/* Calculate FPS MIN/MAX FPS values. */
			if (_minMaxIntervalsSkipped < MINMAX_SKIP_INTERVALS)
			{
				if (!forceUpdate) _minMaxIntervalsSkipped++;
			}
			else
			{
				if (fpsMin == -1) fpsMin = fps;
				else if (fps < fpsMin) fpsMin = fps;
				if (fpsMax == -1) fpsMax = fps;
				else if (fps > fpsMax) fpsMax = fps;
			}

			/* Calculate Average FPS value. */
			if (_avgSamples == 0)
			{
				_currentAVGSamples++;
				_currentAVGRaw += (fps - _currentAVGRaw) / _currentAVGSamples;
			}
			else
			{
				_accAVGSamples[_currentAVGSamples % _avgSamples] = fps;
				_currentAVGSamples++;
				_currentAVGRaw = GetAccumulatedAVGSamples();
			}
			int rounded = Mathf.RoundToInt(_currentAVGRaw);
			if (fpsAvg != rounded || forceUpdate) fpsAvg = rounded;

			/* Calculate fixed update rate. */
			fixedUpdateRate = 1.0f / Time.fixedDeltaTime;

			/* Calculate MEM values. */
			memTotal = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / MEMORY_DIVIDER;
			memAlloc = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / MEMORY_DIVIDER;
			memMono = GC.GetTotalMemory(false) / MEMORY_DIVIDER;

			/* Calculate objects count/rendered objects count. */
			_intervalTimeCount2 += timeElapsed;
			if (_intervalTimeCount2 >= _objectsCountInterval || timeElapsed < 0.0f || forceUpdate)
			{
				GameObject[] allObjects = FindObjectsOfType<GameObject>();
				objectCount = allObjects.Length;
				renderObjectCount = renderedObjectCount = 0;
				foreach (GameObject obj in allObjects)
				{
					Renderer r = obj.GetComponent<Renderer>();
					if (r != null)
					{
						++renderObjectCount;
						if (r.isVisible) ++renderedObjectCount;
					}
				}
				_intervalTimeCount2 = 0.0f;
			}
		}


		private void UpdateView(float timeElapsed = -1.0f)
		{
			/* Add up interval time count to update text/graph views at their own intervals. */
			_intervalTimeCount += timeElapsed;

			/* Update textual views. */
			if (_intervalTimeCount >= _statsUpdateInterval || timeElapsed < 0.0f)
			{
				if (_fpsView != null) _fpsView.Update();
				if (_statsView != null) _statsView.Update();
				if (_sysInfoView != null) _sysInfoView.Update();
				if (_statsUpdateInterval > _graphUpdateInterval) _intervalTimeCount = 0.0f;
			}

			/* Update graph views. */
			if (_intervalTimeCount >= _graphUpdateInterval || timeElapsed < 0.0f)
			{
				if (_graphView != null) _graphView.Update();
                if (_customView != null) _customView.Update();
                if (_graphUpdateInterval >= _statsUpdateInterval) _intervalTimeCount = 0.0f;
			}
		}


		private float GetAccumulatedAVGSamples()
		{
			float totalFPS = 0;
			for (int i = 0; i < _avgSamples; i++) totalFPS += _accAVGSamples[i];
			return _currentAVGSamples < _avgSamples ? totalFPS / _currentAVGSamples : totalFPS / _avgSamples;
		}


		private void RefreshThrottledFrameRate()
		{
			// ReSharper disable once IntroduceOptionalParameters.Local
			RefreshThrottledFrameRate(false);
		}


		private void RefreshThrottledFrameRate(bool disable)
		{
			if (_throttleFrameRate && !disable)
			{
				if (_cachedVSync == -1)
				{
					_cachedVSync = QualitySettings.vSyncCount;
					_cachedFrameRate = Application.targetFrameRate;
					QualitySettings.vSyncCount = 0;
				}
				Application.targetFrameRate = _throttledFrameRate;
			}
			else
			{
				if (_cachedVSync != -1)
				{
					QualitySettings.vSyncCount = _cachedVSync;
					Application.targetFrameRate = _cachedFrameRate;
					_cachedVSync = -1;
				}
			}
		}


		private void DetermineActualUpdateInterval()
		{
			/* We need to run the interval at whichever update interval value is smaller. */
			_actualUpdateInterval = _graphUpdateInterval < _statsUpdateInterval ? _graphUpdateInterval : _statsUpdateInterval;
		}


		private void RestartCoroutine()
		{
			StopCoroutine("Interval");
			StartCoroutine("Interval");
		}


		private void UpdateFont()
		{
			/* Load default font used for stats text display. */
			if (fontFace != null) return;
			fontFace = (Font) Resources.Load("Fonts/terminalstats", typeof (Font));
			if (fontFace == null) fontFace = (Font) Resources.GetBuiltinResource(typeof (Font), "Arial.ttf");
		}


		private void CreateBackground()
		{
			if (Math.Abs(colorBGUpper.a) < 0.01f && Math.Abs(colorBGLower.a) < 0.01f)
			{
				DisposeChild(ViewType.Background);
			}
			else if (_background == null)
			{
				_gradient = new Texture2D(2, 2);
				_gradient.filterMode = FilterMode.Bilinear;
				_gradient.wrapMode = TextureWrapMode.Clamp;
				_background = gameObject.AddComponent<RawImage>();
				_background.color = Color.white;
				_background.texture = _gradient;
			}

			if (_background != null)
			{
				_gradient.SetPixel(0, 0, colorBGLower);
				_gradient.SetPixel(1, 0, colorBGLower);
				_gradient.SetPixel(0, 1, colorBGUpper);
				_gradient.SetPixel(1, 1, colorBGUpper);
				_gradient.Apply();
			}
		}


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private void UpdateTouchArea(int newScale = -1)
		{
			/* If new scale is given by AlignLater, take it over! */
			if (newScale > -1) scale = newScale;
			int scaledW = _totalWidth * scale;
			int scaledH = _totalHeight * scale;

			switch (_alignment)
			{
				case Alignment.UpperLeft:
					_touchArea = new Rect(0, Screen.height - scaledH, scaledW, scaledH); break;
				case Alignment.UpperCenter:
					_touchArea = new Rect((Screen.width * .5f) - (scaledW * .5f), Screen.height - scaledH, scaledW, scaledH); break;
				case Alignment.UpperRight:
					_touchArea = new Rect(Screen.width - scaledW, Screen.height - scaledH, scaledW, scaledH); break;
				case Alignment.LowerRight:
					_touchArea = new Rect(Screen.width - scaledW, 0, scaledW, scaledH); break;
				case Alignment.LowerCenter:
					_touchArea = new Rect((Screen.width * .5f) - (scaledW * .5f), 0, scaledW, scaledH); break;
				case Alignment.LowerLeft:
					_touchArea = new Rect(0, 0, scaledW, scaledH); break;
			}
		}
#endif


		// ----------------------------------------------------------------------------
		// Coroutines
		// ----------------------------------------------------------------------------

		private IEnumerator Interval()
		{
			while (true)
			{
				/* Calculate new FPS value. */
				float previousUpdateTime = Time.unscaledTime;
				int previousUpdateFrames = Time.frameCount;
				yield return new WaitForSeconds(_actualUpdateInterval);
				float timeElapsed = Time.unscaledTime - previousUpdateTime;
				int framesChanged = Time.frameCount - previousUpdateFrames;
				_fpsNew = (framesChanged / timeElapsed);
				UpdateData(false, timeElapsed);
				UpdateView(timeElapsed);
			}
			// ReSharper disable once FunctionNeverReturns
		}


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		private IEnumerator UpdateTouchAreaLater(int newScale)
		{
			/* Calls UpdateTouchArea() one frame later. We need this to have correct screen
			 * width/height in UpdateTouchArea() after calling Invalidate(). */
			yield return 0;
			UpdateTouchArea(newScale);
		}
#endif


        // ----------------------------------------------------------------------------
        // Unity Callbacks
        // ----------------------------------------------------------------------------


        private void Awake()
		{
			fpsMin = fpsMax = -1;
			_accAVGSamples = new float[_avgSamples];
			_isInitialized = true;

            

        }


		private void Update()
		{
			if (!_isInitialized || !inputEnabled) return;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
			if (Input.anyKeyDown)
			{
				/* Check for toggle key combinations. */
				if (((modKeyToggle != KeyCode.None && hotKeyToggle != KeyCode.None) && (Input.GetKey(modKeyToggle) && Input.GetKeyDown(hotKeyToggle))) || ((modKeyToggle == KeyCode.None && hotKeyToggle != KeyCode.None) && Input.GetKeyDown(hotKeyToggle)))
				{
					Toggle();
				}
				/* Aligment- and Style toggle should only work if statsmonitor is visible. */
				/* Check for _alignment key combinations. */
				else if (_mode == Mode.Active && (((modKeyAlignment != KeyCode.None && hotKeyAlignment != KeyCode.None) && (Input.GetKey(modKeyAlignment) && Input.GetKeyDown(hotKeyAlignment))) || ((modKeyAlignment == KeyCode.None && hotKeyAlignment != KeyCode.None) && Input.GetKeyDown(hotKeyAlignment))))
				{
					NextAlignment();
				}
				/* Check for _style key combinations. */
				else if (_mode == Mode.Active && (((modKeyStyle != KeyCode.None && hotKeyStyle != KeyCode.None) && (Input.GetKey(modKeyStyle) && Input.GetKeyDown(hotKeyStyle))) || ((modKeyStyle == KeyCode.None && hotKeyStyle != KeyCode.None) && Input.GetKeyDown(hotKeyStyle))))
				{
					NextStyle();
				}
			}
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			if (toggleTouchCount > 0 && Input.touchCount == toggleTouchCount)
			{
				Touch touch = Input.GetTouch(toggleTouchCount - 1);
				if (touch.phase == TouchPhase.Began)
				{
					_touchOrigin = touch.position;
				}
				else if (touch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
				{
					_touchOrigin = -Vector2.one;
					Toggle();
				}
			}
			else if (switchStyleTapCount > 0 && (_mode == Mode.Active && Input.touchCount == 2))
			{
				Touch touchA = Input.GetTouch(0);
				Touch touchB = Input.GetTouch(1);
				/* First finger must be inside area, second finger must be outside. */
				if (_touchArea.Contains(touchA.position) && !_touchArea.Contains(touchB.position))
				{
					if (touchB.phase == TouchPhase.Began && touchB.tapCount == switchStyleTapCount)
					{
						_touchOrigin = touchB.position;
					}
					else if (touchB.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
					{
						_touchOrigin = -Vector2.one;
						NextStyle();
					}
				}
			}
			else if (switchAlignmentTapCount > 0 && (_mode == Mode.Active && Input.touchCount == 1))
			{
				Touch touch = Input.GetTouch(0);
				if (_touchArea.Contains(touch.position))
				{
					if (touch.phase == TouchPhase.Began && touch.tapCount == switchAlignmentTapCount)
					{
						_touchOrigin = touch.position;
					}
					else if (touch.phase == TouchPhase.Ended && _touchOrigin.x >= 0)
					{
						_touchOrigin = -Vector2.one;
						NextAlignment();
					}
				}
			}
#endif
            }


		private void OnEnable()
		{
			if (!_isInitialized || _mode == Mode.Inactive) return;

			if (StatsMonitorWrapper.instance != null)
				StatsMonitorWrapper.instance.MoveToFront();

			fps = 0;
			memTotal = 0.0f;
			memAlloc = 0.0f;
			memMono = 0.0f;
			_intervalTimeCount = 0.0f;
			_intervalTimeCount2 = 0.0f;

			ResetMinMaxFPS();
			ResetAverageFPS();
			DetermineActualUpdateInterval();

			if (_mode == Mode.Active) CreateChildren();

			StartCoroutine("Interval");
			UpdateView();
			Invoke("RefreshThrottledFrameRate", 0.5f);
		}


		private void OnDisable()
		{
			if (!_isInitialized) return;
			StopCoroutine("Interval");
			if (IsInvoking("RefreshThrottledFrameRate")) CancelInvoke("RefreshThrottledFrameRate");
			RefreshThrottledFrameRate(true);
			DisposeChildren();
		}


		private void OnDestroy()
		{
			if (_isInitialized)
			{
				DisposeChildren();
				_isInitialized = false;
			}
			Destroy(gameObject);
		}


		private void OnLevelWasLoaded(int level)
		{
			if (!_isInitialized) return;
			if (!_keepAlive) Dispose();
			else
			{
				ResetMinMaxFPS();
				ResetAverageFPS();
			}
		}

        public void RegistProfiler(IProfiler profiler)
        {
            profilerList.Add(profiler);
        }

        public void UnRegistProfiler(IProfiler profiler)
        {
            profilerList.Remove(profiler);
        }

    }
}
