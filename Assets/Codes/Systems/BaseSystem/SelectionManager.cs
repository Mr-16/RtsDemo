using Assets.Codes.Game;
using Assets.Codes.Systems.BuildingSystem;
using Assets.Codes.Systems.FlowFieldSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    [SerializeField] private RectTransform selectionBox; // Canvas 上的 Image
    private Vector2 dragStart;
    private bool isDragging;
    private List<Unit> selectedUnitList = new List<Unit>();
    public LayerMask groundLayer;
    [SerializeField] private GameObject moveCmdMarkPrefab;
    [SerializeField] private Building testBuildingPrefab;

    void Awake()
    {
        Instance = this;
        if (selectionBox != null)
            selectionBox.gameObject.SetActive(false); // 初始隐藏
    }

    void Update()
    {
        HandleInput();
    }

    async Task HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragStart = Mouse.current.position.ReadValue();
            isDragging = true;
            if (selectionBox != null) selectionBox.gameObject.SetActive(true);
        }

        if (isDragging)
        {
            Vector2 current = Mouse.current.position.ReadValue();
            UpdateSelectionBox(dragStart, current);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 dragEnd = Mouse.current.position.ReadValue();
            isDragging = false;

            if (selectionBox != null) selectionBox.gameObject.SetActive(false);

            if (Vector2.Distance(dragStart, dragEnd) < 5f)
                SingleSelect(dragEnd);
            else
                BoxSelect(dragStart, dragEnd);
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            //Debug.Log("[rightButton]");

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                GameObject mark = Instantiate(moveCmdMarkPrefab, hit.point, Quaternion.identity);
                FlowField flowField = new FlowField(2);
                flowField.ComputeFlowField(hit.point);
                foreach (Unit unit in selectedUnitList)
                {
                    unit.flowField = flowField;
                }
                await Task.Delay(1000);
                Destroy(mark);
            }
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                //GameObject mark = Instantiate(moveCmdMarkPrefab, hit.point, Quaternion.identity);
                Debug.Log(hit.point);
                if(BuildingManager.Instance.CanPlace(hit.point, testBuildingPrefab))
                {
                    Vector3 buildPos = BuildingManager.Instance.GetBuildPos(hit.point);
                    Building building = Instantiate(testBuildingPrefab, buildPos, Quaternion.identity);
                    BuildingManager.Instance.Place(hit.point, building);
                }
                else
                {
                    Debug.Log("Cannot place building here!");
                }
            }
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                Debug.Log(hit.point);
                Building rmBd = BuildingManager.Instance.GetBuilding(hit.point);
                if (rmBd != null)
                {
                    BuildingManager.Instance.Remove(rmBd);
                    Destroy(rmBd.gameObject);
                }
            }
        }
    }

    private void UpdateSelectionBox(Vector2 start, Vector2 end)
    {
        if (selectionBox == null) return;

        RectTransform canvasRect = selectionBox.parent.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, start, null, out Vector2 localStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, end, null, out Vector2 localEnd);

        Vector2 min = Vector2.Min(localStart, localEnd);
        Vector2 max = Vector2.Max(localStart, localEnd);
        Vector2 size = max - min;

        selectionBox.anchoredPosition = min;
        selectionBox.sizeDelta = size;
    }

    void SingleSelect(Vector2 screenPos)
    {
        ClearSelection();

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                selectedUnitList.Add(unit);
                unit.SetSelected(true);
            }
        }
    }

    void BoxSelect(Vector2 start, Vector2 end)
    {
        ClearSelection();

        Rect selectionRect = new Rect(
            Mathf.Min(start.x, end.x),
            Mathf.Min(start.y, end.y),
            Mathf.Abs(end.x - start.x),
            Mathf.Abs(end.y - start.y)
        );

        //Unit[] allUnits = FindObjectsOfType<Unit>();
        List<Unit> unitList = GameManager.Instance().UnitList;
        foreach (Unit unit in unitList)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            // 判断是否在矩形内
            if (selectionRect.Contains(screenPos))
            {
                selectedUnitList.Add(unit);
                unit.SetSelected(true);
            }
        }
    }

    void ClearSelection()
    {
        foreach (Unit unit in selectedUnitList)
            unit.SetSelected(false);
        selectedUnitList.Clear();
    }
}
