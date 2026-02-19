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
        private Vector3 targetPos;
        private NavigationGrid _naviGrid;

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

        // 8方向偏移
        private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        private static readonly int[] dy = { 1, 1, 1, 0, 0, -1, -1, -1 };
        private static readonly float[] moveCost = { 1.414f, 1, 1.414f, 1, 1, 1.414f, 1, 1.414f };

        public void Compute(Vector3 worldTargetPos)
        {
            targetPos = worldTargetPos;
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
                        if (_naviGrid.WalkableList[nx,ny] == false) 
                            continue;

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
            // 转成浮点格子坐标
            Vec2I gridPos = GlobalHelper.WorldToGrid(worldPos, _cellSize);

            int x0 = gridPos.X;
            int y0 = gridPos.Y;
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            // 越界保护（边缘直接返回最近格子方向）
            if (x0 < 0 || y0 < 0 || x1 >= _width || y1 >= _height)
            {
                x0 = Mathf.Clamp(x0, 0, _width - 1);
                y0 = Mathf.Clamp(y0, 0, _height - 1);
                var cell = _grid[x0, y0];
                Vector3 fallback = new Vector3(cell.FlowDirX, 0, cell.FlowDirY);
                if (fallback.sqrMagnitude > 0)
                    fallback.Normalize();
                return fallback;
            }

            float tx = gridPos.X - x0;
            float ty = gridPos.Y - y0;

            Vector3 d00 = new Vector3(_grid[x0, y0].FlowDirX, 0, _grid[x0, y0].FlowDirY);
            Vector3 d10 = new Vector3(_grid[x1, y0].FlowDirX, 0, _grid[x1, y0].FlowDirY);
            Vector3 d01 = new Vector3(_grid[x0, y1].FlowDirX, 0, _grid[x0, y1].FlowDirY);
            Vector3 d11 = new Vector3(_grid[x1, y1].FlowDirX, 0, _grid[x1, y1].FlowDirY);

            // 双线性插值
            Vector3 dx0 = Vector3.Lerp(d00, d10, tx);
            Vector3 dx1 = Vector3.Lerp(d01, d11, tx);
            Vector3 result = Vector3.Lerp(dx0, dx1, ty);

            if (result.sqrMagnitude > 0)
                result.Normalize();

            return result;
        }

        public Vector3 GetTargetPos()
        {
            return targetPos;
        }
    }
}
