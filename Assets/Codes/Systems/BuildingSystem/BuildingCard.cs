using Assets.Codes.Game;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    //private CanvasGroup canvasGroup;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnBeginDrag");
        //canvasGroup.blocksRaycasts = false;
        GameManager.Instance.IsUiDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        Debug.Log("OnEndDrag");
        //canvasGroup.blocksRaycasts = true;
        GameManager.Instance.IsUiDragging = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
