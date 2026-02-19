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
    //public struct FlowFieldCell
    //{
    //    public Vec2I Pos;
    //    public bool IsObstacle;
    //    public sbyte FlowDirX; // -1,0,1
    //    public sbyte FlowDirY; // -1,0,1
    //    public float Cost;     // 到目标的累计代价
    //    public FlowFieldCell(Vec2I pos)
    //    {
    //        Pos = pos;
    //        IsObstacle = false;
    //        FlowDirX = 0;
    //        FlowDirY = 0;
    //        Cost = float.MaxValue;
    //    }
    //}

    //public class FlowField
    //{
    //    private int _width;
    //    private int _height;
    //    private float _cellSize;
    //    private FlowFieldCell[,] _grid;
    //    private Vector3 targetPos;
    //    private NavigationGrid _naviGrid;

    //    public FlowField()
    //    {
    //        _naviGrid = NavigationGrid.Instance();
    //        _cellSize = _naviGrid.CellSize;
    //        _width = _naviGrid.Width;
    //        _height = _naviGrid.Height;

    //        _grid = new FlowFieldCell[_width, _height];
    //        for (int x = 0; x < _width; x++)
    //            for (int y = 0; y < _height; y++)
    //                _grid[x, y] = new FlowFieldCell(new Vec2I(x, y));
    //    }

    //    // 8方向偏移
    //    private static readonly int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
    //    private static readonly int[] dy = { 1, 1, 1, 0, 0, -1, -1, -1 };
    //    private static readonly float[] moveCost = { 1.414f, 1, 1.414f, 1, 1, 1.414f, 1, 1.414f };

    //    public void Compute(Vector3 worldTargetPos)
    //    {
    //        targetPos = worldTargetPos;
    //        Vec2I target = GlobalHelper.WorldToGrid(worldTargetPos, _cellSize);

    //        // 初始化
    //        for (int x = 0; x < _width; x++)
    //            for (int y = 0; y < _height; y++)
    //            {
    //                _grid[x, y].FlowDirX = 0;
    //                _grid[x, y].FlowDirY = 0;
    //                _grid[x, y].Cost = float.MaxValue;
    //            }

    //        MyPriorityQueue<Vec2I> pq = new MyPriorityQueue<Vec2I>();
    //        _grid[target.X, target.Y].Cost = 0;
    //        pq.Enqueue(target, 0);

    //        while (pq.Count > 0)
    //        {
    //            var current = pq.Dequeue();
    //            ref var currentCell = ref _grid[current.X, current.Y];

    //            for (int i = 0; i < 8; i++)
    //            {
    //                int nx = current.X + dx[i];
    //                int ny = current.Y + dy[i];
    //                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height)
    //                {
    //                    var neighbor = _grid[nx, ny];
    //                    if (_naviGrid.WalkableList[nx,ny] == false) 
    //                        continue;

    //                    float newCost = currentCell.Cost + moveCost[i];
    //                    if (newCost < neighbor.Cost)
    //                    {
    //                        neighbor.Cost = newCost;
    //                        // 方向指向目标
    //                        neighbor.FlowDirX = (sbyte)Math.Clamp(current.X - nx, -1, 1);
    //                        neighbor.FlowDirY = (sbyte)Math.Clamp(current.Y - ny, -1, 1);

    //                        _grid[nx, ny] = neighbor;
    //                        pq.Enqueue(new Vec2I(nx, ny), newCost);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public Vector3 GetDir(Vector3 worldPos)
    //    {
    //        // 转成浮点格子坐标
    //        Vec2I gridPos = GlobalHelper.WorldToGrid(worldPos, _cellSize);

    //        int x0 = gridPos.X;
    //        int y0 = gridPos.Y;
    //        int x1 = x0 + 1;
    //        int y1 = y0 + 1;

    //        // 越界保护（边缘直接返回最近格子方向）
    //        if (x0 < 0 || y0 < 0 || x1 >= _width || y1 >= _height)
    //        {
    //            x0 = Mathf.Clamp(x0, 0, _width - 1);
    //            y0 = Mathf.Clamp(y0, 0, _height - 1);
    //            var cell = _grid[x0, y0];
    //            Vector3 fallback = new Vector3(cell.FlowDirX, 0, cell.FlowDirY);
    //            if (fallback.sqrMagnitude > 0)
    //                fallback.Normalize();
    //            return fallback;
    //        }

    //        float tx = gridPos.X - x0;
    //        float ty = gridPos.Y - y0;

    //        Vector3 d00 = new Vector3(_grid[x0, y0].FlowDirX, 0, _grid[x0, y0].FlowDirY);
    //        Vector3 d10 = new Vector3(_grid[x1, y0].FlowDirX, 0, _grid[x1, y0].FlowDirY);
    //        Vector3 d01 = new Vector3(_grid[x0, y1].FlowDirX, 0, _grid[x0, y1].FlowDirY);
    //        Vector3 d11 = new Vector3(_grid[x1, y1].FlowDirX, 0, _grid[x1, y1].FlowDirY);

    //        // 双线性插值
    //        Vector3 dx0 = Vector3.Lerp(d00, d10, tx);
    //        Vector3 dx1 = Vector3.Lerp(d01, d11, tx);
    //        Vector3 result = Vector3.Lerp(dx0, dx1, ty);

    //        if (result.sqrMagnitude > 0)
    //            result.Normalize();

    //        return result;
    //    }

    //    public Vector3 GetTargetPos()
    //    {
    //        return targetPos;
    //    }
    //}

    public class FlowField
    {
        private Vector3 _targetPos;

        private ushort[,] _costList;
        private byte[,] _dirList;
        private NavigationGrid _naviGrid;
        static readonly Vector2Int[] DirTable =
        {
            new Vector2Int(0,1),    // 0
            new Vector2Int(1,1),    // 1
            new Vector2Int(1,0),    // 2
            new Vector2Int(1,-1),   // 3
            new Vector2Int(0,-1),   // 4
            new Vector2Int(-1,-1),  // 5
            new Vector2Int(-1,0),   // 6
            new Vector2Int(-1,1)    // 7
        };
        static readonly ushort[] DirCost =
        {
            10, // 上
            14, // 右上
            10, // 右
            14, // 右下
            10, // 下
            14, // 左下
            10, // 左
            14  // 左上
        };

        public FlowField()
        {
            _naviGrid = NavigationGrid.Instance();
            _costList = new ushort[_naviGrid.Width, _naviGrid.Height];
            _dirList = new byte[_naviGrid.Width, _naviGrid.Height];
        }
        public void Compute(Vector3 worldTargetPos)
        {
            _targetPos = worldTargetPos;
            Vector2Int target = _naviGrid.WorldToGrid(worldTargetPos);

            if (target.x < 0 || target.y < 0 ||
                target.x >= _naviGrid.Width || target.y >= _naviGrid.Height)
                return;

            if (!_naviGrid.WalkableList[target.x, target.y])
                return;

            int width = _naviGrid.Width;
            int height = _naviGrid.Height;

            // 1 初始化
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    _costList[x, y] = ushort.MaxValue;

            MyPriorityQueue<Vector2Int> open = new();

            _costList[target.x, target.y] = 0;
            open.Enqueue(target, 0);

            // 2 Dijkstra
            while (open.Count > 0)
            {
                Vector2Int cur = open.Dequeue();
                ushort curCost = _costList[cur.x, cur.y];

                // 如果已经是无效节点（理论上不会为 Max，但做保护）
                if (curCost == ushort.MaxValue)
                    continue;

                for (int i = 0; i < 8; i++)
                {
                    int nx = cur.x + DirTable[i].x;
                    int ny = cur.y + DirTable[i].y;

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    if (!_naviGrid.WalkableList[nx, ny])
                        continue;

                    // 防止穿角
                    if (i % 2 == 1) // 对角方向
                    {
                        int adj1x = cur.x + DirTable[i].x;
                        int adj1y = cur.y;

                        int adj2x = cur.x;
                        int adj2y = cur.y + DirTable[i].y;

                        if (!_naviGrid.WalkableList[adj1x, adj1y] ||
                            !_naviGrid.WalkableList[adj2x, adj2y])
                            continue;
                    }

                    ushort newCost = (ushort)(curCost + DirCost[i]);

                    if (newCost < _costList[nx, ny])
                    {
                        _costList[nx, ny] = newCost;
                        open.Enqueue(new Vector2Int(nx, ny), newCost);
                    }
                }
            }

            // 3 生成方向
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    ushort bestCost = _costList[x, y];
                    byte bestDir = 255;

                    if (bestCost == ushort.MaxValue)
                    {
                        _dirList[x, y] = 255;
                        continue;
                    }

                    for (byte i = 0; i < 8; i++)
                    {
                        int nx = x + DirTable[i].x;
                        int ny = y + DirTable[i].y;

                        if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                            continue;

                        if (_costList[nx, ny] < bestCost)
                        {
                            bestCost = _costList[nx, ny];
                            bestDir = i;
                        }
                    }

                    _dirList[x, y] = bestDir;
                }
            }
        }


        public Vector3 GetDir(Vector3 worldPos)
        {
            Vector2Int grid = _naviGrid.WorldToGrid(worldPos);

            if (grid.x < 0 || grid.y < 0 || grid.x >= _naviGrid.Width || grid.y >= _naviGrid.Height)
                return Vector3.zero;

            byte dir = _dirList[grid.x, grid.y];
            if (dir == 255)
                return Vector3.zero;

            Vector2Int d = DirTable[dir];
            return new Vector3(d.x, 0, d.y).normalized;
        }

        public Vector3 GetTargetPos()
        {
            return _targetPos;
        }
        public Vector2Int GetDir(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _naviGrid.Width || y >= _naviGrid.Height)
                return Vector2Int.zero;

            byte dir = _dirList[x, y];
            if (dir == 255)
                return Vector2Int.zero;
            return DirTable[dir];
        }
    }
}
