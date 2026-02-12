using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace Assets.Codes.Systems.BuildingSystem
{
    public class BuildingGrid
    {
        private static BuildingGrid _instance = new BuildingGrid();
        public static BuildingGrid Instance()
        {
            return _instance;
        }

        private int _width = 100;
        private int _height = 100;
        private float _cellSize = 3;
        private Vector3 _worldStart = new Vector3(-150, 0, -150);
        private bool[,] _gridArray;
        //private TextMeshPro[,] _tmpArray;


        private BuildingGrid()
        {

            _gridArray = new bool[_width, _height];
            //_tmpArray = new TextMeshPro[width, height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 origin = _worldStart + new Vector3(x * _cellSize, 0, y * _cellSize);

                    //Debug.DrawLine(origin, origin + new Vector3(0, 0, cellSize), Color.red, 100f);
                    //Debug.DrawLine(origin, origin + new Vector3(cellSize, 0, 0), Color.white, 100f);

                    //GameObject textObj = new GameObject($"GridText_{x}_{y}");
                    //textObj.transform.position =
                    //    origin + new Vector3(cellSize * 0.5f, 1f, cellSize * 0.5f);

                    //var tmp = textObj.AddComponent<TextMeshPro>();
                    //tmp.text = $"{x},{y}";
                    //tmp.fontSize = 15;
                    //tmp.alignment = TextAlignmentOptions.Center;
                    //tmp.color = Color.black;

                    //_tmpArray[x, y] = tmp;
                }
            }

            //Debug.DrawLine(worldStart + new Vector3(0, 0, height * cellSize),
            //               worldStart + new Vector3(width * cellSize, 0, height * cellSize),
            //               Color.yellow, 100f);

            //Debug.DrawLine(worldStart + new Vector3(width * cellSize, 0, 0),
            //               worldStart + new Vector3(width * cellSize, 0, height * cellSize),
            //               Color.green, 100f);
        }

        public void SetText(int x, int y, string text)
        {
            //_tmpArray[x, y].text = text;
        }
        public bool TryGetXY(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPos.x - _worldStart.x) / _cellSize);
            y = Mathf.FloorToInt((worldPos.z - _worldStart.z) / _cellSize);

            if (x >= 0 && y >= 0 && x < _width && y < _height)
                return true;

            return false;
        }
        public bool CanPlaceBuilding(int x, int y, int bdWidth, int bdHeight)
        {
            //检查越界
            int startX = x - bdWidth / 2;
            int startY = y - bdHeight / 2;
            if (startX < 0 || startY < 0 || startX + bdWidth >= _width || startY + bdHeight >= _height)
                return false;
            for (int i = startX; i < startX + bdWidth; i++)
            {
                for (int j = startY; j < startY + bdHeight; j++)
                {
                    if (_gridArray[i, j] == true)
                        return false;
                }
            }
            return true;
        }
        public Vector3 GetPlacePos(Vector3 hitPos)
        {
            // 1. 转成相对坐标
            float relativeX = hitPos.x - _worldStart.x;
            float relativeZ = hitPos.z - _worldStart.z;

            // 2. 转成格子索引
            int x = Mathf.FloorToInt(relativeX / _cellSize);
            int y = Mathf.FloorToInt(relativeZ / _cellSize);

            // 3. 还原为格子中心世界坐标
            float worldX = _worldStart.x + x * _cellSize + _cellSize * 0.5f;
            float worldZ = _worldStart.z + y * _cellSize + _cellSize * 0.5f;

            return new Vector3(worldX, 0, worldZ);
        }
        public bool PlaceBuilding(int x, int y, int bdWidth, int bdHeight)
        {
            //检查越界
            int startX = x - bdWidth / 2;
            int startY = y - bdHeight / 2;
            if (startX < 0 || startY < 0 || startX + bdWidth >= _width || startY + bdHeight >= _height)
                return false;
            for(int i = startX; i < startX + bdWidth; i++)
            {
                for(int j = startY; j < startY + bdHeight; j++)
                {
                    _gridArray[i, j] = true;
                    //_tmpArray[i, j].text = "Set";
                }
            }
            return true;
        }
    }
}
