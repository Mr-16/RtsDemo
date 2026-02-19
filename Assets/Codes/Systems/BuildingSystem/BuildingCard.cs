using Assets.Codes.Game;
using Assets.Codes.Systems.BuildingSystem;
using Assets.Codes.Systems.FlowFieldSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private BuildingData buildingData;
    public GameObject BdPreviewPrefab;
    public GameObject BdPrefab;
    
    private GameObject BdPreview;
    private Camera mainCamera;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        buildingData = new BuildingData();
        buildingData.Width = 10;
        buildingData.Height = 10;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        //Debug.Log("OnBeginDrag");
        BdPreview = Instantiate(BdPreviewPrefab);
        UpdatePreviewPosition(eventData);
        GameManager.Instance.BdGrid.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        //Debug.Log("OnDrag");
        //rectTransform.anchoredPosition += eventData.delta;
        UpdatePreviewPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        //Debug.Log("OnEndDrag");
        Destroy(BdPreview);
        GameManager.Instance.BdGrid.SetActive(false);

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            //BuildingGrid.Instance().TryGetXY(worldPos, out int x, out int y);
            Vector3 placePos = BuildingGrid.Instance().GetPlacePos(worldPos);
            if (BuildingGrid.Instance().CanPlaceBuilding(placePos, buildingData.Width, buildingData.Height) == false)
            {
                Debug.Log("CanPlaceBuilding False");
                return;
            }

            if (BuildingGrid.Instance().PlaceBuilding(placePos, buildingData.Width, buildingData.Height) == false)
            {
                Debug.Log("PlaceBuilding False");
                return;
            }
            GameObject bdObj = Instantiate(BdPrefab, placePos, Quaternion.identity);
            Building bd = bdObj.GetComponent<Building>();
            GameManager.Instance.BdSh.Add(bd, bd.transform.position);
            NavigationGrid.Instance().SetWalkable(placePos, buildingData.Width, buildingData.Height, false);
        }
    }

    private void UpdatePreviewPosition(PointerEventData eventData)
    {
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Vector3 placePos = BuildingGrid.Instance().GetPlacePos(worldPos);
            //BuildingGrid.Instance().TryGetXY(worldPos, out int x, out int y);
            BuildingPreview comp = BdPreview.GetComponent<BuildingPreview>();
            if (BuildingGrid.Instance().CanPlaceBuilding(placePos, buildingData.Width, buildingData.Height))
                comp.SetCanPlace(true);
            else
                comp.SetCanPlace(false);
            BdPreview.transform.position = placePos;
        }
    }
}
