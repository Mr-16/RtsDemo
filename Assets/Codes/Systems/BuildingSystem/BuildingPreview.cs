using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public GameObject BdMesh;

    private Renderer _renderer;
    private MaterialPropertyBlock _block;

    private void Awake()
    {
        _renderer = BdMesh.GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
    }

    public void SetCanPlace(bool canPlace)
    {
        _renderer.GetPropertyBlock(_block);
        //Debug.Log("canPlace : " + canPlace);
        Color color = canPlace ? Color.green : Color.red;

        _block.SetColor("_BaseColor", color);
        _renderer.SetPropertyBlock(_block);
    }
}
