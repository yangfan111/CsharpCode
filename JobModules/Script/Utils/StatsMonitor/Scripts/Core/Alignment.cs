// 
// Alignment.cs
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using UnityEngine;


namespace StatsMonitor.Core
{
	public enum Alignment : byte
	{
		UpperLeft,
		UpperCenter,
		UpperRight,
		LowerRight,
		LowerCenter,
		LowerLeft
	}


	public class Anchor
	{
		public Vector2 position;
		public Vector2 min;
		public Vector2 max;
		public Vector2 pivot;

		public Anchor(float x, float y, float minX, float minY, float maxX, float maxY,
			float pivotX, float pivotY)
		{
			position = new Vector2(x, y);
			min = new Vector2(minX, minY);
			max = new Vector2(maxX, maxY);
			pivot = new Vector2(pivotX, pivotY);
		}
	}


	public class Anchors
	{
		public Anchor upperLeft = new Anchor(0, 0, 0, 1, 0, 1, 0, 1);
		public Anchor upperCenter = new Anchor(0, 0, .5f, 1, .5f, 1, .5f, 1);
		public Anchor upperRight = new Anchor(0, 0, 1, 1, 1, 1, 1, 1);
		public Anchor lowerRight = new Anchor(0, 0, 1, 0, 1, 0, 1, 0);
		public Anchor lowerCenter = new Anchor(0, 0, .5f, 0, .5f, 0, .5f, 0);
		public Anchor lowerLeft = new Anchor(0, 0, 0, 0, 0, 0, 0, 0);
	}
}
