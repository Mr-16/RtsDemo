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












//public Vector3 GetDir(Vector3 worldPos)
//{
//    // 转为浮点格子坐标（根据你的实现替换）
//    Vector2 gridPos = _naviGrid.WorldToGrid(worldPos);
//    float gx = gridPos.x;
//    float gy = gridPos.y;

//    int width = _naviGrid.Width;
//    int height = _naviGrid.Height;

//    // 基础格子
//    int x0 = Mathf.FloorToInt(gx);
//    int y0 = Mathf.FloorToInt(gy);
//    int x1 = x0 + 1;
//    int y1 = y0 + 1;

//    // Clamp 防止死区
//    x0 = Mathf.Clamp(x0, 0, width - 1);
//    y0 = Mathf.Clamp(y0, 0, height - 1);
//    x1 = Mathf.Clamp(x1, 0, width - 1);
//    y1 = Mathf.Clamp(y1, 0, height - 1);

//    // 小数部分
//    float tx = Mathf.Clamp01(gx - x0);
//    float ty = Mathf.Clamp01(gy - y0);

//    // 读取四个方向
//    Vector3 d00 = Vector3.zero;
//    Vector3 d10 = Vector3.zero;
//    Vector3 d01 = Vector3.zero;
//    Vector3 d11 = Vector3.zero;

//    byte dir;

//    dir = _dirList[x0, y0];
//    if (dir != 255)
//    {
//        Vector2Int d = DirTable[dir];
//        d00 = new Vector3(d.x, 0, d.y);
//    }

//    dir = _dirList[x1, y0];
//    if (dir != 255)
//    {
//        Vector2Int d = DirTable[dir];
//        d10 = new Vector3(d.x, 0, d.y);
//    }

//    dir = _dirList[x0, y1];
//    if (dir != 255)
//    {
//        Vector2Int d = DirTable[dir];
//        d01 = new Vector3(d.x, 0, d.y);
//    }

//    dir = _dirList[x1, y1];
//    if (dir != 255)
//    {
//        Vector2Int d = DirTable[dir];
//        d11 = new Vector3(d.x, 0, d.y);
//    }

//    // 双线性插值
//    Vector3 d0 = Vector3.LerpUnclamped(d00, d10, tx);
//    Vector3 d1 = Vector3.LerpUnclamped(d01, d11, tx);
//    Vector3 result = Vector3.LerpUnclamped(d0, d1, ty);

//    return result.sqrMagnitude > 0.0001f ? result.normalized : Vector3.zero;
//}