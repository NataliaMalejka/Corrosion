using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TurnText;
    public void UpdateTurnCounter(int turns)
    {
        TurnText.text = "Moves left: " + turns.ToString();
    }
}