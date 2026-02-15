using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codes.Common
{
    public class SpatialHash2D<T>
    {
        private readonly float cellSize;
        private readonly Dictionary<(int, int), HashSet<T>> grid = new Dictionary<(int, int), HashSet<T>>();

        // 用于快速更新
        private readonly Dictionary<T, (int, int)> objectToCell = new Dictionary<T, (int, int)>();

        public SpatialHash2D(float cellSize)
        {
            this.cellSize = cellSize;
        }

        // 计算格子坐标
        private (int, int) GetCell(Vector3 pos)
        {
            int x = (int)MathF.Floor(pos.x / cellSize);
            int y = (int)MathF.Floor(pos.z / cellSize);
            return (x, y);
        }

        public void Add(T obj, Vector3 position)
        {
            var cell = GetCell(position);

            if (!grid.TryGetValue(cell, out var bucket))
            {
                bucket = new HashSet<T>();
                grid[cell] = bucket;
            }

            bucket.Add(obj);
            objectToCell[obj] = cell;
        }

        public void Remove(T obj)
        {
            if (!objectToCell.TryGetValue(obj, out var cell))
                return;

            if (grid.TryGetValue(cell, out var bucket))
            {
                bucket.Remove(obj);
                if (bucket.Count == 0)
                    grid.Remove(cell);
            }

            objectToCell.Remove(obj);
        }

        public void Update(T obj, Vector3 newPosition)
        {
            if (!objectToCell.TryGetValue(obj, out var oldCell))
            {
                Add(obj, newPosition);
                return;
            }

            var newCell = GetCell(newPosition);

            if (oldCell.Equals(newCell))
                return;

            Remove(obj);
            Add(obj, newPosition);
        }

        // 范围查询（圆形）
        public List<T> Query(Vector3 center, float radius)
        {
            List<T> result = new List<T>();

            int minX = (int)MathF.Floor((center.x - radius) / cellSize);
            int maxX = (int)MathF.Floor((center.x + radius) / cellSize);
            int minY = (int)MathF.Floor((center.z - radius) / cellSize);
            int maxY = (int)MathF.Floor((center.z + radius) / cellSize);

            float r2 = radius * radius;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (grid.TryGetValue((x, y), out var bucket))
                    {
                        foreach (var obj in bucket)
                        {
                            result.Add(obj);
                        }
                    }
                }
            }

            return result;
        }

        public void Clear()
        {
            grid.Clear();
            objectToCell.Clear();
        }
    }
}
