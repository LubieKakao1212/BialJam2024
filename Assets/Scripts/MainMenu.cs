using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
        }
    }

    public void StartButton()
    {
        SceneManager.LoadSceneAsync("Test 2");
    }
    public void SetFHD()
    {
        Screen.SetResolution(1920, 1080, true);
    }
    public void SetQHD()
    {
        Screen.SetResolution(2560, 1440, true);
    }
    public void Set4k()
    {
        Screen.SetResolution(3840, 2160, true);
    }
    public void Credits()
    {

    }
    public void Quit()
    {
        Application.Quit();
    }
}
