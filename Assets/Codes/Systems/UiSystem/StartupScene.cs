using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartupScene : MonoBehaviour
{
    public Button PlayBtn;
    public Button ExitBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBtn.onClick.AddListener(OnPlayBtnClicked);
        ExitBtn.onClick.AddListener(OnExitBtnClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayBtnClicked()
    {
        Debug.Log("OnPlayBtnClicked");
        SceneManager.LoadScene(1);
    }
    private void OnExitBtnClicked()
    {
        Debug.Log("OnExitBtnClicked");
        Application.Quit();
    }
}
