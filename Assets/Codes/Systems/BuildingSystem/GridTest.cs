using Assets.Codes.Systems.BuildingSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridTest : MonoBehaviour
{
    MyGrid _grid;

    private void Start()
    {
        _grid = new MyGrid(50, 50, 6, new Vector3(-150, 0, -150));
    }

    private void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (_grid.TryGetXY(hit.point, out int x, out int y))
                {
                    _grid.SetText(x, y, "FUCK");
                }
            }
        }
    }
}
