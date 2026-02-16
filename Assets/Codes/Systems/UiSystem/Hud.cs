using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public TMP_Text fpsTmp;
    float smoothFps = 0f;
    void Start()
    {
        
    }

    void Update()
    {
        float currentFps = 1f / Time.unscaledDeltaTime;
        smoothFps = Mathf.Lerp(smoothFps, currentFps, 0.1f);
        fpsTmp.text = $"FPS: {Mathf.RoundToInt(smoothFps)}";
    }
}
