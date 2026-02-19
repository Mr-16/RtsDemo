using Assets.Codes.Common;
using Assets.Codes.Game;
using System;
using UnityEngine;

namespace Assets.Codes.Systems.FlowFieldSystem
{
    public struct FlowFieldCell
    {
        public Vec2I Pos;
        public sbyte FlowDirX;
        public sbyte FlowDirY;
        public float Cost;

        public FlowFieldCell(Vec2I pos)
        {
            Pos = pos;
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
        private Vector3 _targetPos;
        private NavigationGrid _naviGrid;

        // 8方向
        private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        private static readonly int[] dy = { 1, 1, 1, 0, 0, -1, -1, -1 };
        private static readonly float[] moveCost = { 1.414f, 1, 1.414f, 1, 1, 1.414f, 1, 1.414f };

        public FlowField()
        {
            _naviGrid = NavigationGrid.Instance();
            _cellSize = _naviGrid.CellSize;
            _width = _naviGrid.Width;
            _height = _naviGrid.Height;

            _grid = new FlowFieldCell[_width, _height];
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _grid[x, y] = new FlowFieldCell(new Vec2I(x, y));
        }

        // =========================================
        // 入口
        // =========================================
        public void Compute(Vector3 worldTargetPos, float unitRadius = 0f)
        {
            _targetPos = worldTargetPos;

            Vec2I target = GlobalHelper.WorldToGrid(worldTargetPos, _cellSize);
            if (!InBounds(target.X, target.Y))
                return;

            ResetGrid();

            bool[,] walkable = BuildWalkableMap(unitRadius);

            if (!walkable[target.X, target.Y])
                return;

            MyPriorityQueue<Vec2I> pq = new MyPriorityQueue<Vec2I>();
            _grid[target.X, target.Y].Cost = 0;
            pq.Enqueue(target, 0);

            while (pq.Count > 0)
            {
                Vec2I current = pq.Dequeue();
                ref FlowFieldCell currentCell = ref _grid[current.X, current.Y];

                for (int i = 0; i < 8; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (!InBounds(nx, ny))
                        continue;

                    if (!walkable[nx, ny])
                        continue;

                    // 禁止对角穿角
                    if (IsDiagonal(i))
                    {
                        int adj1x = current.X + dx[i];
                        int adj1y = current.Y;
                        int adj2x = current.X;
                        int adj2y = current.Y + dy[i];

                        if (!walkable[adj1x, adj1y] || !walkable[adj2x, adj2y])
                            continue;
                    }

                    float newCost = currentCell.Cost + moveCost[i];
                    ref FlowFieldCell neighbor = ref _grid[nx, ny];

                    if (newCost < neighbor.Cost)
                    {
                        neighbor.Cost = newCost;
                        neighbor.FlowDirX = (sbyte)Math.Clamp(current.X - nx, -1, 1);
                        neighbor.FlowDirY = (sbyte)Math.Clamp(current.Y - ny, -1, 1);
                        pq.Enqueue(new Vec2I(nx, ny), newCost);
                    }
                }
            }
        }

        // =========================================
        // 方向查询
        // =========================================
        public Vector3 GetDir(Vector3 worldPos)
        {
            Vec2I gridPos = GlobalHelper.WorldToGrid(worldPos, _cellSize);

            int x = Mathf.Clamp(gridPos.X, 0, _width - 1);
            int y = Mathf.Clamp(gridPos.Y, 0, _height - 1);

            FlowFieldCell cell = _grid[x, y];

            if (cell.Cost == float.MaxValue)
                return Vector3.zero;

            Vector3 dir = new Vector3(cell.FlowDirX, 0, cell.FlowDirY);

            if (dir.sqrMagnitude > 0)
                dir.Normalize();

            return dir;
        }

        public Vector3 GetTargetPos()
        {
            return _targetPos;
        }

        // =========================================
        // 工具函数
        // =========================================
        private void ResetGrid()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                {
                    _grid[x, y].Cost = float.MaxValue;
                    _grid[x, y].FlowDirX = 0;
                    _grid[x, y].FlowDirY = 0;
                }
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        private bool IsDiagonal(int index)
        {
            return index == 0 || index == 2 || index == 5 || index == 7;
        }

        // =========================================
        // 障碍膨胀（单位半径支持）
        // =========================================
        private bool[,] BuildWalkableMap(float unitRadius)
        {
            bool[,] walkable = new bool[_width, _height];

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    walkable[x, y] = _naviGrid.WalkableList[x, y];

            if (unitRadius <= 0f)
                return walkable;

            int expand = Mathf.CeilToInt(unitRadius / _cellSize);

            bool[,] inflated = new bool[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (!walkable[x, y])
                    {
                        for (int ix = -expand; ix <= expand; ix++)
                        {
                            for (int iy = -expand; iy <= expand; iy++)
                            {
                                int nx = x + ix;
                                int ny = y + iy;
                                if (InBounds(nx, ny))
                                    inflated[nx, ny] = true;
                            }
                        }
                    }
                }
            }

            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    if (inflated[x, y])
                        walkable[x, y] = false;

            return walkable;
        }
    }
}
