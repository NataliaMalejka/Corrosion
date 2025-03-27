using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TurnText;
    [SerializeField] Slider Slider;
    public void UpdateTurnCounter(int turns, int maxTurns)
    {
        TurnText.text = turns.ToString() + "/" + maxTurns.ToString();
        Slider.value = (float)turns/(float)maxTurns;
    }
}