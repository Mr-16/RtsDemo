using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyScene : MonoBehaviour
{
    public Button GoBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoBtn.onClick.AddListener(OnGoBtnClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGoBtnClicked()
    {
        Debug.Log("OnGoBtnClicked");
        SceneManager.LoadScene(2);
    }
}
