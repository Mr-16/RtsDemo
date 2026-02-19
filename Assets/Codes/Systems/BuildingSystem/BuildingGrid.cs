using UnityEngine;

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
        private Vector3 _worldStartPos = new Vector3(-150, 0, -150);
        private bool[,] _cellList;
        //private TextMeshPro[,] _tmpArray;


        private BuildingGrid()
        {

            _cellList = new bool[_width, _height];
            //_tmpArray = new TextMeshPro[width, height];

            //for (int x = 0; x < _width; x++)
            //{
            //    for (int y = 0; y < _height; y++)
            //    {
            //        Vector3 origin = _worldStartPos + new Vector3(x * _cellSize, 0, y * _cellSize);

            //        Debug.DrawLine(origin, origin + new Vector3(0, 0, cellSize), Color.red, 100f);
            //        Debug.DrawLine(origin, origin + new Vector3(cellSize, 0, 0), Color.white, 100f);

            //        GameObject textObj = new GameObject($"GridText_{x}_{y}");
            //        textObj.transform.position =
            //            origin + new Vector3(cellSize * 0.5f, 1f, cellSize * 0.5f);

            //        var tmp = textObj.AddComponent<TextMeshPro>();
            //        tmp.text = $"{x},{y}";
            //        tmp.fontSize = 15;
            //        tmp.alignment = TextAlignmentOptions.Center;
            //        tmp.color = Color.black;

            //        _tmpArray[x, y] = tmp;
            //    }
            //}

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
        public bool CanPlaceBuilding(Vector3 worldPos, float bdWorldWidth, float bdWorldHeight)
        {
            Vector3 min = new Vector3(worldPos.x - bdWorldWidth * 0.5f, 0, worldPos.z - bdWorldHeight * 0.5f);
            Vector3 max = new Vector3(worldPos.x + bdWorldWidth * 0.5f, 0, worldPos.z + bdWorldHeight * 0.5f);
            int startX = Mathf.FloorToInt((min.x - _worldStartPos.x) / _cellSize);
            int startY = Mathf.FloorToInt((min.z - _worldStartPos.z) / _cellSize);
            int endX = Mathf.FloorToInt((max.x - _worldStartPos.x) / _cellSize);
            int endY = Mathf.FloorToInt((max.z - _worldStartPos.z) / _cellSize);
            if (startX < 0 || startY < 0 || endX >= _width || endY >= _height)
                return false;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    if (_cellList[x, y] == true)
                        return false;
                }
            }
            return true;
        }

        public Vector3 GetPlacePos(Vector3 hitPos)
        {
            // 1. 转成相对坐标
            float relativeX = hitPos.x - _worldStartPos.x;
            float relativeZ = hitPos.z - _worldStartPos.z;

            // 2. 转成格子索引
            int x = Mathf.FloorToInt(relativeX / _cellSize);
            int y = Mathf.FloorToInt(relativeZ / _cellSize);

            // 3. 还原为格子中心世界坐标
            float worldX = _worldStartPos.x + x * _cellSize + _cellSize * 0.5f;
            float worldZ = _worldStartPos.z + y * _cellSize + _cellSize * 0.5f;

            return new Vector3(worldX, 0, worldZ);
        }
        public bool PlaceBuilding(Vector3 worldPos, float bdWorldWidth, float bdWorldHeight)
        {
            Vector3 min = new Vector3(worldPos.x - bdWorldWidth * 0.5f, 0, worldPos.z - bdWorldHeight * 0.5f);
            Vector3 max = new Vector3(worldPos.x + bdWorldWidth * 0.5f, 0, worldPos.z + bdWorldHeight * 0.5f);
            int startX = Mathf.FloorToInt((min.x - _worldStartPos.x) / _cellSize);
            int startY = Mathf.FloorToInt((min.z - _worldStartPos.z) / _cellSize);
            int endX = Mathf.FloorToInt((max.x - _worldStartPos.x) / _cellSize);
            int endY = Mathf.FloorToInt((max.z - _worldStartPos.z) / _cellSize);
            if (startX < 0 || startY < 0 || endX >= _width || endY >= _height)
                return false;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    _cellList[x, y] = true;
                }
            }
            return true;
        }
    }
}
