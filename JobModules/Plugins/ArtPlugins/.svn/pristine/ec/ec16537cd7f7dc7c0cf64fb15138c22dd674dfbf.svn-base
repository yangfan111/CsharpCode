using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArtPlugins
{
      struct IntVector2
    {
        public int x;
        public int y;


        public IntVector2(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }
    }
    [System.Serializable]
    public class MapSectionItemBase {
        public Vector3 position;
    }
    public  class MapSectionBase<I> where    I : MapSectionItemBase
    {
         public int rectX;
        public int rectZ;

        

        public virtual void AddItem(I item)
        {
    
        }

        internal void init(int rectX, int rectZ)
        {
            this.rectX = rectX;
            this.rectZ = rectZ;
        }
    }
    public class MapSectionManager <S,I> where S:MapSectionBase<I> ,new()  where I:MapSectionItemBase
    {
        public int rectWidth = 100;
        public int rectWidthMax = 8000;
        public S[,] rectInstances;
        private IntVector2 lastPos;
        private int rectCols;
        private Action<List<S>> setup;
        public MapSectionManager(Action<List<S>> setup, int rectWidth, int rectWidthMax=8000)
        {
            this.setup = setup;
            this.rectWidth = rectWidth;
            this.rectWidthMax = rectWidthMax;
            rectCols = rectWidthMax / rectWidth;
            lastPos = new IntVector2(-1, -1);
            rectInstances = new S[rectCols, rectCols];
        }



     public   bool updateShowRects(bool forceFlush = false)
        {

            var pos = Camera.main.transform.position + new Vector3(rectWidthMax / 2, 0, rectWidthMax / 2);


            var rectPos = new IntVector2();
            rectPos.x = (int)(pos.x / rectWidth);
            rectPos.y = (int)(pos.z / rectWidth);
            if (forceFlush == false)
            {
                if (rectPos.x == lastPos.x && rectPos.y == lastPos.y) return false;
            }
 

            List<IntVector2> rects = new List<IntVector2>();
            lastPos.x = rectPos.x;
            lastPos.y = rectPos.y;
 
            for (int i = Mathf.Max(0, rectPos.x - 1); i <= Mathf.Min(rectCols - 1, rectPos.x + 1); i++)
            {
                for (int j = (int)Mathf.Max(0, rectPos.y - 1); j <= Mathf.Min(rectCols - 1, rectPos.y + 1); j++)
                {
                    rects.Add(new IntVector2(i, j));
                   // Debug.Log("add section" + i + "," + j);
                }
            }


         var sections=   updateShowRects(rects);
            setup(sections);
            return true;
       


        }

       

        public void splitObjectsToRect(List<I> objects)
        {
            if (objects == null) return;
            foreach (var item in objects)
            {
                int rectX = (int)((item.position.x + rectWidthMax / 2) / rectWidth);
                int rectZ = (int)((item.position.z + rectWidthMax / 2) / rectWidth);
                if (rectX < 0 || rectX >= rectCols) continue;
                if (rectZ < 0 || rectZ >= rectCols) continue;
                var group = rectInstances[rectX, rectZ];
                if (group == null)
                {
                    group = rectInstances[rectX, rectZ] = new S();
                    group.init(rectX, rectZ);
                }

                group.AddItem(item);

            }
        }

        List<S> updateShowRects(List<IntVector2> rects)
        {
            var showItems = new List<S>();
            foreach (var rect in rects)
            {
                var rectItems = rectInstances[rect.x, rect.y];
                if (rectItems != null)
                {
                    showItems.Add(rectItems);
                }
            }
            return showItems;

        }
    }
}