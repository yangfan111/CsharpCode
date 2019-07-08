// 
// Created 9/1/2015 15:00:16
// Copyright © Hexagon Star Softworks. All Rights Reserved.
// http://www.hexagonstar.com/
//  

using UnityEngine;


namespace StatsMonitor.Core
{
	public enum ViewInvalidationType : byte
	{
		All,
		Style,
		Layout
	}


	/// <summary>
	///		Base class for 2D views.
	/// </summary>
	public abstract class View2D
	{
		// ----------------------------------------------------------------------------
		// Properties
		// ----------------------------------------------------------------------------

		/// <summary>
		///		The game object of this view. All child objects should be added
		///		inside this game object.
		/// </summary>
		internal GameObject gameObject;


		// ----------------------------------------------------------------------------
		// Accessors
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Returns the RectTransform for the view. If the views doesn't have a
		///		RectTransform compoents yet, one is added automatically.
		/// </summary>
		public RectTransform RTransform
		{
			get
			{
				RectTransform t = gameObject.GetComponent<RectTransform>();
				if (t == null) t = gameObject.AddComponent<RectTransform>();
				return t;
			}
		}


		/// <summary>
		///		The width of the view.
		/// </summary>
		public float Width
		{
			get { return RTransform.rect.width; }
			set { RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); }
		}


		/// <summary>
		///		The height of the view.
		/// </summary>
		public float Height
		{
			get { return RTransform.rect.height; }
			set { RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value); }
		}


		/// <summary>
		///		The X position of the view.
		/// </summary>
		public float X
		{
			get { return RTransform.anchoredPosition.x; }
			set { RTransform.anchoredPosition = new Vector2(value, Y); }
		}


		/// <summary>
		///		The Y position of the view.
		/// </summary>
		public float Y
		{
			get { return RTransform.anchoredPosition.y; }
			set { RTransform.anchoredPosition = new Vector2(X, value); }
		}


		/// <summary>
		///		The pivot vector of the view.
		/// </summary>
		public Vector2 Pivot
		{
			get { return RTransform.pivot; }
			set { RTransform.pivot = value; }
		}


		/// <summary>
		///		The min anchor vector of the view.
		/// </summary>
		public Vector2 AnchorMin
		{
			get { return RTransform.anchorMin; }
			set { RTransform.anchorMin = value; }
		}


		/// <summary>
		///		The max anchor vector of the view.
		/// </summary>
		public Vector2 AnchorMax
		{
			get { return RTransform.anchorMax; }
			set { RTransform.anchorMax = value; }
		}


		// ----------------------------------------------------------------------------
		// Public Methods
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Sets the x and y position of the view.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void SetPosition(float x, float y)
		{
			RTransform.anchoredPosition = new Vector2(x, y);
		}


		/// <summary>
		///		Sets the width and height of the view.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public void SetSize(float width, float height)
		{
			RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}


		/// <summary>
		///		Sets the pivot, min and max to the same vector.
		/// </summary>
		/// <param name="vector"></param>
		public void SetPivotAndAnchor(Vector2 vector)
		{
			Pivot = AnchorMin = AnchorMax = vector;
		}


		/// <summary>
		///		Allows to set all rect transform values at once.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="pivotAndAnchor"></param>
		public void SetRTransformValues(float x, float y, float width, float height, Vector2 pivotAndAnchor)
		{
			RTransform.anchoredPosition = new Vector2(x, y);
			RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			Pivot = AnchorMin = AnchorMax = pivotAndAnchor;
		}


		/// <summary>
		///		Invalidates the view. This methods takes care of that all child
		///		objects have been created and then takes care that the style and layout
		///		is updated. This method needs to be called from a sub class constructor!
		/// </summary>
		public void Invalidate(ViewInvalidationType type = ViewInvalidationType.All)
		{
			if (gameObject == null) gameObject = CreateChildren();
			if (type == ViewInvalidationType.Style || type == ViewInvalidationType.All)
				UpdateStyle();
			if (type == ViewInvalidationType.Layout || type == ViewInvalidationType.All)
				UpdateLayout();
		}


		/// <summary>
		///		Resets the view and all its child objects.
		/// </summary>
		public virtual void Reset()
		{
		}


		/// <summary>
		///		Updates the view.
		/// </summary>
		public virtual void Update()
		{
		}


		/// <summary>
		///		Disposes the view and all its children.
		/// </summary>
		public virtual void Dispose()
		{
			Destroy(gameObject);
			gameObject = null;
		}


		// ----------------------------------------------------------------------------
		// Internal Methods
		// ----------------------------------------------------------------------------

		/// <summary>
		///		Static helper method to destroy child objects.
		/// </summary>
		/// <param name="obj"></param>
		internal static void Destroy(Object obj)
		{
			Object.Destroy(obj);
		}


		// ----------------------------------------------------------------------------
		// Protected & Private Methods
		// ----------------------------------------------------------------------------



		/// <summary>
		///		Used to create any child objects for the view. Should only be called
		///		once per object lifetime. This method must return the game object of
		///		the view!
		/// </summary>
		protected virtual GameObject CreateChildren()
		{
			return null;
		}


		/// <summary>
		///		Used to update the style of child objects. This can be used to make
		///		visual changes that don't require the layout to be updated, for example
		///		color changes.
		/// </summary>
		protected virtual void UpdateStyle()
		{
		}


		/// <summary>
		///		Used to layout all child objects in the view. This updates the
		///		transformations of all children and should be called to change the
		///		size, position, etc. of child objects.
		/// </summary>
		protected virtual void UpdateLayout()
		{
		}
	}
}
