using Assets.Codes.Game;
using Assets.Codes.Systems.BuildingSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private BuildingData buildingData;
    public GameObject BdPreviewPrefab;
    private GameObject BdPreview;
    private Camera mainCamera;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnBeginDrag");
        BdPreview = Instantiate(BdPreviewPrefab);
        UpdatePreviewPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnDrag");
        //rectTransform.anchoredPosition += eventData.delta;
        UpdatePreviewPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnEndDrag");
        Destroy(BdPreview);
    }

    private void UpdatePreviewPosition(PointerEventData eventData)
    {
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        // 示例：射到一个平面 (Y = 0)
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Vector3 placePos = BuildingGrid.Instance().GetPlacePos(worldPos);
            BdPreview.transform.position = placePos;
        }
    }
}
