using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{

    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    private int currentPanelIdx = 0;

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void ChangeSlide(bool changeLeft)
    {
        panels[currentPanelIdx].SetActive(false);
        currentPanelIdx += changeLeft ? -1 : 1;
        if (currentPanelIdx < 0)
        {
            currentPanelIdx = panels.Count - 1;
        }
        else if (currentPanelIdx >= panels.Count)
        {
            currentPanelIdx = 0;
        }
        panels[currentPanelIdx].SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
