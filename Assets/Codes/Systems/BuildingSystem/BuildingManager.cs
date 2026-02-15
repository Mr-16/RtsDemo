using Assets.Codes.Common;
using Assets.Codes.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Codes.Systems.BuildingSystem
{


    public class BuildingManager
    {
        private static BuildingManager instance = new BuildingManager();
        public static BuildingManager Instance => instance;
        private int width;
        private int height;
        private float cellSize = 4;
        private Dictionary<Vec2I, Building> buildingMap = new Dictionary<Vec2I, Building>();
        private Dictionary<Building, List<Vec2I>> cellListMap = new Dictionary<Building, List<Vec2I>>();
        private BuildingManager()
        {
            width = (int)MathF.Ceiling(GlobalConstant.WorldWidth / cellSize);
            height = (int)MathF.Ceiling(GlobalConstant.WorldHeight / cellSize);
        }


        public bool CanPlace(Vector3 worldPos, Building building)
        {
            Vec2I startPos = GlobalHelper.WorldToGrid(worldPos, cellSize);
            startPos.X -= building.Width / 2;
            startPos.Y -= building.Height / 2;
            if (startPos.X < 0 || startPos.X + building.Width > width || startPos.Y < 0 || startPos.Y + building.Height > height)
                return false;

            for (int i = 0; i < building.Height; i++)
            {
                for (int j = 0; j < building.Width; j++)
                {
                    Vec2I cellPos = new Vec2I(startPos.X + j, startPos.Y + i);
                    if (buildingMap.ContainsKey(cellPos))
                        return false;
                }
            }
            return true;
        }

        public void Place(Vector3 worldPos, Building building)
        {
            if (CanPlace(worldPos, building) == false)
                return;

            Vec2I startPos = GlobalHelper.WorldToGrid(worldPos, cellSize);
            startPos.X -= building.Width / 2;
            startPos.Y -= building.Height / 2;
            List<Vec2I> cellList = new List<Vec2I>();
            for (int i = 0; i < building.Height; i++)
            {
                for (int j = 0; j < building.Width; j++)
                {
                    Vec2I cellPos = new Vec2I(startPos.X + j, startPos.Y + i);
                    buildingMap[cellPos] = building;
                    cellList.Add(cellPos);
                }
            }
            cellListMap[building] = cellList;
        }

        public void Remove(Building building)
        {
            if (cellListMap.TryGetValue(building, out List<Vec2I>? cellList) == false)
                return;
            foreach (Vec2I gridPos in cellList)
            {
                buildingMap.Remove(gridPos);
            }
            cellListMap.Remove(building);
        }
        public Building GetBuilding(Vector3 worldPos)
        {
            Vec2I cellPos = GlobalHelper.WorldToGrid(worldPos, cellSize);
            if (buildingMap.TryGetValue(cellPos, out Building building) == false)
                return null;
            return building;
        }

        public Vector3 GetBuildPos(Vector3 worldPos)
        {

            // 对每个轴进行对齐
            float x = Mathf.Floor(worldPos.x / cellSize) * cellSize;
            float z = Mathf.Floor(worldPos.z / cellSize) * cellSize;

            return new Vector3(x, 0, z);
        }
    }
}
