using Assets.Codes.Systems.FlowFieldSystem;
using TMPro;
using UnityEngine;

public class FlowFieldGridTest : MonoBehaviour
{
    private static FlowFieldGridTest _instance;
    public static FlowFieldGridTest Instance()
    {
        return _instance;
    }

    //这个类用来测其他grid
    public NavigationGrid NaviGrid;
    private TextMeshPro[,] _tmpList;
    public FlowField CurFlowField;

    private void Start()
    {
        _instance = this;
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
        if (Time.frameCount % 10 != 0)
        {
            return;
        }

        if (CurFlowField == null)
            return;
        for(int x = 0; x < NaviGrid.Width; x++)
        {
            for(int y = 0; y < NaviGrid.Height; y++)
            {
                Vector3 origin = NaviGrid.WorldStartPos + new Vector3(x * NaviGrid.CellSize, 0, y * NaviGrid.CellSize);
                Debug.DrawLine(origin, origin + new Vector3(0, 0, NaviGrid.CellSize), Color.white,1);
                Debug.DrawLine(origin, origin + new Vector3(NaviGrid.CellSize, 0, 0), Color.white,1);
                
                Vector2Int dir = CurFlowField.GetDir(x, y);
                //string arrow = "";
                //if (dir == Vector2Int.zero)
                //{
                //    arrow = "·";   // 无方向，可自定义
                //}
                //else if (dir.x == 0 && dir.y == 1) arrow = "↑";
                //else if (dir.x == 0 && dir.y == -1) arrow = "↓";
                //else if (dir.x == -1 && dir.y == 0) arrow = "←";
                //else if (dir.x == 1 && dir.y == 0) arrow = "→";
                //else if (dir.x == -1 && dir.y == 1) arrow = "↖";
                //else if (dir.x == 1 && dir.y == 1) arrow = "↗";
                //else if (dir.x == -1 && dir.y == -1) arrow = "↙";
                //else if (dir.x == 1 && dir.y == -1) arrow = "↘";
                _tmpList[x, y].text = $"{dir.x},{dir.y}";

            }

        }
        Debug.DrawLine(NaviGrid.WorldStartPos + new Vector3(0, 0, NaviGrid.Height * NaviGrid.CellSize),
                       NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, NaviGrid.Height * NaviGrid.CellSize),
                       Color.white,1);

        Debug.DrawLine(NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, 0),
                       NaviGrid.WorldStartPos + new Vector3(NaviGrid.Width * NaviGrid.CellSize, 0, NaviGrid.Height * NaviGrid.CellSize),
                       Color.white,1);
    }
}
