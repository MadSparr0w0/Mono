using TMPro;
using UnityEngine;

public class TurnDebugUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText;

    private void Update()
    {
        if (MonopolyGameManager.Instance == null)
        {
            if (infoText != null)
                infoText.text = "GameManager not found";
            return;
        }

        if (infoText != null)
        {
            infoText.text = "Current Turn: Player " + (MonopolyGameManager.Instance.CurrentTurnIndex + 1);
        }
    }

    public void Roll()
    {
        if (MonopolyGameManager.Instance == null)
            return;

        MonopolyGameManager.Instance.RollAndMove();
    }
}
