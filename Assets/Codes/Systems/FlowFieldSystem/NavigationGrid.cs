using UnityEngine;

namespace Assets.Codes.Systems.FlowFieldSystem
{
    public class NavigationGrid
    {
        //这个类是记录地图数据的权威, 每个流场都应该按照这个类的数据来计算
        private static NavigationGrid _instance = new NavigationGrid();
        public static NavigationGrid Instance()
        {
            return _instance;
        }

        public int Width = 100;
        public int Height = 100;
        public float CellSize = 3;
        public Vector3 WorldStartPos = new Vector3(-150, 0, -150);
        public bool[,] WalkableList;

        private NavigationGrid()
        {
            WalkableList = new bool[Width, Height];
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    WalkableList[x, y] = true;
                }
            }
        }

        public void SetWalkable(Vector3 worldPos, float width, float height, bool canWalk)
        {
            Vector3 min = new Vector3(worldPos.x - width * 0.5f, 0, worldPos.z - height * 0.5f);
            Vector3 max = new Vector3(worldPos.x + width * 0.5f, 0, worldPos.z + height * 0.5f);
            int startX = Mathf.FloorToInt((min.x - WorldStartPos.x) / CellSize);
            int startY = Mathf.FloorToInt((min.z - WorldStartPos.z) / CellSize);
            int endX = Mathf.FloorToInt((max.x - WorldStartPos.x) / CellSize);
            int endY = Mathf.FloorToInt((max.z - WorldStartPos.z) / CellSize);
            if (startX < 0 || startY < 0 || endX >= Width || endY >= Height)
                return;
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    WalkableList[x, y] = canWalk;
                }
            }
        }
    }
}
