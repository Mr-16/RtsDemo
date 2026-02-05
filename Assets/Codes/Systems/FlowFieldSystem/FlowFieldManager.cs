using Assets.Codes.Common;
using Assets.Codes.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codes.Systems.FlowFieldSystem
{
    public struct FlowFieldCell
    {
        public Vec2I Pos;
        public bool IsObstacle;
        public sbyte FlowDirX; // -1,0,1
        public sbyte FlowDirY; // -1,0,1
        public float Cost;     // 到目标的累计代价
        public FlowFieldCell(Vec2I pos)
        {
            Pos = pos;
            IsObstacle = false;
            FlowDirX = 0;
            FlowDirY = 0;
            Cost = float.MaxValue;
        }
    }

    public class FlowField
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private FlowFieldCell[,] _grid;

        public FlowField(float cellSize)
        {
            this._cellSize = cellSize;
            _width = (int)MathF.Ceiling(GlobalConstant.WorldWidth / cellSize);
            _height = (int)MathF.Ceiling(GlobalConstant.WorldHeight / cellSize);

            _grid = new FlowFieldCell[_width, _height];
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _grid[x, y] = new FlowFieldCell(new Vec2I(x, y));
        }

        // 8方向偏移
        private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        private static readonly int[] dy = { 1, 1, 1, 0, 0, -1, -1, -1 };
        private static readonly float[] moveCost = { 1.414f, 1, 1.414f, 1, 1, 1.414f, 1, 1.414f };

        public void ComputeFlowField(Vector3 worldTargetPos)
        {
            Vec2I target = GlobalHelper.WorldToGrid(worldTargetPos, _cellSize);

            // 初始化
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y].FlowDirX = 0;
                    _grid[x, y].FlowDirY = 0;
                    _grid[x, y].Cost = float.MaxValue;
                }

            MyPriorityQueue<Vec2I> pq = new MyPriorityQueue<Vec2I>();
            _grid[target.X, target.Y].Cost = 0;
            pq.Enqueue(target, 0);

            while (pq.Count > 0)
            {
                var current = pq.Dequeue();
                ref var currentCell = ref _grid[current.X, current.Y];

                for (int i = 0; i < 8; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];
                    if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
                    {
                        var neighbor = _grid[nx, ny];
                        if (neighbor.IsObstacle) continue;

                        float newCost = currentCell.Cost + moveCost[i];
                        if (newCost < neighbor.Cost)
                        {
                            neighbor.Cost = newCost;
                            // 方向指向目标
                            neighbor.FlowDirX = (sbyte)Math.Clamp(current.X - nx, -1, 1);
                            neighbor.FlowDirY = (sbyte)Math.Clamp(current.Y - ny, -1, 1);

                            _grid[nx, ny] = neighbor;
                            pq.Enqueue(new Vec2I(nx, ny), newCost);
                        }
                    }
                }
            }
        }

        public Vector3 GetDir(Vector3 worldPos)
        {
            Vec2I cellPos = GlobalHelper.WorldToGrid(worldPos, _cellSize);
            FlowFieldCell cell = _grid[cellPos.X, cellPos.Y];
            Vector3 dir = new Vector3(cell.FlowDirX, 0, cell.FlowDirY);
            if (dir.sqrMagnitude > 0)
                dir.Normalize();
            return dir;
        }

        public void SetObstacle(int x, int y, bool isObstacle)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                _grid[x, y].IsObstacle = isObstacle;
        }
    }

    public class FlowFieldManager
    {
    }
}
