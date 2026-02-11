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
    public class MyGrid
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private Vector3 _worldStart;
        private int[,] _gridArray;
        private TextMeshPro[,] _tmpArray;


        public MyGrid(int width, int height, float cellSize, Vector3 worldStart)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _worldStart = worldStart;
            _gridArray = new int[width, height];
            _tmpArray = new TextMeshPro[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 origin = worldStart + new Vector3(x * cellSize, 0, y * cellSize);

                    Debug.DrawLine(origin, origin + new Vector3(0, 0, cellSize), Color.red, 100f);
                    Debug.DrawLine(origin, origin + new Vector3(cellSize, 0, 0), Color.white, 100f);

                    GameObject textObj = new GameObject($"GridText_{x}_{y}");
                    textObj.transform.position =
                        origin + new Vector3(cellSize * 0.5f, 1f, cellSize * 0.5f);

                    var tmp = textObj.AddComponent<TextMeshPro>();
                    tmp.text = $"{x},{y}";
                    tmp.fontSize = 15;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = Color.black;

                    _tmpArray[x, y] = tmp;
                }
            }

            Debug.DrawLine(worldStart + new Vector3(0, 0, height * cellSize),
                           worldStart + new Vector3(width * cellSize, 0, height * cellSize),
                           Color.yellow, 100f);

            Debug.DrawLine(worldStart + new Vector3(width * cellSize, 0, 0),
                           worldStart + new Vector3(width * cellSize, 0, height * cellSize),
                           Color.green, 100f);
        }

        public void SetText(int x, int y, string text)
        {
            _tmpArray[x, y].text = text;
        }
        public bool TryGetXY(Vector3 worldPos, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPos.x - _worldStart.x) / _cellSize);
            y = Mathf.FloorToInt((worldPos.z - _worldStart.z) / _cellSize);

            if (x >= 0 && y >= 0 && x < _width && y < _height)
                return true;

            return false;
        }

    }
}
