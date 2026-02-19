using Assets.Codes.Systems.BuildingSystem;
using Assets.Codes.Systems.FlowFieldSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Burst.Intrinsics.X86.Avx;

public class GridTest : MonoBehaviour
{
    //这个类用来测其他grid
    public NavigationGrid NaviGrid;
    private TextMeshPro[,] _tmpList;

    private void Start()
    {
        NaviGrid = NavigationGrid.Instance();
        _tmpList = new TextMeshPro[NaviGrid.Width, NaviGrid.Height];
        NaviGrid = NavigationGrid.Instance();
        _tmpList = new TextMeshPro[NaviGrid.Width, NaviGrid.Height];

        for (int x = 0; x < NaviGrid.Width; x++)
        {
            for (int y = 0; y < NaviGrid.Height; y++)
            {
                Vector3 origin = NaviGrid.WorldStartPos + new Vector3(x * NaviGrid.CellSize, 0, y * NaviGrid.CellSize);

                GameObject textObj = new GameObject($"GridText_{x}_{y}");
                textObj.transform.position = origin + new Vector3(NaviGrid.CellSize * 0.5f, 1f, NaviGrid.CellSize * 0.5f);
                TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
                tmp.text = $"{x},{y}";
                tmp.fontSize = 10;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.black;
                _tmpList[x, y] = tmp;
            }
        }
    }

    private void Update()
    {
        for(int x = 0; x < NaviGrid.Width; x++)
        {
            for(int y = 0; y < NaviGrid.Height; y++)
            {
                Vector3 origin = NaviGrid.WorldStartPos + new Vector3(x * NaviGrid.CellSize, 0, y * NaviGrid.CellSize);
                Debug.DrawLine(origin, origin + new Vector3(0, 0, NaviGrid.CellSize), Color.red);
                Debug.DrawLine(origin, origin + new Vector3(NaviGrid.CellSize, 0, 0), Color.white);
                _tmpList[x, y].text = NaviGrid.WalkableList[x,y] == true ? "Yes" : "No";
                //GameObject textObj = new GameObject($"GridText_{x}_{y}");
                //textObj.transform.position = origin + new Vector3(NaviGrid.CellSize * 0.5f, 1f, NaviGrid.CellSize * 0.5f);
                //TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
                //tmp.text = $"{x},{y}";
                //tmp.fontSize = 10;
                //tmp.alignment = TextAlignmentOptions.Center;
                //tmp.color = Color.black;
                //_tmpList[x, y] = tmp;
            }

        }
        Debug.DrawLine(NaviGrid.WorldStartPos + new Vector3(0, 0, NaviGrid.Height * NaviGrid.CellSize),
                       NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, NaviGrid.Height * NaviGrid.CellSize),
                       Color.yellow);

        Debug.DrawLine(NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, 0),
                       NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, NaviGrid.Height * NaviGrid.CellSize),
                       Color.green);
    }
}
