//using Assets.Codes.Systems.BuildingSystem;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class GridTest : MonoBehaviour
//{
//    BuildingGrid _grid;
//    public GameObject BuildingPrefab;

//    private void Start()
//    {
//        _grid = new BuildingGrid(100, 100, 3, new Vector3(-150, 0, -150));
//    }

//    private void Update()
//    {
//        if(Keyboard.current.digit1Key.wasPressedThisFrame)
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            if(Physics.Raycast(ray, out RaycastHit hit))
//            {
//                if (_grid.TryGetXY(hit.point, out int x, out int y) == false)
//                {
//                    Debug.Log("TryGetXY False");
//                    return;
//                }
                
//                //模拟一个建筑卡
//                BuildingData testData = new BuildingData();
//                testData.Name = "测试建筑数据";
//                testData.Width = 1;
//                testData.Height = 1;
                
//                if(_grid.CanPlaceBuilding(x, y, testData.Width, testData.Height) == false)
//                {
//                    Debug.Log("CanPlaceBuilding False");
//                    return;
//                }

//                if (_grid.PlaceBuilding(x, y, testData.Width, testData.Height) == false)
//                {
//                    Debug.Log("PlaceBuilding False");
//                    return;
//                }

//                Vector3 pos = _grid.GetPlacePos(hit.point);
//                Instantiate(BuildingPrefab, pos, Quaternion.identity);
//            }
//        }
//    }
//}
