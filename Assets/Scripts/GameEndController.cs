using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameEndController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI nextLevelButtonText;
    [SerializeField] private string GameWonMessage = "You Won! The Corrosion has spread!"; 
    [SerializeField] private string GameLostMessage = "You Lost! Rust is gone!";
    [SerializeField] private string GameLostMessage2 = "You Lost! Time run out!";
    [SerializeField] private int LevelSelectionSceneIndex = 0;

    private bool IsWin = false;

    public void EndGame(bool isWin, bool lostByElimination)
    {
        IsWin = isWin;
        if (isWin)
        {
            gameOverText.text = GameWonMessage;
            nextLevelButtonText.text = "Next Level";
        }
        else
        {
            gameOverText.text = lostByElimination ? GameLostMessage : GameLostMessage2;
            nextLevelButtonText.text = "Repeat Level";
        }
    }

    public void NextLevelOrRepeat()
    {
        if (IsWin)
        {
            try
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                SoundsManager.Instance.PlaySounds(Sounds.ButtonClick);
            }
            catch (System.Exception)
            {
                Debug.LogError("No next level found");
                GoToLevelSelection();
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GoToLevelSelection()
    {
        try
        {
            SceneManager.LoadScene(LevelSelectionSceneIndex);
            SoundsManager.Instance.PlaySounds(Sounds.ButtonClick);
        }
        catch (System.Exception)
        {
            Debug.LogError("LevelSelectionSceneIndex is not set in GameEndController");
        }
    }
}
