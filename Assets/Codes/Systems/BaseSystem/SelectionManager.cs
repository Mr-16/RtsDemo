using Assets.Codes.Game;
using Assets.Codes.Systems.BuildingSystem;
using Assets.Codes.Systems.FlowFieldSystem;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (EventSystem.current.IsPointerOverGameObject())
                return;

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
            if (selectionBox != null) selectionBox.gameObject.SetActive(false);

            if (Vector2.Distance(dragStart, dragEnd) < 5f)
                SingleSelect(dragEnd);
            else if(isDragging)
            {
                BoxSelect(dragStart, dragEnd);
            }
            isDragging = false;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            //Debug.Log("[rightButton]");
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                GameObject mark = Instantiate(moveCmdMarkPrefab, hit.point, Quaternion.identity);
                FlowField flowField = new FlowField();
                flowField.Compute(hit.point);
                //foreach (Unit unit in selectedUnitList)
                //{
                //    unit.curFlowField = flowField;
                //}
                int count = selectedUnitList.Count;
                if (count == 0)
                    return;

                // 每个单位之间的间距
                float spacing = 3f;
                // 计算方阵的列数（尽量接近正方形）
                int col = Mathf.CeilToInt(Mathf.Sqrt(count));
                int row = Mathf.CeilToInt((float)count / col);
                // 让阵型以点击点为中心
                Vector3 center = hit.point;
                // 左上角起始偏移
                float width = (col - 1) * spacing;
                float height = (row - 1) * spacing;
                Vector3 startOffset = new Vector3(-width * 0.5f, 0, height * 0.5f);
                for (int i = 0; i < count; i++)
                {
                    int r = i / col;
                    int c = i % col;
                    Vector3 offset = new Vector3(c * spacing, 0, -r * spacing);
                    Vector3 targetPos = center + startOffset + offset;
                    Unit unit = selectedUnitList[i];
                    unit.SelfTargetPos = targetPos;
                    unit.curFlowField = flowField;
                }

                await Task.Delay(1000);
                Destroy(mark);
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
        List<Unit> unitList = GameManager.Instance.UnitList;
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
