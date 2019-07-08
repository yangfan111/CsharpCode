// 
// Created 8/28/2015 01:15:50
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using UnityEditor;


namespace StatsMonitor
{
	[CustomEditor(typeof(StatsMonitorWrapper))]
	public class StatsMonitorWrapperEditor : Editor
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		private StatsMonitorWrapper _self;


		// ----------------------------------------------------------------------------
		// Unity Editor Callbacks
		// ----------------------------------------------------------------------------

		public void OnEnable()
		{
			_self = (target as StatsMonitorWrapper);
		}


		public override void OnInspectorGUI()
		{
			if (_self == null) return;
			serializedObject.Update();
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(StatsMonitor.NAME + " v" + StatsMonitor.VERSION + " by Hexagon Star Softworks\n\nTo edit the stats monitor parameters drill down to the child object named StatsMonitor and edit its parameters.", MessageType.None);
			EditorGUILayout.Space();
			serializedObject.ApplyModifiedProperties();
		}
	}
}
