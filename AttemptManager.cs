using UnityEngine;
using TMPro;

/*
*   Class to track the number of attempts per game
*/

public class AttemptManager : MonoBehaviour
{
    [Header("Attempt Settings")]
    public int Attempts = 0;
    private int currentAttempt;

    [Header("Text Reference")]
    public TextMeshProUGUI attemptText;

    public void Start()
    {
        currentAttempt = Attempts;
        UpdateAttemptUI();
    }

    public void IncreaseAttempt()
    {
        currentAttempt++;
        UpdateAttemptUI();
    }

    private void UpdateAttemptUI()
    {
        // Update the TMP text with the current attempt
        attemptText.text = $"Wrong Attempts: {currentAttempt.ToString()}";
    }
}
