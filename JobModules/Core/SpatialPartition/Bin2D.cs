using System;
using System.Collections.Generic;
using Core.ObjectPool;
using Core.Utils;
using Core.Utils.System46;
using UnityEngine;

namespace Core.SpatialPartition
{
    /// <summary>
    /// A simple GC-friendly two-dimensional <see href="https://en.wikipedia.org/wiki/Bin_(computational_geometry)">Bin (aka Spatial Grid)</see> implementation.
    /// 
    /// When you're done with the Bin, you should <see cref="Dispose"/> it so its resources can be freed in their object pool. If you forget this, no harm will be done but memory will be GC'ed.
    /// 
    /// </summary>
    /// <typeparam name="T">The type of items this Bin will hold.</typeparam>
    public class Bin2D<T> : IDisposable
    {

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(Bin2D<T>));
        public MyDictionary<T, int> AllocateCellContainer()
        {
            return ObjectAllocatorHolder<MyDictionary<T, int>>.Allocate();
        }
        public void FreeCellContainer(MyDictionary<T, int> container)
        {
            ObjectAllocatorHolder<MyDictionary<T, int>>.Free(container);
        }

        private readonly Grid2D<MyDictionary<T, int>> _grid;
        private readonly Vector2 _bottomLeft;
        private readonly Vector2 _topRight;

        /// <summary>
        /// Gets the width (number of columns) of the Bin.
        /// </summary>
        public int Width { get { return _grid.Width; } }

        /// <summary>
        /// Gets the height (number of rows) of the Bin.
        /// </summary>
        public int Height { get { return _grid.Height; } }

        /// <summary>
        /// Gets the width of cells in the Bin.
        /// </summary>
        public float CellWidth { get; private set; }

        /// <summary>
        /// Gets the height of cells in the Bin.
        /// </summary>
        public float CellHeight { get; private set; }

        /// <summary>
        /// The coordinate at which this bin's bottom left corner lies.
        /// </summary>
        public Vector2 Origin { get { return _bottomLeft; } }


        /// <summary>
        /// Creates a new Bin.
        /// </summary>
        public Bin2D(Bin2DConfig config)
        {
            _grid = new Grid2D<MyDictionary<T, int>>(config.GridWidth, config.GridHeight);

            CellWidth = config.CellWidth;
            CellHeight = config.CellHeight;
            _bottomLeft = config.BottomLeft;
            _topRight = new Vector2(_bottomLeft.x + config.GridWidth * config.CellWidth,
                _bottomLeft.y + config.GridHeight * config.CellHeight);
        }

        /// <summary>
        /// Inserts an item with the given bounds into the Bin.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="pos"></param>
        public void Insert(T item, Vector2 pos)
        {
            if (IsOutOfBounds(pos))
            {
                _logger.ErrorFormat("out of bounds for insert {0}:{1}", pos, item);
                return;
            }

            var cell = GetCell(pos);
            var cellContainer = _grid[cell.x, cell.y];
            if (cellContainer == null)
            {
                cellContainer = AllocateCellContainer();
                _grid[cell.x, cell.y] = cellContainer;
            }
            cellContainer.Add(item, 0);
            
        }

        /// <summary>
        /// Goes through all cells and removes the specified item if they contain it.
        /// If you can you should use <see cref="Remove(T, Rect)"/> instead.
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void Remove(T item, Vector2 pos)
        {
            if (IsOutOfBounds(pos))
            {
                _logger.ErrorFormat("out of bounds for remove {0}: {1}", pos, item);
                return;
            }
                

            var cell = GetCell(pos);
            var cellContainer = _grid[cell.x, cell.y];
            if (cellContainer != null)
            {
                cellContainer.Remove(item);
            }
        }

        /// <summary>
        /// Removes and reinserts an item with new bounds, essentially moving it.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public void Update(T item, Vector2 prevPos, Vector2 newPos)
        {
            var prevCell = GetCell(prevPos);
            var nowCell = GetCell(newPos);
            if (!(prevCell == nowCell))
            {
                Remove(item, prevPos);
                Insert(item, newPos);
            }
        }

        /// <summary>
        /// Gets all items in the Bin that could potentially intersect with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to check for.</param>
        /// <param name="results">Where to add results to.</param>
        public void Retrieve(Rect bounds, List<T> results)
        {
            Retrieve(bounds, results.Add);
        }
        public void Retrieve(Rect bounds, Action<T> action)
        {
            if (IsOutOfBounds(bounds))
            {
                _logger.ErrorFormat("out of bounds for retrieve {0}", bounds);
                return;
            }

            var internalBounds = GetInternalBounds(bounds);

            for (var y = internalBounds.MinY; y <= internalBounds.MaxY; y++)
            {
                for (var x = internalBounds.MinX; x <= internalBounds.MaxX; x++)
                {
                    var cell = _grid[x, y];

                    if (cell == null)
                        continue;

                    foreach (var item in cell.Keys)
                    {
                        action(item);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Removes all items from the Bin.
        /// </summary>
        public void Clear()
        {
            for (var y = 0; y < _grid.Height; y++)
            {
                for (var x = 0; x < _grid.Width; x++)
                {
                    var cell = _grid[x, y];

                    if (cell == null)
                        continue;

                    cell.Clear();
                    FreeCellContainer(cell);

                    _grid[x, y] = null;
                }
            }
        }

        /// <summary>
        /// Frees (clears) used resources that can be recycled. 
        /// 
        /// Call this when you're done with the Bin.
        /// </summary>
        public void Dispose()
        {
            Clear();
        }

        private bool IsOutOfBounds(Vector2 pos)
        {
            return !(pos.x > _bottomLeft.x
                && pos.x < _topRight.x
                && pos.y > _bottomLeft.y
                && pos.y < _topRight.y);
        }

        private Cell GetCell(Vector2 pos)
        {
            return new Cell((int)((pos.x - _bottomLeft.x) / CellWidth), (int)((pos.y - _bottomLeft.y) / CellHeight));
        }

        private struct Cell
        {
            public Cell(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;

            public static bool operator ==(Cell l, Cell r)
            {
                return l.x == r.x && l.y == r.y;
            }

            public static bool operator !=(Cell l, Cell r)
            {
                return !(l == r);
            }
        }

        private bool IsOutOfBounds(Rect bounds)
        {
            return !(bounds.xMax > _bottomLeft.x
                     && bounds.xMin < _topRight.x
                     && bounds.yMax > _bottomLeft.y
                     && bounds.yMin < _topRight.y);
        }


        private InternalBounds GetInternalBounds(Rect bounds)
        {
            var internalBounds = new InternalBounds
            {
                MinX = Mathf.Max(0, (int)((bounds.xMin - _bottomLeft.x) / CellWidth)),
                MinY = Mathf.Max(0, (int)((bounds.yMin - _bottomLeft.y) / CellHeight)),
                MaxX = Mathf.Min(Width - 1, (int)((bounds.xMax - _bottomLeft.x) / CellWidth)),
                MaxY = Mathf.Min(Height - 1, (int)((bounds.yMax - _bottomLeft.y) / CellHeight))
            };

            return internalBounds;
        }

        private struct InternalBounds
        {
            public int MinX, MinY,
                MaxX, MaxY;

            public int CellCount {get { return (MaxX - MinX + 1) * (MaxY - MinY + 1); }}
        }
    }
}